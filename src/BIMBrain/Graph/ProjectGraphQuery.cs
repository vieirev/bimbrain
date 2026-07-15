using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class ProjectGraphQuery
    {
        private readonly ProjectGraph _graph;

        public ProjectGraphQuery(ProjectGraph graph)
        {
            _graph = graph;
        }

        public GraphNode FindNode(ElementId id)
        {
            return _graph?.GetNode(id);
        }

        public IReadOnlyList<GraphNode> FindNodesByCategory(string category)
        {
            return _graph?.FindByCategory(category) ?? new List<GraphNode>();
        }

        public IReadOnlyList<GraphNode> FindNodesByName(string name)
        {
            return _graph?.FindByName(name) ?? new List<GraphNode>();
        }

        public IReadOnlyList<GraphNode> GetNeighbors(GraphNode node)
        {
            if (node == null || _graph == null) return new List<GraphNode>();

            var ids = _graph.Edges
                .Where(e => e.Source == node.ElementId || e.Target == node.ElementId)
                .Select(e => e.Source == node.ElementId ? e.Target : e.Source)
                .ToList();

            return _graph.Nodes.Where(n => ids.Contains(n.ElementId)).ToList();
        }

        public IReadOnlyList<GraphNode> GetNeighbors(GraphNode node, GraphRelation relation)
        {
            if (node == null || _graph == null) return new List<GraphNode>();

            var ids = _graph.Edges
                .Where(e => e.Relation == relation &&
                            (e.Source == node.ElementId || e.Target == node.ElementId))
                .Select(e => e.Source == node.ElementId ? e.Target : e.Source)
                .ToList();

            return _graph.Nodes.Where(n => ids.Contains(n.ElementId)).ToList();
        }

        public IReadOnlyList<GraphNode> GetChildren(GraphNode node)
        {
            if (node == null || _graph == null) return new List<GraphNode>();

            var ids = _graph.Edges
                .Where(e => e.Source == node.ElementId)
                .Select(e => e.Target)
                .ToList();

            return _graph.Nodes.Where(n => ids.Contains(n.ElementId)).ToList();
        }

        public IReadOnlyList<GraphNode> GetParents(GraphNode node)
        {
            if (node == null || _graph == null) return new List<GraphNode>();

            var ids = _graph.Edges
                .Where(e => e.Target == node.ElementId)
                .Select(e => e.Source)
                .ToList();

            return _graph.Nodes.Where(n => ids.Contains(n.ElementId)).ToList();
        }

        public IReadOnlyList<GraphNode> GetDescendants(GraphNode node)
        {
            return TraverseDirection(node, outgoing: true);
        }

        public IReadOnlyList<GraphNode> GetAncestors(GraphNode node)
        {
            return TraverseDirection(node, outgoing: false);
        }

        public bool Contains(GraphNode node)
        {
            if (node == null || _graph == null) return false;

            return _graph.Nodes.Any(n =>
                (n.ElementId != ElementId.InvalidElementId && n.ElementId == node.ElementId) ||
                (n.Category == node.Category && n.Name == node.Name && n.DocumentName == node.DocumentName));
        }

        public IReadOnlyList<GraphNode> FindPath(GraphNode source, GraphNode target)
        {
            var result = new List<GraphNode>();

            if (source == null || target == null || _graph == null) return result;
            if (!Contains(source) || !Contains(target)) return result;

            var queue = new Queue<GraphNode>();
            var cameFrom = new Dictionary<ElementId, ElementId>();
            var visited = new HashSet<ElementId>();

            queue.Enqueue(source);
            visited.Add(source.ElementId);

            bool found = false;
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current.ElementId == target.ElementId)
                {
                    found = true;
                    break;
                }

                foreach (var child in GetChildren(current))
                {
                    if (visited.Add(child.ElementId))
                    {
                        cameFrom[child.ElementId] = current.ElementId;
                        queue.Enqueue(child);
                    }
                }
            }

            if (!found) return result;

            var pathIds = new List<ElementId>();
            var step = target.ElementId;
            while (true)
            {
                pathIds.Insert(0, step);
                if (step == source.ElementId) break;
                if (!cameFrom.TryGetValue(step, out var prev)) break;
                step = prev;
            }

            foreach (var id in pathIds)
            {
                var n = _graph.GetNode(id);
                if (n != null) result.Add(n);
            }

            return result;
        }

        public IReadOnlyList<GraphNode> ShortestPath(GraphNode source, GraphNode target)
        {
            return FindPath(source, target);
        }

        public TraversalResult Traverse(GraphNode start, int? maxDepth = null)
        {
            var result = new TraversalResult();

            if (start == null || _graph == null) return result;
            if (!Contains(start)) return result;

            var queue = new Queue<(GraphNode node, int depth)>();
            var visited = new HashSet<ElementId>();

            queue.Enqueue((start, 0));
            visited.Add(start.ElementId);
            result.VisitedNodes.Add(start);

            int maxReached = 0;

            while (queue.Count > 0)
            {
                var (current, depth) = queue.Dequeue();
                if (maxDepth.HasValue && depth >= maxDepth.Value) continue;

                foreach (var edge in _graph.Edges.Where(e => e.Source == current.ElementId))
                {
                    var targetNode = _graph.GetNode(edge.Target);
                    if (targetNode == null) continue;

                    if (visited.Add(targetNode.ElementId))
                    {
                        queue.Enqueue((targetNode, depth + 1));
                        maxReached = System.Math.Max(maxReached, depth + 1);
                        result.VisitedNodes.Add(targetNode);
                        result.VisitedEdges.Add(edge);
                    }
                }
            }

            result.Depth = maxReached;
            return result;
        }

        private IReadOnlyList<GraphNode> TraverseDirection(GraphNode node, bool outgoing)
        {
            var result = new List<GraphNode>();

            if (node == null || _graph == null) return result;
            if (!Contains(node)) return result;

            var queue = new Queue<GraphNode>();
            var visited = new HashSet<ElementId>();

            queue.Enqueue(node);
            visited.Add(node.ElementId);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                var edges = outgoing
                    ? _graph.Edges.Where(e => e.Source == current.ElementId)
                    : _graph.Edges.Where(e => e.Target == current.ElementId);

                foreach (var edge in edges)
                {
                    var nextId = outgoing ? edge.Target : edge.Source;
                    if (visited.Add(nextId))
                    {
                        var nextNode = _graph.GetNode(nextId);
                        if (nextNode != null)
                        {
                            result.Add(nextNode);
                            queue.Enqueue(nextNode);
                        }
                    }
                }
            }

            return result;
        }
    }
}
