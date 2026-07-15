using System;

namespace BIMBrain
{
    public class CopilotExecutionResult
    {
        public CopilotContext Context { get; set; }
        public CopilotRoute Route { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
