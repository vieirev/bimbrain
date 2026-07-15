using System.Collections.Generic;

namespace BIMBrain
{
    public class ImpactAnalysisResult
    {
        public GraphNode RootNode { get; set; }
        public List<GraphNode> AffectedNodes { get; set; } = new List<GraphNode>();
        public List<GraphNode> DependencyNodes { get; set; } = new List<GraphNode>();
        public int AffectedCount { get; set; }
        public int DependencyCount { get; set; }
        public string Summary { get; set; }
    }
}
