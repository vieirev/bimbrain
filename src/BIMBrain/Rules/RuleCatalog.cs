using BIMBrain.Rules.Standards.NBR5410;
using System.Collections.Generic;

namespace BIMBrain.Rules
{
    public static class RuleCatalog
    {
        public static List<RuleSet> All { get; } = new List<RuleSet>
        {
            new RuleSet(
                "NBR 5410",
                "Instalações Elétricas de Baixa Tensão",
                new EngineeringRule[]
                {
                    new NBR5410_001_ConnectedOutletsRule(),
                    new NBR5410_002_IdentifiedPanelsRule()
                })
        };
    }
}
