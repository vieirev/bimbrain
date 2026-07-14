# BIMBrain Capabilities

Este documento cataloga todas as capacidades atualmente implementadas no BIMBrain. Ele é a referência única para saber exatamente o que o plugin sabe fazer. Nenhuma funcionalidade futura, roadmap ou ideia deve constar aqui — apenas o estado atual do projeto.

---

## Interface

- Plugin Revit registrado no Add-in Manager (Autodesk Revit 2025)
- Ribbon com aba BIMBrain e botão de ativação
- Janela WPF code-only (sem XAML)
- Layout em 4 regiões: informações do projeto, consulta, resposta+histórico, barra de status
- Dimensão inicial 900×700, redimensionável (mínimo 700×500)
- Campo "Consulta" com texto de placeholder
- Botão "Executar" (Enter também aciona a consulta)
- Resposta exibida em TextBox somente leitura com scroll e quebra de linha
- Histórico de consultas com copiar, limpar e clique para reexibir
- Barra de status inferior com Status, Origem, Tempo e Modelos carregados

---

## Modelo

- Nome do projeto ativo
- Modelo ativo (filename do documento) com tipo, status, níveis, rooms, elementos e caminho
- Descoberta de modelos vinculados (`RevitLinkInstance`)
- Status de cada link (carregado / descarregado)
- Quantidade de Rooms por documento vinculado
- Quantidade de Levels por documento vinculado
- Quantidade total de elementos por documento vinculado
- Caminho do arquivo por documento (quando disponível)
- Indicação de documento principal
- Deduplicação de links por nome do documento
- Detecção de duplicidade de nomes entre links
- Total de documentos analisados (ativo + vinculados)
- Relatório diagnóstico com checks de integridade (links descarregados, duplicados)

---

## Consultas BIM

| # | Consulta | Descrição |
|---|----------|-----------|
| 1 | Tomadas | Quantidade de `ElectricalFixtures` |
| 2 | Interruptores | Quantidade de `LightingDevices` |
| 3 | Luminárias | Quantidade de `LightingFixtures` |
| 4 | Quadros | Quantidade de `ElectricalEquipment` |
| 5 | Níveis | Quantidade de `Level` |
| 6 | Ambientes | Quantidade de `Rooms` |
| 7 | Circuitos elétricos | Quantidade de `ElectricalSystem` |
| 8 | Área construída | Soma da área de todos os `Rooms` (m²) |
| 9 | Conduítes | Comprimento total de `Conduit` (m) |
| 10 | Potência total instalada | Carga aparente de `ElectricalFixtures` + `ElectricalEquipment` (VA) |
| 11 | Vistas | Quantidade de `View` (excluindo templates) |
| 12 | Folhas | Quantidade de `ViewSheet` |
| 13 | Categoria com mais elementos | Compara 7 categorias e retorna a de maior contagem |
| 14 | Famílias carregadas | Lista todas as famílias com quantidade de instâncias |
| 15 | Resumo do projeto | Relatório executivo consolidando #1 a #14 |
| 16 | Contexto do projeto | Documento ativo + modelos vinculados com resumo quantitativo, detalhamento por modelo (níveis, rooms, elementos, status, caminho) e diagnóstico de integridade (duplicados, descarregados) |
| 17 | Diagnóstico do modelo | Análise completa do projeto elétrico com checks e status geral |
| 18 | Diagnóstico avançado dos modelos vinculados | Detecção de links descarregados, duplicidade de nomes, resumo quantitativo de todos os documentos do projeto |
| 19 | Distribuição por nível | Elementos elétricos agrupados por nível com resumo quantitativo (suporta tomadas, interruptores, luminárias, quadros) |
| 20 | Análise estrutural dos circuitos | Listagem de todos os circuitos com painel e contagem de tomadas, luminárias e interruptores conectados |
| 21 | Análise estrutural dos painéis | Listagem de todos os painéis com circuitos, tomadas, luminárias e interruptores associados |
| 22 | Verificação de integridade | Detecção de elementos elétricos sem circuito (tomadas, luminárias, interruptores, painéis) |

---

## Inteligência

- Reconhecimento de perguntas por normalização textual
- Normalização: trim → lowercase → remoção de diacríticos → substituições léxicas → verificação `StartsWith`
- Fallback para Ollama (modelo `qwen3`, servidor `localhost:11434`)
- Tool calling com 14 schemas de função para perguntas não reconhecidas diretamente
- Timeout configurado para 120 segundos
- Mensagens de erro diferenciadas por tipo de falha (timeout, conexão, função não encontrada)
- Detecção de origem da resposta (BIMBrain quando consulta direta, IA quando passou pelo Ollama)
- Histórico mantém pergunta, resposta, tempo de execução e origem

---

## Base Normativa

- Base normativa estruturada em `knowledge/standards/` com documentação por norma (NBR 5410)
- Índice de regras normativas com ID, nome, status e task relacionada
- Separação clara entre conhecimento (`knowledge/standards/`) e implementação (`src/BIMBrain/Rules/`)

## Catálogo de RuleSets

- RuleSet: agrupamento lógico de regras (Name, Description, List<EngineeringRule>)
- RuleCatalog: catálogo estático que expõe todos os RuleSets disponíveis
- RuleSet NBR 5410 registrado com NBR5410-001 e NBR5410-002

## Identificação de elementos afetados

- RuleResult.AffectedElements: lista de ElementIds que originaram cada inconsistência
- Todas as regras de consistência e normativas preenchem AffectedElements com os elementos específicos

## Seleção automática dos elementos de uma regra

- RuleSelectionService.Select(UIDocument, RuleResult): seleciona os elementos afetados no Revit
- RuleSelectionService.ClearSelection(UIDocument): limpa a seleção atual
- Retorna false se AffectedElements estiver vazio (sem alterar seleção)
- Nenhuma transação criada, nenhum elemento alterado

## Navegação automática até elementos de uma regra

- RuleNavigationService.ZoomTo(UIDocument, RuleResult): navega para todos os elementos afetados
- RuleNavigationService.ZoomToFirst(UIDocument, RuleResult): navega para o primeiro elemento afetado
- Retorna false se AffectedElements estiver vazio (sem alterar vista)
- Nenhuma transação criada, nenhum elemento alterado

## Infraestrutura para destaque visual de elementos

- RuleHighlightService.Highlight(UIDocument, View, RuleResult): API pública para destaque visual
- RuleHighlightService.Clear(UIDocument, View): API pública para remover destaque visual
- Implementação futura com OverrideGraphicSettings (NotImplementedException nesta versão)
- Nenhum elemento alterado, nenhuma informação gravada no modelo

## Camada de orquestração das ações sobre RuleResult

- RuleActionService: encapsula RuleSelectionService, RuleNavigationService e RuleHighlightService
- Select(UIDocument, RuleResult): delega para RuleSelectionService.Select
- Navigate(UIDocument, RuleResult): delega para RuleNavigationService.ZoomTo
- Highlight(UIDocument, View, RuleResult): delega para RuleHighlightService.Highlight
- ClearHighlight(UIDocument, View): delega para RuleHighlightService.Clear
- Nenhuma lógica adicional, nenhum modelo alterado

## Infraestrutura

- Classes base para execução de regras de engenharia (`EngineeringRule`, `RuleResult`, `RuleSeverity`, `RuleRunner`)
- Arquitetura extensível: novas regras herdam de `EngineeringRule` e são executadas pelo `RuleRunner`
- Nenhuma norma técnica implementada nesta versão — apenas infraestrutura

## Regras Normativas

- NBR5410-001: Tomadas sem circuito — verifica se todas as tomadas estão associadas a um circuito (reusa ElementsWithoutCircuitRule)
- NBR5410-002: Identificação de painéis — verifica se todos os painéis possuem nome preenchido e único (reusa DuplicatePanelNameRule + PanelAnalyzer)

## Regras de Consistência

- Circuitos sem painel — identifica circuitos elétricos sem BaseEquipment associado (RuleRunner + CircuitWithoutPanelRule)
- Painéis sem circuitos — identifica painéis elétricos sem nenhum circuito associado (RuleRunner + PanelWithoutCircuitRule, reusa PanelAnalyzer)
- Elementos sem circuito — identifica tomadas, interruptores e luminárias sem circuito associado (RuleRunner + ElementsWithoutCircuitRule, reusa ElectricalCircuitAnalyzer)
- Modelos vinculados descarregados — identifica modelos Revit Link que não estão carregados (RuleRunner + UnloadedLinksRule, reusa ModelContext)
- Painéis com nomes duplicados — identifica painéis elétricos com nomes repetidos (RuleRunner + DuplicatePanelNameRule, reusa PanelAnalyzer)

---

## Limitações Conhecidas

- Não interpreta `Space` (apenas `Room`)
- Não interpreta circuitos elétricos inexistentes (apenas `ElectricalSystem` criados)
- Não modifica o modelo Revit
- Não cria elementos nem executa comandos
- Não salva alterações no documento
- Não exporta PDF, DWG ou IFC
- Não analisa ambientes de modelos de arquitetura vinculados por link
- Potência elétrica limitada aos parâmetros `Apparent Load` / `Carga Aparente` / `Potência`
- Tool calling com `qwen3` pode selecionar uma função mesmo quando nenhuma é ideal (escopo limitado a 15 funções)
- Não realiza cruzamento entre disciplinas (ex.: tomadas por ambiente)

---

## Tabela Resumo

| Capacidade | Status |
|------------|--------|
| Plugin Revit | ✅ |
| Janela WPF redimensionável | ✅ |
| Histórico de consultas | ✅ |
| Barra de status | ✅ |
| Nome do projeto ativo | ✅ |
| Modelos vinculados (Revit Links) | ✅ |
| Níveis por documento vinculado | ✅ |
| Elementos por documento vinculado | ✅ |
| Caminho do arquivo por documento | ✅ |
| Detecção de duplicidade entre links | ✅ |
| Diagnóstico de links (descarregados/duplicados) | ✅ |
| Rooms por documento vinculado | ✅ |
| Contagem de tomadas | ✅ |
| Contagem de interruptores | ✅ |
| Contagem de luminárias | ✅ |
| Contagem de quadros | ✅ |
| Contagem de níveis | ✅ |
| Contagem de ambientes | ✅ |
| Contagem de circuitos elétricos | ✅ |
| Área construída (m²) | ✅ |
| Comprimento de conduítes (m) | ✅ |
| Potência total instalada (VA) | ✅ |
| Contagem de vistas | ✅ |
| Contagem de folhas | ✅ |
| Categoria com mais elementos | ✅ |
| Listagem de famílias carregadas | ✅ |
| Resumo do projeto | ✅ |
| Contexto do projeto | ✅ |
| Fallback para Ollama | ✅ |
| Tool calling | ✅ |
| Diagnóstico automático | ✅ |
| Diagnóstico avançado dos modelos vinculados | ✅ |
| Distribuição de elementos por nível | ✅ |
| Análise estrutural dos circuitos elétricos | ✅ |
| Análise estrutural dos painéis elétricos | ✅ |
| Verificação de integridade do modelo elétrico | ✅ |
| Infraestrutura para execução de regras de engenharia | ✅ |
| Regra de consistência: circuitos sem painel | ✅ |
| Regra de consistência: painéis sem circuitos | ✅ |
| Regra de consistência: elementos sem circuito | ✅ |
| Regra de consistência: modelos vinculados descarregados | ✅ |
| Regra de consistência: painéis com nomes duplicados | ✅ |
| Regra normativa: NBR5410-001 (tomadas sem circuito) | ✅ |
| Regra normativa: NBR5410-002 (identificação de painéis) | ✅ |
| Clash Detection | ❌ |
| Spaces | ❌ |
| IFC | ❌ |
| Modificação do modelo | ❌ |
