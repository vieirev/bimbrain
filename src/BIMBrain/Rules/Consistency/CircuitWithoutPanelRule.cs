using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using System.Collections.Generic;
using System.Linq;

namespace BIMBrain.Rules.Consistency
{
    public class CircuitWithoutPanelRule : EngineeringRule
    {
        public CircuitWithoutPanelRule()
        {
            Name = "Circuitos sem painel";
            Description = "Verifica se existem circuitos elétricos que não estão associados a nenhum painel.";
            Category = "Consistência";
        }

        public override List<RuleResult> Execute(Document doc)
        {
            var circuits = new FilteredElementCollector(doc)
                .OfClass(typeof(ElectricalSystem))
                .ToElements()
                .Cast<ElectricalSystem>();

            var results = new List<RuleResult>();

            foreach (var circuit in circuits)
            {
                if (circuit.BaseEquipment != null)
                    continue;

                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = false,
                    Severity = RuleSeverity.Warning,
                    Message = $"Circuito '{circuit.Name}' (Id {circuit.Id.Value}) não possui painel associado.",
                    AffectedElements = new List<ElementId> { circuit.Id }
                });
            }

            if (results.Count == 0)
            {
                results.Add(new RuleResult
                {
                    RuleName = Name,
                    Success = true,
                    Severity = RuleSeverity.Information,
                    Message = "Todos os circuitos possuem painel associado."
                });
            }

            return results;
        }
    }
}
