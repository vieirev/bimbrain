using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Grid = System.Windows.Controls.Grid;

namespace BIMBrain.UI
{
    public class MainWindow : Window
    {
        private TextBox _questionBox;
        private Document _doc;
        private string _projectName;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private TextBlock _responseStatus;
        private TextBlock _responseProject;
        private TextBlock _responseQuestion;
        private TextBlock _responseResultLabel;
        private TextBlock _responseResultValue;
        private TextBlock _responseElapsed;

        private ListBox _historyList;
        private static readonly HttpClient _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(120)
        };

        public MainWindow(string projectName, Document doc)
        {
            _doc = doc;
            _projectName = projectName;
            Title = "BIMBrain";
            Width = 520;
            Height = 600;
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

            var testIaBtn = new Button
            {
                Content = "Testar IA",
                Width = 80,
                Height = 25,
                Margin = new Thickness(10, 0, 0, 0)
            };
            testIaBtn.Click += OnTestIaClick;

            inputPanel.Children.Add(_questionBox);
            inputPanel.Children.Add(consultarBtn);
            inputPanel.Children.Add(testIaBtn);
            Grid.SetRow(inputPanel, 5);

            var responsePanel = new StackPanel
            {
                Margin = new Thickness(0, 10, 0, 0)
            };

            _responseStatus = new TextBlock
            {
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var responseSep = new Separator { Margin = new Thickness(0, 0, 0, 5) };

            _responseProject = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 2)
            };

            _responseQuestion = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 5)
            };

            _responseResultLabel = new TextBlock
            {
                Text = "Resultado:",
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 3)
            };

            _responseResultValue = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10, 0, 0, 5)
            };

            _responseElapsed = new TextBlock
            {
                Margin = new Thickness(0, 3, 0, 0)
            };

            SetInitialState();

            responsePanel.Children.Add(_responseStatus);
            responsePanel.Children.Add(responseSep);
            responsePanel.Children.Add(_responseProject);
            responsePanel.Children.Add(_responseQuestion);
            responsePanel.Children.Add(_responseResultLabel);
            responsePanel.Children.Add(_responseResultValue);
            responsePanel.Children.Add(_responseElapsed);
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

        private void SetInitialState()
        {
            _responseStatus.Text = "Aguardando consulta...";
            _responseProject.Visibility = System.Windows.Visibility.Collapsed;
            _responseQuestion.Text = "Nenhuma pergunta informada.\nDigite uma pergunta antes de consultar.";
            _responseResultLabel.Visibility = System.Windows.Visibility.Collapsed;
            _responseResultValue.Visibility = System.Windows.Visibility.Collapsed;
            _responseElapsed.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetResponseSuccess(string question, string answer, long elapsedMs)
        {
            _responseStatus.Text = "Consulta concluída";
            _responseProject.Text = $"Projeto: {_projectName}";
            _responseProject.Visibility = System.Windows.Visibility.Visible;
            _responseQuestion.Text = $"Pergunta: {question}";
            _responseQuestion.Visibility = System.Windows.Visibility.Visible;
            _responseResultLabel.Visibility = System.Windows.Visibility.Visible;
            _responseResultValue.Text = answer;
            _responseResultValue.Visibility = System.Windows.Visibility.Visible;
            _responseElapsed.Text = $"Tempo: {elapsedMs} ms";
            _responseElapsed.Visibility = System.Windows.Visibility.Visible;
        }

        private void SetResponseError(string message)
        {
            _responseStatus.Text = "Consulta não realizada";
            _responseProject.Visibility = System.Windows.Visibility.Collapsed;
            _responseQuestion.Visibility = System.Windows.Visibility.Collapsed;
            _responseResultLabel.Visibility = System.Windows.Visibility.Collapsed;
            _responseResultValue.Visibility = System.Windows.Visibility.Collapsed;
            _responseElapsed.Visibility = System.Windows.Visibility.Collapsed;
            _responseQuestion.Text = $"Motivo:\n{message}";
            _responseQuestion.Visibility = System.Windows.Visibility.Visible;
        }

        private void SetResponseSimple(string status, string message)
        {
            _responseStatus.Text = status;
            _responseProject.Visibility = System.Windows.Visibility.Collapsed;
            _responseQuestion.Text = message;
            _responseQuestion.Visibility = System.Windows.Visibility.Visible;
            _responseResultLabel.Visibility = System.Windows.Visibility.Collapsed;
            _responseResultValue.Visibility = System.Windows.Visibility.Collapsed;
            _responseElapsed.Visibility = System.Windows.Visibility.Collapsed;
        }

        private async void OnConsultarClick(object sender, RoutedEventArgs e)
        {
            var question = _questionBox.Text.Trim();

            if (string.IsNullOrEmpty(question))
            {
                SetResponseSimple("Atenção", "Nenhuma pergunta informada.\nDigite uma pergunta antes de consultar.");
                return;
            }

            var normalized = Normalize(question);
            _stopwatch.Restart();
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
            else if (normalized.StartsWith("quantas vistas existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(View))
                    .ToElements()
                    .Cast<View>()
                    .Count(v => !v.IsTemplate);
                answer = $"Foram encontradas {count} vistas.";
            }
            else if (normalized.StartsWith("quantas folhas existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(ViewSheet))
                    .ToElements().Count;
                answer = $"Foram encontradas {count} folhas.";
            }
            else if (normalized.StartsWith("qual categoria possui mais elementos"))
            {
                answer = FindCategoryWithMostElements();
            }
            else if (normalized.StartsWith("quais familias estao carregadas"))
            {
                answer = ListLoadedFamilies();
            }
            else
            {
                SetResponseSimple("Consultando IA...", "Aguardando resposta da IA.\nIsso pode levar até 2 minutos para perguntas mais complexas.");
                answer = await TryResolveWithOllamaAsync(question);
            }

            var elapsedMs = _stopwatch.ElapsedMilliseconds;

            if (answer != null)
            {
                if (answer.StartsWith("Pergunta ainda não suportada") ||
                    answer.StartsWith("A IA demorou") ||
                    answer.StartsWith("O BIMBrain tentou"))
                {
                    SetResponseError(answer);
                }
                else
                {
                    SetResponseSuccess(question, answer, elapsedMs);
                }
            }

            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            _historyList.Items.Add($"[{timestamp}] {question}  →  {answer} ({elapsedMs} ms)");
            _historyList.ScrollIntoView(_historyList.Items[_historyList.Items.Count - 1]);
        }

        private async Task<string> TryResolveWithOllamaAsync(string question)
        {
            try
            {
                var payload = new
                {
                    model = "qwen3",
                    messages = new[] { new { role = "user", content = question } },
                    tools = GetToolSchemas(),
                    stream = false
                };
                var requestJson = JsonSerializer.Serialize(payload);
                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                var httpResponse = await _httpClient.PostAsync("http://localhost:11434/api/chat", httpContent);
                var responseBody = await httpResponse.Content.ReadAsStringAsync();
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var chatResponse = JsonSerializer.Deserialize<OllamaChatResponse>(responseBody, opts);

                if (chatResponse?.Message?.ToolCalls != null && chatResponse.Message.ToolCalls.Count > 0)
                {
                    var toolCall = chatResponse.Message.ToolCalls[0];
                    var result = ExecuteToolByName(toolCall.Function.Name);
                    if (result != null)
                        return result;
                }

                return "O BIMBrain tentou entender sua pergunta com IA, mas não encontrou uma função correspondente. Pergunta ainda não suportada.";
            }
            catch (TaskCanceledException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BIMBrain-IA] Timeout na pergunta: \"{question}\" | {ex.GetType().FullName} — {ex.Message}");
                return "A IA demorou demais para responder. Tente novamente ou reformule a pergunta.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[BIMBrain-IA] Pergunta: \"{question}\" | Erro: {ex.GetType().FullName} — {ex.Message}");
                return "Pergunta ainda não suportada pelo BIMBrain.";
            }
        }

        private static object[] GetToolSchemas()
        {
            return new object[]
            {
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_tomadas",
                        description = "Retorna quantas tomadas elétricas (Electrical Fixtures) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_interruptores",
                        description = "Retorna quantos interruptores (Lighting Devices) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_luminarias",
                        description = "Retorna quantas luminárias (Lighting Fixtures) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_quadros",
                        description = "Retorna quantos quadros elétricos (Electrical Equipment) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_niveis",
                        description = "Retorna quantos níveis (Levels) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_ambientes",
                        description = "Retorna quantos ambientes/cômodos (Rooms) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_circuitos_eletricos",
                        description = "Retorna quantos circuitos elétricos (ElectricalSystems) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "area_construida",
                        description = "Calcula a área total construída somando a área de todos os ambientes (Rooms) do modelo.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "comprimento_eletrodutos",
                        description = "Calcula o comprimento total de eletrodutos (Conduit) no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "potencia_total_instalada",
                        description = "Calcula a potência total instalada somando a carga aparente de tomadas e quadros elétricos.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_vistas",
                        description = "Retorna quantas vistas (Views, excluindo templates) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "contar_folhas",
                        description = "Retorna quantas folhas/pranchas (ViewSheets) existem no modelo BIM.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "categoria_com_mais_elementos",
                        description = "Retorna qual categoria (entre tomadas, interruptores, luminárias, quadros, níveis, ambientes e eletrodutos) possui mais elementos no modelo.",
                        parameters = new { type = "object", properties = new { } }
                    }
                },
                new
                {
                    type = "function",
                    function = new
                    {
                        name = "familias_carregadas",
                        description = "Lista todas as famílias carregadas no modelo com suas respectivas quantidades de instâncias.",
                        parameters = new { type = "object", properties = new { } }
                    }
                }
            };
        }

        private string ExecuteToolByName(string toolName)
        {
            switch (toolName)
            {
                case "contar_tomadas":
                    return $"Foram encontradas {CountByCategory(BuiltInCategory.OST_ElectricalFixtures)} tomadas.";
                case "contar_interruptores":
                    return $"Foram encontrados {CountByCategory(BuiltInCategory.OST_LightingDevices)} interruptores.";
                case "contar_luminarias":
                    return $"Foram encontradas {CountByCategory(BuiltInCategory.OST_LightingFixtures)} luminárias.";
                case "contar_quadros":
                    return $"Foram encontrados {CountByCategory(BuiltInCategory.OST_ElectricalEquipment)} quadros.";
                case "contar_niveis":
                    return $"Foram encontrados {new FilteredElementCollector(_doc).OfClass(typeof(Level)).ToElements().Count} níveis.";
                case "contar_ambientes":
                    {
                        var count = new FilteredElementCollector(_doc)
                            .OfCategory(BuiltInCategory.OST_Rooms)
                            .WhereElementIsNotElementType()
                            .ToElements().Count;
                        var msg = $"Foram encontrados {count} ambientes.";
                        if (count == 0)
                            msg += " Nenhum ambiente encontrado no arquivo local — se o modelo de arquitetura estiver vinculado por link, os ambientes dele ainda não são contados pelo BIMBrain.";
                        return msg;
                    }
                case "contar_circuitos_eletricos":
                    {
                        var count = new FilteredElementCollector(_doc)
                            .OfClass(typeof(ElectricalSystem))
                            .ToElements().Count;
                        var msg = $"Foram encontrados {count} circuitos elétricos.";
                        if (count == 0)
                            msg += " Nenhum circuito elétrico encontrado — os elementos podem ainda não estar atrelados a circuitos nos quadros.";
                        return msg;
                    }
                case "area_construida":
                    {
                        var (sum, filled, total) = SumParameter(BuiltInCategory.OST_Rooms, BuiltInParameter.ROOM_AREA);
                        var area = sum * 0.092903;
                        if (filled < total)
                            return $"Área total construída: {area:F2} m² (considerando apenas {filled} de {total} ambientes com área preenchida).";
                        if (area == 0)
                            return "Área total construída: 0,00 m². Nenhum ambiente encontrado no arquivo local — se o modelo de arquitetura estiver vinculado por link, os ambientes dele ainda não são contados pelo BIMBrain.";
                        return $"Área total construída: {area:F2} m².";
                    }
                case "comprimento_eletrodutos":
                    {
                        var (sum, filled, total) = SumParameter(BuiltInCategory.OST_Conduit, BuiltInParameter.CURVE_ELEM_LENGTH);
                        var length = sum * 0.3048;
                        if (filled < total)
                            return $"Comprimento total de eletrodutos: {length:F2} m (considerando apenas {filled} de {total} eletrodutos com comprimento preenchido).";
                        return $"Comprimento total de eletrodutos: {length:F2} m.";
                    }
                case "potencia_total_instalada":
                    {
                        var (sumFixtures, filledFixtures, totalFixtures) = SumPowerParameter(BuiltInCategory.OST_ElectricalFixtures);
                        var (sumEquip, filledEquip, totalEquip) = SumPowerParameter(BuiltInCategory.OST_ElectricalEquipment);
                        var totalPower = sumFixtures + sumEquip;
                        var filled = filledFixtures + filledEquip;
                        var total = totalFixtures + totalEquip;
                        if (totalPower == 0)
                        {
                            var circuitCount = new FilteredElementCollector(_doc)
                                .OfClass(typeof(ElectricalSystem))
                                .ToElements().Count;
                            if (circuitCount == 0)
                                return "Potência total instalada: 0,00 VA. Nenhum circuito elétrico encontrado — a potência é consolidada a partir dos circuitos atrelados aos elementos.";
                            if (filled < total)
                                return $"Potência total instalada: {totalPower:F2} VA (considerando apenas {filled} de {total} elementos com potência preenchida).";
                            return $"Potência total instalada: {totalPower:F2} VA.";
                        }
                        if (filled < total)
                            return $"Potência total instalada: {totalPower:F2} VA (considerando apenas {filled} de {total} elementos com potência preenchida).";
                        return $"Potência total instalada: {totalPower:F2} VA.";
                    }
                case "contar_vistas":
                    {
                        var count = new FilteredElementCollector(_doc)
                            .OfClass(typeof(View))
                            .ToElements()
                            .Cast<View>()
                            .Count(v => !v.IsTemplate);
                        return $"Foram encontradas {count} vistas.";
                    }
                case "contar_folhas":
                    {
                        var count = new FilteredElementCollector(_doc)
                            .OfClass(typeof(ViewSheet))
                            .ToElements().Count;
                        return $"Foram encontradas {count} folhas.";
                    }
                case "categoria_com_mais_elementos":
                    return FindCategoryWithMostElements();
                case "familias_carregadas":
                    return ListLoadedFamilies();
                default:
                    return null;
            }
        }

        private async void OnTestIaClick(object sender, RoutedEventArgs e)
        {
            SetResponseSimple("Teste de IA", "Conectando ao Ollama...");

            try
            {
                var json = "{\"model\":\"qwen3\",\"messages\":[{\"role\":\"user\",\"content\":\"Diga apenas \\\"BIMBrain conectado com sucesso\\\".\"}],\"stream\":false}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:11434/api/chat", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                var chatResponse = JsonSerializer.Deserialize<OllamaChatResponse>(responseBody);
                var reply = chatResponse?.Message?.Content ?? "Resposta vazia do Ollama.";
                SetResponseSimple("Teste de IA", $"Resposta do Ollama:\n{reply}");
            }
            catch (HttpRequestException)
            {
                SetResponseSimple("Teste de IA", "Não foi possível conectar ao Ollama. Verifique se o Ollama está rodando em localhost:11434.");
            }
            catch (TaskCanceledException)
            {
                SetResponseSimple("Teste de IA", "Conexão com Ollama excedeu o tempo limite.");
            }
            catch (JsonException)
            {
                SetResponseSimple("Teste de IA", "Resposta inválida recebida do Ollama (erro de JSON).");
            }
            catch (Exception ex)
            {
                SetResponseSimple("Teste de IA", $"Erro: {ex.Message}");
            }
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

        private string FindCategoryWithMostElements()
        {
            var categories = new List<(string name, int count)>
            {
                ("tomadas", CountByCategory(BuiltInCategory.OST_ElectricalFixtures)),
                ("interruptores", CountByCategory(BuiltInCategory.OST_LightingDevices)),
                ("luminárias", CountByCategory(BuiltInCategory.OST_LightingFixtures)),
                ("quadros", CountByCategory(BuiltInCategory.OST_ElectricalEquipment)),
                ("níveis", new FilteredElementCollector(_doc).OfClass(typeof(Level)).ToElements().Count),
                ("ambientes", new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType().ToElements().Count),
                ("eletrodutos", new FilteredElementCollector(_doc).OfCategory(BuiltInCategory.OST_Conduit)
                    .WhereElementIsNotElementType().ToElements().Count),
            };

            var max = categories.OrderByDescending(c => c.count).First();
            return $"A categoria com mais elementos é '{max.name}', com {max.count} elementos.";
        }

        private string ListLoadedFamilies()
        {
            var groups = new FilteredElementCollector(_doc)
                .OfClass(typeof(FamilyInstance))
                .ToElements()
                .Cast<FamilyInstance>()
                .GroupBy(fi => fi.Symbol.Family.Name)
                .OrderBy(g => g.Key)
                .Select(g => $"{g.Key}: {g.Count()} instâncias")
                .ToList();

            if (groups.Count == 0)
                return "Nenhuma família carregada encontrada.";

            var sb = new StringBuilder();
            sb.AppendLine($"Foram encontradas {groups.Count} famílias carregadas:");
            foreach (var line in groups)
                sb.AppendLine($"- {line}");
            return sb.ToString().TrimEnd();
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
                SetResponseSimple("Histórico", "Nenhum histórico para copiar.");
                return;
            }

            var lines = _historyList.Items
                .Cast<string>()
                .ToArray();
            var text = string.Join(Environment.NewLine, lines);
            Clipboard.SetText(text);
            SetResponseSimple("Histórico", $"Histórico copiado ({lines.Length} perguntas).");
        }
    }

    internal class OllamaChatResponse
    {
        public OllamaMessage Message { get; set; }
    }

    internal class OllamaMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }

        [JsonPropertyName("tool_calls")]
        public List<OllamaToolCall> ToolCalls { get; set; }
    }

    internal class OllamaToolCall
    {
        public string Type { get; set; }
        public OllamaFunctionCall Function { get; set; }
    }

    internal class OllamaFunctionCall
    {
        public string Name { get; set; }
        public JsonElement Arguments { get; set; }
    }
}
