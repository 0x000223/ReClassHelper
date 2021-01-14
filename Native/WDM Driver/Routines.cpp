#include "Routines.h"


/// <summary>
/// Read from target process memory address
/// </summary>
/// <param name="pReadMemory"></param>
/// <returns></returns>
NTSTATUS ReadMemory(_In_ PREAD_REQUEST pReadMemory)
{
    KdPrint(("[#] ReClassHelper : Invoked ReadMemory()\n"));
	
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS peTargetProcess = NULL;

    // Returns pointer to EPROCESS
    status = PsLookupProcessByProcessId((HANDLE)pReadMemory->ProcessId, &peTargetProcess);
    if (status == STATUS_INVALID_CID)
    {
        KdPrint(("Kernel Helper: PsLookupProcessByProcessId failed\n"));
        return STATUS_CANCELLED;
    }

    // Probe for read
    __try
    {
        ProbeForRead(pReadMemory->Address, pReadMemory->Size, 1);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KdPrint(("Kernel Helper: ProbeForRead has failed with exception code %X\n", GetExceptionCode()));
        return STATUS_CANCELLED;
    }

    // Probe to write
    __try
    {
        ProbeForWrite(pReadMemory->Buffer, pReadMemory->Size, 1);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KdPrint(("Kernel Helper: ProbeForRead has failed with exception code %X\n", GetExceptionCode()));
        return STATUS_CANCELLED;
    }

    // Read memory
    __try 
    {
        SIZE_T bytesCopied = 0;

        // Copy memory data from targetAddress to readRequest struct
        status = MmCopyVirtualMemory(peTargetProcess, pReadMemory->Address, PsGetCurrentProcess(),
            pReadMemory->Buffer, pReadMemory->Size, KernelMode, &bytesCopied);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KdPrint(("Kernel Helper: MmCopyVirtualMemory failed and raised exception code %X\n", GetExceptionCode()));
        return STATUS_CANCELLED;
    }

    return status;
}

/// <summary>
/// Write to a target process memory address
/// </summary>
/// <param name="pWriteMemory"></param>
/// <returns></returns>
NTSTATUS WriteMemory(_In_ PWRITE_REQUEST pWriteMemory)
{
	KdPrint(("[#] ReClassHelper : Invoked WriteMemory()\n"));
	
	NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS pEprocess = NULL;

    // Returns pointer to EPROCESS
    status = PsLookupProcessByProcessId((HANDLE)pWriteMemory->ProcessId, &pEprocess);
    if (status == STATUS_INVALID_CID)
    {
        DbgPrint("Kernel Helper: PsLookupProcessByProcessId failed: Invalid Process Id!\n");
        DbgPrint("Kernel Helper: ProcessId = %d", pWriteMemory->ProcessId);
        return STATUS_CANCELLED;
    }

    // Write to target address
    __try
    {
        SIZE_T bytesCopied = 0;
        status = MmCopyVirtualMemory(PsGetCurrentProcess(), pWriteMemory->Buffer, pEprocess, pWriteMemory->Address, pWriteMemory->Size, KernelMode, &bytesCopied);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        DbgPrint("Kernel Helper: MmCopyVirtualMemory has failed %X\n", GetExceptionCode());
        DbgPrint("Address = %p, ProcessId = %d, Size = %d, Buffer = %p", pWriteMemory->Address, pWriteMemory->ProcessId, pWriteMemory->Size, pWriteMemory->Buffer);
        return STATUS_CANCELLED;
    }

    DbgPrint("Address = %p, ProcessId = %d, Size = %d, Buffer = %p", pWriteMemory->Address, pWriteMemory->ProcessId, pWriteMemory->Size, pWriteMemory->Buffer);

    return status;
}

/// <summary>
/// 
/// </summary>
/// <param name="pProcessInfo"></param>
/// <returns></returns>
NTSTATUS GetProcessInfo(_Out_ PPROCESS_INFO pProcessInfo)
{
    KdPrint(("[#] ReClassHelper : Invoked GetProcessInfo()\n"));

    DbgPrint("[#] ReClassHelper : GetProcessInfo() - processId: %ul\n", pProcessInfo->ProcessId);
	
    PEPROCESS pEProcess;

	NTSTATUS status = PsLookupProcessByProcessId((HANDLE)pProcessInfo->ProcessId, &pEProcess);
    if (status == STATUS_INVALID_CID)
    {
        KdPrint(("[#] ReClassHelper : PsLookupProcessByProcessId failed: Invalid Process Id!\n"));
        return STATUS_CANCELLED;
    }

	pProcessInfo->IsWow64 = PsGetProcessWow64Process(pEProcess) != NULL;

	if(pProcessInfo->IsWow64) 
	{
		// 32 bit process
		
        PPEB32 pPEB32 = (PPEB32)PsGetProcessWow64Process(pEProcess);
		
		pProcessInfo->PebAddress = (PVOID)pPEB32;
	}
    else 
    {
        // 64 bit process
    	
        PPEB pPEB = (PPEB)PsGetProcessPeb(pEProcess);

    	pProcessInfo->PebAddress = (PVOID)pPEB;
    }

    ObDereferenceObject(pEProcess);

	return status;
}
