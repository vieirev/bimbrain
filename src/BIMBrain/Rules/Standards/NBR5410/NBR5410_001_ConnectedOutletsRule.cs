using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Standards.NBR5410
{
    public class NBR5410_001_ConnectedOutletsRule : EngineeringRule
    {
        public NBR5410_001_ConnectedOutletsRule()
        {
            Name = "NBR5410-001";
            Description = "Verifica se todas as tomadas elétricas estão associadas a um circuito.";
            Category = "Normativa";
        }

        public override List<RuleResult> Execute(Document doc)
        {
            var elementsRule = new Consistency.ElementsWithoutCircuitRule();
            var results = elementsRule.Execute(doc);

            var outletResults = results
                .Where(r => !r.Success && r.Message.Contains("tomada"))
                .ToList();

            if (outletResults.Count == 0)
            {
                return new List<RuleResult>
                {
                    new RuleResult
                    {
                        RuleName = Name,
                        Success = true,
                        Severity = RuleSeverity.Information,
                        Message = "Todas as tomadas estão associadas a um circuito."
                    }
                };
            }

            var affected = outletResults
                .SelectMany(r => r.AffectedElements)
                .ToList();

            return new List<RuleResult>
            {
                new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = "Foram encontradas tomadas sem circuito associado.",
                    AffectedElements = affected
                }
            };
        }
    }
}
