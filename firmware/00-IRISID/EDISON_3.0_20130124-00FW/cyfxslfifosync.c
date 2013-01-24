/*
 ## Cypress USB 3.0 Platform source file (cyfxslfifosync.c)
 ## ===========================
 ##
 ##  Copyright Cypress Semiconductor Corporation, 2010-2011,
 ##  All Rights Reserved
 ##  UNPUBLISHED, LICENSED SOFTWARE.
 ##
 ##  CONFIDENTIAL AND PROPRIETARY INFORMATION
 ##  WHICH IS THE PROPERTY OF CYPRESS.
 ##
 ##  Use of this file is governed
 ##  by the license agreement included in the file
 ##
 ##     <install>/license/license.txt
 ##
 ##  where <install> is the Cypress software
 ##  installation root directory path.
 ##
 ## ===========================
*/

/* This file illustrates the Slave FIFO Synchronous mode example */

/*
   This example comprises of two USB bulk endpoints. A bulk OUT endpoint acts as the
   producer of data from the host. A bulk IN endpoint acts as the consumer of data to
   the host. Appropriate vendor class USB enumeration descriptors with these two bulk
   endpoints are implemented.

   The GPIF configuration data for the Synchronous Slave FIFO operation is loaded onto
   the appropriate GPIF registers. The p-port data transfers are done via the producer
   p-port socket and the consumer p-port socket.

   This example implements two DMA Channels in MANUAL mode one for P to U data transfer
   and one for U to P data transfer.

   The U to P DMA channel connects the USB producer (OUT) endpoint to the consumer p-port
   socket. And the P to U DMA channel connects the producer p-port socket to the USB 
   consumer (IN) endpoint.

   Upon every reception of data in the DMA buffer from the host or from the p-port, the
   CPU is signalled using DMA callbacks. There are two DMA callback functions implemented
   each for U to P and P to U data paths. The CPU then commits the DMA buffer received so
   that the data is transferred to the consumer.

   The DMA buffer size for each channel is defined based on the USB speed. 64 for full
   speed, 512 for high speed and 1024 for super speed. CY_FX_SLFIFO_DMA_BUF_COUNT in the
   header file defines the number of DMA buffers per channel.

   The constant CY_FX_SLFIFO_GPIF_16_32BIT_CONF_SELECT in the header file is used to
   select 16bit or 32bit GPIF data bus configuration.
 */

#include "cyu3system.h"
#include "cyu3os.h"
#include "cyu3dma.h"
#include "cyu3error.h"
#include "cyu3usb.h"
#include "cyu3i2c.h"
#include "cyu3uart.h"
#include "cyfxslfifosync.h"
#include "cyu3gpif.h"
#include "cyu3pib.h"
#include "pib_regs.h"

/* This file should be included only once as it contains
 * structure definitions. Including it in multiple places
 * can result in linker error. */
#include "cyfxgpif_syncsf.h"


CyU3PThread slFifoAppThread;	        /* Slave FIFO application thread structure */
CyU3PDmaChannel glChHandleBulkLpOut;     /* DMA MANUAL_OUT channel handle.         */
CyU3PDmaChannel glChHandleSlFifoPtoU;   /* DMA Channel handle for P2U transfer. */
CyU3PDmaChannel glChHandleSlFifoPtoU1;   /* DMA Channel handle for P2U transfer. */
CyU3PDmaChannel glChHandleSlFifoPtoU2;   /* DMA Channel handle for P2U transfer. */
CyU3PDmaChannel glChHandleSlFifoPtoU3;   /* DMA Channel handle for P2U transfer. */

uint32_t glDMARxCount = 0;               /* Counter to track the number of buffers received from USB. */
uint32_t glDMATxCount = 0;               /* Counter to track the number of buffers sent to USB. */
CyBool_t glIsApplnActive = CyFalse;      /* Whether the loopback application is active or not. */
uint8_t Command_Flag = 0;
uint8_t glEP0IN_buff[512];

struct EP1OUTBuffer_t {
	uint8_t header; /* header. On 0xF5 is ..... 0xXX */
	uint8_t cmd;	/* command or event type */
	uint8_t ret;	/* return */
	uint8_t size;
	uint8_t data[60]; /* data to be sent */
};

struct EP1OUTBuffer_t glEP1OUTBuffer;

////////////////////////////////////
/////////// I2C  ///////////////////
////////////////////////////////////

uint16_t glI2cPageSize = 0x40;   /* I2C Page size to be used for transfers. */
CyU3PReturnStatus_t
CyFxUsbI2cTransfer (
        uint16_t  byteAddress,
        uint8_t   devAddr,
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead);


////////////////////////////////////
////////EDISON FUNCTION ////////////
////////////////////////////////////
void CommandDevice(uint8_t *buff, uint16_t readCount);
CyU3PReturnStatus_t SendHost();
void TD_PoLL();

/* Application Error Handler */
void
CyFxAppErrorHandler (
        CyU3PReturnStatus_t apiRetStatus    /* API return status */
        )
{
    /* Application failed with the error code apiRetStatus */

    /* Add custom debug or recovery actions here */

    /* Loop Indefinitely */
    for (;;)
    {
        /* Thread sleep : 100 ms */
        CyU3PThreadSleep (100);
    }
}

/* This function initializes the debug module. The debug prints
 * are routed to the UART and can be seen using a UART console
 * running at 115200 baud rate. */
void
CyFxSlFifoApplnDebugInit (void)
{
    CyU3PUartConfig_t uartConfig;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;

    /* Initialize the UART for printing debug messages */
    apiRetStatus = CyU3PUartInit();
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        /* Error handling */
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Set UART configuration */
    CyU3PMemSet ((uint8_t *)&uartConfig, 0, sizeof (uartConfig));
    uartConfig.baudRate = CY_U3P_UART_BAUDRATE_115200;
    uartConfig.stopBit = CY_U3P_UART_ONE_STOP_BIT;
    uartConfig.parity = CY_U3P_UART_NO_PARITY;
    uartConfig.txEnable = CyTrue;
    uartConfig.rxEnable = CyFalse;
    uartConfig.flowCtrl = CyFalse;
    uartConfig.isDma = CyTrue;

    apiRetStatus = CyU3PUartSetConfig (&uartConfig, NULL);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Set the UART transfer to a really large value. */
    apiRetStatus = CyU3PUartTxSetBlockXfer (0xFFFFFFFF);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Initialize the debug module. */
    apiRetStatus = CyU3PDebugInit (CY_U3P_LPP_SOCKET_UART_CONS, 8);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyFxAppErrorHandler(apiRetStatus);
    }
}

/* DMA callback function to handle the produce events for U to P transfers. */
void
CyFxSlFifoUtoPDmaCallback (
        CyU3PDmaChannel   *chHandle,
        CyU3PDmaCbType_t  type,
        CyU3PDmaCBInput_t *input
        )
{
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    if (type == CY_U3P_DMA_CB_PROD_EVENT)
    {
        /* This is a produce event notification to the CPU. This notification is 
         * received upon reception of every buffer. The buffer will not be sent
         * out unless it is explicitly committed. The call shall fail if there
         * is a bus reset / usb disconnect or if there is any application error. */
        status = CyU3PDmaChannelCommitBuffer (chHandle, input->buffer_p.count, 0);
        if (status != CY_U3P_SUCCESS)
        {
            CyU3PDebugPrint (4, "CyU3PDmaChannelCommitBuffer failed, Error code = %d\n", status);
        }

        /* Increment the counter. */
        glDMARxCount++;
    }
}

/* DMA callback function to handle the produce events for P to U transfers. */
void
CyFxSlFifoPtoUDmaCallback (
        CyU3PDmaChannel   *chHandle,
        CyU3PDmaCbType_t  type,
        CyU3PDmaCBInput_t *input
        )
{
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    if (type == CY_U3P_DMA_CB_PROD_EVENT)
    {
        /* This is a produce event notification to the CPU. This notification is 
         * received upon reception of every buffer. The buffer will not be sent
         * out unless it is explicitly committed. The call shall fail if there
         * is a bus reset / usb disconnect or if there is any application error. */
        status = CyU3PDmaChannelCommitBuffer (chHandle, input->buffer_p.count, 0);
        if (status != CY_U3P_SUCCESS)
        {
            CyU3PDebugPrint (4, "CyU3PDmaChannelCommitBuffer failed, Error code = %d\n", status);
        }

        /* Increment the counter. */
        glDMATxCount++;
    }
}

/* This function starts the slave FIFO loop application. This is called
 * when a SET_CONF event is received from the USB host. The endpoints
 * are configured and the DMA pipe is setup in this function. */
void
CyFxSlFifoApplnStart (
        void)
{
    uint16_t size = 0;
    CyU3PEpConfig_t epCfg;
    CyU3PDmaChannelConfig_t dmaCfg;
    CyU3PDmaChannelConfig_t dmaCfg2;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;
    CyU3PUSBSpeed_t usbSpeed = CyU3PUsbGetSpeed();

    /* First identify the usb speed. Once that is identified,
     * create a DMA channel and start the transfer on this. */

    /* Based on the Bus Speed configure the endpoint packet size */
    switch (usbSpeed)
    {
        case CY_U3P_FULL_SPEED:
            size = 64;
            break;

        case CY_U3P_HIGH_SPEED:
            size = 512;
            break;

        case  CY_U3P_SUPER_SPEED:
            size = 1024;
            break;

        default:
            CyU3PDebugPrint (4, "Error! Invalid USB speed.\n");
            CyFxAppErrorHandler (CY_U3P_ERROR_FAILURE);
            break;
    }
////////////////// EndPoint SETTING  ////////////////////////////////////////////
    CyU3PMemSet ((uint8_t *)&epCfg, 0, sizeof (epCfg));
    epCfg.enable = CyTrue;
    epCfg.epType = CY_U3P_USB_EP_BULK;
    epCfg.burstLen = 1;//BURST_LEN; // epCfg.burstLen = 1; //test 1
    epCfg.streams = 0;
    epCfg.pcktSize = size;

    /* Consumer endpoint 1 configuration */
	apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_1, &epCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler (apiRetStatus);
	}
	 /* Consumer endpoint 2 configuration */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_2, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

    /* Consumer endpoint 3 configuration */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_3, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

    /* Consumer endpoint 4 configuration */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_4, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

    /* Consumer endpoint 5 configuration */
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_5, &epCfg);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler (apiRetStatus);
    }

//////////////////////// BULK EP  ////////////////////////////////////////////
	/* Create a DMA MANUAL channel for U2P transfer.
	 * DMA size is set based on the USB speed. */
	dmaCfg2.size  = size;
	dmaCfg2.count = CY_FX_SLFIFO_DMA_BUF_COUNT;
	dmaCfg2.prodSckId = CY_U3P_CPU_SOCKET_PROD;
	dmaCfg2.consSckId = CY_FX_CONSUMER_USB_SOCKET_1;
	dmaCfg2.dmaMode = CY_U3P_DMA_MODE_BYTE;
	/* Enabling the callback for produce event. */
	dmaCfg2.notification = 0;
	dmaCfg2.cb = 0;
	dmaCfg2.prodHeader = 0;
	dmaCfg2.prodFooter = 0;
	dmaCfg2.consHeader = 0;
	dmaCfg2.prodAvailCount = 0;

	/* Create a DMA MANUAL_OUT channel for the consumer socket. */

	apiRetStatus = CyU3PDmaChannelCreate (&glChHandleBulkLpOut,
			CY_U3P_DMA_TYPE_MANUAL_OUT, &dmaCfg2);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

	apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleBulkLpOut, CY_FX_BULKLP_DMA_TX_SIZE);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

/////////////////// SLFIFO EP /////////////////////////

	/* Create a DMA AUTO channel for P2U transfer. EP2*/
	dmaCfg.size  = DMA_BUF_SIZE*size; //increase buffer size for higher performance
	dmaCfg.count = CY_FX_SLFIFO_DMA_BUF_COUNT_P_2_U; // increase buffer count for higher performance
	dmaCfg.prodSckId = CY_FX_PRODUCER_PPORT_SOCKET;
	dmaCfg.consSckId = CY_FX_CONSUMER_USB_SOCKET_2;
	dmaCfg.dmaMode = CY_U3P_DMA_MODE_BYTE;
	/* Enabling the callback for produce event. */
	dmaCfg.notification = 0;
	dmaCfg.cb = NULL;
	dmaCfg.prodHeader = 0;
	dmaCfg.prodFooter = 0;
	dmaCfg.consHeader = 0;
	dmaCfg.prodAvailCount = 0;

	apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoPtoU,
	CY_U3P_DMA_TYPE_AUTO, &dmaCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

	/* Flush the Endpoint memory */
	CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_2);

	apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU, CY_FX_SLFIFO_DMA_RX_SIZE);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

	//////////////////////
	/* Create a DMA AUTO channel for P2U transfer. EP3*/
	dmaCfg.size  = DMA_BUF_SIZE*size; //increase buffer size for higher performance
	dmaCfg.count = CY_FX_SLFIFO_DMA_BUF_COUNT_P_2_U; // increase buffer count for higher performance
	dmaCfg.prodSckId = CY_FX_PRODUCER_PPORT_SOCKET1;
	dmaCfg.consSckId = CY_FX_CONSUMER_USB_SOCKET_3;
	dmaCfg.dmaMode = CY_U3P_DMA_MODE_BYTE;
	/* Enabling the callback for produce event. */
	dmaCfg.notification = 0;
	dmaCfg.cb = NULL;
	dmaCfg.prodHeader = 0;
	dmaCfg.prodFooter = 0;
	dmaCfg.consHeader = 0;
	dmaCfg.prodAvailCount = 0;


	apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoPtoU1,
	CY_U3P_DMA_TYPE_AUTO, &dmaCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

	/* Flush the Endpoint memory */
	CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_3);

	apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU1, CY_FX_SLFIFO_DMA_RX_SIZE);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

	/////////////////////////
	/* Create a DMA AUTO channel for P2U transfer. EP4*/
	dmaCfg.size  = DMA_BUF_SIZE*size; //increase buffer size for higher performance
	dmaCfg.count = CY_FX_SLFIFO_DMA_BUF_COUNT_P_2_U; // increase buffer count for higher performance
	dmaCfg.prodSckId = CY_FX_PRODUCER_PPORT_SOCKET2;
	dmaCfg.consSckId = CY_FX_CONSUMER_USB_SOCKET_4;
	dmaCfg.dmaMode = CY_U3P_DMA_MODE_BYTE;
	/* Enabling the callback for produce event. */
	dmaCfg.notification = 0;
	dmaCfg.cb = NULL;
	dmaCfg.prodHeader = 0;
	dmaCfg.prodFooter = 0;
	dmaCfg.consHeader = 0;
	dmaCfg.prodAvailCount = 0;


	apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoPtoU2,
	CY_U3P_DMA_TYPE_AUTO, &dmaCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
	CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
	CyFxAppErrorHandler(apiRetStatus);
	}

	/* Flush the Endpoint memory */
	CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_4);

	apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU2, CY_FX_SLFIFO_DMA_RX_SIZE);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
	CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
	CyFxAppErrorHandler(apiRetStatus);
	}

	//////////////////
	/* Create a DMA AUTO channel for P2U transfer. EP5*/
	dmaCfg.size  = DMA_BUF_SIZE*size; //increase buffer size for higher performance
	dmaCfg.count = CY_FX_SLFIFO_DMA_BUF_COUNT_P_2_U; // increase buffer count for higher performance
	dmaCfg.prodSckId = CY_FX_PRODUCER_PPORT_SOCKET3;
	dmaCfg.consSckId = CY_FX_CONSUMER_USB_SOCKET_5;
	dmaCfg.dmaMode = CY_U3P_DMA_MODE_BYTE;
	/* Enabling the callback for produce event. */
	dmaCfg.notification = 0;
	dmaCfg.cb = NULL;
	dmaCfg.prodHeader = 0;
	dmaCfg.prodFooter = 0;
	dmaCfg.consHeader = 0;
	dmaCfg.prodAvailCount = 0;


	apiRetStatus = CyU3PDmaChannelCreate (&glChHandleSlFifoPtoU3,
		CY_U3P_DMA_TYPE_AUTO, &dmaCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelCreate failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}

	/* Flush the Endpoint memory */
	CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_5);

	apiRetStatus = CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU3, CY_FX_SLFIFO_DMA_RX_SIZE);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PDmaChannelSetXfer Failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler(apiRetStatus);
	}


    /* Update the status flag. */
    glIsApplnActive = CyTrue;
}

/* This function stops the slave FIFO loop application. This shall be called
 * whenever a RESET or DISCONNECT event is received from the USB host. The
 * endpoints are disabled and the DMA pipe is destroyed by this function. */
void
CyFxSlFifoApplnStop (
        void)
{
    CyU3PEpConfig_t epCfg;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;

    /* Update the flag. */
    glIsApplnActive = CyFalse;

    /* Flush the endpoint memory */

    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_1);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_2);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_3);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_4);
    CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_5);

    /* Destroy the channel */
//    CyU3PDmaChannelDestroy (&glChHandleBulkLpIn);
	CyU3PDmaChannelDestroy (&glChHandleBulkLpOut);
//    CyU3PDmaChannelDestroy (&glChHandleSlFifoUtoP);
    CyU3PDmaChannelDestroy (&glChHandleSlFifoPtoU);
    CyU3PDmaChannelDestroy (&glChHandleSlFifoPtoU1);
    CyU3PDmaChannelDestroy (&glChHandleSlFifoPtoU2);
    CyU3PDmaChannelDestroy (&glChHandleSlFifoPtoU3);

    /* Disable endpoints. */
    CyU3PMemSet ((uint8_t *)&epCfg, 0, sizeof (epCfg));
    epCfg.enable = CyFalse;

    /* Consumer endpoint configuration. EP1*/
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_1, &epCfg);
  	if (apiRetStatus != CY_U3P_SUCCESS)
  	{
  		CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
  		CyFxAppErrorHandler (apiRetStatus);
  	}

	/* Consumer endpoint configuration. EP2*/
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_2, &epCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler (apiRetStatus);
	}

	/* Consumer endpoint configuration. EP3*/
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_3, &epCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler (apiRetStatus);
	}

	/* Consumer endpoint configuration. EP4*/
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_4, &epCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler (apiRetStatus);
	}

	/* Consumer endpoint configuration. EP5*/
    apiRetStatus = CyU3PSetEpConfig(CY_FX_EP_CONSUMER_5, &epCfg);
	if (apiRetStatus != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "CyU3PSetEpConfig failed, Error code = %d\n", apiRetStatus);
		CyFxAppErrorHandler (apiRetStatus);
	}


}

/* Callback to handle the USB setup requests. */
CyBool_t
CyFxSlFifoApplnUSBSetupCB (
        uint32_t setupdat0,
        uint32_t setupdat1
    )
{
    /* Fast enumeration is used. Only requests addressed to the interface, class,
     * vendor and unknown control requests are received by this function.
     * This application does not support any class or vendor requests. */

	// EP0 Buffer
    uint8_t  bRequest, bReqType;
    uint8_t  bType, bTarget;
    uint16_t wValue, wIndex, wLength;;
    CyBool_t isHandled = CyFalse;
	uint16_t readCount;
	//uint8_t buff[512];

    /* Decode the fields from the setup request. */
    bReqType = (setupdat0 & CY_U3P_USB_REQUEST_TYPE_MASK);
    bType    = (bReqType & CY_U3P_USB_TYPE_MASK);
    bTarget  = (bReqType & CY_U3P_USB_TARGET_MASK);
    bRequest = ((setupdat0 & CY_U3P_USB_REQUEST_MASK) >> CY_U3P_USB_REQUEST_POS);
    wValue   = ((setupdat0 & CY_U3P_USB_VALUE_MASK)   >> CY_U3P_USB_VALUE_POS);
    wIndex   = ((setupdat1 & CY_U3P_USB_INDEX_MASK)   >> CY_U3P_USB_INDEX_POS);
    wLength   = ((setupdat1 & CY_U3P_USB_LENGTH_MASK)   >> CY_U3P_USB_LENGTH_POS);

    if (bType == CY_U3P_USB_VENDOR_RQT)
    {
    	isHandled = CyTrue;
    	 switch (bRequest)
    	 {
			 case 0xAA:

				// Place Holder
				CyU3PUsbGetEP0Data( ((wLength + 15) & 0xFFF0), glEP0IN_buff, &readCount);

				//if (buff[0] == 0xF0) CommandDevice(buff, readCount);
				Command_Flag = CyTrue;
				break;

			 default:
	                /* This is unknown request. */
	                isHandled = CyFalse;
	            break;
    	 }

    }

    if (bType == CY_U3P_USB_STANDARD_RQT)
    {
        /* Handle SET_FEATURE(FUNCTION_SUSPEND) and CLEAR_FEATURE(FUNCTION_SUSPEND)
         * requests here. It should be allowed to pass if the device is in configured
         * state and failed otherwise. */
        if ((bTarget == CY_U3P_USB_TARGET_INTF) && ((bRequest == CY_U3P_USB_SC_SET_FEATURE)
                    || (bRequest == CY_U3P_USB_SC_CLEAR_FEATURE)) && (wValue == 0))
        {
            if (glIsApplnActive)
                CyU3PUsbAckSetup ();
            else
                CyU3PUsbStall (0, CyTrue, CyFalse);

            isHandled = CyTrue;
        }

        /* CLEAR_FEATURE request for endpoint is always passed to the setup callback
         * regardless of the enumeration model used. When a clear feature is received,
         * the previous transfer has to be flushed and cleaned up. This is done at the
         * protocol level. Since this is just a loopback operation, there is no higher
         * level protocol. So flush the EP memory and reset the DMA channel associated
         * with it. If there are more than one EP associated with the channel reset both
         * the EPs. The endpoint stall and toggle / sequence number is also expected to be
         * reset. Return CyFalse to make the library clear the stall and reset the endpoint
         * toggle. Or invoke the CyU3PUsbStall (ep, CyFalse, CyTrue) and return CyTrue.
         * Here we are clearing the stall. */
        if ((bTarget == CY_U3P_USB_TARGET_ENDPT) && (bRequest == CY_U3P_USB_SC_CLEAR_FEATURE)
                && (wValue == CY_U3P_USBX_FS_EP_HALT))
        {
            if (glIsApplnActive)
            {

				if (wIndex == CY_FX_EP_CONSUMER_2)
				{
					CyU3PDmaChannelReset (&glChHandleSlFifoPtoU);
					CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_2);
					CyU3PUsbResetEp (CY_FX_EP_CONSUMER_2);
					CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU, CY_FX_SLFIFO_DMA_RX_SIZE);
				}
				if (wIndex == CY_FX_EP_CONSUMER_3)
				{
				   CyU3PDmaChannelReset (&glChHandleSlFifoPtoU1);
				   CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_3);
				   CyU3PUsbResetEp (CY_FX_EP_CONSUMER_3);
				   CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU1, CY_FX_SLFIFO_DMA_RX_SIZE);
				}
				if (wIndex == CY_FX_EP_CONSUMER_4)
				{
					CyU3PDmaChannelReset (&glChHandleSlFifoPtoU2);
					CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_4);
					CyU3PUsbResetEp (CY_FX_EP_CONSUMER_4);
					CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU2, CY_FX_SLFIFO_DMA_RX_SIZE);
				}
				if (wIndex == CY_FX_EP_CONSUMER_5)
				{
				   CyU3PDmaChannelReset (&glChHandleSlFifoPtoU3);
				   CyU3PUsbFlushEp(CY_FX_EP_CONSUMER_5);
				   CyU3PUsbResetEp (CY_FX_EP_CONSUMER_5);
				   CyU3PDmaChannelSetXfer (&glChHandleSlFifoPtoU3, CY_FX_SLFIFO_DMA_RX_SIZE);
				}


                CyU3PUsbStall (wIndex, CyFalse, CyTrue);
                isHandled = CyTrue;
            }
        }
    }

    return isHandled;
}

/* This is the callback function to handle the USB events. */
void
CyFxSlFifoApplnUSBEventCB (
    CyU3PUsbEventType_t evtype,
    uint16_t            evdata
    )
{
    switch (evtype)
    {
        case CY_U3P_USB_EVENT_SETCONF:
            /* Stop the application before re-starting. */
            if (glIsApplnActive)
            {
                CyFxSlFifoApplnStop ();
            }
            /* Start the loop back function. */
            CyFxSlFifoApplnStart ();
            break;

        case CY_U3P_USB_EVENT_RESET:
        case CY_U3P_USB_EVENT_DISCONNECT:
            /* Stop the loop back function. */
            if (glIsApplnActive)
            {
                CyFxSlFifoApplnStop ();
            }
            break;

        default:
            break;
    }
}

/* Callback function to handle LPM requests from the USB 3.0 host. This function is invoked by the API
   whenever a state change from U0 -> U1 or U0 -> U2 happens. If we return CyTrue from this function, the
   FX3 device is retained in the low power state. If we return CyFalse, the FX3 device immediately tries
   to trigger an exit back to U0.

   This application does not have any state in which we should not allow U1/U2 transitions; and therefore
   the function always return CyTrue.
 */
CyBool_t
CyFxApplnLPMRqtCB (
        CyU3PUsbLinkPowerMode link_mode)
{
    return CyTrue;
}

/* I2c initialization for EEPROM programming. */
CyU3PReturnStatus_t
CyFxI2cInit (uint16_t pageLen)
{
    CyU3PI2cConfig_t i2cConfig;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;

    /* Initialize and configure the I2C master module. */
    status = CyU3PI2cInit ();
    if (status != CY_U3P_SUCCESS)
    {
        return status;
    }


    /* Start the I2C master block. The bit rate is set at 100KHz.
     * The data transfer is done via DMA. */
    CyU3PMemSet ((uint8_t *)&i2cConfig, 0, sizeof(i2cConfig));
    i2cConfig.bitRate    = CY_FX_USBI2C_I2C_BITRATE;
    i2cConfig.busTimeout = 0xFFFFFFFF;
    i2cConfig.dmaTimeout = 0xFFFF;
    i2cConfig.isDma      = CyFalse;

    status = CyU3PI2cSetConfig (&i2cConfig, NULL);
    if (status == CY_U3P_SUCCESS)
    {
        glI2cPageSize = pageLen;
    }

    return status;
}


/* This function initializes the GPIF interface and initializes
 * the USB interface. */
void
CyFxSlFifoApplnInit (void)
{
    CyU3PPibClock_t pibClock;
    CyU3PReturnStatus_t apiRetStatus = CY_U3P_SUCCESS;

    /* EDISON INIT */
    CyU3PMemSet((uint8_t*)&glEP1OUTBuffer, 0, sizeof(glEP1OUTBuffer));

    /* Initialize the p-port block. */
    pibClock.clkDiv = 2;
    pibClock.clkSrc = CY_U3P_SYS_CLK;
    pibClock.isHalfDiv = CyFalse;
    /* Disable DLL for sync GPIF */
    pibClock.isDllEnable = CyFalse;
    apiRetStatus = CyU3PPibInit(CyTrue, &pibClock);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "P-port Initialization failed, Error Code = %d\n",apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Load the GPIF configuration for Slave FIFO sync mode. */
    apiRetStatus = CyU3PGpifLoad (&Sync_Slave_Fifo_2Bit_CyFxGpifConfig);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PGpifLoad failed, Error Code = %d\n",apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    CyU3PGpifSocketConfigure (0,CY_U3P_PIB_SOCKET_0,6,CyFalse,1);  ////test 1

   CyU3PGpifSocketConfigure (3,CY_U3P_PIB_SOCKET_3,6,CyFalse,1);   //test 1

    /* Start the state machine. */
    apiRetStatus = CyU3PGpifSMStart (SYNC_SLAVE_FIFO_2BIT_RESET, SYNC_SLAVE_FIFO_2BIT_ALPHA_RESET);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PGpifSMStart failed, Error Code = %d\n",apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Initialize the I2C interface for the EEPROM of page size 64 bytes. */
    apiRetStatus = CyFxI2cInit (CY_FX_USBI2C_I2C_PAGE_SIZE);
     if (apiRetStatus != CY_U3P_SUCCESS)
     {
    	    CyU3PDebugPrint (4, "CyFxI2cInit failed, Error Code = %d\n",apiRetStatus);
    	    CyFxAppErrorHandler(apiRetStatus);
     }



    /* Start the USB functionality. */
    apiRetStatus = CyU3PUsbStart();
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "CyU3PUsbStart failed to Start, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* The fast enumeration is the easiest way to setup a USB connection,
     * where all enumeration phase is handled by the library. Only the
     * class / vendor requests need to be handled by the application. */
    CyU3PUsbRegisterSetupCallback(CyFxSlFifoApplnUSBSetupCB, CyTrue);

    /* Setup the callback to handle the USB events. */
    CyU3PUsbRegisterEventCallback(CyFxSlFifoApplnUSBEventCB);

    /* Register a callback to handle LPM requests from the USB 3.0 host. */
    CyU3PUsbRegisterLPMRequestCallback(CyFxApplnLPMRqtCB);    

    /* Set the USB Enumeration descriptors */

    /* Super speed device descriptor. */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_SS_DEVICE_DESCR, NULL, (uint8_t *)CyFxUSB30DeviceDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set device descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* High speed device descriptor. */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_HS_DEVICE_DESCR, NULL, (uint8_t *)CyFxUSB20DeviceDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set device descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* BOS descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_SS_BOS_DESCR, NULL, (uint8_t *)CyFxUSBBOSDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set configuration descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Device qualifier descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_DEVQUAL_DESCR, NULL, (uint8_t *)CyFxUSBDeviceQualDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set device qualifier descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Super speed configuration descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_SS_CONFIG_DESCR, NULL, (uint8_t *)CyFxUSBSSConfigDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set configuration descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* High speed configuration descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_HS_CONFIG_DESCR, NULL, (uint8_t *)CyFxUSBHSConfigDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB Set Other Speed Descriptor failed, Error Code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Full speed configuration descriptor */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_FS_CONFIG_DESCR, NULL, (uint8_t *)CyFxUSBFSConfigDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB Set Configuration Descriptor failed, Error Code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* String descriptor 0 */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_STRING_DESCR, 0, (uint8_t *)CyFxUSBStringLangIDDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set string descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* String descriptor 1 */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_STRING_DESCR, 1, (uint8_t *)CyFxUSBManufactureDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set string descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* String descriptor 2 */
    apiRetStatus = CyU3PUsbSetDesc(CY_U3P_USB_SET_STRING_DESCR, 2, (uint8_t *)CyFxUSBProductDscr);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB set string descriptor failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }

    /* Connect the USB Pins with super speed operation enabled. */
    apiRetStatus = CyU3PConnectState(CyTrue, CyTrue);
    if (apiRetStatus != CY_U3P_SUCCESS)
    {
        CyU3PDebugPrint (4, "USB Connect failed, Error code = %d\n", apiRetStatus);
        CyFxAppErrorHandler(apiRetStatus);
    }
}

/* Entry function for the slFifoAppThread. */
void
SlFifoAppThread_Entry (
        uint32_t input)
{
    /* Initialize the debug module */
    CyFxSlFifoApplnDebugInit();

    /* Initialize the slave FIFO application */
    CyFxSlFifoApplnInit();
    for(;;)
    {
    	if (Command_Flag == CyTrue) TD_PoLL();
    }
}

/* Application define function which creates the threads. */
void
CyFxApplicationDefine (
        void)
{
    void *ptr = NULL;
    uint32_t retThrdCreate = CY_U3P_SUCCESS;

    /* Allocate the memory for the thread */
    ptr = CyU3PMemAlloc (CY_FX_SLFIFO_THREAD_STACK);

    /* Create the thread for the application */
    retThrdCreate = CyU3PThreadCreate (&slFifoAppThread,           /* Slave FIFO app thread structure */
                          "21:Slave_FIFO_sync",                    /* Thread ID and thread name */
                          SlFifoAppThread_Entry,                   /* Slave FIFO app thread entry function */
                          0,                                       /* No input parameter to thread */
                          ptr,                                     /* Pointer to the allocated thread stack */
                          CY_FX_SLFIFO_THREAD_STACK,               /* App Thread stack size */
                          CY_FX_SLFIFO_THREAD_PRIORITY,            /* App Thread priority */
                          CY_FX_SLFIFO_THREAD_PRIORITY,            /* App Thread pre-emption threshold */
                          CYU3P_NO_TIME_SLICE,                     /* No time slice for the application thread */
                          CYU3P_AUTO_START                         /* Start the thread immediately */
                          );

    /* Check the return code */
    if (retThrdCreate != 0)
    {
        /* Thread Creation failed with the error code retThrdCreate */

        /* Add custom recovery or debug actions here */

        /* Application cannot continue */
        /* Loop indefinitely */
        while(1);
    }
}

/*
 * Main function
 */
int
main (void)
{
    CyU3PIoMatrixConfig_t io_cfg;
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
    CyU3PSysClockConfig_t clkCfg;		//test 1

        /* setSysClk400 clock configurations */   //test 1
        clkCfg.setSysClk400 = CyTrue;   /* FX3 device's master clock is set to a frequency > 400 MHz */
        clkCfg.cpuClkDiv = 2;           /* CPU clock divider */
        clkCfg.dmaClkDiv = 2;           /* DMA clock divider */
        clkCfg.mmioClkDiv = 2;          /* MMIO clock divider */
        clkCfg.useStandbyClk = CyFalse; /* device has no 32KHz clock supplied */
        clkCfg.clkSrc = CY_U3P_SYS_CLK; /* Clock source for a peripheral block  */

    /* Initialize the device */
    status = CyU3PDeviceInit (&clkCfg);	//status = CyU3PDeviceInit (NULL);  //test 1
    if (status != CY_U3P_SUCCESS)
    {
        goto handle_fatal_error;
    }

    /* Initialize the caches. Enable instruction cache and keep data cache disabled.
     * The data cache is useful only when there is a large amount of CPU based memory
     * accesses. When used in simple cases, it can decrease performance due to large 
     * number of cache flushes and cleans and also it adds to the complexity of the
     * code. */
    status = CyU3PDeviceCacheControl (CyTrue, CyFalse, CyFalse);  // status = CyU3PDeviceCacheControl (CyTrue, CyTrue, CyTrue);  //test 1
    if (status != CY_U3P_SUCCESS)
    {
        goto handle_fatal_error;
    }

    /* Configure the IO matrix for the device. On the FX3 DVK board, the COM port 
     * is connected to the IO(53:56). This means that either DQ32 mode should be
     * selected or lppMode should be set to UART_ONLY. Here we are choosing
     * UART_ONLY configuration for 16 bit slave FIFO configuration and setting
     * isDQ32Bit for 32-bit slave FIFO configuration. */
    io_cfg.useUart   = CyTrue;
    io_cfg.useI2C    = CyTrue;
    io_cfg.useI2S    = CyFalse;
    io_cfg.useSpi    = CyFalse;

#if (CY_FX_SLFIFO_GPIF_16_32BIT_CONF_SELECT == 0)
    io_cfg.isDQ32Bit = CyFalse;
    io_cfg.lppMode   = CY_U3P_IO_MATRIX_LPP_UART_ONLY;
#else
    io_cfg.isDQ32Bit = CyTrue;
    io_cfg.lppMode   = CY_U3P_IO_MATRIX_LPP_DEFAULT;
#endif
    /* No GPIOs are enabled. */
    io_cfg.gpioSimpleEn[0]  = 0;
    io_cfg.gpioSimpleEn[1]  = 0;
    io_cfg.gpioComplexEn[0] = 0;
    io_cfg.gpioComplexEn[1] = 0;
    status = CyU3PDeviceConfigureIOMatrix (&io_cfg);
    if (status != CY_U3P_SUCCESS)
    {
        goto handle_fatal_error;
    }

    /* This is a non returnable call for initializing the RTOS kernel */
    CyU3PKernelEntry ();

    /* Dummy return to make the compiler happy */
    return 0;

handle_fatal_error:

    /* Cannot recover from this error. */
    while (1);
}


void CommandDevice(uint8_t *inbuff, uint16_t size)
{
	uint8_t tmp = CyTrue;
	uint16_t Offset;
	CyU3PReturnStatus_t ret = CY_U3P_SUCCESS;

	glEP1OUTBuffer.cmd = inbuff[1];
	glEP1OUTBuffer.size = 0;

	switch(inbuff[1])
	{
		case 0x70:
			glEP1OUTBuffer.data[0] = 1;
			glEP1OUTBuffer.data[1] = 2;
			glEP1OUTBuffer.data[2] = 3;

			glEP1OUTBuffer.size = 3 ;
			tmp = CyTrue;
			break;

		case 0xA0:	/// IIC Read Command
			Offset = (inbuff[3] <<8) + inbuff[4] ;
			if (inbuff[5] == 0 ) inbuff[5] = 1; // Minimum read count number
			if (inbuff[2] == 0){
				CyFxUsbI2cTransfer (Offset, EEPROM_ID, inbuff[5], glEP1OUTBuffer.data, CyTrue);
				glEP1OUTBuffer.size = inbuff[5];
				tmp = CyTrue;
			}else if(inbuff[2] == 1){
				CyFxUsbI2cTransfer (Offset, FPGA_ID, inbuff[5], glEP1OUTBuffer.data, CyTrue);
				glEP1OUTBuffer.size = inbuff[5];
				tmp = CyTrue;
			}else {
				tmp = CyFalse;
			}
			break;

		case 0xA1:  /// IIC Write Command
			Offset = (inbuff[3] <<8) + inbuff[4];
			if (inbuff[5] == 0 ) inbuff[5] = 1; // Minimum read count number
			if (inbuff[2] == 0){
				CyFxUsbI2cTransfer (Offset, EEPROM_ID, inbuff[5], (inbuff+6), CyFalse);
				tmp = CyTrue;
			}else if(inbuff[2] == 1){
				tmp = CyTrue;
			}else {
				tmp = CyFalse;
			}
			break;
		case 0xA2:
			break;

		default :
			tmp = CyFalse;
			break;
	}


	if (tmp == CyTrue)
		glEP1OUTBuffer.ret = 0xF5;
	else
		glEP1OUTBuffer.ret = 0xF0;


	ret = SendHost();
	CyU3PDebugPrint (4, "SendHost return : %d line, ret = %d\n", __LINE__, ret);
	if(ret != CY_U3P_SUCCESS)
	{
		CyU3PDebugPrint (4, "SendHost failure : %d line, Error code = %d\n", __LINE__, ret);
	}

}
CyU3PReturnStatus_t SendHost()
{
    // EP1 Return
    CyU3PDmaBuffer_t outBuf_p;
	CyU3PReturnStatus_t status = CY_U3P_SUCCESS;


	/* Wait for a free buffer to transmit the received data. The failure cases are same as above. */
	status = CyU3PDmaChannelGetBuffer (&glChHandleBulkLpOut, &outBuf_p, CYU3P_WAIT_FOREVER);
	if (status != CY_U3P_SUCCESS)
	{
			CyU3PDebugPrint (4, "CyU3PDmaChannelGetBuffer : %d line, Error code = %d\n", __LINE__, status);
			CyFxAppErrorHandler(status);
	}

	/* Commit the received data to the consumer pipe so that the data can be
	 * transmitted back to the USB host. Since the same data is sent back, the
	 * count shall be same as received and the status field of the call shall
	 * be 0 for default use case. */

	glEP1OUTBuffer.header = 0xF5;
	outBuf_p.count = 4/*fix*/ + glEP1OUTBuffer.size;

	CyU3PMemCopy( outBuf_p.buffer, (uint8_t*)&glEP1OUTBuffer, outBuf_p.count);
	status = CyU3PDmaChannelCommitBuffer (&glChHandleBulkLpOut, outBuf_p.count, 0);

	CyU3PMemSet((uint8_t*)&glEP1OUTBuffer, 0, sizeof(glEP1OUTBuffer)); // EP1out buffer init;


	return status;

}

/* I2C read / write for programmer application. */
CyU3PReturnStatus_t
CyFxUsbI2cTransfer (
        uint16_t  byteAddress,
        uint8_t   devAddr,
        uint16_t  byteCount,
        uint8_t  *buffer,
        CyBool_t  isRead)
{
    CyU3PI2cPreamble_t preamble;
    uint16_t pageCount = (byteCount / glI2cPageSize);
    CyU3PReturnStatus_t status = CY_U3P_SUCCESS;
    uint16_t resCount = glI2cPageSize;

    if (byteCount == 0)
    {
        return CY_U3P_SUCCESS;
    }

    if ((byteCount % glI2cPageSize) != 0)
    {
        pageCount ++;
        resCount = byteCount % glI2cPageSize;
    }

    while (pageCount != 0)
    {


        if (isRead)
        {


            /* Update the preamble information. */
            preamble.length    = 4;
            preamble.buffer[0] = devAddr;
            preamble.buffer[1] = (uint8_t)(byteAddress >> 8);
            preamble.buffer[2] = (uint8_t)(byteAddress & 0xFF);
            preamble.buffer[3] = (devAddr | 0x01);
            preamble.ctrlMask  = 0x0004;


            status = CyU3PI2cReceiveBytes (&preamble, buffer, (pageCount == 1) ? resCount : glI2cPageSize, 0);
            if (status != CY_U3P_SUCCESS)
            {
                return status;
            }
            CyU3PDebugPrint (4, "CyU3PI2cReceiveBytes : %d line, read = %d\n", __LINE__, status);
        }
        else /* Write */
        {
            /* Update the preamble information. */
            preamble.length    = 3;
            preamble.buffer[0] = devAddr;
            preamble.buffer[1] = (uint8_t)(byteAddress >> 8);
            preamble.buffer[2] = (uint8_t)(byteAddress & 0xFF);
            preamble.ctrlMask  = 0x0000;

            status = CyU3PI2cTransmitBytes (&preamble, buffer, (pageCount == 1) ? resCount : glI2cPageSize, 0);
            if (status != CY_U3P_SUCCESS)
            {
                return status;
            }


            /* Wait for the write to complete. */
            preamble.length = 1;
            status = CyU3PI2cWaitForAck(&preamble, 200);
            if (status != CY_U3P_SUCCESS)
            {
                return status;
            }
        }

        /* An additional delay seems to be required after receiving an ACK. */
        CyU3PThreadSleep (1);

        /* Update the parameters */
        byteAddress  += glI2cPageSize;
        buffer += glI2cPageSize;
        pageCount --;
    }

    return CY_U3P_SUCCESS;
}
void TD_PoLL()
{
	Command_Flag = CyFalse;
	if (glEP0IN_buff[0] == 0xF0) CommandDevice(glEP0IN_buff, 64);

}
