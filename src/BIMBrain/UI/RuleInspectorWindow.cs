using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMBrain.Knowledge;
using BIMBrain.Rules;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Grid = System.Windows.Controls.Grid;

namespace BIMBrain.UI
{
    public class RuleInspectorWindow : Window
    {
        private readonly UIDocument _uiDoc;
        private readonly RuleResult _result;
        private InspectorItem _selected;

        private readonly TextBlock _detailId;
        private readonly TextBlock _detailName;
        private readonly TextBlock _detailCategory;
        private readonly TextBlock _detailFamily;
        private readonly TextBlock _detailType;
        private readonly TextBlock _detailLevel;
        private readonly Button _btnSelect;
        private readonly Button _btnNavigate;

        public RuleInspectorWindow(UIDocument uiDoc, RuleResult result)
        {
            _uiDoc = uiDoc;
            _result = result;

            Title = "BIMBrain — Inspector: " + (result?.RuleName ?? "");
            Width = 760;
            Height = 620;
            MinWidth = 560;
            MinHeight = 460;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var root = new Grid();
            root.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            root.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var listView = new ListView { Margin = new Thickness(10) };
            var gridView = new GridView();

            gridView.Columns.Add(new GridViewColumn { Header = "Id", DisplayMemberBinding = new System.Windows.Data.Binding("Id"), Width = 70 });
            gridView.Columns.Add(new GridViewColumn { Header = "Nome", DisplayMemberBinding = new System.Windows.Data.Binding("Name"), Width = 160 });
            gridView.Columns.Add(new GridViewColumn { Header = "Categoria", DisplayMemberBinding = new System.Windows.Data.Binding("Category"), Width = 120 });
            gridView.Columns.Add(new GridViewColumn { Header = "Família", DisplayMemberBinding = new System.Windows.Data.Binding("Family"), Width = 120 });
            gridView.Columns.Add(new GridViewColumn { Header = "Tipo", DisplayMemberBinding = new System.Windows.Data.Binding("Type"), Width = 140 });
            gridView.Columns.Add(new GridViewColumn { Header = "Nível", DisplayMemberBinding = new System.Windows.Data.Binding("Level"), Width = 100 });

            listView.View = gridView;
            listView.SelectionChanged += (s, e) =>
            {
                _selected = listView.SelectedItem as InspectorItem;
                UpdateDetail();
            };

            foreach (var item in BuildItems())
            {
                listView.Items.Add(item);
            }

            Grid.SetRow(listView, 0);
            root.Children.Add(listView);

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
            detailGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            detailGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            _detailId = AddDetailRow(detailGrid, 0, "Id:", "-");
            _detailName = AddDetailRow(detailGrid, 1, "Nome:", "-");
            _detailCategory = AddDetailRow(detailGrid, 2, "Categoria:", "-");
            _detailFamily = AddDetailRow(detailGrid, 3, "Família:", "-");
            _detailType = AddDetailRow(detailGrid, 4, "Tipo:", "-");
            _detailLevel = AddDetailRow(detailGrid, 5, "Nível:", "-");

            detail.Child = detailGrid;
            Grid.SetRow(detail, 1);
            root.Children.Add(detail);

            var recommendation = BuildRecommendationPanel(
                new RecommendationRepository().GetByRuleName(result?.RuleName));
            Grid.SetRow(recommendation, 2);
            root.Children.Add(recommendation);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(10, 0, 10, 10)
            };

            _btnSelect = new Button { Content = "Selecionar", Width = 90, Margin = new Thickness(4) };
            _btnNavigate = new Button { Content = "Navegar", Width = 90, Margin = new Thickness(4) };
            var btnClose = new Button { Content = "Fechar", Width = 90, Margin = new Thickness(4) };

            _btnSelect.Click += (s, e) => OnSelect();
            _btnNavigate.Click += (s, e) => OnNavigate();
            btnClose.Click += (s, e) => Close();

            buttonPanel.Children.Add(_btnSelect);
            buttonPanel.Children.Add(_btnNavigate);
            buttonPanel.Children.Add(btnClose);

            Grid.SetRow(buttonPanel, 3);
            root.Children.Add(buttonPanel);

            Content = root;

            if (listView.Items.Count > 0)
            {
                listView.SelectedIndex = 0;
            }
            else
            {
                UpdateDetail();
            }
        }

        private List<InspectorItem> BuildItems()
        {
            var items = new List<InspectorItem>();

            if (_result?.AffectedElements == null || _uiDoc?.Document == null)
            {
                return items;
            }

            var doc = _uiDoc.Document;
            foreach (var id in _result.AffectedElements)
            {
                var element = doc.GetElement(id);
                if (element == null)
                {
                    items.Add(new InspectorItem
                    {
                        ElementId = id,
                        Id = id.ToString(),
                        Name = "(elemento não encontrado)",
                        Category = "-",
                        Family = "-",
                        Type = "-",
                        Level = "-"
                    });
                    continue;
                }

                items.Add(new InspectorItem
                {
                    ElementId = id,
                    Id = id.ToString(),
                    Name = element.Name ?? "-",
                    Category = element.Category?.Name ?? "-",
                    Family = GetFamily(element),
                    Type = GetType(element),
                    Level = GetLevel(element, doc)
                });
            }

            return items;
        }

        private static string GetFamily(Element e)
        {
            if (e is FamilyInstance fi && fi.Symbol?.Family != null)
            {
                return fi.Symbol.Family.Name;
            }

            return e.Category?.Name ?? "-";
        }

        private static string GetType(Element e)
        {
            if (e is FamilyInstance fi && fi.Symbol != null)
            {
                return fi.Symbol.Name;
            }

            return e.Name ?? "-";
        }

        private static string GetLevel(Element e, Document doc)
        {
            var levelId = e.LevelId;
            if (levelId != null && levelId != ElementId.InvalidElementId)
            {
                var level = doc.GetElement(levelId);
                if (level != null)
                {
                    return level.Name;
                }
            }

            var param = e.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM);
            if (param != null && param.HasValue)
            {
                var level = doc.GetElement(param.AsElementId());
                if (level != null)
                {
                    return level.Name;
                }
            }

            return "-";
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

        private static Border BuildRecommendationPanel(RuleRecommendation rec)
        {
            var border = new Border
            {
                Margin = new Thickness(10, 0, 10, 10),
                Padding = new Thickness(10),
                BorderBrush = Brushes.LightSteelBlue,
                BorderThickness = new Thickness(1),
                Background = Brushes.AliceBlue
            };

            var stack = new StackPanel();

            stack.Children.Add(new TextBlock
            {
                Text = "RECOMENDAÇÃO",
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.DarkSlateBlue,
                Margin = new Thickness(0, 0, 0, 6)
            });

            var hasContent = rec != null &&
                (!string.IsNullOrEmpty(rec.Problem) || !string.IsNullOrEmpty(rec.Impact) ||
                 (rec.RecommendedActions != null && rec.RecommendedActions.Count > 0));

            if (!hasContent)
            {
                stack.Children.Add(new TextBlock
                {
                    Text = "Sem recomendação cadastrada para esta regra.",
                    TextWrapping = TextWrapping.Wrap
                });
                border.Child = stack;
                return border;
            }

            if (!string.IsNullOrEmpty(rec.Title))
            {
                stack.Children.Add(new TextBlock
                {
                    Text = rec.Title,
                    FontStyle = FontStyles.Italic,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 0, 0, 6)
                });
            }

            stack.Children.Add(LabelValue("Problema:", rec.Problem ?? "-"));
            stack.Children.Add(LabelValue("Impacto:", rec.Impact ?? "-"));

            var actions = rec.RecommendedActions != null && rec.RecommendedActions.Count > 0
                ? string.Join("\n", rec.RecommendedActions.Select((a, i) => $"{i + 1}. {a}"))
                : "-";
            stack.Children.Add(LabelValue("Como corrigir:", actions));
            stack.Children.Add(LabelValue("Referência:", rec.Reference ?? "-"));

            border.Child = stack;
            return border;
        }

        private static UIElement LabelValue(string label, string value)
        {
            var panel = new StackPanel { Margin = new Thickness(0, 2, 0, 6) };

            panel.Children.Add(new TextBlock
            {
                Text = label,
                FontWeight = FontWeights.SemiBold
            });

            panel.Children.Add(new TextBlock
            {
                Text = value,
                TextWrapping = TextWrapping.Wrap
            });

            return panel;
        }

        private void UpdateDetail()
        {
            if (_selected == null)
            {
                _detailId.Text = "-";
                _detailName.Text = "-";
                _detailCategory.Text = "-";
                _detailFamily.Text = "-";
                _detailType.Text = "-";
                _detailLevel.Text = "-";
                _btnSelect.IsEnabled = false;
                _btnNavigate.IsEnabled = false;
                return;
            }

            _detailId.Text = _selected.Id;
            _detailName.Text = _selected.Name;
            _detailCategory.Text = _selected.Category;
            _detailFamily.Text = _selected.Family;
            _detailType.Text = _selected.Type;
            _detailLevel.Text = _selected.Level;

            _btnSelect.IsEnabled = true;
            _btnNavigate.IsEnabled = true;
        }

        private void OnSelect()
        {
            if (_selected == null) return;

            _uiDoc.Selection.SetElementIds(new List<ElementId> { _selected.ElementId });
        }

        private void OnNavigate()
        {
            if (_selected == null) return;

            _uiDoc.ShowElements(new List<ElementId> { _selected.ElementId });
        }

        private class InspectorItem
        {
            public ElementId ElementId { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public string Family { get; set; }
            public string Type { get; set; }
            public string Level { get; set; }
        }
    }
}
