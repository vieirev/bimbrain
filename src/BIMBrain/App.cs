using Autodesk.Revit.UI;

namespace BIMBrain
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            app.CreateRibbonTab("BIMBrain");

            var panel = app.CreateRibbonPanel("BIMBrain", "Home");

            var buttonData = new PushButtonData(
                "BIMBrain.Open",
                "Abrir BIMBrain",
                typeof(Command).Assembly.Location,
                typeof(Command).FullName);

            panel.AddItem(buttonData);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }
    }
}
