using System.Windows;
using System.Windows.Controls;

namespace BIMBrain.UI
{
    public class MainWindow : Window
    {
        public MainWindow(
            string projectName,
            int totalElements,
            int totalViews,
            int totalLevels)
        {
            Title = "BIMBrain";
            Width = 400;
            Height = 300;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var grid = new Grid();
            grid.Margin = new Thickness(20);
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            var title = new TextBlock
            {
                Text = "BIMBrain",
                FontSize = 20,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetRow(title, 0);

            var lblProject = new TextBlock
            {
                Text = "Projeto:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(lblProject, 1);

            var valProject = new TextBlock
            {
                Text = projectName,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 0, 0, 0)
            };
            Grid.SetRow(valProject, 2);

            var lblElements = new TextBlock
            {
                Text = "Elementos: " + totalElements,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(lblElements, 3);

            var stack = new StackPanel
            {
                Margin = new Thickness(0, 10, 0, 0),
                Orientation = Orientation.Horizontal
            };
            stack.Children.Add(new TextBlock
            {
                Text = "Vistas: " + totalViews,
                Margin = new Thickness(0, 0, 20, 0)
            });
            stack.Children.Add(new TextBlock
            {
                Text = "Níveis: " + totalLevels
            });
            Grid.SetRow(stack, 4);

            grid.Children.Add(title);
            grid.Children.Add(lblProject);
            grid.Children.Add(valProject);
            grid.Children.Add(lblElements);
            grid.Children.Add(stack);

            Content = grid;
        }
    }
}
