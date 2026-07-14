using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Consistency
{
    public class UnloadedLinksRule : EngineeringRule
    {
        public UnloadedLinksRule()
        {
            Name = "Modelos descarregados";
            Description = "Verifica se existem modelos vinculados (Revit Links) que não estão carregados no projeto.";
            Category = "Consistência";
        }

        public override List<RuleResult> Execute(Document doc)
        {
            var ctx = new ModelContext(doc);
            var results = new List<RuleResult>();

            foreach (var link in ctx.Links.Where(l => !l.IsLoaded))
            {
                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = $"O modelo '{link.Name}' está descarregado."
                });
            }

            if (results.Count == 0)
            {
                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = true,
                    Severity = RuleSeverity.Information,
                    Message = "Todos os modelos vinculados estão carregados."
                });
            }

            return results;
        }
    }
}
