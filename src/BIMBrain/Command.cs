using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMBrain
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            var projectName = doc.Title;
            var totalElements = new FilteredElementCollector(doc).ToElements().Count;
            var totalViews = new FilteredElementCollector(doc)
                .OfClass(typeof(View)).ToElements().Count;
            var totalLevels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level)).ToElements().Count;

            var window = new UI.MainWindow(projectName, doc);

            window.Show();

            return Result.Succeeded;
        }
    }
}
