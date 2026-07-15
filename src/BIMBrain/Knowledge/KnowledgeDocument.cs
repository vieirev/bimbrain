using System;

namespace BIMBrain.Knowledge
{
    public class KnowledgeDocument
    {
        public string Title { get; set; }
        public string RuleId { get; set; }
        public string Standard { get; set; }
        public string Markdown { get; set; }
        public bool Exists { get; set; }
    }
}
