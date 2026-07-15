using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMBrain
{
    public class CopilotExecutor
    {
        private readonly CopilotContextBuilder _builder;
        private readonly CopilotOrchestrator _orchestrator;

        public CopilotExecutor(UIDocument uiDoc, CopilotContextBuilder builder, CopilotOrchestrator orchestrator)
        {
            _builder = builder;
            _orchestrator = orchestrator;
        }

        public CopilotExecutionResult Execute()
        {
            return Execute(null, null);
        }

        public CopilotExecutionResult Execute(string question)
        {
            return Execute(question, null);
        }

        public CopilotExecutionResult Execute(ElementId elementId)
        {
            return Execute(null, elementId);
        }

        public CopilotExecutionResult Execute(string question, ElementId elementId)
        {
            var context = _builder.Build(question, elementId);
            var route = _orchestrator.Resolve(context);

            return new CopilotExecutionResult
            {
                Context = context,
                Route = route,
                Success = route != null && route.CanExecute,
                Message = route?.Reason ?? "",
                Timestamp = System.DateTime.Now
            };
        }
    }
}
