## Copyright Cypress Semiconductor Corporation, 2010-2011,
## All Rights Reserved
## UNPUBLISHED, LICENSED SOFTWARE.
##
## CONFIDENTIAL AND PROPRIETARY INFORMATION 
## WHICH IS THE PROPERTY OF CYPRESS.
##
## Use of this file is governed 
## by the license agreement included in the file 
##
##	<install>/license/license.txt
##
## where <install> is the Cypress software
## installation root directory path.
##

# how to build, select the toolchain and include the correct files
# NOTE: the FX3FWROOT has to be defined in each makefile before
# this file is included !

# the common include path
Include	=-I. \
		-I$(FX3PFWROOT)/inc

# Default option for Config is fx3_debug
ifeq ($(CYCONFOPT),)
        CYCONFOPT=fx3_debug
endif

# the common compiler options
CCFLAGS	= -g -O0		\
		-DTX_ENABLE_EVENT_TRACE -DDEBUG -DCYU3P_FX3=1	\
		-D__CYU3P_TX__=1 $(Include)

# If the build target is FPGA, make sure that CYU3P_FPGA is defined as part of CCFLAGS
ifeq ($(CYCONFOPT), fx3_fpga_debug)
	CCFLAGS += -DCYU3P_FPGA
endif

# the common linker options
LDFLAGS	= --entry CyU3PFirmwareEntry $(LDLIBS)

# the required assembly files

ifeq ($(CYFXBUILD), gcc)
	SOURCE_ASM=$(FX3FWROOT)/common/cyfx_gcc_startup.S
endif
ifeq ($(CYFXBUILD), g++)
	SOURCE_ASM=$(FX3FWROOT)/common/cyfx_gcc_startup.S
endif
ifeq ($(CYFXBUILD), arm)
	SOURCE_ASM= \
		$(FX3FWROOT)/common/cyfx_startup.S
endif		
ifeq ($(CYFXBUILD),)
	SOURCE_ASM=$(FX3FWROOT)/common/cyfx_gcc_startup.S
endif

# the common source file for all builds
ifeq ($(CYFXBUILD), g++)
SOURCE = $(FX3FWROOT)/common/cyfxtx.cpp
SOURCE += $(FX3FWROOT)/common/cyfxcppsyscall.cpp
else
SOURCE = $(FX3FWROOT)/common/cyfxtx.c
endif

# the common libraries
# NOTE: This order is important for GNU linker. Do not change
LDLIBS = $(FX3PFWROOT)/lib/$(CYCONFOPT)/cyfxapi.a \
	$(FX3PFWROOT)/lib/$(CYCONFOPT)/cyu3lpp.a \
	$(FX3PFWROOT)/lib/$(CYCONFOPT)/cyu3threadx.a

# now include the compile specific build options
# arm compiler is default
ifeq ($(CYFXBUILD), gcc)
	include $(FX3FWROOT)/common/fx3_armgcc_config.mak
endif
ifeq ($(CYFXBUILD), g++)
	include $(FX3FWROOT)/common/fx3_armg++_config.mak
endif
ifeq ($(CYFXBUILD), arm)
	include $(FX3FWROOT)/common/fx3_armrvds_config.mak
endif	
ifeq ($(CYFXBUILD),)
	include $(FX3FWROOT)/common/fx3_armgcc_config.mak
endif

#[]
