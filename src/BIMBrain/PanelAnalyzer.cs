using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class PanelInfo
    {
        public ElementId Id { get; set; }
        public string Name { get; set; }
        public int CircuitCount { get; set; }
        public int TomadaCount { get; set; }
        public int LuminariaCount { get; set; }
        public int InterruptorCount { get; set; }
    }

    public class PanelAnalyzer
    {
        private readonly Document _doc;

        public PanelAnalyzer(Document doc)
        {
            _doc = doc;
        }

        public List<PanelInfo> Analyze()
        {
            var circuitAnalyzer = new ElectricalCircuitAnalyzer(_doc);
            var circuits = circuitAnalyzer.Analyze();

            var panels = new FilteredElementCollector(_doc)
                .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                .WhereElementIsNotElementType()
                .ToElements()
                .Cast<FamilyInstance>();

            var result = new List<PanelInfo>();

            foreach (var panel in panels)
            {
                var panelCircuits = circuits
                    .Where(c => c.PanelName == panel.Name)
                    .ToList();

                result.Add(new PanelInfo
                {
                    Id = panel.Id,
                    Name = panel.Name,
                    CircuitCount = panelCircuits.Count,
                    TomadaCount = panelCircuits.Sum(c => c.TomadaCount),
                    LuminariaCount = panelCircuits.Sum(c => c.LuminariaCount),
                    InterruptorCount = panelCircuits.Sum(c => c.InterruptorCount)
                });
            }

            return result.OrderBy(p => p.Name).ToList();
        }
    }
}
