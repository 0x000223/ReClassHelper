#pragma once
#include <ntdef.h>
#include <ntifs.h>
#include <windef.h>

extern "C"
{
	NTKERNELAPI NTSTATUS NTAPI MmCopyVirtualMemory
    (
        IN PEPROCESS FromProcess,
        IN PVOID FromAddress,
        IN PEPROCESS ToProcess,
        OUT PVOID ToAddress,
        IN SIZE_T BufferSize,
        IN KPROCESSOR_MODE PreviousMode,
        OUT PSIZE_T NumberOfBytesCopied
    );

    NTKERNELAPI PPEB NTAPI PsGetProcessPeb
    (   
        IN PEPROCESS Process
    );

    NTKERNELAPI PVOID NTAPI	PsGetProcessWow64Process
	(
		IN PEPROCESS Process
	);
}
