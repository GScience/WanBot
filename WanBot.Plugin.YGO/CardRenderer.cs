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
        public static Grid GenCard(string cardName, string cardDesc, SKImage cardImage)
        {
            var grid = new Grid();

            var horizontalLayout = new HorizontalLayout();
            horizontalLayout.Space = 10;

            // 卡图
            var cardImg = new ImageBox();
            cardImg.Width = 397;
            cardImg.Height = 578;
            cardImg.Image = cardImage;
            cardImg.Margin = new Margin(0, 0);

            horizontalLayout.Children.Add(cardImg);

            // 卡片介绍
            var helper = new VerticalHelper();
            helper.Box(cardName, 38, 5).Box(cardDesc, 21, 15, SKTextAlign.Left).Width(397).Space(10).Center();
            horizontalLayout.Children.Add(helper.VerticalLayout);
            grid.Children.Add(horizontalLayout);

            return grid;
        }

        public static async Task<SKImage> GenCardsImageAsync(UIRenderer renderer, string search, List<YgoCard> cards, int gridWidth = 3, int gridHeight = 2)
        {
            var grid = new Grid();

            var gridBg = new Rectangle();
            gridBg.Paint.Color = SKColors.White;
            grid.Children.Add(gridBg);

            var verticalLayout = new VerticalLayout();
            verticalLayout.Space = 10;
            var imgList = new Dictionary<int, SKImage>();
            var tasks = new List<Task>();
            grid.Children.Add(verticalLayout);

            var title = new TextBox();
            title.FontPaint.TextSize = 48;
            title.Text = $"查找结果：{search}";
            verticalLayout.Children.Add(title);

            try
            {
                IEnumerable<YgoCard> cardEnumerator = cards;

                var max = gridWidth * gridHeight;
                if (cards.Count > max)
                    cardEnumerator = cards.Take(max);

                foreach (var card in cardEnumerator)
                {
                    var task = YgoCardImage.LoadFromIdAsync(card.Id);
                    task.GetAwaiter().OnCompleted(() => imgList[card.Id] = task.Result!);
                    tasks.Add(task);
                }

                foreach (var task in tasks)
                    await task;

                var cardIndex = 0;
                for (var x = 0; x < gridWidth; ++x)
                {
                    var horizontalLayout = new HorizontalLayout();
                    horizontalLayout.Space = 10;
                    for (var y = 0; y < gridHeight; ++y)
                    {
                        var card = cards[cardIndex++];
                        var cardImage = imgList[card.Id];

                        horizontalLayout.Children.Add(GenCard(card.Name, card.Desc, cardImage));

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
                foreach (var pair in imgList)
                    pair.Value?.Dispose();
                grid.Dispose();
            }
        }
    }
}
