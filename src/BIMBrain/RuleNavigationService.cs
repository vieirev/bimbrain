using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMBrain.Rules;
using System.Collections.Generic;

namespace BIMBrain
{
    public class RuleNavigationService
    {
        public bool ZoomTo(UIDocument uiDoc, RuleResult result)
        {
            if (result.AffectedElements.Count == 0)
                return false;

            uiDoc.ShowElements(result.AffectedElements);
            return true;
        }

        public bool ZoomToFirst(UIDocument uiDoc, RuleResult result)
        {
            if (result.AffectedElements.Count == 0)
                return false;

            uiDoc.ShowElements(new List<ElementId> { result.AffectedElements[0] });
            return true;
        }
    }
}
