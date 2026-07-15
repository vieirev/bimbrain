using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMBrain
{
    public class SelectionContext
    {
        public ElementId SelectedElementId { get; set; }
        public GraphNode SelectedNode { get; set; }
        public string SelectedCategory { get; set; }
        public string SelectedName { get; set; }
        public string SelectedModel { get; set; }
        public string SelectedLevel { get; set; }
        public List<GraphNode> Parents { get; set; } = new List<GraphNode>();
        public List<GraphNode> Children { get; set; } = new List<GraphNode>();
        public string Explanation { get; set; }
        public ImpactAnalysisResult Impact { get; set; }
    }
}
