using Autodesk.Revit.DB;

namespace BIMBrain.Classification
{
    public class ElementClassification
    {
        public ElementId ElementId { get; set; }
        public ElementClassificationType Classification { get; set; }
        public double Confidence { get; set; }
        public string Reason { get; set; }
    }
}
