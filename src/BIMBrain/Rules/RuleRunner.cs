using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules
{
    public class RuleRunner
    {
        private readonly List<EngineeringRule> _rules;

        public RuleRunner(IEnumerable<EngineeringRule> rules)
        {
            _rules = new List<EngineeringRule>(rules);
        }

        public List<RuleResult> RunAll(Document doc)
        {
            return _rules.SelectMany(rule => rule.Execute(doc)).ToList();
        }
    }
}
