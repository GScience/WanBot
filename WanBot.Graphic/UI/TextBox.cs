using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic.UI.Layout;

namespace WanBot.Graphic.UI
{
    public class TextBox : UIElement
    {
        public TextVerticalAlignment TextVerticalAlignment { get; set; } = TextVerticalAlignment.Center;

        public string Text { get; set; } = string.Empty;

        private List<string> _lines = new();
        private List<SKPoint> _pos = new();

        public SKPaint FontPaint { get; set; } = new()
        {
            TextSize = 62.0f,
            IsAntialias = true,
            Color = new SKColor(0x42, 0x81, 0xA4),
            IsStroke = false,
            TextAlign = SKTextAlign.Center,
            Typeface = Fonts.Default
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

            if (inputRect.Width <= 0)
                return ContentRect;

            _lines.Clear();
            _pos.Clear();

            var text = Text;
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");

            var currentY = RenderRect.Top;

            while (text.Length != 0)
            {
                var splitePos = FontPaint.BreakText(text, RenderRect.Width, out var mesuredWidth);

                if (splitePos == 0)
                    return ContentRect;

                // 处理换行
                var newLineIndex = text.IndexOf('\n', 0, (int)splitePos);
                string line;

                if (newLineIndex != -1)
                {
                    if (newLineIndex == 0)
                    {
                        // 第一个字符是换行符，则跳过换行符重新循环
                        text = text[1..^0];
                        continue;
                    }
                    else
                    {
                        // 最后一个字符是换行符
                        splitePos = newLineIndex;
                        line = text[0..(int)splitePos];
                    }
                }
                else
                    line = text[0..(int)splitePos];
                
                text = text[(int)splitePos..^0];
                _lines.Add(line);

                float lineX;
                if (FontPaint.TextAlign == SKTextAlign.Left)
                    lineX = RenderRect.Left;
                else if (FontPaint.TextAlign == SKTextAlign.Center)
                    lineX = RenderRect.Left + RenderRect.Width / 2;
                else
                    lineX = RenderRect.Left + RenderRect.Width;

                _pos.Add(new SKPoint(lineX, currentY + FontPaint.FontSpacing));

                currentY += FontPaint.FontSpacing;
            }
            var newHeight = currentY - RenderRect.Top + FontPaint.FontSpacing / 2;

            var oldHeight = Height;

            if (Height != null && newHeight < Height)
            {
                var offset = Height.Value - newHeight;

                switch (TextVerticalAlignment)
                {
                    case TextVerticalAlignment.Top:
                        break;
                    case TextVerticalAlignment.Center:
                        for (var i = 0; i < _pos.Count; ++i)
                            _pos[i] = new SKPoint(_pos[i].X, _pos[i].Y + offset / 2);
                        break;
                    case TextVerticalAlignment.Bottom:
                        for (var i = 0; i < _pos.Count; ++i)
                            _pos[i] = new SKPoint(_pos[i].X, _pos[i].Y + offset);
                        break;
                }
            }
            else
                Height = newHeight;

            var finalResult = base.UpdateLayout(inputRect);
            Height = oldHeight;
            return finalResult;
        }

        public override void Dispose()
        {
            base.Dispose();
            FontPaint.Dispose();
        }
    }
}
