using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BIMBrain
{
    [Transaction(TransactionMode.Manual)]
    public class ExplainSelectionCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null)
            {
                return Result.Succeeded;
            }

            var selectedIds = uidoc.Selection?.GetElementIds();
            if (selectedIds == null || selectedIds.Count == 0)
            {
                TaskDialog.Show("BIMBrain — Explicar", "Selecione um elemento primeiro.");
                return Result.Succeeded;
            }

            var elementId = selectedIds.First();

            var doc = uidoc.Document;
            var graph = new ProjectGraphBuilder(doc).Build();
            var query = new ProjectGraphQuery(graph);
            var analyzer = new ProjectImpactAnalyzer(graph);
            var explainer = new ElementExplanationService(graph, query, analyzer);
            var selectionService = new SelectionContextService(uidoc, graph, query, analyzer, explainer);

            var builder = new CopilotContextBuilder(uidoc, graph, query, analyzer, explainer, selectionService);
            var context = builder.Build(elementId);
            var selection = context.SelectionContext;

            if (selection == null || selection.SelectedNode == null)
            {
                TaskDialog.Show(
                    "BIMBrain — Explicar",
                    "O elemento selecionado não está mapeado no Project Graph atual.\n\n" +
                    "O grafo cobre apenas: Projeto, Modelo, Nível, Painel e Circuito.");
                return Result.Succeeded;
            }

            var node = selection.SelectedNode;
            var explanation = explainer.Explain(node);

            var text = BuildDialogText(explanation, explainer);
            TaskDialog.Show("BIMBrain — Explicar", text);

            return Result.Succeeded;
        }

        private static string BuildDialogText(ElementExplanation explanation, ElementExplanationService explainer)
        {
            var node = explanation.Node;

            var hierarchy = explanation.Dependencies.AsEnumerable().Reverse().ToList();
            hierarchy.Add(node);

            var sb = new StringBuilder();
            sb.AppendLine("================================");
            sb.AppendLine("EXPLICAÇÃO DO ELEMENTO");
            sb.AppendLine("================================");
            sb.AppendLine();
            sb.AppendLine("Nome");
            sb.AppendLine(node.Name);
            sb.AppendLine();
            sb.AppendLine("Categoria");
            sb.AppendLine(node.Category);
            sb.AppendLine();
            sb.AppendLine("Modelo");
            sb.AppendLine(node.DocumentName);
            sb.AppendLine();
            sb.AppendLine("Nível");
            sb.AppendLine(explanation.Dependencies.FirstOrDefault(n => n.Category == "Nível")?.Name ?? "-");
            sb.AppendLine();
            sb.AppendLine("Hierarquia");
            for (int i = 0; i < hierarchy.Count; i++)
            {
                sb.AppendLine(hierarchy[i].Name);
                if (i < hierarchy.Count - 1)
                {
                    sb.AppendLine("↓");
                }
            }

            sb.AppendLine();
            sb.AppendLine("Dependências");
            sb.AppendLine(explanation.Dependencies.Count.ToString());
            sb.AppendLine();
            sb.AppendLine("Elementos afetados");
            sb.AppendLine(explanation.AffectedElements.Count.ToString());
            sb.AppendLine();
            sb.AppendLine("Resumo");
            sb.AppendLine(explainer.BuildSummary(node));

            return sb.ToString().TrimEnd();
        }
    }
}
