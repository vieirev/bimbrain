using Autodesk.Revit.DB;

namespace BIMBrain
{
    public class GraphNode
    {
        public ElementId ElementId { get; }
        public string Category { get; }
        public string Name { get; }
        public string DocumentName { get; }

        public GraphNode(ElementId elementId, string category, string name, string documentName)
        {
            ElementId = elementId ?? ElementId.InvalidElementId;
            Category = category ?? "";
            Name = name ?? "";
            DocumentName = documentName ?? "";
        }
    }
}
