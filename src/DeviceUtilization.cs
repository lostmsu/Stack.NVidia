namespace NVidiaGPUInfo {
    using System;

    using ManagedCuda.Nvml;

    public class DeviceUtilization : DeviceInfoBase {
        nvmlUtilization utilization;

        internal DeviceUtilization(Device device) : base(device) { }

        public uint Compute => this.utilization.gpu;
        public uint MemoryIO => this.utilization.memory;

        protected internal override void RefreshInternal() {
            var callResult = NvmlNativeMethods.nvmlDeviceGetUtilizationRates(this.device.nativeDevice, ref this.utilization);
            if (callResult != nvmlReturn.Success)
                throw new InvalidOperationException(NvmlNativeMethods.nvmlErrorString(callResult));
            this.OnPropertyChanged(nameof(this.Compute));
            this.OnPropertyChanged(nameof(this.MemoryIO));
        }
    }
}
