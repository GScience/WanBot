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

        public static async Task<SKImage> GenCardsImageAsync(UIRenderer renderer, string search, List<YgoCard> cards, int gridWidth = 3, int gridHeight = 2)
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
            title.Text = $"查找结果：{search}";
            verticalLayout.Children.Add(title);

            try
            {
                IEnumerable<YgoCard> cardEnumerator = cards;

                var cardIndex = 0;
                for (var x = 0; x < gridWidth; ++x)
                {
                    var horizontalLayout = new HorizontalLayout();
                    horizontalLayout.Space = 10;
                    for (var y = 0; y < gridHeight; ++y)
                    {
                        var card = cards[cardIndex++];
                        var cardImage = await YgoCardImage.LoadFromIdAsync(card.Id);
                        if (cardImage != null)
                            imgList.Add(cardImage);

                        horizontalLayout.Children.Add(GenCard(card.Name, card.Desc, cardImage!));

                        if (cards.Count <= cardIndex)
                            break;
                    }
                    verticalLayout.Children.Add(horizontalLayout);

                    if (cards.Count <= cardIndex)
                        break;
                }

                return renderer.Draw(grid);
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
