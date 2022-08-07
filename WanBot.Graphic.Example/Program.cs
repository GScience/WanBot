
using SkiaSharp;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;

var content = new VerticalLayout();
for (var k = 0; k < 3; ++k)
{
    var horizontalLayout = new HorizontalLayout();
    for (var j = 0; j < 3; ++j)
    {
        var verticalLayout = new VerticalLayout();
        verticalLayout.Margin = new Margin(5, 5, 5, 5);
        for (var i = 0; i < 3; ++i)
        {
            var image = new ImageBox();
            image.Margin = new Margin(5, 5, 5, 5);
            image.Width = (i + 1) * 50;
            image.Height = (i + 1) * 50;

            verticalLayout.Children.Add(image);

            var imageText = new TextBox();
            imageText.Text = $"Image({i}.{j})";
            imageText.FontPaint.TextSize = 16;
            imageText.Margin = new Margin(10, 10, 10, 10);
            verticalLayout.Children.Add(imageText);
        }
        horizontalLayout.Children.Add(verticalLayout);
    }
    content.Children.Add(horizontalLayout);

    var text = new TextBox();
    text.Text = "Test For Text Rendering";
    text.Margin = new Margin(10, 10, 10, 10);
    content.Children.Add(text);
}

var rect = content.UpdateLayout(new SKRect(0, 0, 600, 300));

var imageInfo = new SKImageInfo((int)rect.Width, (int)rect.Height);
var surface = SKSurface.Create(imageInfo);
content.DrawDebug(surface.Canvas);
using (var output = File.Create("test.png"))
    surface.Snapshot().Encode().SaveTo(output);
return 0;