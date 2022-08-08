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
        public static VkContext Current = new();

        private Instance instance;
        public GRContext GrContext { get; }

        internal VkContext()
        {
            var grVkBackendContext = new GRSharpVkBackendContext();

            instance = Instance.Create(Array.Empty<string>(), Array.Empty<string>());
            grVkBackendContext.VkInstance = instance;

            var physicalDevices = instance.EnumeratePhysicalDevices();
            var physicalDevice = physicalDevices[0];
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

            var device = physicalDevice.CreateDevice(queueInfos, null, null);
            grVkBackendContext.VkDevice = device;

            var graphicsQueue = device.GetQueue((uint)graphicsFamily.Value, 0);
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
                    ptr = instance.GetProcedureAddress(name);

                if (ptr == IntPtr.Zero)
                    throw new Exception("Null ptr");
                return ptr;
            };

            GrContext = GRContext.CreateVulkan(grVkBackendContext, null);
        }

        public void Dispose()
        {
            GrContext.Dispose();
            instance.Dispose();

            GC.SuppressFinalize(this);
        }

        ~VkContext()
        {
            Dispose();
        }
    }
}
