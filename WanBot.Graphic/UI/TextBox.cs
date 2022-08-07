using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WanBot.Graphic.UI
{
    public class TextBox : UIElement
    {
        public string Text { get; set; } = string.Empty;

        private List<string> _lines = new();
        private List<SKPoint> _pos = new();

        public SKPaint FontPaint = new()
        {
            TextSize = 62.0f,
            IsAntialias = true,
            Color = new SKColor(0x42, 0x81, 0xA4),
            IsStroke = false,
            TextAlign = SKTextAlign.Center
        };

        public override void Draw(SKCanvas canvas)
        {
            for (var i = 0; i < _lines.Count; i++)
                canvas.DrawText(_lines[i], _pos[i], FontPaint);
        }
        public override void DrawDebug(SKCanvas canvas)
        {
            base.DrawDebug(canvas);
            Draw(canvas);
        }

        public override SKRect UpdateLayout(SKRect inputRect)
        {
            base.UpdateLayout(inputRect);

            if (inputRect.Width == 0)
                return ContentRect;

            _lines.Clear();
            _pos.Clear();

            var text = Text;
            var currentY = RenderRect.Top;

            while (text.Length != 0)
            {
                var linePos = FontPaint.BreakText(text, RenderRect.Width, out var mesuredWidth);
                var line = text[0..(int)linePos];
                text = text[(int)linePos..^0];
                _lines.Add(line);
                var lineRect = new SKRect();
                FontPaint.MeasureText(line, ref lineRect);
                var lineHeight = lineRect.Size.Height;

                float lineX;
                if (FontPaint.TextAlign == SKTextAlign.Left)
                    lineX = RenderRect.Left;
                else if (FontPaint.TextAlign == SKTextAlign.Center)
                    lineX = RenderRect.Left + RenderRect.Width / 2;
                else
                    lineX = RenderRect.Left + RenderRect.Width;

                _pos.Add(new SKPoint(lineX, currentY + lineHeight));

                currentY += lineHeight;
            }
            Height = currentY - RenderRect.Top;

            var finalResult = base.UpdateLayout(inputRect);
            return finalResult;
        }

        public override void Dispose()
        {
            base.Dispose();
            FontPaint.Dispose();
        }
    }
}
