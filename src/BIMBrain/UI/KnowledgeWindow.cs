using BIMBrain.Knowledge;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BIMBrain.UI
{
    public class KnowledgeWindow : Window
    {
        public KnowledgeWindow(KnowledgeDocument doc)
        {
            Title = "BIMBrain — " + (string.IsNullOrEmpty(doc.Title) ? "Conhecimento" : doc.Title);
            Width = 640;
            Height = 480;
            MinWidth = 420;
            MinHeight = 320;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var textBlock = new TextBlock
            {
                Text = doc.Markdown,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(12),
                FontFamily = new FontFamily("Consolas"),
                FontSize = 12
            };

            var scroll = new ScrollViewer
            {
                Content = textBlock,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            Content = scroll;
        }
    }
}
