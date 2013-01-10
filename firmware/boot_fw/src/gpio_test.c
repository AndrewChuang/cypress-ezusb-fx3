#include <cyfx3gpio.h>

CyFx3BootErrorCode_t
testGpio ()
{
    CyFx3BootErrorCode_t status;
    CyBool_t temp = CyFalse;

    CyFx3BootGpioSimpleConfig_t gpioCfg;

    CyFx3BootGpioInit ();

    /* Output mode. */
    gpioCfg.outValue    = CyTrue;
    gpioCfg.driveLowEn  = CyTrue;
    gpioCfg.driveHighEn = CyTrue;
    gpioCfg.inputEn     = CyFalse;
    gpioCfg.intrMode    = CY_FX3_BOOT_GPIO_NO_INTR;
    status = CyFx3BootGpioSetSimpleConfig (45, &gpioCfg);

    if (status != CY_FX3_BOOT_SUCCESS)
    {
        return status;
    }

    /* Input mode. */
    gpioCfg.outValue    = CyTrue;
    gpioCfg.driveLowEn  = CyFalse;
    gpioCfg.driveHighEn = CyFalse;
    gpioCfg.inputEn     = CyTrue;
    gpioCfg.intrMode    = CY_FX3_BOOT_GPIO_NO_INTR;
    status = CyFx3BootGpioSetSimpleConfig (57, &gpioCfg);

    if (status != CY_FX3_BOOT_SUCCESS)
    {
        return status;
    }

    /* Set the value to low */
    status = CyFx3BootGpioSetValue (45, CyFalse);

    if (status != CY_FX3_BOOT_SUCCESS)
    {
        return status;
    }

    /* Set the value to high */
    status = CyFx3BootGpioSetValue (45, CyTrue);
    if (status != CY_FX3_BOOT_SUCCESS)
    {
        return status;
    }

    status = CyFx3BootGpioGetValue (57, &temp);
    return status;
}
