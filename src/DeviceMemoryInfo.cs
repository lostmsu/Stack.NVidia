namespace NVidiaGPUInfo {
    using System;
    using System.Windows;

    using ManagedCuda.Nvml;
    public class DeviceMemoryInfo : DeviceInfoBase {
        internal DeviceMemoryInfo(Device device) : base(device) { }

        static readonly DependencyPropertyKey totalKey = DependencyProperty.RegisterReadOnly(
            nameof(TotalBytes),
            typeof(ulong),
            typeof(DeviceMemoryInfo),
            new PropertyMetadata(0ul));
        public ulong TotalBytes {
            get => (ulong)this.GetValue(totalKey.DependencyProperty);
            private set => this.SetValue(totalKey, value);
        }

        static readonly DependencyPropertyKey usedKey = DependencyProperty.RegisterReadOnly(
            nameof(UsedBytes),
            typeof(ulong),
            typeof(DeviceMemoryInfo),
            new PropertyMetadata(0ul));
        public ulong UsedBytes {
            get => (ulong)this.GetValue(usedKey.DependencyProperty);
            private set => this.SetValue(usedKey, value);
        }

        static readonly DependencyPropertyKey freeKey = DependencyProperty.RegisterReadOnly(
            nameof(FreeBytes),
            typeof(ulong),
            typeof(DeviceMemoryInfo),
            new PropertyMetadata(0ul));
        public ulong FreeBytes {
            get => (ulong)this.GetValue(freeKey.DependencyProperty);
            private set => this.SetValue(freeKey, value);
        }

        protected internal override void RefreshInternal() {
            nvmlMemory mem = default;
            var callResult = NvmlNativeMethods.nvmlDeviceGetMemoryInfo(this.device.nativeDevice, ref mem);
            if (callResult != nvmlReturn.Success)
                throw new InvalidOperationException(NvmlNativeMethods.nvmlErrorString(callResult));

            this.TotalBytes = mem.total;
            this.UsedBytes = mem.used;
            this.FreeBytes = mem.free;
        }
    }
}
