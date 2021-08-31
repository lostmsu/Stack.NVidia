namespace NVidiaGPUInfo {
    using System;
    using System.Windows;

    using ManagedCuda.Nvml;
    public class DeviceMemoryInfo : DeviceInfoBase {
        internal DeviceMemoryInfo(Device device) : base(device) { }

        static readonly DependencyPropertyKey totalKey = DependencyProperty.RegisterReadOnly(
            nameof(TotalBytes),
            typeof(long),
            typeof(DeviceMemoryInfo),
            new PropertyMetadata(0L));
        public long TotalBytes => (long)this.GetValue(totalKey.DependencyProperty);

        static readonly DependencyPropertyKey usedKey = DependencyProperty.RegisterReadOnly(
            nameof(UsedBytes),
            typeof(long),
            typeof(DeviceMemoryInfo),
            new PropertyMetadata(0L));
        public long UsedBytes => (long)this.GetValue(usedKey.DependencyProperty);

        static readonly DependencyPropertyKey freeKey = DependencyProperty.RegisterReadOnly(
            nameof(FreeBytes),
            typeof(long),
            typeof(DeviceMemoryInfo),
            new PropertyMetadata(0L));
        public long FreeBytes => (long)this.GetValue(freeKey.DependencyProperty);

        protected internal override void RefreshInternal() {
            nvmlMemory mem = default;
            var callResult = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(this.device.nativeDevice, ref mem);
            if (callResult != nvmlReturn.Success)
                throw new InvalidOperationException(NvmlNativeMethods.nvmlErrorString(callResult));

            this.SetValue(totalKey, (long)mem.total);
            this.SetValue(usedKey, (long)mem.used);
            this.SetValue(freeKey, (long)mem.free);
        }
    }
}
