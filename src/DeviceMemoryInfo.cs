namespace NVidiaGPUInfo {
    using System;

    using ManagedCuda.Nvml;
    public class DeviceMemoryInfo : DeviceInfoBase {
        nvmlMemory mem;

        internal DeviceMemoryInfo(Device device) : base(device) { }

        public ulong TotalBytes => this.mem.total;
        public ulong UsedBytes => this.mem.used;
        public ulong FreeBytes => this.mem.free;

        protected override void RefreshInternal() {
            var callResult = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(this.device.nativeDevice, ref this.mem);
            if (callResult != nvmlReturn.Success)
                throw new InvalidOperationException(NvmlNativeMethods.nvmlErrorString(callResult));
            this.OnPropertyChanged(nameof(this.TotalBytes));
            this.OnPropertyChanged(nameof(this.UsedBytes));
            this.OnPropertyChanged(nameof(this.FreeBytes));
        }
    }
}
