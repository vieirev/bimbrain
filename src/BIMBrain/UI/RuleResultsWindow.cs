using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMBrain.Knowledge;
using BIMBrain.Rules;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Grid = System.Windows.Controls.Grid;

namespace BIMBrain.UI
{
    public class RuleResultsWindow : Window
    {
        private readonly UIDocument _uiDoc;
        private readonly RuleActionService _actionService = new RuleActionService();
        private readonly List<RuleResult> _results;
        private RuleResult _selected;

        private readonly TextBlock _detailName;
        private readonly TextBlock _detailSeverity;
        private readonly TextBlock _detailCount;
        private readonly TextBlock _detailMessage;
        private readonly Button _btnSelect;
        private readonly Button _btnNavigate;
        private readonly Button _btnKnowledge;
        private readonly Button _btnInspector;

        public RuleResultsWindow(UIDocument uiDoc, List<RuleResult> results)
        {
            _uiDoc = uiDoc;
            _results = results ?? new List<RuleResult>();

            Title = "BIMBrain — Resultados das Regras";
            Width = 720;
            Height = 520;
            MinWidth = 560;
            MinHeight = 400;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var list = new ListBox { Margin = new Thickness(10) };
            list.SelectionChanged += (s, e) =>
            {
                var item = list.SelectedItem as ListBoxItem;
                _selected = item?.Tag as RuleResult;
                UpdateDetail();
            };

            foreach (var r in _results)
            {
                list.Items.Add(BuildRow(r));
            }

            Grid.SetRow(list, 0);
            root.Children.Add(list);

            var detail = new Border
            {
                Margin = new Thickness(10, 0, 10, 10),
                Padding = new Thickness(10),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Background = Brushes.WhiteSmoke
            };

            var detailGrid = new Grid();
            detailGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            detailGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            detailGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            detailGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            _detailName = AddDetailRow(detailGrid, 0, "Regra:", "-");
            _detailSeverity = AddDetailRow(detailGrid, 1, "Severidade:", "-");
            _detailCount = AddDetailRow(detailGrid, 2, "Elementos afetados:", "0");

            _detailMessage = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 6, 0, 0)
            };
            detailGrid.Children.Add(_detailMessage);
            Grid.SetRow(_detailMessage, 3);

            detail.Child = detailGrid;
            Grid.SetRow(detail, 1);
            root.Children.Add(detail);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 0, 10, 10)
            };

            _btnSelect = new Button { Content = "Selecionar", Width = 90, Margin = new Thickness(4) };
            _btnNavigate = new Button { Content = "Navegar", Width = 90, Margin = new Thickness(4) };
            _btnKnowledge = new Button { Content = "Conhecimento", Width = 100, Margin = new Thickness(4) };
            _btnInspector = new Button { Content = "Inspector", Width = 90, Margin = new Thickness(4) };
            var btnExport = new Button { Content = "Exportar", Width = 90, Margin = new Thickness(4) };
            var btnClose = new Button { Content = "Fechar", Width = 90, Margin = new Thickness(4) };

            _btnSelect.Click += (s, e) => OnSelect();
            _btnNavigate.Click += (s, e) => OnNavigate();
            _btnKnowledge.Click += (s, e) => OnKnowledge();
            _btnInspector.Click += (s, e) => OnInspector();
            btnExport.Click += (s, e) => TaskDialog.Show("BIMBrain", "Funcionalidade em desenvolvimento.");
            btnClose.Click += (s, e) => Close();

            buttonPanel.Children.Add(_btnSelect);
            buttonPanel.Children.Add(_btnNavigate);
            buttonPanel.Children.Add(_btnKnowledge);
            buttonPanel.Children.Add(_btnInspector);
            buttonPanel.Children.Add(btnExport);
            buttonPanel.Children.Add(btnClose);

            Grid.SetRow(buttonPanel, 2);
            root.Children.Add(buttonPanel);

            Content = root;

            if (_results.Count > 0)
            {
                list.SelectedIndex = 0;
            }
            else
            {
                UpdateDetail();
            }
        }

        private static ListBoxItem BuildRow(RuleResult r)
        {
            var item = new ListBoxItem { Tag = r };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(130) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var icon = new TextBlock
            {
                Text = StatusGlyph(r),
                FontSize = 14,
                Margin = new Thickness(0, 0, 8, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var name = new TextBlock
            {
                Text = r.RuleName,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center
            };

            var status = new TextBlock
            {
                Text = r.Success ? "Aprovada" : (r.Severity == RuleSeverity.Error ? "Erro" : "Aviso"),
                VerticalAlignment = VerticalAlignment.Center
            };

            var message = new TextBlock
            {
                Text = r.Message,
                VerticalAlignment = VerticalAlignment.Center,
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            grid.Children.Add(icon);
            Grid.SetColumn(icon, 0);
            grid.Children.Add(name);
            Grid.SetColumn(name, 1);
            grid.Children.Add(status);
            Grid.SetColumn(status, 2);
            grid.Children.Add(message);
            Grid.SetColumn(message, 3);

            item.Content = grid;
            return item;
        }

        private static TextBlock AddDetailRow(Grid grid, int row, string label, string value)
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 2)
            };

            panel.Children.Add(new TextBlock
            {
                Text = label + " ",
                FontWeight = FontWeights.SemiBold
            });

            var valueBlock = new TextBlock { Text = value };
            panel.Children.Add(valueBlock);

            grid.Children.Add(panel);
            Grid.SetRow(panel, row);

            return valueBlock;
        }

        private void UpdateDetail()
        {
            if (_selected == null)
            {
                _detailName.Text = "-";
                _detailSeverity.Text = "-";
                _detailCount.Text = "0";
                _detailMessage.Text = "Selecione uma regra para ver os detalhes.";
                _btnSelect.IsEnabled = false;
                _btnNavigate.IsEnabled = false;
                _btnKnowledge.IsEnabled = false;
                _btnInspector.IsEnabled = false;
                return;
            }

            _detailName.Text = _selected.RuleName;
            _detailSeverity.Text = SeverityText(_selected);
            _detailCount.Text = _selected.AffectedElements.Count.ToString();
            _detailMessage.Text = _selected.Message;

            var hasElements = _selected.AffectedElements.Count > 0;
            _btnSelect.IsEnabled = hasElements;
            _btnNavigate.IsEnabled = hasElements;
            _btnKnowledge.IsEnabled = true;
            _btnInspector.IsEnabled = true;
        }

        private void OnSelect()
        {
            if (_selected == null) return;

            var ok = _actionService.Select(_uiDoc, _selected);
            if (!ok)
            {
                TaskDialog.Show("BIMBrain", "Nenhum elemento afetado para selecionar.");
            }
        }

        private void OnNavigate()
        {
            if (_selected == null) return;

            var ok = _actionService.Navigate(_uiDoc, _selected);
            if (!ok)
            {
                TaskDialog.Show("BIMBrain", "Nenhum elemento afetado para navegar.");
            }
        }

        private void OnKnowledge()
        {
            if (_selected == null) return;

            var doc = new KnowledgeRepository().GetByRuleName(_selected.RuleName);
            if (!doc.Exists)
            {
                TaskDialog.Show("BIMBrain", "Documentação não encontrada.");
                return;
            }

            var window = new KnowledgeWindow(doc);
            window.ShowDialog();
        }

        private void OnInspector()
        {
            if (_selected == null) return;

            var window = new RuleInspectorWindow(_uiDoc, _selected);
            window.ShowDialog();
        }

        private static string StatusGlyph(RuleResult r)
        {
            if (r.Success) return "✔";
            if (r.Severity == RuleSeverity.Error) return "❌";
            return "⚠";
        }

        private static string SeverityText(RuleResult r)
        {
            switch (r.Severity)
            {
                case RuleSeverity.Error:
                    return "Erro";
                case RuleSeverity.Warning:
                    return "Aviso";
                default:
                    return "Informação";
            }
        }
    }
}
