using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class SelectionContextService
    {
        private readonly UIDocument _uiDoc;
        private readonly ProjectGraph _graph;
        private readonly ProjectGraphQuery _query;
        private readonly ProjectImpactAnalyzer _analyzer;
        private readonly ElementExplanationService _explainer;

        public SelectionContextService(
            UIDocument uiDoc,
            ProjectGraph graph,
            ProjectGraphQuery query,
            ProjectImpactAnalyzer analyzer,
            ElementExplanationService explainer)
        {
            _uiDoc = uiDoc;
            _graph = graph;
            _query = query;
            _analyzer = analyzer;
            _explainer = explainer;
        }

        public SelectionContext Create()
        {
            return Create(GetCurrentSelectionId());
        }

        public SelectionContext Create(ElementId id)
        {
            var context = new SelectionContext { SelectedElementId = id };

            if (id == null || id == ElementId.InvalidElementId || _graph == null)
                return context;

            var node = _query.FindNode(id);
            context.SelectedNode = node;

            if (node == null) return context;

            context.SelectedCategory = node.Category;
            context.SelectedName = node.Name;
            context.SelectedModel = node.DocumentName;

            var nivel = _analyzer
                .GetDependencyNodes(node)
                .FirstOrDefault(n => n.Category == "Nível")?.Name;
            context.SelectedLevel = nivel ?? "";

            context.Parents = _query.GetParents(node).ToList();
            context.Children = _query.GetChildren(node).ToList();
            context.Explanation = _explainer.Explain(node).Summary;
            context.Impact = _analyzer.GetImpactAnalysis(node);

            return context;
        }

        public bool HasSelection()
        {
            var ids = GetCurrentSelectionIds();
            return ids != null && ids.Count > 0;
        }

        public void Clear()
        {
            if (_uiDoc?.Selection != null)
                _uiDoc.Selection.SetElementIds(new List<ElementId>());
        }

        private ElementId GetCurrentSelectionId()
        {
            var ids = GetCurrentSelectionIds();
            return ids != null && ids.Count > 0 ? ids.First() : null;
        }

        private ICollection<ElementId> GetCurrentSelectionIds()
        {
            try
            {
                return _uiDoc?.Selection?.GetElementIds();
            }
            catch
            {
                return null;
            }
        }
    }
}
