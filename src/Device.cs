namespace NVidiaGPUInfo {
    using System;

    using ManagedCuda.Nvml;
    public class Device {
        static readonly nvmlReturn initResult;
        internal nvmlDevice nativeDevice;
        readonly nvmlReturn handleReturn = nvmlReturn.Success;

        Device(nvmlDevice nativeDevice, uint index) : this() {
            this.nativeDevice = nativeDevice;
            this.Index = index;
        }

        public uint? Index { get; }
        public DeviceUtilization Utilization { get; }
        public string? Error => this.handleReturn == nvmlReturn.Success
            ? null : NvmlNativeMethods.nvmlErrorString(this.handleReturn);

        public static Device[] Devices {
            get {
                if (initResult != nvmlReturn.Success)
                    return Array.Empty<Device>();

                uint count = 0;
                var callResult = NvmlNativeMethods.nvmlDeviceGetCount(ref count);
                if (callResult != nvmlReturn.Success)
                    return Array.Empty<Device>();

                var result = new Device[count];
                for (uint deviceIndex = 0; deviceIndex < count; deviceIndex++) {
                    nvmlDevice nativeDevice = default;
                    callResult = NvmlNativeMethods.nvmlDeviceGetHandleByIndex(deviceIndex, ref nativeDevice);
                    result[deviceIndex] = callResult == nvmlReturn.Success
                        ? new Device(nativeDevice, deviceIndex)
                        : new Device(callResult, deviceIndex);
                }
                return result;
            }
        }

        static Device() {
            initResult = NvmlNativeMethods.nvmlInit();
        }

        Device(nvmlReturn handleReturn, uint index) : this() {
            if (handleReturn == nvmlReturn.Success)
                throw new InvalidOperationException();

            this.handleReturn = handleReturn;
            this.Index = index;
        }

        Device() {
            this.Utilization = new DeviceUtilization(this);
        }
    }
}
