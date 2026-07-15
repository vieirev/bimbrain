using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMBrain.Queries
{
    public class QueryContext
    {
        public string Question { get; set; }

        public CopilotContext CopilotContext { get; set; }

        public UIDocument UIDocument { get; set; }

        public Document Document { get; set; }
    }
}
