using Autodesk.Revit.DB;
using BIMBrain.Rules;
using BIMBrain.Rules.Consistency;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BIMBrain
{
    public class ModelIntegrityResult
    {
        public int TomadasSemCircuito { get; set; }
        public int LuminariasSemCircuito { get; set; }
        public int InterruptoresSemCircuito { get; set; }
        public int PaineisSemCircuitos { get; set; }
    }

    public class ModelIntegrityAnalyzer
    {
        private readonly Document _doc;

        public ModelIntegrityAnalyzer(Document doc)
        {
            _doc = doc;
        }

        public ModelIntegrityResult Analyze()
        {
            var rule = new ElementsWithoutCircuitRule();
            var runner = new RuleRunner(new[] { rule });
            var results = runner.RunAll(_doc);

            int tomadas = 0, luminarias = 0, interruptores = 0;

            foreach (var r in results)
            {
                if (r.Success) continue;
                var match = Regex.Match(r.Message, @"\d+");
                if (!match.Success) continue;
                var count = int.Parse(match.Value);
                if (r.Message.Contains("tomada"))
                    tomadas = count;
                else if (r.Message.Contains("luminária"))
                    luminarias = count;
                else if (r.Message.Contains("interruptor"))
                    interruptores = count;
            }

            var panelAnalyzer = new PanelAnalyzer(_doc);
            var panels = panelAnalyzer.Analyze();

            return new ModelIntegrityResult
            {
                TomadasSemCircuito = tomadas,
                LuminariasSemCircuito = luminarias,
                InterruptoresSemCircuito = interruptores,
                PaineisSemCircuitos = panels.Count(p => p.CircuitCount == 0)
            };
        }
    }
}
