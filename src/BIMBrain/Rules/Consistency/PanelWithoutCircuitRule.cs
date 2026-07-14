using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Consistency
{
    public class PanelWithoutCircuitRule : EngineeringRule
    {
        public PanelWithoutCircuitRule()
        {
            Name = "Painéis sem circuitos";
            Description = "Verifica se existem painéis elétricos que não possuem nenhum circuito associado.";
            Category = "Consistência";
        }

        public override List<RuleResult> Execute(Document doc)
        {
            var analyzer = new PanelAnalyzer(doc);
            var panels = analyzer.Analyze();

            var affectedIdsLookup = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                .WhereElementIsNotElementType()
                .ToElements()
                .Cast<FamilyInstance>()
                .GroupBy(p => p.Name)
                .ToDictionary(g => g.Key, g => g.Select(p => p.Id).ToList());

            var results = new List<RuleResult>();

            foreach (var panel in panels.Where(p => p.CircuitCount == 0))
            {
                affectedIdsLookup.TryGetValue(panel.Name, out var ids);
                if (ids == null) ids = new List<ElementId>();

                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = $"Painel '{panel.Name}' não possui nenhum circuito associado.",
                    AffectedElements = ids
                });
            }

            if (results.Count == 0)
            {
                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = true,
                    Severity = RuleSeverity.Information,
                    Message = "Todos os painéis possuem ao menos um circuito associado."
                });
            }

            return results;
        }
    }
}
