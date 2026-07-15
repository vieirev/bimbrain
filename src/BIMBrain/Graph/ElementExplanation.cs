using System.Collections.Generic;

namespace BIMBrain
{
    public class ElementExplanation
    {
        public GraphNode Node { get; set; }
        public List<GraphNode> Parents { get; set; } = new List<GraphNode>();
        public List<GraphNode> Children { get; set; } = new List<GraphNode>();
        public List<GraphNode> Dependencies { get; set; } = new List<GraphNode>();
        public List<GraphNode> AffectedElements { get; set; } = new List<GraphNode>();
        public string Summary { get; set; }
    }
}
