using System;
using System.Runtime.InteropServices;

namespace VhdAttachService {

    [ComImport, Guid("118610b7-8d94-4030-b5b8-500889788e4e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumVdsObject {
        void Next(uint numberOfObjects, [MarshalAs(UnmanagedType.IUnknown)] out object objectUnk, out uint numberFetched);
        void Skip(uint NumberOfObjects);
        void Reset();
        void Clone(out IEnumVdsObject Enum);
    }


    [ComImport, Guid("07e5c822-f00c-47a1-8fce-b244da56fd06"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsDisk {
        void GetProperties(out VDS_DISK_PROP diskProperties);
        void GetPack(); // Unported method
        void GetIdentificationData(IntPtr lunInfo);
        void QueryExtents(); // Unported method
        void slot4();
        void SetFlags(); // Unported method
        void ClearFlags(); // Unported method
    }


    [ComImport, Guid("0818a8ef-9ba9-40d8-a6f9-e22833cc771e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsService {
        [PreserveSig]
        int IsServiceReady();
        [PreserveSig]
        int WaitForServiceReady();
        void GetProperties(); // Unported method
        void QueryProviders(VDS_QUERY_PROVIDER_FLAG mask, out IEnumVdsObject providers);
        void QueryMaskedDisks(out IEnumVdsObject disks);
        void QueryUnallocatedDisks(out IEnumVdsObject disks);
        void GetObject(); // Unported method
        void QueryDriveLetters(); // Unported method
        void QueryFileSystemTypes(out IntPtr fileSystemTypeProps, out uint numberOfFileSystems);
        void Reenumerate();
        void Refresh();
        void CleanupObsoleteMountPoints();
        void Advise(); // Unported method
        void Unadvise(); // Unported method
        void Reboot();
        void SetFlags(); // Unported method
        void ClearFlags(); // Unported method
    }


    [ComImport, Guid("e0393303-90d4-4a97-ab71-e9b671ee2729"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsServiceLoader {
        void LoadService([In, MarshalAs(UnmanagedType.LPWStr)] string machineName, out IVdsService vdsService);
    }


    [ComImport, Guid("1e062b84-e5e6-4b4b-8a25-67b81e8f13e8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsVDisk {
        void Open(); // Unported method
        void GetProperties(out VDS_VDISK_PROPERTIES pDiskProperties);
        void GetHostVolume(); // Unported method
        void GetDeviceName(); // Unported method
    }


    [ComImport, Guid("b481498c-8354-45f9-84a0-0bdd2832a91f"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsVdProvider {
        void QueryVDisks(out IEnumVdsObject ppEnum);
        void CreateVDisk(); // Unported method
        void AddVDisk(); // Unported method
        void GetDiskFromVDisk(IVdsVDisk pVDisk, out IVdsDisk ppDisk);
        void GetVDiskFromDisk(IVdsDisk pDisk, out IVdsVDisk ppVDisk);
    }


    [ComImport, Guid("9c38ed61-d565-4728-aeee-c80952f0ecde")]
    public class VdsServiceLoader {
    }


    [StructLayout(LayoutKind.Explicit)]
    public struct Signature {
        [FieldOffset(0)]
        public uint dwSignature;
        [FieldOffset(0)]
        public Guid DiskGuid;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_DISK_PROP {
        public Guid Id;
        public VDS_DISK_STATUS Status;
        public VDS_LUN_RESERVE_MODE ReserveMode;
        public VDS_HEALTH health;
        public uint dwDeviceType;
        public uint dwMediaType;
        public ulong ullSize;
        public uint ulBytesPerSector;
        public uint ulSectorsPerTrack;
        public uint ulTracksPerCylinder;
        public uint ulFlags;
        public VDS_STORAGE_BUS_TYPE BusType;
        public VDS_PARTITION_STYLE PartitionStyle;
        public Signature dwSignature;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszDiskAddress;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszFriendlyName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszAdaptorName;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pwszDevicePath;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct VIRTUAL_STORAGE_TYPE {
        public uint DeviceId;
        public Guid VendorId;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_VDISK_PROPERTIES {
        public Guid Id;
        public VDS_VDISK_STATE State;
        public VIRTUAL_STORAGE_TYPE VirtualDeviceType;
        public ulong VirtualSize;
        public ulong PhysicalSize;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String pPath;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String pDeviceName;
        public DEPENDENT_DISK_FLAG DiskFlag;
        public bool bIsChild;
        [MarshalAs(UnmanagedType.LPWStr)]
        public String pParentPath;
    }


    public enum DEPENDENT_DISK_FLAG {
        DEPENDENT_DISK_FLAG_NONE = 0x00000000,
        DEPENDENT_DISK_FLAG_MULT_BACKING_FILES = 0x00000001,
        DEPENDENT_DISK_FLAG_FULLY_ALLOCATED = 0x00000002,
        DEPENDENT_DISK_FLAG_READ_ONLY = 0x00000004,
        DEPENDENT_DISK_FLAG_REMOTE = 0x00000008,
        DEPENDENT_DISK_FLAG_SYSTEM_VOLUME = 0x00000010,
        DEPENDENT_DISK_FLAG_SYSTEM_VOLUME_PARENT = 0x00000020,
        DEPENDENT_DISK_FLAG_REMOVABLE = 0x00000040,
        DEPENDENT_DISK_FLAG_NO_DRIVE_LETTER = 0x00000080,
        DEPENDENT_DISK_FLAG_PARENT = 0x00000100,
        DEPENDENT_DISK_FLAG_NO_HOST_DISK = 0x00000200,
        DEPENDENT_DISK_FLAG_PERMANENT_LIFETIME = 0x00000400,
    }


    public enum VDS_DISK_STATUS {
        VDS_DS_UNKNOWN = 0,
        VDS_DS_ONLINE = 1,
        VDS_DS_NOT_READY = 2,
        VDS_DS_NO_MEDIA = 3,
        VDS_DS_FAILED = 5,
        VDS_DS_MISSING = 6,
        VDS_DS_OFFLINE = 4
    }


    public enum VDS_HEALTH {
        VDS_H_UNKNOWN = 0,
        VDS_H_HEALTHY = 1,
        VDS_H_REBUILDING = 2,
        VDS_H_STALE = 3,
        VDS_H_FAILING = 4,
        VDS_H_FAILING_REDUNDANCY = 5,
        VDS_H_FAILED_REDUNDANCY = 6,
        VDS_H_FAILED_REDUNDANCY_FAILING = 7,
        VDS_H_FAILED = 8,
        VDS_H_REPLACED = 9,
        VDS_H_PENDING_FAILURE = 10,
        VDS_H_DEGRADED = 11
    }


    public enum VDS_LUN_RESERVE_MODE {
        VDS_LRM_NONE = 0,
        VDS_LRM_EXCLUSIVE_RW = 1,
        VDS_LRM_EXCLUSIVE_RO = 2,
        VDS_LRM_SHARED_RO = 3,
        VDS_LRM_SHARED_RW = 4
    }


    public enum VDS_PARTITION_STYLE {
        VDS_PST_UNKNOWN = 0,
        VDS_PST_MBR = 1,
        VDS_PST_GPT = 2
    }


    public enum VDS_QUERY_PROVIDER_FLAG {
        VDS_QUERY_SOFTWARE_PROVIDERS = 0x1,
        VDS_QUERY_HARDWARE_PROVIDERS = 0x2,
        VDS_QUERY_VIRTUALDISK_PROVIDERS = 0x4
    }


    public enum VDS_STORAGE_BUS_TYPE {
        VDSBusTypeUnknown = 0,
        VDSBusTypeScsi = 0x1,
        VDSBusTypeAtapi = 0x2,
        VDSBusTypeAta = 0x3,
        VDSBusType1394 = 0x4,
        VDSBusTypeSsa = 0x5,
        VDSBusTypeFibre = 0x6,
        VDSBusTypeUsb = 0x7,
        VDSBusTypeRAID = 0x8,
        VDSBusTypeiScsi = 0x9,
        VDSBusTypeSas = 0xa,
        VDSBusTypeSata = 0xb,
        VDSBusTypeSd = 0xc,
        VDSBusTypeMmc = 0xd,
        VDSBusTypeMax = 0xe,
        VDSBusTypeFileBackedVirtual = 0xf,
        VDSBusTypeMaxReserved = 0x7f
    }


    public enum VDS_VDISK_STATE {
        VDS_VST_UNKNOWN = 0,
        VDS_VST_ADDED,
        VDS_VST_OPEN,
        VDS_VST_ATTACH_PENDING,
        VDS_VST_ATTACHED_NOT_OPEN,
        VDS_VST_ATTACHED,
        VDS_VST_DETACH_PENDING,
        VDS_VST_COMPACTING,
        VDS_VST_MERGING,
        VDS_VST_EXPANDING,
        VDS_VST_DELETED,
        VDS_VST_MAX
    }

}
