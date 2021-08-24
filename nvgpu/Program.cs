namespace nvgpu {
    using System;

    using NVidiaGPUInfo;

    class Program {
        static void Main() {
            foreach(var device in Device.Devices) {
                device.Utilization.RefreshCommand.Execute(null);
                Console.WriteLine($"GPU{device.Index}: {device.Utilization.Compute}% memIO: {device.Utilization.MemoryIO}%");
            }
        }
    }
}
