using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Consistency
{
    public class ElementsWithoutCircuitRule : EngineeringRule
    {
        public ElementsWithoutCircuitRule()
        {
            Name = "Elementos sem circuito";
            Description = "Verifica se existem tomadas, luminárias ou interruptores que não pertencem a nenhum circuito.";
            Category = "Consistência";
        }

        public override List<RuleResult> Execute(Document doc)
        {
            var circuitAnalyzer = new ElectricalCircuitAnalyzer(doc);
            var connectedIds = circuitAnalyzer.GetConnectedElementIds();

            var categoryMap = new Dictionary<BuiltInCategory, (string Label, int Connected)>
            {
                [BuiltInCategory.OST_ElectricalFixtures] = ("tomadas", 0),
                [BuiltInCategory.OST_LightingFixtures] = ("luminárias", 0),
                [BuiltInCategory.OST_LightingDevices] = ("interruptores", 0)
            };

            foreach (var id in connectedIds)
            {
                var element = doc.GetElement(id);
                if (element?.Category == null) continue;
                var cat = element.Category.BuiltInCategory;
                if (categoryMap.ContainsKey(cat))
                {
                    var entry = categoryMap[cat];
                    categoryMap[cat] = (entry.Label, entry.Connected + 1);
                }
            }

            var results = new List<RuleResult>();

            foreach (var kvp in categoryMap)
            {
                var allIds = new FilteredElementCollector(doc)
                    .OfCategory(kvp.Key)
                    .OfClass(typeof(FamilyInstance))
                    .ToElements()
                    .Cast<FamilyInstance>()
                    .Select(fi => fi.Id)
                    .ToList();

                var unconnected = allIds.Count - kvp.Value.Connected;

                if (unconnected <= 0)
                    continue;

                var unconnectedIds = allIds
                    .Where(id => !connectedIds.Contains(id))
                    .ToList();

                var label = unconnected == 1
                    ? kvp.Value.Label.TrimEnd('s')
                    : kvp.Value.Label;

                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = $"Foram encontradas {unconnected} {label} sem circuito.",
                    AffectedElements = unconnectedIds
                });
            }

            if (results.Count == 0)
            {
                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = true,
                    Severity = RuleSeverity.Information,
                    Message = "Todos os elementos analisados possuem circuito."
                });
            }

            return results;
        }
    }
}
