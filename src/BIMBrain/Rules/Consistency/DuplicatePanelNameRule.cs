using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Consistency
{
    public class DuplicatePanelNameRule : EngineeringRule
    {
        public DuplicatePanelNameRule()
        {
            Name = "Nomes duplicados de painéis";
            Description = "Verifica se existem painéis elétricos com nomes duplicados no projeto.";
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

            var groups = panels
                .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                .GroupBy(p => p.Name)
                .Where(g => g.Count() > 1);

            var results = new List<RuleResult>();

            foreach (var group in groups)
            {
                affectedIdsLookup.TryGetValue(group.Key, out var ids);
                if (ids == null) ids = new List<ElementId>();

                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = $"Foram encontrados {group.Count()} painéis com o nome '{group.Key}'.",
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
                    Message = "Todos os painéis possuem identificação única."
                });
            }

            return results;
        }
    }
}
