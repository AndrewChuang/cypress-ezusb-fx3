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
  The last 32KB (0x40078000 to 0x40080000 is reserved for Fx3 Boot loader.
  Descriptor area Base: 0x40000000 Size: 8KB
  Code area       Base: 0x40078000 Size: 24KB
  Data area       Base: 0x4007E000 Size: 4KB
  Tmp Buffer area Base: 0x4007F000 Size: 4KB

  The first 256 bytes of ITCM are reserved for Exception Vectors.
  These vectors will be loaded to ITCM 0x0 as part of the Device Init.

*/

ENTRY(__vectors);


MEMORY
{
	SYS_MEM		: ORIGIN = 0x40078000	LENGTH = 0x7000
	DATA		: ORIGIN = 0x4007F000	LENGTH = 0x1000
}

SECTIONS
{
	. = 0x40078000;
	.text :
	{
		*(RESET)
		*(.text)
		*(.rodata)
		*(.constdata)
		*(.emb_text)
		*(CYU3P_EXCEPTION_VECTORS);
		 _etext = .;
	} > SYS_MEM

	. = 0x4007F000;
	.data :
	{
		_data = .;
		*(.data)
		*(+RW, +ZI)
		*(Heap)
		*(ISR_Stacks)
		*(Stacks)
		_edata = .;
	} > DATA

	.bss :
	{
		_bss_start = .;
		*(.bss)
	} >DATA 
	. = ALIGN(4);
	_bss_end = . ;

	.ARM.extab   : { *(.ARM.extab* .gnu.linkonce.armextab.*) }
		 __exidx_start = .;
	.ARM.exidx   : { *(.ARM.exidx* .gnu.linkonce.armexidx.*) }
		 __exidx_end = .;

}
