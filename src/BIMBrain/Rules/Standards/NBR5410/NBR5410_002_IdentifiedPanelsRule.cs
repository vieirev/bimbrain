using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Standards.NBR5410
{
    public class NBR5410_002_IdentifiedPanelsRule : EngineeringRule
    {
        public NBR5410_002_IdentifiedPanelsRule()
        {
            Name = "NBR5410-002";
            Description = "Verifica se todos os painéis elétricos possuem identificação válida e única.";
            Category = "Normativa";
        }

        public override List<RuleResult> Execute(Document doc)
        {
            var duplicateRule = new Consistency.DuplicatePanelNameRule();
            var duplicateResults = duplicateRule.Execute(doc);
            var hasDuplicates = duplicateResults.Any(r => !r.Success);

            var analyzer = new PanelAnalyzer(doc);
            var panels = analyzer.Analyze();
            var emptyPanels = panels.Where(p => string.IsNullOrWhiteSpace(p.Name)).ToList();
            var hasEmptyNames = emptyPanels.Count > 0;

            if (!hasDuplicates && !hasEmptyNames)
            {
                return new List<RuleResult>
                {
                    new RuleResult
                    {
                        RuleName = Name,
                        Success = true,
                        Severity = RuleSeverity.Information,
                        Message = "Todos os painéis possuem identificação válida e única."
                    }
                };
            }

            var affected = new List<ElementId>();
            affected.AddRange(duplicateResults
                .Where(r => !r.Success)
                .SelectMany(r => r.AffectedElements));

            if (hasEmptyNames)
            {
                var emptyNames = new HashSet<string>(emptyPanels.Select(p => p.Name));
                var emptyIds = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_ElectricalEquipment)
                    .WhereElementIsNotElementType()
                    .ToElements()
                    .Cast<FamilyInstance>()
                    .Where(fi => string.IsNullOrWhiteSpace(fi.Name))
                    .Select(fi => fi.Id);
                affected.AddRange(emptyIds);
            }

            return new List<RuleResult>
            {
                new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = "Foram encontrados painéis com identificação inválida.",
                    AffectedElements = affected
                }
            };
        }
    }
}
