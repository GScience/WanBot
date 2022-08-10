
using SharpVk;
using SkiaSharp;
using System.Diagnostics;
using WanBot.Graphic;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;

var vkContext = new VkContext();

void Draw(bool useVk, string savePath = "test.png", bool isDebug = true)
{
    using var grid = new Grid();

    var gridBg = new Rectangle();
    gridBg.Margin = new Margin(0, 0, 0, 0);
    gridBg.Radius = new SKSize(35, 35);
    gridBg.Paint.Color = SKColors.Gray;
    grid.Children.Add(gridBg);

    var content = new VerticalLayout();
    for (var k = 0; k < 6; ++k)
    {
        var bgGrid = new Grid();

        var bg = new Rectangle();
        bg.Radius = new SKSize(25, 25);
        bg.Margin = new Margin(25, 25, 25, 25);
        bgGrid.Children.Add(bg);

        var text = new TextBox();
        text.Text = "Test For Text Rendering";
        text.Margin = new Margin(50, 50, 50, 50);
        bgGrid.Children.Add(text);

        content.Children.Add(bgGrid);

        var horizontalLayout = new HorizontalLayout();
        horizontalLayout.HorizontalAlignment = (HorizontalAlignment)(k % 3);
        horizontalLayout.Margin = new Margin(0, 0);
        for (var j = 0; j < 6; ++j)
        {
            var verticalLayout = new VerticalLayout();
            verticalLayout.VerticalAlignment = (VerticalAlignment)(j % 3);
            verticalLayout.Margin = new Margin(0, 0);

            var titleText = new TextBox();
            titleText.Text = $"Title";
            titleText.FontPaint.TextSize = 16;
            titleText.Margin = new Margin(10, 10, 10, 10);
            verticalLayout.Children.Add(titleText);

            var rand = new Random();
            for (var i = 0; i < 6; ++i)
            {
                var image = new ImageBox();
                var size = (float)rand.NextDouble() * i * 50 + 50;
                image.Margin = new Margin(5, 5, 5, 5);
                image.Width = size;
                image.Height = size;

                verticalLayout.Children.Add(image);

                var imageGrid = new Grid();

                var imageText = new TextBox();
                imageText.Text = $"Image({i}.{j})";
                imageText.FontPaint.TextSize = 16;
                imageText.Margin = new Margin(10, 10, 10, 10);

                var imageGridBg = new Rectangle();
                imageGridBg.Margin = new Margin(0, 0, 0, 0);
                imageGridBg.Paint.Color = SKColors.Yellow;
                imageGrid.Children.Add(imageGridBg);
                imageGrid.Children.Add(imageText);

                verticalLayout.Children.Add(imageGrid);
            }
            horizontalLayout.Children.Add(verticalLayout);
        }
        content.Children.Add(horizontalLayout);
    }

    var text1 = new TextBox();
    text1.Text = "Single Line";
    text1.Margin = new Margin(10, 10, 10, 10);
    text1.FontPaint.TextSize = 32;
    content.Children.Add(text1);

    var text2 = new TextBox();
    text2.Text = "Single Line 2";
    text2.Margin = new Margin(10, 10, 10, 10);
    content.Children.Add(text2);

    for (var i = 0; i < 3; ++i)
    {
        var title = new TextBox();
        title.Text = $"Text Vertical Layout Test {i}";
        title.Margin = new Margin(10, 10, 10, 10);
        content.Children.Add(title);

        var image1 = new ImageBox();
        image1.Width = 150;
        image1.Height = 150;
        image1.Margin = new Margin(10, 10, 10, 10);

        var text3 = new TextBox();
        text3.Text = "Image With Text";
        text3.FontPaint.TextSize = 32;
        text3.Height = 150;
        text3.Margin = new Margin(170, 10, 10, 10);
        text3.TextVerticalAlignment = (TextVerticalAlignment)i;

        var textWithImage = new Grid();
        textWithImage.Margin = new Margin(0, 0);
        textWithImage.Height = 170;
        textWithImage.Children.Add(image1);
        textWithImage.Children.Add(text3);
        content.Children.Add(textWithImage);
    }

    grid.Children.Add(content);

    var rect = grid.UpdateLayout(new SKRect(0, 0, 2000, 300));
    var imageInfo = new SKImageInfo((int)rect.Width, (int)rect.Height);

    if (useVk)
    {
        using var surface = SKSurface.Create(vkContext.GrContext, false, imageInfo);

        if (isDebug)
            grid.DrawDebug(surface.Canvas);
        else
            grid.Draw(surface.Canvas);

        vkContext.GrContext?.Flush();

        if (!string.IsNullOrEmpty(savePath))
        {
            using (var output = File.Create(savePath))
            {
                using var img = surface.Snapshot();
                img.Encode().SaveTo(output);
            }
        }
    }
    else
    {
        using var surface = SKSurface.Create(imageInfo);

        if (isDebug)
            grid.DrawDebug(surface.Canvas);
        else
            grid.Draw(surface.Canvas);

        if (!string.IsNullOrEmpty(savePath))
        {
            using (var output = File.Create(savePath))
            {
                using var img = surface.Snapshot();
                img.Encode().SaveTo(output);
            }
        }
    }
}

Draw(true, "vulkan_debug.png");
Draw(false, "test_debug.png");

Draw(true, "vulkan.png", false);
Draw(false, "test.png", false);

for (var i = 0; i < 10; ++i)
{
    Console.WriteLine($"Start {i}");

    var sw1 = new Stopwatch();
    sw1.Start();

    for (var j = 0; j < 1; ++j)
    {
        Draw(false, "");
    }

    sw1.Stop();
    Console.WriteLine("CPU draw 500 times use {0}s", sw1.Elapsed);

    if (vkContext.GrContext != null)
    {
        Console.WriteLine($"Start {i}");

        var sw2 = new Stopwatch();
        sw2.Start();

        for (var j = 0; j < 10000; ++j)
        {
            Draw(true, "");
        }

        sw2.Stop();
        Console.WriteLine("GPU draw 500 times use {0}s", sw2.Elapsed);
    }
}