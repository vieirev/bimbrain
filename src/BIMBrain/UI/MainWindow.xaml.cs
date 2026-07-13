using Autodesk.Revit.DB;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Grid = System.Windows.Controls.Grid;

namespace BIMBrain.UI
{
    public class MainWindow : Window
    {
        private TextBox _questionBox;
        private TextBlock _responseText;
        private ListBox _historyList;
        private Document _doc;

        public MainWindow(string projectName, Document doc)
        {
            _doc = doc;
            Title = "BIMBrain";
            Width = 450;
            Height = 450;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var grid = new Grid();
            grid.Margin = new Thickness(20);
            for (int i = 0; i < 8; i++)
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

            var lblHistory = new TextBlock
            {
                Text = "Histórico:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 10, 0, 0)
            };
            Grid.SetRow(lblHistory, 7);

            _historyList = new ListBox
            {
                Height = 90,
                Margin = new Thickness(0, 5, 0, 0)
            };

            grid.Children.Add(lblHistory);
            grid.Children.Add(_historyList);

            Content = grid;
        }

        private void OnConsultarClick(object sender, RoutedEventArgs e)
        {
            var question = _questionBox.Text.Trim();

            if (string.IsNullOrEmpty(question))
            {
                _responseText.Text = "Digite uma pergunta.";
                return;
            }

            var normalized = question.ToLowerInvariant();
            string answer;

            if (normalized == "quantas tomadas existem"
                || normalized == "quantas tomadas existem?"
                || normalized.StartsWith("quantas tomadas existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_ElectricalFixtures);
                answer = $"Foram encontradas {count} tomadas.";
            }
            else if (normalized == "quantos interruptores existem"
                || normalized == "quantos interruptores existem?"
                || normalized.StartsWith("quantos interruptores existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_LightingDevices);
                answer = $"Foram encontrados {count} interruptores.";
            }
            else if (normalized == "quantas luminárias existem"
                || normalized == "quantas luminarias existem"
                || normalized == "quantas luminárias existem?"
                || normalized == "quantas luminarias existem?"
                || normalized.StartsWith("quantas luminárias")
                || normalized.StartsWith("quantas luminarias"))
            {
                var count = CountByCategory(BuiltInCategory.OST_LightingFixtures);
                answer = $"Foram encontradas {count} luminárias.";
            }
            else if (normalized == "quantos quadros existem"
                || normalized == "quantos quadros existem?"
                || normalized.StartsWith("quantos quadros existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_ElectricalEquipment);
                answer = $"Foram encontrados {count} quadros.";
            }
            else if (normalized == "quantos níveis existem"
                || normalized == "quantos níveis existem?"
                || normalized == "quantos niveis existem"
                || normalized == "quantos niveis existem?"
                || normalized.StartsWith("quantos níveis")
                || normalized.StartsWith("quantos niveis"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Level)).ToElements().Count;
                answer = $"Foram encontrados {count} níveis.";
            }
            else
            {
                answer = "Pergunta ainda não suportada pelo BIMBrain.";
            }

            _responseText.Text = answer;
            _historyList.Items.Add($"Q: {question}  →  {answer}");
            _historyList.ScrollIntoView(_historyList.Items[_historyList.Items.Count - 1]);
        }

        private int CountByCategory(BuiltInCategory category, string familyNameFilter = null)
        {
            var collector = new FilteredElementCollector(_doc)
                .OfCategory(category)
                .OfClass(typeof(FamilyInstance));

            if (familyNameFilter == null)
                return collector.ToElements().Count;

            return collector
                .ToElements()
                .OfType<FamilyInstance>()
                .Count(fi => fi.Symbol.Family.Name.IndexOf(familyNameFilter, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
