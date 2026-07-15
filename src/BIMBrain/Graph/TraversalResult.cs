using System.Collections.Generic;

namespace BIMBrain
{
    public class TraversalResult
    {
        public List<GraphNode> VisitedNodes { get; } = new List<GraphNode>();
        public List<GraphEdge> VisitedEdges { get; } = new List<GraphEdge>();
        public int Depth { get; set; }
    }
}
