/*
   Cypress USB 3.0 Platform linker script file (fx3.ld)
 
   Copyright Cypress Semiconductor Corporation, 2010-2011,
   All Rights Reserved
   UNPUBLISHED, LICENSED SOFTWARE.

   CONFIDENTIAL AND PROPRIETARY INFORMATION
   WHICH IS THE PROPERTY OF CYPRESS.

   Use of this file is governed
   by the license agreement included in the file
 
      <install>/license/license.txt

   where <install> is the Cypress software
   installation root directory path.
*/

/*
   This is the GNU linker file for FX3 memory map.
   The FX3 device has 16 KB of I-TCM memory which can be used for
   code (typically ISRs) and 512 KB of SYSTEM RAM which is shared
   between code, data and DMA buffers.

   The default memory map used for FX3 applications is as follows:

   Descriptor area Base: 0x40000000 Size: 12KB
   Code area       Base: 0x40003000 Size: 256KB
   Data area       Base: 0x40033000 Size: 20KB
   Compiler heap   Base: 0x40050000 Size: 32KB
   Driver heap     Base: 0x40058000 Size: 32KB  (Update cyfxtx.c to change this.)
   Buffer area     Base: 0x40060000 Size: 128KB (Update cyfxtx.c to change this.)

   Interrupt handlers to be placed in I-TCM (16KB).
   The first 256 bytes of ITCM are reserved for Exception Vectors.
   These vectors will be loaded to ITCM at address 0x0 as part of
   device initialization.

   Kernel stacks are to be placed in the D-TCM (8KB).
   This is done internal to the library as part of CyU3PFirmwareEntry()
   and is not expected to be modified by the FX3 application.
   SYS_STACK       Base: 0x10000000 Size 2KB
   ABT_STACK       Base: 0x10000800 Size 256B
   UND_STACK       Base: 0x10000900 Size 256B
   FIQ_STACK       Base: 0x10000A00 Size 512B
   IRQ_STACK       Base: 0x10000C00 Size 1KB
   SVC_STACK       Base: 0x10001000 Size 4KB
*/

ENTRY(CyU3PFirmwareEntry);

MEMORY
{
	I-TCM		: ORIGIN = 0x100,	LENGTH = 0x3F00
	SYS_MEM	: ORIGIN = 0x40003000	LENGTH = 0x40000
	DATA		: ORIGIN = 0x40043000	LENGTH = 0x5000
	ARM		: ORIGIN = 0x40048000	LENGTH = 0x8000
}

SECTIONS
{
	. = 0x100;
	.vectors :
	{
		*(CYU3P_ITCM_SECTION);
	} >I-TCM

	. = 0x40003000;
	.text :
	{
		*(.text)
		*(.rodata)
		*(.constdata)
		*(.emb_text)
		*(CYU3P_EXCEPTION_VECTORS);
		 _etext = .;
	} > SYS_MEM

	. = 0x40043000;
	.data :
	{
		_data = .;
		*(.data)
		* (+RW, +ZI)
		_edata = .;
	} > DATA

	.bss :
	{
		_bss_start = .;
		*(.bss)
	} >DATA 
	. = ALIGN(4);
	_bss_end = . ;

	.ARM.extab   : 
        { 
            *(.ARM.extab* .gnu.linkonce.armextab.*) 
        } > ARM
	. = ALIGN(4);

        __exidx_start = .;
        PROVIDE(__exidx_start = __exidx_start);
	.ARM.exidx   : 
        { 
            *(.ARM.exidx* .gnu.linkonce.armexidx.*) 
        } >ARM
	
        . = ALIGN(4);
        __exidx_end = .;

        PROVIDE(__exidx_end = __exidx_end);
        
        . = ALIGN(4);
        __heap_start = 0x40050000;
        PROVIDE(__heap_start = __heap_start);
        
        . = ALIGN(4);
        __heap_end = 0x40058000;
        PROVIDE(__heap_end = __heap_end);
	
	PROVIDE(__heap_size = __heap_end - __heap_start);
}
