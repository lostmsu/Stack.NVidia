namespace NVidiaGPUInfo
{
    using System;
    using System.Windows.Input;

    using LostTech.Stack.Widgets.DataBinding;
    using LostTech.Stack.Widgets.DataSources;

    using ManagedCuda.Nvml;

    using Prism.Commands;

    public class DeviceUtilization : DependencyObjectNotifyBase, IRefreshable {
        readonly Device device;
        nvmlUtilization utilization;

        internal DeviceUtilization(Device device) {
            this.device = device;
            this.RefreshCommand = new DelegateCommand(this.RefreshInternal);
        }

        public uint Compute => this.utilization.gpu;
        public uint MemoryIO => this.utilization.memory;

        public ICommand RefreshCommand { get; }

        void RefreshInternal() {
            var callResult = NvmlNativeMethods.nvmlDeviceGetUtilizationRates(this.device.nativeDevice, ref this.utilization);
            if (callResult != nvmlReturn.Success)
                throw new InvalidOperationException(NvmlNativeMethods.nvmlErrorString(callResult));
            this.OnPropertyChanged(nameof(this.Compute));
            this.OnPropertyChanged(nameof(this.MemoryIO));
        }
    }
}
