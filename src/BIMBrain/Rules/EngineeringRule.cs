using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMBrain.Rules
{
    public abstract class EngineeringRule
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string Category { get; protected set; }

        public abstract List<RuleResult> Execute(Document doc);
    }
}
