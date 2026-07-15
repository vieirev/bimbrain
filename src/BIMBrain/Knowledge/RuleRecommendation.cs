using System.Collections.Generic;

namespace BIMBrain.Knowledge
{
    public class RuleRecommendation
    {
        public string RuleName { get; set; }
        public string Title { get; set; }
        public string Problem { get; set; }
        public string Impact { get; set; }
        public List<string> RecommendedActions { get; set; } = new List<string>();
        public string Reference { get; set; }
    }
}
