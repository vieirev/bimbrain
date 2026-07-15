using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BIMBrain
{
    public class ElementExplanationService
    {
        private readonly ProjectGraph _graph;
        private readonly ProjectGraphQuery _query;
        private readonly ProjectImpactAnalyzer _analyzer;

        public ElementExplanationService(ProjectGraph graph, ProjectGraphQuery query, ProjectImpactAnalyzer analyzer)
        {
            _graph = graph;
            _query = query;
            _analyzer = analyzer;
        }

        public ElementExplanation Explain(GraphNode node)
        {
            var result = new ElementExplanation { Node = node };

            if (node == null || _graph == null) return result;

            result.Parents = _query.GetParents(node).ToList();
            result.Children = _query.GetChildren(node).ToList();
            result.Dependencies = _analyzer.GetDependencyNodes(node).ToList();
            result.AffectedElements = _analyzer.GetAffectedNodes(node).ToList();
            result.Summary = BuildExplanationText(node);

            return result;
        }

        public ElementExplanation Explain(ElementId id)
        {
            var node = _query.FindNode(id);
            return Explain(node);
        }

        public string BuildSummary(GraphNode node)
        {
            if (node == null) return "";

            var ancestors = _analyzer.GetDependencyNodes(node);
            var nivel = ancestors.FirstOrDefault(n => n.Category == "Nível")?.Name ?? "-";
            var circuito = ancestors.FirstOrDefault(n => n.Category == "Circuito")?.Name ?? "-";
            var painel = ancestors.FirstOrDefault(n => n.Category == "Painel")?.Name ?? "-";

            var sb = new StringBuilder();
            sb.AppendLine("Elemento");
            sb.AppendLine(node.Name);
            sb.AppendLine("Categoria");
            sb.AppendLine(node.Category);
            sb.AppendLine("Modelo");
            sb.AppendLine(node.DocumentName);
            sb.AppendLine("Localização");
            sb.AppendLine(nivel);
            sb.AppendLine("Circuito");
            sb.AppendLine(circuito);
            sb.AppendLine("Painel");
            sb.AppendLine(painel);
            sb.AppendLine("Dependências");
            sb.AppendLine(ancestors.Count.ToString());
            sb.AppendLine("Elementos dependentes");
            sb.AppendLine(_analyzer.GetAffectedNodes(node).Count.ToString());
            sb.AppendLine("Situação");
            sb.AppendLine(GetSituation(node));

            return sb.ToString().TrimEnd();
        }

        private string BuildExplanationText(GraphNode node)
        {
            var ancestors = _analyzer.GetDependencyNodes(node);
            var nivel = ancestors.FirstOrDefault(n => n.Category == "Nível")?.Name ?? "-";
            var circuito = ancestors.FirstOrDefault(n => n.Category == "Circuito")?.Name ?? "-";
            var painel = ancestors.FirstOrDefault(n => n.Category == "Painel")?.Name ?? "-";
            var projeto = ancestors.FirstOrDefault(n => n.Category == "Projeto")?.Name ?? "-";

            var hierarchy = ancestors.AsEnumerable().Reverse().ToList();
            hierarchy.Add(node);

            var sb = new StringBuilder();
            sb.AppendLine("================================");
            sb.AppendLine("EXPLICAÇÃO DO ELEMENTO");
            sb.AppendLine("================================");
            sb.AppendLine();
            sb.AppendLine("Elemento");
            sb.AppendLine(node.Name);
            sb.AppendLine("Categoria");
            sb.AppendLine(node.Category);
            sb.AppendLine("Projeto");
            sb.AppendLine(projeto);
            sb.AppendLine("Modelo");
            sb.AppendLine(node.DocumentName);
            sb.AppendLine("Hierarquia");
            for (int i = 0; i < hierarchy.Count; i++)
            {
                sb.AppendLine(hierarchy[i].Name);
                if (i < hierarchy.Count - 1)
                    sb.AppendLine("↓");
            }
            sb.AppendLine("Dependências");
            sb.AppendLine(ancestors.Count.ToString());
            sb.AppendLine("Elementos afetados");
            sb.AppendLine(_analyzer.GetAffectedNodes(node).Count.ToString());
            sb.AppendLine("Resumo");
            sb.AppendLine(BuildProse(node, circuito, painel, nivel));
            sb.AppendLine();
            sb.AppendLine("================================");

            return sb.ToString().TrimEnd();
        }

        private static string BuildProse(GraphNode node, string circuito, string painel, string nivel)
        {
            if (circuito != "-" && painel != "-" && nivel != "-")
                return $"A {node.Category.ToLower()} {node.Name} pertence ao circuito {circuito}, " +
                       $"alimentado pelo painel {painel}, localizado no {nivel}.";

            if (circuito != "-")
                return $"A {node.Category.ToLower()} {node.Name} pertence ao circuito {circuito}.";

            return $"A {node.Category.ToLower()} {node.Name} não possui dependências mapeadas no grafo.";
        }

        private string GetSituation(GraphNode node)
        {
            if (_query.GetChildren(node).Count == 0)
                return "Elemento terminal (Leaf)";
            if (_query.GetParents(node).Count == 0)
                return "Elemento raiz (Root)";
            return "Elemento intermediário";
        }
    }
}
