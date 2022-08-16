using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanBot.Graphic;
using WanBot.Graphic.UI;
using WanBot.Graphic.UI.Layout;
using WanBot.Graphic.Util;
using WanBot.Plugin.Essential.Graphic;

namespace WanBot.Plugin.YGO
{
    /// <summary>
    /// 卡牌渲染器
    /// </summary>
    public static class CardRenderer
    {
        public static int CardWidth = 397 / 2;
        public static int CardHeight = 578 / 2;
        public static float FontSize = 48 / 2;

        public static Grid GenCard(string cardName, string cardDesc, SKImage cardImage)
        {
            var grid = new Grid();

            var horizontalLayout = new HorizontalLayout();
            horizontalLayout.Space = 10;

            // 卡图
            var cardImg = new ImageBox();
            cardImg.Width = CardWidth;
            cardImg.Height = CardHeight;
            cardImg.Image = cardImage;
            cardImg.Margin = new Margin(0, 0);

            horizontalLayout.Children.Add(cardImg);

            // 卡片介绍
            var helper = new VerticalHelper();
            helper.Box(cardName, 0.8f * FontSize, 5).Box(cardDesc, FontSize * 0.5f, 15, textAlignment: SKTextAlign.Left).Width(CardWidth).Space(10).Center();
            horizontalLayout.Children.Add(helper.VerticalLayout);
            grid.Children.Add(horizontalLayout);

            return grid;
        }

        public static async Task<(SKImage image, int count)> GenCardsImageAsync(UIRenderer renderer, string search, IEnumerator<YgoCard> cards, string titlePrefix, int gridWidth = 3, int gridHeight = 2)
        {
            using var grid = new Grid();

            var gridBg = new Rectangle();
            gridBg.Paint.Color = SKColors.White;
            grid.Children.Add(gridBg);

            var verticalLayout = new VerticalLayout();
            verticalLayout.Space = 10;
            grid.Children.Add(verticalLayout);

            var imgList = new List<SKImage>();
            var title = new TextBox();
            title.FontPaint.TextSize = FontSize;
            title.Text = $"{titlePrefix}{search}";
            verticalLayout.Children.Add(title);

            try
            {
                var cardIndex = 0;
                var isEmpty = false;

                for (var x = 0; x < gridWidth; ++x)
                {
                    var horizontalLayout = new HorizontalLayout();
                    horizontalLayout.Space = 10;
                    for (var y = 0; y < gridHeight; ++y)
                    {
                        if (!cards.MoveNext())
                        {
                            isEmpty = true;
                            break;
                        }
                        ++cardIndex;

                        var card = cards.Current;
                        var cardImage = await YgoCardImage.LoadFromIdAsync(card.Id);
                        if (cardImage != null)
                            imgList.Add(cardImage);

                        horizontalLayout.Children.Add(GenCard(card.Name, card.Desc, cardImage!));
                    }
                    verticalLayout.Children.Add(horizontalLayout);

                    if (isEmpty)
                        break;
                }

                // 下一页
                if (cards.MoveNext())
                {
                    var textBox = new TextBox();
                    textBox.Text = "如需查找更多，请输入继续查";
                    textBox.FontPaint.Color = SKColors.IndianRed;
                    textBox.FontPaint.TextSize = FontSize;
                    textBox.FontPaint.TextAlign = SKTextAlign.Right;
                    textBox.Height = 50;
                    verticalLayout.Children.Add(textBox);
                }

                return (renderer.Draw(grid), cardIndex);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                foreach (var img in imgList)
                    img.Dispose();
            }
        }
    }
}
