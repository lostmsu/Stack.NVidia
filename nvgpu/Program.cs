namespace nvgpu {
    using System;

    using NVidiaGPUInfo;

    class Program {
        static void Main() {
            foreach(var device in Device.Devices) {
                device.RefreshCommand.Execute(null);
                device.MemoryUsage.RefreshCommand.Execute(null);
                device.Utilization.RefreshCommand.Execute(null);
                Console.WriteLine($"GPU{device.Index} (up to {device.PowerLimitMilliwatts}W): {device.Utilization.Compute}% memIO: {device.Utilization.MemoryIO}%");
            }
        }
    }
}
