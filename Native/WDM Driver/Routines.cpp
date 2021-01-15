#include "Routines.h"

NTSTATUS ReadMemory(_In_ PREAD_REQUEST pReadMemory)
{
    KdPrint(("[#] ReClassHelper : Invoked ReadMemory()\n"));
	
    NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS pEprocess = NULL;

    status = PsLookupProcessByProcessId((HANDLE)pReadMemory->ProcessId, &pEprocess);
    if (status == STATUS_INVALID_CID)
    {
        KdPrint(("[#] ReClassHelper : PsLookupProcessByProcessId failed\n"));
        return STATUS_CANCELLED;
    }

    __try
    {
        ProbeForRead(pReadMemory->Address, pReadMemory->Size, 1);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KdPrint(("[#] ReClassHelper : ProbeForRead has failed with exception code %X\n", GetExceptionCode()));
        return STATUS_CANCELLED;
    }

    __try
    {
        ProbeForWrite(pReadMemory->Buffer, pReadMemory->Size, 1);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KdPrint(("[#] ReClassHelper : ProbeForRead has failed with exception code %X\n", GetExceptionCode()));
        return STATUS_CANCELLED;
    }

    __try 
    {
        SIZE_T bytesCopied = 0;

        status = MmCopyVirtualMemory(pEprocess, pReadMemory->Address, PsGetCurrentProcess(),
            pReadMemory->Buffer, pReadMemory->Size, KernelMode, &bytesCopied);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        KdPrint(("[#] ReClassHelper : MmCopyVirtualMemory failed and raised exception code %X\n", GetExceptionCode()));
        return STATUS_CANCELLED;
    }

	ObDereferenceObject(pEprocess);
	
    return status;
}

NTSTATUS WriteMemory(_In_ PWRITE_REQUEST pWriteMemory)
{
	KdPrint(("[#] ReClassHelper : Invoked WriteMemory()\n"));
	
	NTSTATUS status = STATUS_SUCCESS;
    PEPROCESS pEprocess = NULL;

    status = PsLookupProcessByProcessId((HANDLE)pWriteMemory->ProcessId, &pEprocess);
    if (status == STATUS_INVALID_CID)
    {
        DbgPrint("[#] ReClassHelper : PsLookupProcessByProcessId failed: Invalid Process Id!\n");
        DbgPrint("[#] ReClassHelper : ProcessId = %d", pWriteMemory->ProcessId);
        return STATUS_CANCELLED;
    }

    __try
    {
        SIZE_T bytesCopied = 0;
        status = MmCopyVirtualMemory(PsGetCurrentProcess(), pWriteMemory->Buffer, pEprocess, pWriteMemory->Address, pWriteMemory->Size, KernelMode, &bytesCopied);
    }
    __except (EXCEPTION_EXECUTE_HANDLER)
    {
        DbgPrint("[#] ReClassHelper : MmCopyVirtualMemory has failed %X\n", GetExceptionCode());
        DbgPrint("Address = %p, ProcessId = %d, Size = %d, Buffer = %p", pWriteMemory->Address, pWriteMemory->ProcessId, pWriteMemory->Size, pWriteMemory->Buffer);
        return STATUS_CANCELLED;
    }

    DbgPrint("[#] ReClassHelper : Address = %p, ProcessId = %d, Size = %d, Buffer = %p", pWriteMemory->Address, pWriteMemory->ProcessId, pWriteMemory->Size, pWriteMemory->Buffer);

    ObDereferenceObject(pEprocess);
	
    return status;
}

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
		// 32 bit
		
        PPEB32 pPEB32 = (PPEB32)PsGetProcessWow64Process(pEProcess);
		
		pProcessInfo->PebAddress = (PVOID)pPEB32;
	}
    else 
    {
        // 64 bit
    	
        PPEB pPEB = (PPEB)PsGetProcessPeb(pEProcess);

    	pProcessInfo->PebAddress = (PVOID)pPEB;
    }

    ObDereferenceObject(pEProcess);

	return status;
}