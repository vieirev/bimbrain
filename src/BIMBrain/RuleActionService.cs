using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMBrain.Rules;

namespace BIMBrain
{
    public class RuleActionService
    {
        private readonly RuleSelectionService _selection;
        private readonly RuleNavigationService _navigation;
        private readonly RuleHighlightService _highlight;

        public RuleActionService()
        {
            _selection = new RuleSelectionService();
            _navigation = new RuleNavigationService();
            _highlight = new RuleHighlightService();
        }

        public bool Select(UIDocument uiDoc, RuleResult result)
        {
            return _selection.Select(uiDoc, result);
        }

        public bool Navigate(UIDocument uiDoc, RuleResult result)
        {
            return _navigation.ZoomTo(uiDoc, result);
        }

        public bool Highlight(UIDocument uiDoc, View view, RuleResult result)
        {
            return _highlight.Highlight(uiDoc, view, result);
        }

        public bool ClearHighlight(UIDocument uiDoc, View view)
        {
            return _highlight.Clear(uiDoc, view);
        }
    }
}
