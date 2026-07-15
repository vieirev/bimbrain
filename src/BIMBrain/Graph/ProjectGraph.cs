using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class ProjectGraph
    {
        private readonly List<GraphNode> _nodes = new List<GraphNode>();
        private readonly List<GraphEdge> _edges = new List<GraphEdge>();

        public IReadOnlyList<GraphNode> Nodes => _nodes;
        public IReadOnlyList<GraphEdge> Edges => _edges;

        public void AddNode(GraphNode node)
        {
            if (node == null) return;

            var exists = node.ElementId != ElementId.InvalidElementId
                ? _nodes.Any(n => n.ElementId == node.ElementId)
                : _nodes.Any(n => n.Category == node.Category && n.Name == node.Name);

            if (!exists) _nodes.Add(node);
        }

        public void AddEdge(GraphEdge edge)
        {
            if (edge == null) return;

            var exists = _edges.Any(e =>
                e.Source == edge.Source &&
                e.Target == edge.Target &&
                e.Relation == edge.Relation);

            if (!exists) _edges.Add(edge);
        }

        public GraphNode GetNode(ElementId id)
        {
            return _nodes.FirstOrDefault(n => n.ElementId == id);
        }

        public IReadOnlyList<GraphNode> GetNeighbors(ElementId id)
        {
            var targetIds = _edges
                .Where(e => e.Source == id)
                .Select(e => e.Target)
                .ToList();

            return _nodes.Where(n => targetIds.Contains(n.ElementId)).ToList();
        }

        public IReadOnlyList<GraphNode> FindByCategory(string category)
        {
            if (string.IsNullOrEmpty(category)) return new List<GraphNode>();
            return _nodes.Where(n => n.Category == category).ToList();
        }

        public IReadOnlyList<GraphNode> FindByName(string name)
        {
            if (string.IsNullOrEmpty(name)) return new List<GraphNode>();
            return _nodes.Where(n => n.Name == name).ToList();
        }
    }
}
