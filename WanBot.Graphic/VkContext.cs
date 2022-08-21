using SharpVk;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic
{
    public class VkContext : IDisposable
    {
        public GRContext? GrContext { get; private set; }

        private Instance? _instance;
        private Device? _device;

        public void Init(Action<string> logger)
        {
            var grVkBackendContext = new GRSharpVkBackendContext();

            _instance = Instance.Create(Array.Empty<string>(), Array.Empty<string>());
            grVkBackendContext.VkInstance = _instance;

            var physicalDevices = _instance.EnumeratePhysicalDevices();
            PhysicalDevice? physicalDevice = null;
            string? deviceName = null;

            logger($"All GPU(s):");
            for (var i = 0; i < physicalDevices.Length; ++i)
            {
                var pd = physicalDevices[i];
                var property = pd.GetProperties();
                logger($"GPU {i}: {property.DeviceName}");

                // 默认不使用虚拟Gpu
                if (property.DeviceType == PhysicalDeviceType.VirtualGpu &&
                    physicalDevice != null)
                    continue;
                physicalDevice = pd;
                deviceName = property.DeviceName;
            }

            if (physicalDevice == null)
                throw new Exception("Unable to find physical device");

            logger($"Use {deviceName}");
            grVkBackendContext.VkPhysicalDevice = physicalDevice;

            var queueFamilyProperties = physicalDevice.GetQueueFamilyProperties();

            var graphicsFamily = queueFamilyProperties
                .Select((properties, index) => new { properties, index })
                .SkipWhile(pair => !pair.properties.QueueFlags.HasFlag(QueueFlags.Graphics))
                .FirstOrDefault()?.index;

            if (graphicsFamily == null)
                throw new Exception("Unable to find graphics queue");

            var queueInfos = new[]
            {
                new DeviceQueueCreateInfo { QueueFamilyIndex = (uint)graphicsFamily.Value, QueuePriorities = new[] { 1f } }
            };

            logger($"Create device");
            _device = physicalDevice.CreateDevice(queueInfos, null, null);

            if (_device == null)
                throw new Exception("Failed to create device");

            grVkBackendContext.VkDevice = _device;

            logger($"Get queue");
            var graphicsQueue = _device.GetQueue((uint)graphicsFamily.Value, 0);

            if (graphicsQueue == null)
                throw new Exception("Failed to get queue");

            grVkBackendContext.VkQueue = graphicsQueue;
            grVkBackendContext.GraphicsQueueIndex = (uint)graphicsFamily.Value;

            grVkBackendContext.GetProcedureAddress = (name, ins, dev) =>
            {
                IntPtr ptr;
                if (dev != null)
                    ptr = dev.GetProcedureAddress(name);
                else if (ins != null)
                    ptr = ins.GetProcedureAddress(name);
                else
                    ptr = _instance.GetProcedureAddress(name);

                if (ptr == IntPtr.Zero)
                    logger($"{name} not found");
                return ptr;
            };

            GrContext = GRContext.CreateVulkan(grVkBackendContext, null);

            logger($"Done");
        }

        public void Dispose()
        {
            GrContext?.Dispose();
            _device?.Dispose();
            _device = null!;
            _instance?.Dispose();
            _instance = null!;

            GC.SuppressFinalize(this);
        }

        ~VkContext()
        {
            Dispose();
        }
    }
}
