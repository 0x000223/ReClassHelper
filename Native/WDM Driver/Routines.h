#pragma once
#include <ntdef.h>

#include "Imports.h"
#include "Structs.h"

NTSTATUS ReadMemory(_In_ PREAD_REQUEST pReadMemory);

NTSTATUS WriteMemory(_In_ PWRITE_REQUEST pWriteMemory);

NTSTATUS GetProcessInfo(_Out_ PPROCESS_INFO pProcessInfo);