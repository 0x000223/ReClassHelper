#include "Routines.h"
#include "Structs.h"

// Contorl codes
#define HELPER_DEVICE_TYPE 0x00222222

#define IOCTL_READ			CTL_CODE(HELPER_DEVICE_TYPE, 0x800, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_WRITE			CTL_CODE(HELPER_DEVICE_TYPE, 0x801, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)
#define IOCTL_PROCESS_INFO	CTL_CODE(HELPER_DEVICE_TYPE, 0x802, METHOD_BUFFERED, FILE_READ_ACCESS | FILE_WRITE_ACCESS)

NTSTATUS DeviceControl(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
	KdPrint(("[#] ReClassHelper : Invoked DeviceControl()\n"));
	
	UNREFERENCED_PARAMETER(DeviceObject);

	NTSTATUS status;
	ULONG size;
	PVOID input = Irp->AssociatedIrp.SystemBuffer;
	
	switch(IoGetCurrentIrpStackLocation(Irp)->Parameters.DeviceIoControl.IoControlCode)
	{
	case IOCTL_READ:
		{
			KdPrint(("[#] ReClassHelper : Received IOCTL_READ\n"));

			status = ReadMemory((PREAD_REQUEST)input);
			size = sizeof(READ_REQUEST);
			break;
		}
	case IOCTL_WRITE:
		{
			KdPrint(("[#] ReClassHelper : Received IOCTL_WRITE\n"));

			status = WriteMemory((PWRITE_REQUEST)input);
			size = sizeof(WRITE_REQUEST);
			break;
		}
	case IOCTL_PROCESS_INFO:
		{
			KdPrint(("[#] ReClassHelper : Received IOCTL_PROCESS_INFO\n"));
			
			status = GetProcessInfo((PPROCESS_INFO) input);

			DbgPrint("[#] ReClassHelper : Ended IOCTL_PROCESS_INFO - PebAddress: %p", ((PPROCESS_INFO)input)->PebAddress);			
			size = sizeof(PROCESS_INFO);
			break;
		}
	default:
		{
			status = STATUS_INVALID_DEVICE_REQUEST;
			size = 0;
			break;
		}
	}

	Irp->IoStatus.Status = status;
	Irp->IoStatus.Information = size;

	IoCompleteRequest(Irp, IO_NO_INCREMENT);

	KdPrint(("[#] ReClassHelper : Completed Request\n"));
	
	return status;
}

EXTERN_C
NTSTATUS CreateClose(PDEVICE_OBJECT DeviceObject, PIRP Irp)
{
	KdPrint(("[#] ReClassHelper : Invoked CreateClose()\n"));
	
	UNREFERENCED_PARAMETER(DeviceObject);

	Irp->IoStatus.Status = STATUS_SUCCESS;
	Irp->IoStatus.Information = 0;

	IoCompleteRequest(Irp, IO_NO_INCREMENT);

	return STATUS_SUCCESS;
}

EXTERN_C
VOID UnloadDriver(PDRIVER_OBJECT driverObject)
{
	KdPrint(("[#] ReClassHelper : Invoked UnloadDriver()\n"));
	
	UNICODE_STRING symLink = RTL_CONSTANT_STRING(L"\\??\\ReClassHelper");

	IoDeleteSymbolicLink(&symLink);

	IoDeleteDevice(driverObject->DeviceObject);
}

EXTERN_C
NTSTATUS DriverEntry(PDRIVER_OBJECT DriverObject, PUNICODE_STRING RegistryPath)
{
	KdPrint(("[#] ReClassHelper : Invoked DriverEntry()\n"));

	UNREFERENCED_PARAMETER(RegistryPath);

	DriverObject->DriverUnload = UnloadDriver;
	DriverObject->MajorFunction[IRP_MJ_CREATE] = CreateClose;
	DriverObject->MajorFunction[IRP_MJ_CLOSE] = CreateClose;
	DriverObject->MajorFunction[IRP_MJ_DEVICE_CONTROL] = DeviceControl;

	UNICODE_STRING deviceName = RTL_CONSTANT_STRING(L"\\Device\\ReClassHelper");

	PDEVICE_OBJECT deviceObject;

	NTSTATUS status = IoCreateDevice(
		DriverObject,			// 
		0,						// Extra bytes
		&deviceName,			// Device name
		HELPER_DEVICE_TYPE,		// Device type
		0,						// Characteristics flag
		FALSE,					// Exclusive mode
		&deviceObject);			// 


	if(!NT_SUCCESS(status))
	{
		KdPrint(("[#] ReClassHelper : Failed to create device object\n"));

		return status;
	}

	UNICODE_STRING symLink = RTL_CONSTANT_STRING(L"\\??\\ReClassHelper");

	status = IoCreateSymbolicLink(&symLink, &deviceName);
	if(!NT_SUCCESS(status))
	{
		KdPrint(("[#] ReClassHelper : Failed to create symbol link\n"));
		
		IoDeleteDevice(deviceObject);

		return status;
	}

	return STATUS_SUCCESS;
}