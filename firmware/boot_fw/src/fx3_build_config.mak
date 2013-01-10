## Copyright Cypress Semiconductor Corporation, 2011-2012,
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

# the common include path
Include	=-I. -I../include 


# the common compiler options
CCFLAGS	= -g -O3 $(Include)

# the common linker options
LDFLAGS	= --entry Reset_Handler $(LDLIBS)

# the required assembly files

LDLIBS = ../lib/cyfx3_boot.a

# now include the compile specific build options
# arm compiler is default
ifeq ($(CYFXBUILD), gcc)
	include fx3_armgcc_config.mak
endif	
ifeq ($(CYFXBUILD), arm)
	include fx3_armrvds_config.mak
endif	
ifeq ($(CYFXBUILD),)
	include fx3_armrvds_config.mak
endif

#[]
