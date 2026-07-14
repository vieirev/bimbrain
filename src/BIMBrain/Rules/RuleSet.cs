using System.Collections.Generic;

namespace BIMBrain.Rules
{
    public class RuleSet
    {
        public string Name { get; }
        public string Description { get; }
        public List<EngineeringRule> Rules { get; }

        public RuleSet(string name, string description, IEnumerable<EngineeringRule> rules)
        {
            Name = name;
            Description = description;
            Rules = new List<EngineeringRule>(rules);
        }
    }
}
