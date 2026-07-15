using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMBrain
{
    [Transaction(TransactionMode.Manual)]
    public class PlaceholderCommand : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            TaskDialog.Show("BIMBrain", "Funcionalidade em desenvolvimento.");
            return Result.Succeeded;
        }
    }
}
