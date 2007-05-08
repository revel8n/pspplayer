// ----------------------------------------------------------------------------
// PSP Player Emulation Suite
// Copyright (C) 2006 Ben Vanik (noxa)
// Licensed under the LGPL - see License.txt in the project root for details
// ----------------------------------------------------------------------------

#include "Stdafx.h"
#define WIN32_LEAN_AND_MEAN
#include <Windows.h>

#include "Kernel_Library.h"
#include "Kernel.h"
#include "CpuApi.h"

using namespace System;
using namespace System::Diagnostics;
using namespace Noxa::Emulation::Psp;
using namespace Noxa::Emulation::Psp::Bios;
using namespace Noxa::Emulation::Psp::Bios::Modules;
using namespace Noxa::Emulation::Psp::Cpu;

void* Kernel_Library::QueryNativePointer( uint nid )
{
	//switch( nid )
	//{
	//case 0x092968F4:
	//case 0x5F10D406:
	//case 0x3B84732D:
	//case 0x47A0B729:
	//case 0xB55249D2:
	//}
	return 0;
}

// unsigned int sceKernelCpuSuspendIntr(); (/user/pspintrman.h:77)
int Kernel_Library::sceKernelCpuSuspendIntr()
{
	if( _kernel->Cpu == NULL )
	{
		ICpuCore^ core0 = _kernel->CpuCore;
		int flags = core0->InterruptsFlag;
		core0->InterruptsFlag = 0;
		return flags;
	}
	else
	{
		return _kernel->Cpu->SetInterruptState( 0 );
	}
}

// void sceKernelCpuResumeIntr(unsigned int flags); (/user/pspintrman.h:84)
void Kernel_Library::sceKernelCpuResumeIntr( int flags )
{
	if( _kernel->Cpu == NULL )
	{
		ICpuCore^ core0 = _kernel->CpuCore;
		core0->InterruptsFlag = flags;
	}
	else
	{
		_kernel->Cpu->SetInterruptState( flags );
	}
}

// void sceKernelCpuResumeIntrWithSync(unsigned int flags); (/user/pspintrman.h:91)
void Kernel_Library::sceKernelCpuResumeIntrWithSync( int flags )
{
	if( _kernel->Cpu == NULL )
	{
		ICpuCore^ core0 = _kernel->CpuCore;
		core0->InterruptsFlag = flags;
	}
	else
	{
		_kernel->Cpu->SetInterruptState( flags );
	}
}

// int sceKernelIsCpuIntrSuspended(unsigned int flags); (/user/pspintrman.h:100)
int Kernel_Library::sceKernelIsCpuIntrSuspended( int flags )
{
	if( flags == 0x0 )
		return 1;
	else
		return 0;
}

// int sceKernelIsCpuIntrEnable(); (/user/pspintrman.h:107)
int Kernel_Library::sceKernelIsCpuIntrEnable()
{
	if( _kernel->Cpu == NULL )
	{
		ICpuCore^ core0 = _kernel->CpuCore;
		return ( core0->InterruptsFlag != 0 );
	}
	else
	{
		return _kernel->Cpu->GetInterruptState();
	}
}