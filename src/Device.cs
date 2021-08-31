namespace NVidiaGPUInfo {
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using LostTech.Stack.Widgets.DataBinding;
    using LostTech.Stack.Widgets.DataSources;

    using ManagedCuda.Nvml;

    using Prism.Commands;

    public class Device : DependencyObjectNotifyBase, IRefreshable {
        static readonly nvmlReturn initResult;
        internal nvmlDevice nativeDevice;
        readonly nvmlReturn handleReturn = nvmlReturn.Success;
        readonly DispatcherTimer refreshTimer;

        Device(nvmlDevice nativeDevice, uint index) : this() {
            this.nativeDevice = nativeDevice;
            this.Index = index;
        }

        public uint? Index { get; }
        public DeviceUtilization Utilization { get; }
        public DeviceMemoryInfo MemoryUsage { get; }

        #region PowerLimitMilliwatts
        static readonly DependencyPropertyKey PowerLimitMilliwattsKey = DependencyProperty.RegisterReadOnly(
            nameof(PowerLimitMilliwatts),
            typeof(uint),
            typeof(Device),
            new PropertyMetadata(0u));
        public uint PowerLimitMilliwatts => (uint)this.GetValue(PowerLimitMilliwattsKey.DependencyProperty);
        #endregion PowerLimitMilliwatts

        public string? Error => this.handleReturn == nvmlReturn.Success
            ? null : NvmlNativeMethods.nvmlErrorString(this.handleReturn);

        void RefreshInternal() {
            uint powerLimit = 0;
            var callResult = NvmlNativeMethods.nvmlDeviceGetPowerManagementLimit(this.nativeDevice, ref powerLimit);
            if (callResult != nvmlReturn.Success)
                throw new InvalidOperationException(NvmlNativeMethods.nvmlErrorString(callResult));
            this.SetValue(PowerLimitMilliwattsKey, powerLimit);
            this.Utilization.RefreshInternal();
            this.MemoryUsage.RefreshInternal();
        }

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

        public ICommand RefreshCommand { get; }

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
            this.MemoryUsage = new DeviceMemoryInfo(this);
            this.RefreshCommand = new DelegateCommand(this.RefreshInternal);
            this.refreshTimer = new DispatcherTimer {
                Interval = TimeSpan.FromSeconds(2),
            };
            this.refreshTimer.Tick += delegate { this.RefreshInternal(); };
            this.refreshTimer.IsEnabled = true;
        }
    }
}
