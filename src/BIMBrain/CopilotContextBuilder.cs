using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace BIMBrain
{
    public class CopilotContextBuilder
    {
        private readonly UIDocument _uiDoc;
        private readonly ProjectGraph _graph;
        private readonly SelectionContextService _selectionService;

        public CopilotContextBuilder(
            UIDocument uiDoc,
            ProjectGraph graph,
            ProjectGraphQuery query,
            ProjectImpactAnalyzer analyzer,
            ElementExplanationService explainer,
            SelectionContextService selectionService)
        {
            _uiDoc = uiDoc;
            _graph = graph;
            _selectionService = selectionService;
        }

        public CopilotContext Build()
        {
            return Build(null, null);
        }

        public CopilotContext Build(string question)
        {
            return Build(question, null);
        }

        public CopilotContext Build(ElementId elementId)
        {
            return Build(null, elementId);
        }

        public CopilotContext Build(string question, ElementId elementId)
        {
            var context = new CopilotContext
            {
                Question = question,
                HasQuestion = !string.IsNullOrEmpty(question),
                Timestamp = DateTime.Now,
                ProjectGraph = _graph,
                ProjectContext = GetProjectName()
            };

            SelectionContext selection = null;

            if (elementId != null && elementId != ElementId.InvalidElementId)
            {
                selection = _selectionService.Create(elementId);
            }
            else if (_selectionService.HasSelection())
            {
                selection = _selectionService.Create();
            }

            context.SelectionContext = selection;
            context.HasSelection = selection != null;

            return context;
        }

        private string GetProjectName()
        {
            try
            {
                return _uiDoc?.Document?.Title ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}
