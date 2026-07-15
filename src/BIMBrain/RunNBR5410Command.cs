using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMBrain.Rules;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    [Transaction(TransactionMode.Manual)]
    public class RunNBR5410Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            if (uidoc == null || uidoc.Document == null)
            {
                return Result.Succeeded;
            }

            var ruleSet = RuleCatalog.All.FirstOrDefault(rs => rs.Name == "NBR 5410");
            if (ruleSet == null)
            {
                TaskDialog.Show("BIMBrain — NBR 5410", "RuleSet 'NBR 5410' não encontrado no catálogo.");
                return Result.Succeeded;
            }

            var runner = new RuleRunner(ruleSet.Rules);
            var results = runner.RunAll(uidoc.Document);

            var window = new UI.RuleResultsWindow(uidoc, results);
            window.ShowDialog();

            return Result.Succeeded;
        }
    }
}
