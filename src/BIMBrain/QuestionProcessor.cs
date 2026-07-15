using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using BIMBrain.Classification;
using BIMBrain.Knowledge;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BIMBrain
{
    public class QuestionProcessor
    {
        private readonly Document _doc;
        internal static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(120)
        };

        public QuestionProcessor(Document doc)
        {
            _doc = doc;
        }

        public async Task<string> ProcessQuestionAsync(CopilotContext context)
        {
            var question = context?.Question ?? "";
            return await ProcessQuestionAsync(question);
        }

        public async Task<string> ProcessQuestionAsync(string question)
        {
            var normalized = Normalize(question);
            if (normalized.StartsWith("quantas tomadas existem"))
                return $"Foram encontradas {CountByCategory(BuiltInCategory.OST_ElectricalFixtures)} tomadas.";

            if (normalized.StartsWith("quantas interruptores existem"))
                return $"Foram encontrados {CountByCategory(BuiltInCategory.OST_LightingDevices)} interruptores.";

            if (normalized.StartsWith("quantas luminarias existem"))
                return $"Foram encontradas {CountByCategory(BuiltInCategory.OST_LightingFixtures)} luminárias.";

            if (normalized.StartsWith("quantas quadros existem"))
                return $"Foram encontrados {CountByCategory(BuiltInCategory.OST_ElectricalEquipment)} quadros.";

            if (normalized.StartsWith("quantas niveis existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Level)).ToElements().Count;
                return $"Foram encontrados {count} níveis.";
            }

            if (normalized.StartsWith("quantas ambientes existem"))
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

            if (normalized.StartsWith("quantas circuitos eletricos existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(ElectricalSystem))
                    .ToElements().Count;
                var msg = $"Foram encontrados {count} circuitos elétricos.";
                if (count == 0)
                    msg += " Nenhum circuito elétrico encontrado — os elementos podem ainda não estar atrelados a circuitos nos quadros.";
                return msg;
            }

            if (normalized.Contains("circuitos") &&
                (normalized.StartsWith("listar") ||
                 normalized.StartsWith("mostrar") ||
                 normalized.StartsWith("informacoes") ||
                 normalized.StartsWith("resumo") ||
                 normalized.StartsWith("estrutura")))
            {
                var analyzer = new ElectricalCircuitAnalyzer(_doc);
                var circuits = analyzer.Analyze();
                return FormatCircuitSummary(circuits);
            }

            if (normalized == "listar paineis" ||
                normalized == "resumo dos paineis" ||
                normalized == "informacoes dos paineis" ||
                normalized == "estrutura dos paineis" ||
                normalized == "circuitos por painel")
            {
                var analyzer = new PanelAnalyzer(_doc);
                var panels = analyzer.Analyze();
                return FormatPanelSummary(panels);
            }

            if (normalized.StartsWith("qual o total de area construida"))
            {
                var (sum, filled, total) = SumParameter(
                    BuiltInCategory.OST_Rooms, BuiltInParameter.ROOM_AREA);
                var area = sum * 0.092903;
                if (filled < total)
                    return $"Área total construída: {area:F2} m² (considerando apenas {filled} de {total} ambientes com área preenchida).";
                if (area == 0)
                    return "Área total construída: 0,00 m². Nenhum ambiente encontrado no arquivo local — se o modelo de arquitetura estiver vinculado por link, os ambientes dele ainda não são contados pelo BIMBrain.";
                return $"Área total construída: {area:F2} m².";
            }

            if (normalized.StartsWith("qual o comprimento total de eletrodutos"))
            {
                var (sum, filled, total) = SumParameter(
                    BuiltInCategory.OST_Conduit, BuiltInParameter.CURVE_ELEM_LENGTH);
                var length = sum * 0.3048;
                if (filled < total)
                    return $"Comprimento total de eletrodutos: {length:F2} m (considerando apenas {filled} de {total} eletrodutos com comprimento preenchido).";
                return $"Comprimento total de eletrodutos: {length:F2} m.";
            }

            if (normalized.StartsWith("qual a potencia total instalada"))
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

            if (normalized.StartsWith("quantas vistas existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(View))
                    .ToElements()
                    .Cast<View>()
                    .Count(v => !v.IsTemplate);
                return $"Foram encontradas {count} vistas.";
            }

            if (normalized.StartsWith("quantas folhas existem"))
            {
                var count = new FilteredElementCollector(_doc)
                    .OfClass(typeof(ViewSheet))
                    .ToElements().Count;
                return $"Foram encontradas {count} folhas.";
            }

            if (normalized.StartsWith("qual categoria possui mais elementos"))
                return FindCategoryWithMostElements();

            if (normalized.StartsWith("quais familias estao carregadas"))
                return ListLoadedFamilies();

            if (normalized.Contains("por nivel") || normalized.StartsWith("distribuicao"))
            {
                var catInfo = DetectClassification(normalized);
                if (catInfo != null)
                {
                    var (type, alias) = catInfo.Value;
                    var analyzer = new ElementLevelAnalyzer(_doc);
                    var distribution = analyzer.Analyze(type);
                    return FormatDistribution(distribution, alias);
                }
            }

            if (normalized == "resumo do projeto" || normalized.EndsWith("resumo do projeto"))
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== RESUMO DO PROJETO ===");
                sb.AppendLine();

                var projectName = _doc.Title;
                if (string.IsNullOrEmpty(projectName))
                    projectName = "Não identificado";
                sb.AppendLine($"Projeto: {projectName}");

                var nivelCount = new FilteredElementCollector(_doc)
                    .OfClass(typeof(Level)).ToElements().Count;
                sb.AppendLine($"Níveis: {nivelCount}");

                var ambienteCount = new FilteredElementCollector(_doc)
                    .OfCategory(BuiltInCategory.OST_Rooms)
                    .WhereElementIsNotElementType()
                    .ToElements().Count;
                sb.AppendLine($"Ambientes: {ambienteCount}");

                sb.AppendLine($"Tomadas: {CountByCategory(BuiltInCategory.OST_ElectricalFixtures)}");
                sb.AppendLine($"Interruptores: {CountByCategory(BuiltInCategory.OST_LightingDevices)}");
                sb.AppendLine($"Luminárias: {CountByCategory(BuiltInCategory.OST_LightingFixtures)}");
                sb.AppendLine($"Quadros: {CountByCategory(BuiltInCategory.OST_ElectricalEquipment)}");

                var circuitoCount = new FilteredElementCollector(_doc)
                    .OfClass(typeof(ElectricalSystem))
                    .ToElements().Count;
                sb.AppendLine($"Circuitos: {circuitoCount}");

                var (conduitLength, _, _) = SumParameter(BuiltInCategory.OST_Conduit, BuiltInParameter.CURVE_ELEM_LENGTH);
                sb.AppendLine($"Comprimento de conduítes: {conduitLength * 0.3048:F2} m");

                var (area, _, _) = SumParameter(BuiltInCategory.OST_Rooms, BuiltInParameter.ROOM_AREA);
                sb.AppendLine($"Área construída: {area * 0.092903:F2} m²");

                sb.AppendLine();
                sb.Append(ListLoadedFamilies());

                return sb.ToString();
            }

            if (normalized == "modelos carregados" || normalized == "informacoes dos modelos" || normalized == "contexto do projeto")
            {
                var ctx = new ModelContext(_doc);
                var sb = new StringBuilder();
                var eqSep = new string('=', 50);
                var sep = new string('-', 50);

                sb.AppendLine(eqSep);
                sb.AppendLine();
                sb.AppendLine("CONTEXTO DO PROJETO");
                sb.AppendLine();
                sb.AppendLine("Projeto ativo");
                sb.AppendLine();
                sb.AppendLine(ctx.MainDocument.Name);
                sb.AppendLine();
                sb.AppendLine(sep);
                sb.AppendLine();
                sb.AppendLine("Resumo");
                sb.AppendLine();
                sb.AppendLine("Projeto principal:");
                sb.AppendLine("1");
                sb.AppendLine();
                sb.AppendLine("Modelos vinculados:");
                sb.AppendLine(ctx.Links.Count.ToString());
                sb.AppendLine();
                sb.AppendLine("Modelos carregados:");
                sb.AppendLine(ctx.Links.Count(l => l.IsLoaded).ToString());
                sb.AppendLine();
                sb.AppendLine("Modelos descarregados:");
                sb.AppendLine(ctx.Links.Count(l => !l.IsLoaded).ToString());
                sb.AppendLine();
                sb.AppendLine("Total de documentos:");
                sb.AppendLine((ctx.Links.Count + 1).ToString());
                sb.AppendLine();
                sb.AppendLine(sep);
                sb.AppendLine();
                sb.AppendLine("Modelos");
                sb.AppendLine();

                foreach (var link in ctx.Links)
                {
                    sb.Append("✔ ");
                    sb.AppendLine(link.Name);
                    sb.AppendLine();
                    sb.AppendLine("Tipo:");
                    sb.AppendLine(link.Type);
                    sb.AppendLine();
                    sb.AppendLine("Status:");
                    sb.AppendLine(link.IsLoaded ? "Carregado" : "Descarregado");
                    sb.AppendLine();

                    if (link.IsLoaded)
                    {
                        sb.AppendLine("Níveis:");
                        sb.AppendLine(link.LevelCount.ToString());
                        sb.AppendLine();
                        sb.AppendLine("Rooms:");
                        sb.AppendLine(link.RoomCount.ToString());
                        sb.AppendLine();
                        sb.AppendLine("Elementos:");
                        sb.AppendLine(link.ElementCount.ToString());
                    }

                    sb.AppendLine();
                    sb.AppendLine(sep);
                    sb.AppendLine();
                }

                if (ctx.Links.Count == 0)
                {
                    sb.AppendLine("Nenhum modelo vinculado encontrado.");
                    sb.AppendLine();
                    sb.AppendLine(sep);
                    sb.AppendLine();
                }

                sb.AppendLine("Diagnóstico");
                sb.AppendLine();

                if (ctx.HasDuplicateNames)
                    sb.AppendLine("⚠ Foram encontrados modelos com o mesmo nome.");
                else
                    sb.AppendLine("✔ Nenhum modelo duplicado.");

                var unloadedCount = ctx.Links.Count(l => !l.IsLoaded);
                if (unloadedCount > 0)
                    sb.AppendLine("⚠ Existem modelos descarregados.");
                else
                    sb.AppendLine("✔ Todos os links estão carregados.");

                if (ctx.Links.Count > 0)
                    sb.AppendLine("✔ Projeto possui modelos vinculados.");
                else
                    sb.AppendLine("⚠ Nenhum modelo vinculado foi encontrado.");

                sb.AppendLine();
                sb.AppendLine(eqSep);

                return sb.ToString();
            }

            if (normalized == "verificar modelo" ||
                normalized == "verificar integridade" ||
                normalized == "analisar modelo" ||
                normalized == "existem elementos sem circuito" ||
                normalized == "diagnostico de integridade")
            {
                var analyzer = new ModelIntegrityAnalyzer(_doc);
                var result = analyzer.Analyze();
                return FormatIntegrityReport(result);
            }

            if (normalized.StartsWith("diagnostico") ||
                normalized.StartsWith("analisar projeto") ||
                normalized.StartsWith("analisar modelo") ||
                normalized.StartsWith("saude do projeto") ||
                normalized.StartsWith("health check"))
            {
                return BuildProjectDiagnosis();
            }

            return await TryResolveWithOllamaAsync(question);
        }

        private string BuildProjectDiagnosis()
        {
            var sb = new StringBuilder();
            var sep = new string('-', 32);
            var eqSep = new string('=', 30);

            var projectName = _doc.Title ?? "Não identificado";
            var modelCount = 1;
            try { modelCount = new ModelContext(_doc).Links.Count + 1; } catch { }

            var nivelCount = new FilteredElementCollector(_doc)
                .OfClass(typeof(Level)).ToElements().Count;
            var tomadaCount = CountByCategory(BuiltInCategory.OST_ElectricalFixtures);
            var interruptorCount = CountByCategory(BuiltInCategory.OST_LightingDevices);
            var luminariaCount = CountByCategory(BuiltInCategory.OST_LightingFixtures);
            var quadroCount = CountByCategory(BuiltInCategory.OST_ElectricalEquipment);
            var circuitoCount = new FilteredElementCollector(_doc)
                .OfClass(typeof(ElectricalSystem)).ToElements().Count;
            var ambienteCount = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_Rooms)
                .WhereElementIsNotElementType()
                .ToElements().Count;
            var (area, _, _) = SumParameter(BuiltInCategory.OST_Rooms, BuiltInParameter.ROOM_AREA);
            var areaM2 = area * 0.092903;
            var (conduitLength, _, _) = SumParameter(BuiltInCategory.OST_Conduit, BuiltInParameter.CURVE_ELEM_LENGTH);
            var conduitM = conduitLength * 0.3048;

            sb.AppendLine(eqSep);
            sb.AppendLine();
            sb.AppendLine("DIAGNÓSTICO DO MODELO");
            sb.AppendLine();
            sb.AppendLine(eqSep);

            sb.AppendLine();
            sb.AppendLine("Projeto");
            sb.AppendLine(projectName);

            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();

            sb.AppendLine("Resumo");
            sb.AppendLine();
            sb.AppendLine($"Modelos: {modelCount}");
            sb.AppendLine($"Níveis: {nivelCount}");
            sb.AppendLine($"Tomadas: {tomadaCount}");
            sb.AppendLine($"Interruptores: {interruptorCount}");
            sb.AppendLine($"Luminárias: {luminariaCount}");
            sb.AppendLine($"Quadros: {quadroCount}");
            sb.AppendLine($"Circuitos: {circuitoCount}");
            sb.AppendLine($"Ambientes: {ambienteCount}");
            sb.AppendLine($"Área construída: {areaM2:F2} m²");
            sb.AppendLine($"Comprimento de conduítes: {conduitM:F2} m");

            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();

            sb.AppendLine("Análise");
            sb.AppendLine();
            int warnings = 0;

            if (modelCount > 1)
                sb.AppendLine("✔ Modelos vinculados encontrados.");
            else
            { sb.AppendLine("⚠ Nenhum modelo vinculado encontrado."); warnings++; }

            if (tomadaCount > 0)
                sb.AppendLine("✔ Modelo possui tomadas.");
            else
            { sb.AppendLine("⚠ Nenhuma tomada encontrada."); warnings++; }

            if (interruptorCount > 0)
                sb.AppendLine("✔ Modelo possui interruptores.");
            else
            { sb.AppendLine("⚠ Nenhum interruptor encontrado."); warnings++; }

            if (luminariaCount > 0)
                sb.AppendLine("✔ Modelo possui luminárias.");
            else
            { sb.AppendLine("⚠ Nenhuma luminária encontrada."); warnings++; }

            if (quadroCount > 0)
                sb.AppendLine("✔ Modelo possui quadros.");
            else
            { sb.AppendLine("⚠ Nenhum quadro encontrado."); warnings++; }

            if (circuitoCount > 0)
                sb.AppendLine("✔ Circuitos elétricos encontrados.");
            else
            { sb.AppendLine("⚠ Nenhum circuito encontrado."); warnings++; }

            if (ambienteCount > 0)
                sb.AppendLine("✔ Ambientes encontrados.");
            else
            { sb.AppendLine("⚠ Nenhum ambiente encontrado."); warnings++; }

            if (areaM2 > 0)
                sb.AppendLine("✔ Área construída calculada.");
            else
            { sb.AppendLine("⚠ Área construída igual a zero."); warnings++; }

            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Resultado Geral");
            sb.AppendLine();
            sb.AppendLine("Status:");

            if (warnings > 0)
            {
                sb.AppendLine("ATENÇÃO");
                sb.AppendLine();
                sb.AppendLine("O modelo pode estar incompleto");
                sb.AppendLine("ou ainda em desenvolvimento.");
            }
            else
            {
                sb.AppendLine("OK");
                sb.AppendLine();
                sb.AppendLine("Modelo elétrico dentro do esperado.");
            }

            return sb.ToString();
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
                var httpResponse = await HttpClient.PostAsync("http://localhost:11434/api/chat", httpContent);
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
                new { type = "function", function = new { name = "contar_tomadas", description = "Retorna quantas tomadas elétricas (Electrical Fixtures) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_interruptores", description = "Retorna quantos interruptores (Lighting Devices) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_luminarias", description = "Retorna quantas luminárias (Lighting Fixtures) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_quadros", description = "Retorna quantos quadros elétricos (Electrical Equipment) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_niveis", description = "Retorna quantos níveis (Levels) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_ambientes", description = "Retorna quantos ambientes/cômodos (Rooms) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_circuitos_eletricos", description = "Retorna quantos circuitos elétricos (ElectricalSystems) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "area_construida", description = "Calcula a área total construída somando a área de todos os ambientes (Rooms) do modelo.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "comprimento_eletrodutos", description = "Calcula o comprimento total de eletrodutos (Conduit) no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "potencia_total_instalada", description = "Calcula a potência total instalada somando a carga aparente de tomadas e quadros elétricos.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_vistas", description = "Retorna quantas vistas (Views, excluindo templates) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "contar_folhas", description = "Retorna quantas folhas/pranchas (ViewSheets) existem no modelo BIM.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "categoria_com_mais_elementos", description = "Retorna qual categoria (entre tomadas, interruptores, luminárias, quadros, níveis, ambientes e eletrodutos) possui mais elementos no modelo.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "familias_carregadas", description = "Lista todas as famílias carregadas no modelo com suas respectivas quantidades de instâncias.", parameters = new { type = "object", properties = new { } } } },
                new { type = "function", function = new { name = "diagnostico", description = "Realiza um diagnóstico completo do modelo elétrico, analisando quantidades de elementos, circuitos, ambientes e áreas.", parameters = new { type = "object", properties = new { } } } }
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
                case "diagnostico":
                    return BuildProjectDiagnosis();
                default:
                    return null;
            }
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

        private static readonly ClassificationRepository _classificationRepository = new ClassificationRepository();

        private static (ElementClassificationType type, string matchedAlias)? DetectClassification(string question)
        {
            foreach (var kvp in _classificationRepository.GetAllAliases())
            {
                if (!Enum.TryParse<ElementClassificationType>(kvp.Key, out var type))
                    continue;

                foreach (var alias in kvp.Value)
                {
                    if (question.IndexOf(alias, StringComparison.OrdinalIgnoreCase) >= 0)
                        return (type, alias);
                }
            }

            return null;
        }

        private string FormatDistribution(Dictionary<string, int> distribution, string displayName)
        {
            var sb = new StringBuilder();
            var eqSep = new string('=', 50);
            var sep = new string('-', 50);
            var total = distribution.Values.Sum();

            sb.AppendLine(eqSep);

            if (total == 0)
            {
                sb.AppendLine();
                sb.AppendLine($"Nenhuma {displayName.ToLower()} foi encontrada no modelo.");
                sb.AppendLine();
                sb.AppendLine(eqSep);
                return sb.ToString();
            }

            sb.AppendLine();
            sb.AppendLine($"{displayName} POR NÍVEL");
            sb.AppendLine();

            foreach (var kvp in distribution.OrderBy(x => x.Key))
            {
                sb.AppendLine(sep);
                sb.AppendLine();
                sb.AppendLine(kvp.Key);
                sb.AppendLine();
                sb.AppendLine($"{kvp.Value} {displayName.ToLower()}");
                sb.AppendLine();
            }

            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Resumo");
            sb.AppendLine();
            sb.AppendLine("Total de níveis:");
            sb.AppendLine(distribution.Count.ToString());
            sb.AppendLine();
            sb.AppendLine($"Total de {displayName.ToLower()}:");
            sb.AppendLine(total.ToString());
            sb.AppendLine();
            sb.AppendLine(eqSep);

            return sb.ToString();
        }

        private string FormatCircuitSummary(List<CircuitInfo> circuits)
        {
            var sb = new StringBuilder();
            var eqSep = new string('=', 50);
            var sep = new string('-', 50);

            sb.AppendLine(eqSep);

            if (circuits.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("Nenhum circuito elétrico foi encontrado.");
                sb.AppendLine();
                sb.AppendLine("O projeto ainda não possui circuitos modelados.");
                sb.AppendLine();
                sb.AppendLine(eqSep);
                return sb.ToString();
            }

            sb.AppendLine();
            sb.AppendLine("CIRCUITOS ELÉTRICOS");
            sb.AppendLine();

            foreach (var circuit in circuits)
            {
                sb.AppendLine(sep);
                sb.AppendLine();
                sb.AppendLine("Circuito");
                sb.AppendLine();
                sb.AppendLine(circuit.Name);
                sb.AppendLine();
                sb.AppendLine("Painel");
                sb.AppendLine();
                sb.AppendLine(string.IsNullOrEmpty(circuit.PanelName) ? "—" : circuit.PanelName);
                sb.AppendLine();
                sb.AppendLine("Tomadas");
                sb.AppendLine();
                sb.AppendLine(circuit.TomadaCount.ToString());
                sb.AppendLine();
                sb.AppendLine("Luminárias");
                sb.AppendLine();
                sb.AppendLine(circuit.LuminariaCount.ToString());
                sb.AppendLine();
                sb.AppendLine("Interruptores");
                sb.AppendLine();
                sb.AppendLine(circuit.InterruptorCount.ToString());
                sb.AppendLine();
            }

            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Resumo");
            sb.AppendLine();
            sb.AppendLine("Circuitos:");
            sb.AppendLine(circuits.Count.ToString());
            sb.AppendLine();
            sb.AppendLine("Tomadas conectadas:");
            sb.AppendLine(circuits.Sum(c => c.TomadaCount).ToString());
            sb.AppendLine();
            sb.AppendLine("Luminárias conectadas:");
            sb.AppendLine(circuits.Sum(c => c.LuminariaCount).ToString());
            sb.AppendLine();
            sb.AppendLine("Interruptores conectados:");
            sb.AppendLine(circuits.Sum(c => c.InterruptorCount).ToString());
            sb.AppendLine();
            sb.AppendLine(eqSep);

            return sb.ToString();
        }

        private string FormatPanelSummary(List<PanelInfo> panels)
        {
            var sb = new StringBuilder();
            var eqSep = new string('=', 50);
            var sep = new string('-', 50);

            sb.AppendLine(eqSep);

            if (panels.Count == 0)
            {
                sb.AppendLine();
                sb.AppendLine("Nenhum painel elétrico foi encontrado.");
                sb.AppendLine();
                sb.AppendLine(eqSep);
                return sb.ToString();
            }

            sb.AppendLine();
            sb.AppendLine("PAINÉIS ELÉTRICOS");
            sb.AppendLine();

            foreach (var panel in panels)
            {
                sb.AppendLine(sep);
                sb.AppendLine();
                sb.AppendLine("Painel");
                sb.AppendLine();
                sb.AppendLine(panel.Name);
                sb.AppendLine();
                sb.AppendLine("Circuitos");
                sb.AppendLine();
                sb.AppendLine(panel.CircuitCount.ToString());
                sb.AppendLine();
                sb.AppendLine("Tomadas");
                sb.AppendLine();
                sb.AppendLine(panel.TomadaCount.ToString());
                sb.AppendLine();
                sb.AppendLine("Luminárias");
                sb.AppendLine();
                sb.AppendLine(panel.LuminariaCount.ToString());
                sb.AppendLine();
                sb.AppendLine("Interruptores");
                sb.AppendLine();
                sb.AppendLine(panel.InterruptorCount.ToString());
                sb.AppendLine();
            }

            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Resumo");
            sb.AppendLine();
            sb.AppendLine("Painéis:");
            sb.AppendLine(panels.Count.ToString());
            sb.AppendLine();
            sb.AppendLine("Circuitos:");
            sb.AppendLine(panels.Sum(p => p.CircuitCount).ToString());
            sb.AppendLine();
            sb.AppendLine(eqSep);

            return sb.ToString();
        }

        private string FormatIntegrityReport(ModelIntegrityResult result)
        {
            var sb = new StringBuilder();
            var eqSep = new string('=', 50);
            var sep = new string('-', 50);
            var hasIssues = result.TomadasSemCircuito > 0 ||
                            result.LuminariasSemCircuito > 0 ||
                            result.InterruptoresSemCircuito > 0 ||
                            result.PaineisSemCircuitos > 0;

            sb.AppendLine(eqSep);
            sb.AppendLine();
            sb.AppendLine("INTEGRIDADE DO MODELO");
            sb.AppendLine();

            if (!hasIssues)
            {
                sb.AppendLine(sep);
                sb.AppendLine();
                sb.AppendLine("Nenhuma inconsistência encontrada.");
                sb.AppendLine();
                sb.AppendLine("Todos os elementos analisados possuem circuito.");
                sb.AppendLine();
                sb.AppendLine(eqSep);
                return sb.ToString();
            }

            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Tomadas sem circuito");
            sb.AppendLine();
            sb.AppendLine(result.TomadasSemCircuito.ToString());
            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Luminárias sem circuito");
            sb.AppendLine();
            sb.AppendLine(result.LuminariasSemCircuito.ToString());
            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Interruptores sem circuito");
            sb.AppendLine();
            sb.AppendLine(result.InterruptoresSemCircuito.ToString());
            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Painéis sem circuitos");
            sb.AppendLine();
            sb.AppendLine(result.PaineisSemCircuitos.ToString());
            sb.AppendLine();
            sb.AppendLine(sep);
            sb.AppendLine();
            sb.AppendLine("Resultado");
            sb.AppendLine();
            sb.AppendLine("⚠ Foram encontradas inconsistências.");
            sb.AppendLine();
            sb.AppendLine(eqSep);

            return sb.ToString();
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
