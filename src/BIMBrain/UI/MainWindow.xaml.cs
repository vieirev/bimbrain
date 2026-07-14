using Autodesk.Revit.DB;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Grid = System.Windows.Controls.Grid;

namespace BIMBrain.UI
{
    public class MainWindow : Window
    {
        private readonly Document _doc;
        private readonly string _projectName;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly QuestionProcessor _processor;

        private readonly TextBox _questionBox;
        private readonly ResponsePanel _responsePanel;
        private readonly HistoryPanel _historyPanel;
        private readonly StatusBar _statusBar;

        private bool _hasPlaceholder = true;
        private const string PlaceholderText = "Ex: Quantas tomadas existem? | Resumo do projeto | Contexto do projeto";

        public MainWindow(string projectName, Document doc)
        {
            _doc = doc;
            _projectName = projectName;
            _processor = new QuestionProcessor(doc);

            Title = "BIMBrain";
            Width = 900;
            Height = 700;
            MinWidth = 700;
            MinHeight = 500;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            _responsePanel = new ResponsePanel();
            _historyPanel = new HistoryPanel();
            _statusBar = new StatusBar();

            _questionBox = new TextBox
            {
                Height = 26,
                FontSize = 13,
                VerticalContentAlignment = VerticalAlignment.Center,
                Text = PlaceholderText,
                Foreground = Brushes.Gray
            };
            _questionBox.GotFocus += OnQuestionGotFocus;
            _questionBox.LostFocus += OnQuestionLostFocus;
            _questionBox.KeyDown += OnQuestionKeyDown;

            _historyPanel.OnItemSelected += OnHistoryItemSelected;

            Content = BuildLayout();
            UpdateStatusBarModelCount();
        }

        private Grid BuildLayout()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.Margin = new Thickness(16);

            BuildRegion1(grid);
            BuildRegion2(grid);
            BuildRegion3(grid);
            BuildRegion4(grid);

            return grid;
        }

        private void BuildRegion1(Grid grid)
        {
            var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

            panel.Children.Add(new TextBlock
            {
                Text = "BIMBrain",
                FontSize = 22,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            });

            panel.Children.Add(new Separator { Margin = new Thickness(0, 8, 0, 8) });

            var infoGrid = new Grid();
            infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            infoGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            infoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            infoGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            AddInfoRow(infoGrid, "Projeto:", _projectName, 0);
            AddInfoRow(infoGrid, "Modelo ativo:", _doc.Title ?? "Não identificado", 1);

            panel.Children.Add(infoGrid);

            Grid.SetRow(panel, 0);
            grid.Children.Add(panel);
        }

        private static void AddInfoRow(Grid infoGrid, string label, string value, int row)
        {
            var lbl = new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 2, 8, 2)
            };
            Grid.SetRow(lbl, row);
            Grid.SetColumn(lbl, 0);
            infoGrid.Children.Add(lbl);

            var val = new TextBlock
            {
                Text = value,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 2, 0, 2)
            };
            Grid.SetRow(val, row);
            Grid.SetColumn(val, 1);
            infoGrid.Children.Add(val);
        }

        private void BuildRegion2(Grid grid)
        {
            var panel = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };

            panel.Children.Add(new TextBlock
            {
                Text = "Consulta",
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                Margin = new Thickness(0, 0, 0, 5)
            });

            var row = new Grid();
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            Grid.SetColumn(_questionBox, 0);
            _questionBox.Margin = new Thickness(0, 0, 6, 0);
            row.Children.Add(_questionBox);

            var executarBtn = new Button
            {
                Content = "Executar",
                Width = 85,
                Height = 28,
                FontSize = 12,
                IsDefault = true
            };
            executarBtn.Click += OnConsultarClick;
            Grid.SetColumn(executarBtn, 1);
            row.Children.Add(executarBtn);

            panel.Children.Add(row);

            Grid.SetRow(panel, 1);
            grid.Children.Add(panel);
        }

        private void BuildRegion3(Grid grid)
        {
            var innerGrid = new Grid();
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(280) });
            innerGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            Grid.SetColumn(_responsePanel.TextBox, 0);
            innerGrid.Children.Add(_responsePanel.TextBox);

            Grid.SetColumn(_historyPanel.Panel, 1);
            innerGrid.Children.Add(_historyPanel.Panel);

            Grid.SetRow(innerGrid, 2);
            grid.Children.Add(innerGrid);
        }

        private void BuildRegion4(Grid grid)
        {
            Grid.SetRow(_statusBar.Panel, 3);
            grid.Children.Add(_statusBar.Panel);
        }

        private void OnQuestionGotFocus(object sender, RoutedEventArgs e)
        {
            if (_hasPlaceholder)
            {
                _questionBox.Text = "";
                _questionBox.Foreground = Brushes.Black;
                _hasPlaceholder = false;
            }
        }

        private void OnQuestionLostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_questionBox.Text))
            {
                _questionBox.Text = PlaceholderText;
                _questionBox.Foreground = Brushes.Gray;
                _hasPlaceholder = true;
            }
        }

        private void OnQuestionKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                OnConsultarClick(sender, e);
        }

        private void OnHistoryItemSelected(HistoryItem item)
        {
            _responsePanel.ShowSuccess(_projectName, item.Question, item.Answer, item.ElapsedMs, item.Origin);
        }

        private static string DetermineOrigin(string answer)
        {
            if (answer.StartsWith("Pergunta ainda não suportada") ||
                answer.StartsWith("A IA demorou") ||
                answer.StartsWith("O BIMBrain tentou"))
                return "IA";
            return "BIMBrain";
        }

        private async void OnConsultarClick(object sender, RoutedEventArgs e)
        {
            var question = _hasPlaceholder ? "" : _questionBox.Text.Trim();

            if (string.IsNullOrEmpty(question))
            {
                _responsePanel.ShowSimple("Atenção", "Nenhuma pergunta informada.\nDigite uma pergunta antes de consultar.");
                return;
            }

            _responsePanel.ShowSimple("Consultando...", "Processando pergunta...");
            _statusBar.SetStatus("Consultando...");
            _stopwatch.Restart();

            try
            {
                var answer = await _processor.ProcessQuestionAsync(question);
                var elapsedMs = _stopwatch.ElapsedMilliseconds;
                var origin = DetermineOrigin(answer);

                if (origin == "IA")
                {
                    _responsePanel.ShowError(answer);
                }
                else
                {
                    _responsePanel.ShowSuccess(_projectName, question, answer, elapsedMs, origin);
                }

                _historyPanel.AddEntry(question, answer, elapsedMs, origin);
                _statusBar.SetTime(elapsedMs);
                _statusBar.SetOrigin(origin);
                _statusBar.SetStatus("Pronto");
            }
            catch (Exception ex)
            {
                _responsePanel.ShowError(ex.Message);
                _statusBar.SetStatus("Erro");
            }
        }

        private void UpdateStatusBarModelCount()
        {
            try
            {
                var ctx = new ModelContext(_doc);
                _statusBar.SetModelCount(ctx.Links.Count + 1);
            }
            catch
            {
                _statusBar.SetModelCount(1);
            }
        }
    }
}
