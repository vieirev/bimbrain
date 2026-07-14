using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMBrain.Rules;
using System.Collections.Generic;

namespace BIMBrain
{
    public class RuleSelectionService
    {
        public bool Select(UIDocument uiDoc, RuleResult result)
        {
            if (result.AffectedElements.Count == 0)
                return false;

            uiDoc.Selection.SetElementIds(result.AffectedElements);
            return true;
        }

        public void ClearSelection(UIDocument uiDoc)
        {
            uiDoc.Selection.SetElementIds(new List<ElementId>());
        }
    }
}
