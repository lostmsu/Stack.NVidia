namespace NVidiaGPUInfo {
    using System.Windows.Input;

    using LostTech.Stack.Widgets.DataBinding;
    using LostTech.Stack.Widgets.DataSources;

    using Prism.Commands;
    public abstract class DeviceInfoBase : DependencyObjectNotifyBase, IRefreshable {
        protected readonly Device device;

        internal DeviceInfoBase(Device device) {
            this.device = device;
            this.RefreshCommand = new DelegateCommand(this.RefreshInternal);
        }

        public ICommand RefreshCommand { get; }

        protected abstract void RefreshInternal();
    }
}
