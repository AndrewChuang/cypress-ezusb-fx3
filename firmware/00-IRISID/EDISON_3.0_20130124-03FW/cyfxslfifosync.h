/*
 ## Cypress USB 3.0 Platform header file (cyfxslfifosync.h)
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

/* This file contains the constants and definitions used by the Slave FIFO application example */

#ifndef _INCLUDED_CYFXSLFIFOASYNC_H_
#define _INCLUDED_CYFXSLFIFOASYNC_H_

#include "cyu3externcstart.h"
#include "cyu3types.h"
#include "cyu3usbconst.h"

/* 16/32 bit GPIF Configuration select */
/* Set CY_FX_SLFIFO_GPIF_16_32BIT_CONF_SELECT = 0 for 16 bit GPIF data bus.
 * Set CY_FX_SLFIFO_GPIF_16_32BIT_CONF_SELECT = 1 for 32 bit GPIF data bus.
 */
#define CY_FX_SLFIFO_GPIF_16_32BIT_CONF_SELECT (1)

#define CY_FX_SLFIFO_DMA_BUF_COUNT      (2)                     /* Slave FIFO channel buffer count */
#define CY_FX_SLFIFO_DMA_TX_SIZE        (0)	                  	/* DMA transfer size is set to infinite */
#define CY_FX_SLFIFO_DMA_RX_SIZE        (0)	                  	/* DMA transfer size is set to infinite */
#define CY_FX_SLFIFO_THREAD_STACK       (0x0400)                /* Slave FIFO application thread stack size */
#define CY_FX_SLFIFO_THREAD_PRIORITY    (8)                     /* Slave FIFO application thread priority */

#define CY_FX_BULKLP_DMA_BUF_COUNT      (8)                     /* Bulk loop channel buffer count */
#define CY_FX_BULKLP_DMA_TX_SIZE        (0)                     /* DMA transfer size is set to infinite */

#define CY_FX_UARTLP_THREAD_STACK       (0x0400) 				/* UART application thread stack size */
#define CY_FX_UARTLP_THREAD_PRIORITY    (8)      				/* UART application thread priority */

/* Endpoint and socket definitions for the Slave FIFO application */

/* To change the Producer and Consumer EP enter the appropriate EP numbers for the #defines.
 * In the case of IN endpoints enter EP number along with the direction bit.
 * For eg. EP 6 IN endpoint is 0x86
 *     and EP 6 OUT endpoint is 0x06.
 * To change sockets mention the appropriate socket number in the #defines. */

/* Note: For USB 2.0 the endpoints and corresponding sockets are one-to-one mapped
         i.e. EP 1 is mapped to UIB socket 1 and EP 2 to socket 2 so on */

#define CY_FX_EP_PRODUCER_1               0x01    /* EP 1 OUT */
#define CY_FX_EP_PRODUCER_2               0x02    /* EP 2 OUT */

#define CY_FX_EP_CONSUMER_1               0x81    /* EP 1 IN */
#define CY_FX_EP_CONSUMER_2               0x82    /* EP 2 IN */
#define CY_FX_EP_CONSUMER_3               0x83    /* EP 1 IN */
#define CY_FX_EP_CONSUMER_4               0x84    /* EP 2 IN */
#define CY_FX_EP_CONSUMER_5               0x85    /* EP 1 IN */


#define CY_FX_CONSUMER_USB_SOCKET_1    CY_U3P_UIB_SOCKET_CONS_1    /* USB Socket 1 is consumer */
#define CY_FX_CONSUMER_2_USB_SOCKET    CY_U3P_UIB_SOCKET_CONS_2    /* USB Socket 1 is consumer */
#define CY_FX_CONSUMER_3_USB_SOCKET    CY_U3P_UIB_SOCKET_CONS_3    /* USB Socket 1 is consumer */
#define CY_FX_CONSUMER_4_USB_SOCKET    CY_U3P_UIB_SOCKET_CONS_4    /* USB Socket 1 is consumer */
#define CY_FX_CONSUMER_5_USB_SOCKET    CY_U3P_UIB_SOCKET_CONS_5    /* USB Socket 1 is consumer */

/* Used with FX3 Silicon. */
#define CY_FX_PRODUCER_2_PPORT_SOCKET    CY_U3P_PIB_SOCKET_0    /* P-port Socket 0 is producer */
#define CY_FX_PRODUCER_3_PPORT_SOCKET    CY_U3P_PIB_SOCKET_1    /* P-port Socket 0 is producer */
#define CY_FX_PRODUCER_4_PPORT_SOCKET    CY_U3P_PIB_SOCKET_2    /* P-port Socket 0 is producer */
#define CY_FX_PRODUCER_5_PPORT_SOCKET    CY_U3P_PIB_SOCKET_3    /* P-port Socket 0 is producer */


///////////////////////////////////////////////////////
//////////////////// 	I2C  //////////////////////////
///////////////////////////////////////////////////////
/* I2C Data rate */
#define CY_FX_USBI2C_I2C_BITRATE        (100000)

/* The following constant is defined based on the page size that the I2C
 * device support. 24LC256 support 64 byte page write access. */
#define CY_FX_USBI2C_I2C_PAGE_SIZE      (64)
/* Device ID */
#define EEPROM_ID 	0xA8
#define FPGA_ID		0xB0

#ifdef USB_2_0
#define BURST_LEN 1  //for USB2.0
#else
#define BURST_LEN 16 //for USB3.0
#endif

#define DMA_BUF_SIZE					(8)	 //(16)
/* Slave FIFO P_2_U channel buffer count */
#define CY_FX_SLFIFO_DMA_BUF_COUNT_P_2_U      (4)
/* Slave FIFO U_2_P channel buffer count */
#define CY_FX_SLFIFO_DMA_BUF_COUNT_U_2_P 	  (8)


/* Extern definitions for the USB Descriptors */
extern const uint8_t CyFxUSB20DeviceDscr[];
extern const uint8_t CyFxUSB30DeviceDscr[];
extern const uint8_t CyFxUSBDeviceQualDscr[];
extern const uint8_t CyFxUSBFSConfigDscr[];
extern const uint8_t CyFxUSBHSConfigDscr[];
extern const uint8_t CyFxUSBBOSDscr[];
extern const uint8_t CyFxUSBSSConfigDscr[];
extern const uint8_t CyFxUSBStringLangIDDscr[];
extern const uint8_t CyFxUSBManufactureDscr[];
extern const uint8_t CyFxUSBProductDscr[];

#include "cyu3externcend.h"

#endif /* _INCLUDED_CYFXSLFIFOASYNC_H_ */

/*[]*/
