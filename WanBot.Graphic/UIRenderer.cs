using SkiaSharp;
using WanBot.Graphic.UI;

namespace WanBot.Graphic
{
    /// <summary>
    /// 渲染器
    /// </summary>
    public class UIRenderer : IDisposable
    {
        private VkContext _vkContext;
        private GRContext? _grContext;
        private bool _disposed;
        private bool _isDrawing;

        public UIRenderer()
        {
            _vkContext = new();
            _grContext = _vkContext.GrContext;

            _vkContext.GrContext?.SetResourceCacheLimit(1024 * 50);
        }

        /// <summary>
        /// 渲染UI到SKImage
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="layoutRect"></param>
        /// <returns></returns>
        public SKImage Draw(UIElement uiElement, SKRect layoutRect)
        {
            if (_disposed)
                return null!;

            _isDrawing = true;

            var rect = uiElement.UpdateLayout(layoutRect);
            var imageInfo = new SKImageInfo((int)rect.Width, (int)rect.Height);

            if (_grContext == null)
            {
                using var surface = SKSurface.Create(imageInfo);
                uiElement.Draw(surface.Canvas);
                _isDrawing = false;
                return surface.Snapshot();
            }
            else
            {
                lock (_grContext)
                {
                    using var surface = SKSurface.Create(_grContext, false, imageInfo);
                    uiElement.Draw(surface.Canvas);
                    _grContext.Flush();
                    _isDrawing = false;
                    return surface.Snapshot();
                }
            }
        }

        public SKImage Draw(UIElement uiElement, int width, int height)
        {
            var rect = new SKRect(0, 0, width, height);
            return Draw(uiElement, rect);
        }

        public SKImage Draw(UIElement uiElement)
        {
            return Draw(uiElement, SKRect.Empty);
        }

        public void Dispose()
        {
            _disposed = true;
            _vkContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}