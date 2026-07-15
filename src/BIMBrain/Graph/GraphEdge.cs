using Autodesk.Revit.DB;

namespace BIMBrain
{
    public enum GraphRelation
    {
        Contains,
        ConnectedTo,
        FedBy,
        LocatedIn,
        BelongsTo,
        LinkedTo
    }

    public class GraphEdge
    {
        public ElementId Source { get; }
        public ElementId Target { get; }
        public GraphRelation Relation { get; }

        public GraphEdge(ElementId source, ElementId target, GraphRelation relation)
        {
            Source = source ?? ElementId.InvalidElementId;
            Target = target ?? ElementId.InvalidElementId;
            Relation = relation;
        }
    }
}
