namespace BIMBrain
{
    public class CopilotRoute
    {
        public CopilotRequestType RequestType { get; set; }
        public string EngineName { get; set; }
        public string Reason { get; set; }
        public bool CanExecute { get; set; }
    }
}
