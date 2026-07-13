using Autodesk.Revit.DB;
using System.Windows;
using System.Windows.Controls;
using Grid = System.Windows.Controls.Grid;

namespace BIMBrain.UI
{
    public class MainWindow : Window
    {
        private TextBox _questionBox;
        private TextBlock _responseText;
        private Document _doc;

        public MainWindow(string projectName, Document doc)
        {
            _doc = doc;
            Title = "BIMBrain";
            Width = 450;
            Height = 350;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var grid = new Grid();
            grid.Margin = new Thickness(20);
            for (int i = 0; i < 7; i++)
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

            var separator = new Separator { Margin = new Thickness(0, 10, 0, 10) };
            Grid.SetRow(separator, 3);

            var lblQuestion = new TextBlock
            {
                Text = "Faça uma pergunta:",
                FontWeight = FontWeights.SemiBold
            };
            Grid.SetRow(lblQuestion, 4);

            var inputPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 5, 0, 0)
            };

            _questionBox = new TextBox
            {
                Width = 280,
                Height = 25
            };

            var consultarBtn = new Button
            {
                Content = "Consultar",
                Width = 80,
                Height = 25,
                Margin = new Thickness(10, 0, 0, 0)
            };
            consultarBtn.Click += OnConsultarClick;

            inputPanel.Children.Add(_questionBox);
            inputPanel.Children.Add(consultarBtn);
            Grid.SetRow(inputPanel, 5);

            var responsePanel = new StackPanel
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            var lblResponse = new TextBlock
            {
                Text = "Resposta:",
                FontWeight = FontWeights.SemiBold
            };

            _responseText = new TextBlock
            {
                Text = "Aguardando consulta...",
                Margin = new Thickness(10, 5, 0, 0),
                TextWrapping = TextWrapping.Wrap
            };

            responsePanel.Children.Add(lblResponse);
            responsePanel.Children.Add(_responseText);
            Grid.SetRow(responsePanel, 6);

            grid.Children.Add(title);
            grid.Children.Add(lblProject);
            grid.Children.Add(valProject);
            grid.Children.Add(separator);
            grid.Children.Add(lblQuestion);
            grid.Children.Add(inputPanel);
            grid.Children.Add(responsePanel);

            Content = grid;
        }

        private void OnConsultarClick(object sender, RoutedEventArgs e)
        {
            var question = _questionBox.Text.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(question))
            {
                _responseText.Text = "Digite uma pergunta.";
                return;
            }

            if (question == "quantas tomadas existem"
                || question == "quantas tomadas existem?"
                || question.StartsWith("quantas tomadas existem"))
            {
                var collector = new FilteredElementCollector(_doc)
                    .OfCategory(BuiltInCategory.OST_ElectricalFixtures)
                    .OfClass(typeof(FamilyInstance));

                var count = collector.ToElements().Count;
                _responseText.Text = $"Foram encontradas {count} tomadas.";
            }
            else
            {
                _responseText.Text = "Pergunta ainda não suportada pelo BIMBrain.";
            }
        }
    }
}
