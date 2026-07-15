using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BIMBrain
{
    public class ProjectImpactAnalyzer
    {
        private readonly ProjectGraphQuery _query;

        public ProjectImpactAnalyzer(ProjectGraph graph)
        {
            _query = new ProjectGraphQuery(graph);
        }

        public IReadOnlyList<GraphNode> GetAffectedNodes(GraphNode node)
        {
            return GetDownstream(node);
        }

        public IReadOnlyList<GraphNode> GetDependencyNodes(GraphNode node)
        {
            return GetUpstream(node);
        }

        public ImpactAnalysisResult GetImpactAnalysis(GraphNode node)
        {
            var affected = GetAffectedNodes(node);
            var dependencies = GetDependencyNodes(node);

            return new ImpactAnalysisResult
            {
                RootNode = node,
                AffectedNodes = affected.ToList(),
                DependencyNodes = dependencies.ToList(),
                AffectedCount = affected.Count,
                DependencyCount = dependencies.Count,
                Summary = GetImpactSummary(node)
            };
        }

        public IReadOnlyList<GraphNode> GetDownstream(GraphNode node)
        {
            return _query.GetDescendants(node);
        }

        public IReadOnlyList<GraphNode> GetUpstream(GraphNode node)
        {
            return _query.GetAncestors(node);
        }

        public string GetImpactSummary(GraphNode node)
        {
            var affected = GetAffectedNodes(node);
            var circuitos = affected.Count(n => n.Category == "Circuito");
            var niveis = affected.Count(n => n.Category == "Nível");

            var sb = new StringBuilder();
            sb.AppendLine("Elemento analisado:");
            sb.AppendLine(node?.Name ?? "(nulo)");
            sb.AppendLine();
            sb.AppendLine("Impacto encontrado:");
            sb.AppendLine("Circuitos afetados:");
            sb.AppendLine(circuitos.ToString());
            sb.AppendLine();
            sb.AppendLine("Elementos dependentes:");
            sb.AppendLine(affected.Count.ToString());
            sb.AppendLine();
            sb.AppendLine("Níveis impactados:");
            sb.AppendLine(niveis.ToString());

            return sb.ToString().TrimEnd();
        }

        public bool HasImpact(GraphNode node)
        {
            return GetDownstream(node).Count > 0;
        }

        public bool IsLeaf(GraphNode node)
        {
            return _query.GetChildren(node).Count == 0;
        }

        public bool IsRoot(GraphNode node)
        {
            return _query.GetParents(node).Count == 0;
        }
    }
}
