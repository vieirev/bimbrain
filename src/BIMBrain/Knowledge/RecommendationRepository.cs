using System.Collections.Generic;

namespace BIMBrain.Knowledge
{
    public class RecommendationRepository
    {
        private static readonly Dictionary<string, RuleRecommendation> Store =
            new Dictionary<string, RuleRecommendation>
            {
                {
                    "Circuitos sem painel", new RuleRecommendation
                    {
                        RuleName = "Circuitos sem painel",
                        Title = "Circuitos elétricos sem painel associado",
                        Problem = "Existem circuitos elétricos (ElectricalSystem) que não possuem um " +
                                  "painel (BaseEquipment) definido.",
                        Impact = "Circuitos órfãos não são alimentados nem discriminados em quadros, " +
                                 "comprometendo a análise de carga e a segurança da instalação.",
                        RecommendedActions = new List<string>
                        {
                            "Abra o gerenciador de circuitos e identifique os circuitos sem painel.",
                            "Associe cada circuito ao painel de distribuição correspondente.",
                            "Revise a topologia de alimentação para garantir a continuidade."
                        },
                        Reference = "NBR 5410 — Instalações Elétricas de Baixa Tensão"
                    }
                },
                {
                    "Painéis sem circuitos", new RuleRecommendation
                    {
                        RuleName = "Painéis sem circuitos",
                        Title = "Painéis elétricos sem nenhum circuito",
                        Problem = "Existem painéis elétricos (ElectricalEquipment) que não possuem " +
                                  "circuitos associados.",
                        Impact = "Painéis ociosos ou mal dimensionados indicam possível subutilização " +
                                 "ou perda de equipamentos na modelagem.",
                        RecommendedActions = new List<string>
                        {
                            "Identifique os painéis sem circuitos no modelo.",
                            "Associe os circuitos existentes ao painel correto ou remova painéis não utilizados.",
                            "Confirme se o painel foi modelado corretamente."
                        },
                        Reference = "NBR 5410 — Instalações Elétricas de Baixa Tensão"
                    }
                },
                {
                    "Elementos sem circuito", new RuleRecommendation
                    {
                        RuleName = "Elementos sem circuito",
                        Title = "Elementos elétricos sem circuito associado",
                        Problem = "Existem tomadas, luminárias ou interruptores que não estão associados " +
                                  "a nenhum circuito elétrico.",
                        Impact = "Cargas não mapeadas inviabilizam o cálculo de demanda e a verificação " +
                                 "de sobrecarga dos circuitos.",
                        RecommendedActions = new List<string>
                        {
                            "Liste os elementos sem circuito por categoria.",
                            "Atribua cada elemento ao circuito elétrico correspondente.",
                            "Revise a fiação quando a associação não for possível."
                        },
                        Reference = "NBR 5410 — Instalações Elétricas de Baixa Tensão"
                    }
                },
                {
                    "Modelos descarregados", new RuleRecommendation
                    {
                        RuleName = "Modelos descarregados",
                        Title = "Modelos vinculados descarregados",
                        Problem = "Existem modelos Revit Link que não estão carregados no projeto.",
                        Impact = "A ausência de links impede a análise cruzada de disciplinas e pode ocultar " +
                                 "interferências e elementos dependentes.",
                        RecommendedActions = new List<string>
                        {
                            "Abra o gerenciador de caminhos de vínculo e recarregue os modelos descarregados.",
                            "Verifique os caminhos dos arquivos vinculados.",
                            "Confirme se o link é necessário ao escopo do projeto."
                        },
                        Reference = "Boas práticas de coordenação BIM"
                    }
                },
                {
                    "Nomes duplicados de painéis", new RuleRecommendation
                    {
                        RuleName = "Nomes duplicados de painéis",
                        Title = "Painéis com nomes duplicados",
                        Problem = "Existem painéis elétricos com nomes repetidos no modelo.",
                        Impact = "Nomes duplicados dificultam a identificação, a navegação e a associação " +
                                 "de circuitos, gerando risco de erro em quadros e memoriais.",
                        RecommendedActions = new List<string>
                        {
                            "Identifique os grupos de painéis com o mesmo nome.",
                            "Renomeie os painéis para garantir identificação única e padronizada.",
                            "Adote uma convenção de nomenclatura (ex.: QD-01, QD-02)."
                        },
                        Reference = "NBR 5410 — Identificação de equipamentos"
                    }
                },
                {
                    "NBR5410-001", new RuleRecommendation
                    {
                        RuleName = "NBR5410-001",
                        Title = "Tomadas sem circuito associado (NBR 5410)",
                        Problem = "Foram encontradas tomadas elétricas sem associação a um circuito.",
                        Impact = "Desatende ao mapeamento de cargas da NBR 5410, inviabilizando o cálculo " +
                                 "de demanda e a proteção dos pontos.",
                        RecommendedActions = new List<string>
                        {
                            "Liste as tomadas sem circuito.",
                            "Associe cada tomada ao circuito elétrico correspondente.",
                            "Revise a fiação quando a associação não for possível."
                        },
                        Reference = "NBR 5410 — 6.2.2 (Alimentação de tomadas)"
                    }
                },
                {
                    "NBR5410-002", new RuleRecommendation
                    {
                        RuleName = "NBR5410-002",
                        Title = "Painéis com identificação inválida (NBR 5410)",
                        Problem = "Existem painéis com nome vazio ou duplicado.",
                        Impact = "Compromete a rastreabilidade e a conformidade com a identificação dos " +
                                 "equipamentos exigida pela norma.",
                        RecommendedActions = new List<string>
                        {
                            "Identifique painéis com nome vazio ou duplicado.",
                            "Atribua nomes válidos e únicos a todos os painéis.",
                            "Padronize a nomenclatura conforme o projeto."
                        },
                        Reference = "NBR 5410 — 6.1 (Identificação de circuitos e equipamentos)"
                    }
                }
            };

        public RuleRecommendation GetByRuleName(string ruleName)
        {
            if (!string.IsNullOrEmpty(ruleName) && Store.TryGetValue(ruleName, out var rec))
            {
                return rec;
            }

            return new RuleRecommendation { RuleName = ruleName };
        }
    }
}
