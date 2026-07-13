using System.Windows;
using System.Windows.Controls;

namespace BIMBrain.UI
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "BIMBrain";
            Width = 400;
            Height = 300;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var text = new TextBlock
            {
                Text = "BIMBrain iniciado com sucesso.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16
            };

            Content = text;
        }
    }
}
