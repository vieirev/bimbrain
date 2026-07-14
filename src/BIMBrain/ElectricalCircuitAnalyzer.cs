using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain
{
    public class CircuitInfo
    {
        public string Name { get; set; }
        public string PanelName { get; set; }
        public int ElementCount { get; set; }
        public int TomadaCount { get; set; }
        public int LuminariaCount { get; set; }
        public int InterruptorCount { get; set; }
    }

    public class ElectricalCircuitAnalyzer
    {
        private readonly Document _doc;

        public ElectricalCircuitAnalyzer(Document doc)
        {
            _doc = doc;
        }

        public List<CircuitInfo> Analyze()
        {
            var circuits = new FilteredElementCollector(_doc)
                .OfClass(typeof(ElectricalSystem))
                .ToElements()
                .Cast<ElectricalSystem>();

            var result = new List<CircuitInfo>();

            foreach (var circuit in circuits)
            {
                int tomadaCount = 0, luminariaCount = 0, interruptorCount = 0;

                var elementIds = circuit.Elements;
                foreach (ElementId id in elementIds)
                {
                    var element = _doc.GetElement(id);
                    if (element == null) continue;

                    var cat = element.Category?.BuiltInCategory;
                    if (cat == BuiltInCategory.OST_ElectricalFixtures)
                        tomadaCount++;
                    else if (cat == BuiltInCategory.OST_LightingFixtures)
                        luminariaCount++;
                    else if (cat == BuiltInCategory.OST_LightingDevices)
                        interruptorCount++;
                }

                result.Add(new CircuitInfo
                {
                    Name = circuit.Name,
                    PanelName = circuit.BaseEquipment?.Name ?? "",
                    ElementCount = tomadaCount + luminariaCount + interruptorCount,
                    TomadaCount = tomadaCount,
                    LuminariaCount = luminariaCount,
                    InterruptorCount = interruptorCount
                });
            }

            return result;
        }

        public HashSet<ElementId> GetConnectedElementIds()
        {
            var result = new HashSet<ElementId>();
            var circuits = new FilteredElementCollector(_doc)
                .OfClass(typeof(ElectricalSystem))
                .ToElements()
                .Cast<ElectricalSystem>();
            foreach (var circuit in circuits)
                foreach (ElementId id in circuit.Elements)
                    result.Add(id);
            return result;
        }
    }
}
