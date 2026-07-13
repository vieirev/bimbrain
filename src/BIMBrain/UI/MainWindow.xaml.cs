using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
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

            var historyPanel = new StackPanel { Margin = new Thickness(0, 10, 0, 0) };

            var historyHeader = new StackPanel { Orientation = Orientation.Horizontal };

            var lblHistory = new TextBlock
            {
                Text = "Histórico:",
                FontWeight = FontWeights.SemiBold
            };

            var copyBtn = new Button
            {
                Content = "Copiar",
                Width = 60,
                Height = 22,
                Margin = new Thickness(10, 0, 0, 0),
                FontSize = 11
            };
            copyBtn.Click += OnCopyHistoryClick;

            historyHeader.Children.Add(lblHistory);
            historyHeader.Children.Add(copyBtn);

            _historyList = new ListBox
            {
                Height = 90,
                Margin = new Thickness(0, 5, 0, 0)
            };

            historyPanel.Children.Add(historyHeader);
            historyPanel.Children.Add(_historyList);
            Grid.SetRow(historyPanel, 7);

            grid.Children.Add(historyPanel);

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

            var normalized = Normalize(question);
            string answer;

            if (normalized.StartsWith("quantas tomadas existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_ElectricalFixtures);
                answer = $"Foram encontradas {count} tomadas.";
            }
            else if (normalized.StartsWith("quantas interruptores existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_LightingDevices);
                answer = $"Foram encontrados {count} interruptores.";
            }
            else if (normalized.StartsWith("quantas luminarias existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_LightingFixtures);
                answer = $"Foram encontradas {count} luminárias.";
            }
            else if (normalized.StartsWith("quantas quadros existem"))
            {
                var count = CountByCategory(BuiltInCategory.OST_ElectricalEquipment);
                answer = $"Foram encontrados {count} quadros.";
            }
            else if (normalized.StartsWith("quantas niveis existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Level)).ToElements().Count;
                answer = $"Foram encontrados {count} níveis.";
            }
            else if (normalized.StartsWith("quantas ambientes existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .ToElements().Count;
                answer = $"Foram encontrados {count} ambientes.";
                if (count == 0)
                    answer += " Nenhum ambiente encontrado no arquivo local — se o modelo de arquitetura estiver vinculado por link, os ambientes dele ainda não são contados pelo BIMBrain.";
            }
            else if (normalized.StartsWith("quantas circuitos eletricos existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(ElectricalSystem))
                    .ToElements().Count;
                answer = $"Foram encontrados {count} circuitos elétricos.";
                if (count == 0)
                    answer += " Nenhum circuito elétrico encontrado — os elementos podem ainda não estar atrelados a circuitos nos quadros.";
            }
            else if (normalized.StartsWith("qual o total de area construida"))
            {
                var (sum, filled, total) = SumParameter(
                    BuiltInCategory.OST_Rooms, BuiltInParameter.ROOM_AREA);
                var area = sum * 0.092903;
                if (filled < total)
                    answer = $"Área total construída: {area:F2} m² (considerando apenas {filled} de {total} ambientes com área preenchida).";
                else if (area == 0)
                    answer = "Área total construída: 0,00 m². Nenhum ambiente encontrado no arquivo local — se o modelo de arquitetura estiver vinculado por link, os ambientes dele ainda não são contados pelo BIMBrain.";
                else
                    answer = $"Área total construída: {area:F2} m².";
            }
            else if (normalized.StartsWith("qual o comprimento total de eletrodutos"))
            {
                var (sum, filled, total) = SumParameter(
                    BuiltInCategory.OST_Conduit, BuiltInParameter.CURVE_ELEM_LENGTH);
                var length = sum * 0.3048;
                if (filled < total)
                    answer = $"Comprimento total de eletrodutos: {length:F2} m (considerando apenas {filled} de {total} eletrodutos com comprimento preenchido).";
                else
                    answer = $"Comprimento total de eletrodutos: {length:F2} m.";
            }
            else if (normalized.StartsWith("qual a potencia total instalada"))
            {
                var (sumFixtures, filledFixtures, totalFixtures) = SumPowerParameter(
                    BuiltInCategory.OST_ElectricalFixtures);
                var (sumEquip, filledEquip, totalEquip) = SumPowerParameter(
                    BuiltInCategory.OST_ElectricalEquipment);
                var totalPower = sumFixtures + sumEquip;
                var filled = filledFixtures + filledEquip;
                var total = totalFixtures + totalEquip;
                if (totalPower == 0)
                {
                    var circuitCount = new FilteredElementCollector(_doc)
                        .OfClass(typeof(ElectricalSystem))
                        .ToElements().Count;
                    if (circuitCount == 0)
                        answer = "Potência total instalada: 0,00 VA. Nenhum circuito elétrico encontrado — a potência é consolidada a partir dos circuitos atrelados aos elementos.";
                    else if (filled < total)
                        answer = $"Potência total instalada: {totalPower:F2} VA (considerando apenas {filled} de {total} elementos com potência preenchida).";
                    else
                        answer = $"Potência total instalada: {totalPower:F2} VA.";
                }
                else if (filled < total)
                    answer = $"Potência total instalada: {totalPower:F2} VA (considerando apenas {filled} de {total} elementos com potência preenchida).";
                else
                    answer = $"Potência total instalada: {totalPower:F2} VA.";
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

        private (double sum, int filled, int total) SumParameter(
            BuiltInCategory category, BuiltInParameter parameter, Type classType = null)
        {
            var collector = new FilteredElementCollector(_doc)
                .OfCategory(category);

            if (classType != null)
                collector = collector.OfClass(classType);
            else
                collector = collector.WhereElementIsNotElementType();

            double sum = 0;
            int filled = 0;
            int total = 0;
            foreach (var element in collector.ToElements())
            {
                total++;
                var param = element.get_Parameter(parameter);
                if (param != null && param.HasValue)
                {
                    sum += param.AsDouble();
                    filled++;
                }
            }
            return (sum, filled, total);
        }

        private (double sum, int filled, int total) SumPowerParameter(BuiltInCategory category)
        {
            var collector = new FilteredElementCollector(_doc)
                .OfCategory(category)
                .OfClass(typeof(FamilyInstance));

            string[] loadNames = {
                "Apparent Load Phase A", "Apparent Load",
                "Carga Aparente Fase A", "Carga Aparente",
                "Potência", "Potencia"
            };

            double sum = 0;
            int filled = 0;
            int total = 0;
            foreach (Element element in collector.ToElements())
            {
                total++;
                bool found = false;
                foreach (var name in loadNames)
                {
                    var param = element.LookupParameter(name);
                    if (param != null && param.HasValue)
                    {
                        sum += param.AsDouble();
                        filled++;
                        found = true;
                        break;
                    }
                }
            }
            return (sum, filled, total);
        }

        private static string Normalize(string text)
        {
            text = text.Trim().ToLowerInvariant();
            text = text.TrimEnd('?', '!', '.', ' ');
            text = RemoveDiacritics(text);
            text = text.Replace("quantos ", "quantas ");
            text = text.Replace("qual e ", "qual ");
            text = text.Replace("  ", " ");
            return text;
        }

        private static string RemoveDiacritics(string text)
        {
            var formD = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(capacity: formD.Length);
            foreach (var ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private void OnCopyHistoryClick(object sender, RoutedEventArgs e)
        {
            if (_historyList.Items.Count == 0)
            {
                _responseText.Text = "Nenhum histórico para copiar.";
                return;
            }

            var lines = _historyList.Items
                .Cast<string>()
                .ToArray();
            var text = string.Join(Environment.NewLine, lines);
            Clipboard.SetText(text);
            _responseText.Text = $"Histórico copiado ({lines.Length} perguntas).";
        }
    }
}
