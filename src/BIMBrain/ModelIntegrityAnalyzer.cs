using Autodesk.Revit.DB;
using BIMBrain.Rules;
using BIMBrain.Rules.Consistency;
using System.Collections.Generic;
using System.Linq;

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
                if (!r.Data.TryGetValue("Tipo", out var tipoObj)) continue;
                if (!r.Data.TryGetValue("Count", out var countObj)) continue;

                var tipo = tipoObj as string;
                var count = countObj is int ci ? ci : System.Convert.ToInt32(countObj);

                if (tipo == "tomadas")
                    tomadas = count;
                else if (tipo == "luminárias")
                    luminarias = count;
                else if (tipo == "interruptores")
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
