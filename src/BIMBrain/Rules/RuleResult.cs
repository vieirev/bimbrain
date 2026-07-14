using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMBrain.Rules
{
    public class RuleResult
    {
        public string RuleName { get; set; }
        public bool Success { get; set; }
        public RuleSeverity Severity { get; set; }
        public string Message { get; set; }
        public List<ElementId> AffectedElements { get; set; } = new List<ElementId>();
    }
}
