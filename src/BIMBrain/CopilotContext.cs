using System;

namespace BIMBrain
{
    public class CopilotContext
    {
        public string ProjectContext { get; set; }
        public SelectionContext SelectionContext { get; set; }
        public string Question { get; set; }
        public DateTime Timestamp { get; set; }
        public bool HasSelection { get; set; }
        public bool HasQuestion { get; set; }
        public ProjectGraph ProjectGraph { get; set; }
    }
}
