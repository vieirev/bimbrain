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

## Ribbon UX

Aba única `BIMBrain` reorganizada em quatro grupos compactos, com botão
principal em destaque e botões relacionados empilhados (`AddStackedItems`).
Apenas o botão principal `BIMBrain` executa funcionalidade existente; todos os
demais utilizam `PlaceholderCommand`, que exibe `"Funcionalidade em desenvolvimento."`.

| Grupo (Painel) | Botão | Tipo | Comando | Status |
|----------------|-------|------|---------|--------|
| BIMBrain | BIMBrain | Grande (LargeImage) | `Command` (janela de consultas) | ✅ Implementado |
| Copilot | Explicar | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Copilot | Diagnóstico | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Copilot | IA | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Engenharia | NBR 5410 | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Engenharia | Integridade | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Engenharia | Coordenação | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Ferramentas | Automação | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Ferramentas | Configurações | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |
| Ferramentas | Sobre | Empilhado | `PlaceholderCommand` | 🚧 Em desenvolvimento |

- **Botão principal** — `BIMBrain` ocupa o primeiro grupo, em tamanho grande
  (LargeImage), e continua abrindo a janela principal (classe `Command`).
- **Botões empilhados** — cada um dos três grupos seguintes usa
  `AddStackedItems` com três `PushButtonData`, evitando longas fileiras
  horizontais e mantendo a Ribbon compacta.
- **Ícones** — todos os botões possuem `Image` (16×16) e `LargeImage` (32×32).
  Os ícones são temporários (letra sobre cor sólida) em
  `Resources/Icons16/` e `Resources/Icons32/`, embutidos como
  `EmbeddedResource` e carregados via `Assembly.GetManifestResourceStream`.
- **Tooltips** — todo botão define `ToolTip` (texto curto) e `LongDescription`
  (descrição longa) para apresentação na Ribbon.
- `PlaceholderCommand` é uma única classe reutilizável (`IExternalCommand`);
  nenhum comando foi duplicado.

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

## Project Graph

Infraestrutura inicial de grafo navegável do modelo (Model Engine). Esta versão
cria apenas a estrutura; nenhuma consulta ou regra existente foi alterada e a IA
ainda não utiliza o grafo.

- **Nodes** — `GraphNode` representa um elemento do modelo com `ElementId`,
  `Category`, `Name` e `DocumentName`. Categorias criadas nesta versão: Projeto,
  Modelo, Nível, Painel, Circuito.
- **Edges** — `GraphEdge` liga dois nós via `GraphRelation` (Source, Target,
  Relation). Relacionamentos criados: `Contains` (Projeto→Modelo, Modelo→Nível,
  Nível→Painel) e `FedBy` (Painel→Circuito).
- **Navegação** — `ProjectGraph.GetNode(ElementId)` e `GetNeighbors(ElementId)`
  percorrem o grafo a partir de um nó.
- **Busca** — `ProjectGraph.FindByCategory(string)` e `FindByName(string)`
  localizam nós por categoria ou nome.
- **Builder** — `ProjectGraphBuilder.Build()` constrói o grafo a partir de um
  `Document`, reutilizando `ModelContext`, `PanelAnalyzer` e
  `ElectricalCircuitAnalyzer`. Não lança exceções; arestas ausentes são ignoradas.

---

## Project Graph Query

Camada de navegação do Project Graph (Model Engine). Opera exclusivamente sobre
o grafo já construído — nenhuma consulta acessa o Revit diretamente. Genérica e
reutilizável por qualquer disciplina; não contém lógica elétrica, de normas ou de
disciplina.

- **Busca por categoria** — `FindNodesByCategory(string)` retorna todos os nós de
  uma categoria (Projeto, Modelo, Nível, Painel, Circuito).
- **Busca por nome** — `FindNodesByName(string)` retorna nós pelo nome.
- **Busca por ElementId** — `FindNode(ElementId)` retorna o nó correspondente.
- **Navegação** — `GetNeighbors(GraphNode)` (adjacência em qualquer direção),
  `GetNeighbors(GraphNode, GraphRelation)` (filtrado por relação),
  `GetChildren(GraphNode)` (arestas de saída), `GetParents(GraphNode)` (arestas de
  entrada).
- **Descendentes** — `GetDescendants(GraphNode)` percorre arestas de saída (BFS).
- **Ascendentes** — `GetAncestors(GraphNode)` percorre arestas de entrada (BFS).
- **BFS** — `Traverse(GraphNode, maxDepth?)` executa busca em largura com `Queue`
  (sem recursão), retornando `TraversalResult` (VisitedNodes, VisitedEdges, Depth).
- **ShortestPath** — `ShortestPath(source, target)` delega a `FindPath` (BFS),
  preparado para evolução futura.

`FindPath` utiliza BFS sobre arestas de saída; ausência de caminho retorna
coleção vazia e não lança exceções.

---

## Project Impact Analysis

Infraestrutura de análise de impacto do Project Graph (Model Engine). Opera
exclusivamente sobre o `ProjectGraph` por meio de `ProjectGraphQuery` — não acessa
o Revit, `Document` ou `FilteredElementCollector`, e não contém regras de
engenharia nem IA. Genérica e reutilizável por qualquer disciplina.

- **Upstream** — `GetUpstream(GraphNode)` / `GetDependencyNodes(GraphNode)`:
  todos os elementos dos quais o nó depende (arestas de entrada, BFS).
- **Downstream** — `GetDownstream(GraphNode)` / `GetAffectedNodes(GraphNode)`:
  todos os elementos dependentes do nó (arestas de saída, BFS).
- **Dependências** — `DependencyNodes` em `ImpactAnalysisResult` (upstream).
- **Impacto** — `GetImpactAnalysis(GraphNode)` retorna `ImpactAnalysisResult`
  (RootNode, AffectedNodes, DependencyNodes, AffectedCount, DependencyCount,
  Summary).
- **Resumo** — `GetImpactSummary(GraphNode)` gera texto com circuitos afetados,
  elementos dependentes e níveis impactados.
- **Root** — `IsRoot(GraphNode)` (true quando não possui pais).
- **Leaf** — `IsLeaf(GraphNode)` (true quando não possui filhos). `HasImpact`
  indica se há ao menos um elemento dependente.

---

## Element Explanation

Primeiro caso de uso completo do Project Graph (Model Engine), combinando
`ProjectGraph`, `ProjectGraphQuery` e `ProjectImpactAnalyzer`. Explica um elemento
do modelo inteiramente a partir do grafo — sem acesso a `Document`,
`FilteredElementCollector` ou Revit API. Genérico e reutilizável por qualquer
disciplina.

- **Explicação completa** — `ElementExplanationService.Explain(GraphNode)` /
  `Explain(ElementId)` retorna `ElementExplanation` (Node, Parents, Children,
  Dependencies, AffectedElements, Summary).
- **Cadeia hierárquica** — `Summary` inclui a hierarquia top-down (Projeto →
  Modelo → Nível → Painel → Circuito → Elemento) obtida via `ProjectGraphQuery`.
- **Dependências** — lista upstream (`GetDependencyNodes`) do elemento.
- **Impacto** — lista downstream (`GetAffectedNodes`) do elemento.
- **Resumo textual** — `BuildSummary(GraphNode)` gera texto estruturado
  (Elemento, Categoria, Modelo, Localização, Circuito, Painel, Dependências,
  Elementos dependentes, Situação); `Summary` traz o bloco completo com Hierarquia
  e Resumo em prosa.

---

## Selection Context

Infraestrutura oficial de contexto da seleção do usuário (Foundation Engine).
Captura o elemento selecionado e constrói seu contexto inteiramente a partir do
`ProjectGraph` e dos serviços de navegação/impacto/explicação — a seleção apenas
fornece o `ElementId` inicial; não há navegação direta ao Revit. Será consumida
futuramente por IA, Rules, Automation, Coordination e Document Engine.

- **Elemento selecionado** — `SelectionContextService.Create(ElementId)` /
  `Create()` (a partir de `UIDocument.Selection`) monta `SelectionContext` com
  `SelectedElementId`, `SelectedNode`, `SelectedCategory`, `SelectedName`.
- **Contexto completo** — `SelectedModel`, `SelectedLevel`, `Parents`,
  `Children` (via `ProjectGraphQuery`).
- **Explicação automática** — `Explanation` preenchido por
  `ElementExplanationService.Explain(node).Summary`.
- **Impacto** — `Impact` (`ImpactAnalysisResult`) via `ProjectImpactAnalyzer`.
- **Hierarquia** — derivada de `Parents`/`Children` e da explicação.
- `HasSelection()` indica se há seleção ativa; `Clear()` limpa a seleção.

---

## Copilot Context

Camada oficial de resolução de contexto do BIMBrain (Foundation Engine).
Centraliza, em um único objeto, todas as informações disponíveis antes da
execução de qualquer funcionalidade. Não responde perguntas, não usa IA, não
executa Rules e não faz consultas — apenas agrega contexto. Reutiliza
`SelectionContextService` para a seleção e `ProjectGraph` para o modelo.

- **Pergunta** — `CopilotContext.Question` / `HasQuestion`, informada em
  `Build(string)` ou `Build(string, ElementId)`.
- **Seleção** — `CopilotContext.SelectionContext` (via `SelectionContextService`);
  nulo quando não há seleção (`HasSelection`).
- **Projeto** — `CopilotContext.ProjectContext` (nome do projeto ativo).
- **Grafo** — `CopilotContext.ProjectGraph` (instância do Project Graph).
- **Contexto único** — `CopilotContext` reúne pergunta, seleção, projeto e grafo,
  pronto para ser consumido por AI, Rules e Automation.

---

## Copilot Orchestration

Infraestrutura oficial de orquestração do BIMBrain (Foundation Engine). Decide
apenas qual Engine deve atender uma solicitação com base no `CopilotContext` —
não executa nenhuma funcionalidade (sem QuestionProcessor, Rules, IA,
Automation, Coordination ou Document Engine).

- **Resolução de contexto** — `CopilotOrchestrator.Resolve(CopilotContext)` analisa
  `HasQuestion` e `HasSelection`.
- **Seleção de Engine** — pergunta → Question Engine; pergunta + seleção →
  Question Engine; apenas seleção → Selection Engine; nada → Unknown.
- **Roteamento** — `CopilotRoute` carrega `RequestType`, `EngineName`,
  `Reason` e `CanExecute`.
- **Motivo da decisão** — `Reason` explica o critério aplicado (ex.: "Existe
  pergunta e nenhuma seleção."); `CanExecute` é false apenas para Unknown.

---

## Copilot Execution

Ponto único de entrada do BIMBrain (Foundation Engine). Coordena a construção do
contexto e a resolução da rota, preparando o pipeline — não executa nenhum Engine
(sem QuestionProcessor, RuleRunner, IA, Automation, Coordination ou Document
Engine).

- **Ponto único de entrada** — `CopilotExecutor.Execute(...)` recebe pergunta
  e/ou `ElementId` e orquestra o pipeline.
- **Construção automática do contexto** — usa `CopilotContextBuilder` para montar
  o `CopilotContext`.
- **Resolução automática da rota** — usa `CopilotOrchestrator` para obter o
  `CopilotRoute`.
- **Resultado padronizado** — `CopilotExecutionResult` (Context, Route, Success,
  Message, Timestamp) descreve o estado "pronto para execução".

---

## Pipeline Integrado

Integração oficial do pipeline do Copilot ao fluxo do BIMBrain (Foundation +
Model Engine). A UI não chama `QuestionProcessor` diretamente: invoca
`CopilotExecutor`, que monta o `CopilotContext` e resolve o `CopilotRoute`, e
entrega o contexto a `QuestionProcessor` para execução das consultas BIM
existentes. Alteração exclusivamente arquitetural — a interface e o comportamento
observado pelo usuário permanecem idênticos.

- **UI integrada ao Copilot** — `MainWindow` roteia pela `CopilotExecutor` antes
  de consultar.
- **Contexto automático** — `CopilotContextBuilder` monta contexto (pergunta +
  seleção + projeto + grafo) a cada execução.
- **Roteamento automático** — `CopilotOrchestrator` decide o Engine (pergunta →
  Question Engine; pergunta + seleção → Question Engine; seleção → Selection
  Engine; nada → Unknown).
- **Compatibilidade com consultas existentes** — `QuestionProcessor` continua
  responsável por todas as consultas BIM; nenhum branch foi removido e nenhuma
  resposta foi alterada.

---

## Explicar Seleção

Primeira funcionalidade real do Copilot (AI Engine, sobre Foundation + Graph,
EPIC-0004). Explica o elemento selecionado no Revit inteiramente a partir do
Project Graph — sem IA, sem Ollama, sem `QuestionProcessor`, sem acesso direto
a `Document` para relações.

Fluxo: `ExplainSelectionCommand` → `ProjectGraphBuilder` → `CopilotContextBuilder`
→ `SelectionContextService` → `ElementExplanationService` → `TaskDialog`.

- **Seleção** — `ExplainSelectionCommand.Execute` lê `uidoc.Selection.GetElementIds()`;
  se vazia, exibe `"Selecione um elemento primeiro."`.
- **Contexto** — constrói `ProjectGraph` (`ProjectGraphBuilder`), `ProjectGraphQuery`,
  `ProjectImpactAnalyzer`, `ElementExplanationService` e `SelectionContextService`;
  monta `CopilotContext` via `CopilotContextBuilder`.
- **Explicação** — `ElementExplanationService.Explain(node)` retorna `ElementExplanation`
  (Node, Parents, Children, Dependencies, AffectedElements, Summary).
- **Exibição** — `TaskDialog` com Nome, Categoria, Modelo, Nível, Hierarquia,
  Dependências, Elementos afetados e Resumo (via `BuildSummary()`).
- **Cobertura do grafo** — o grafo atual cobre Projeto, Modelo, Nível, Painel e
  Circuito; elementos fora dessas categorias recebem aviso de que não estão
  mapeados no Project Graph.
- 100% Graph: nenhuma regra, nenhuma IA, nenhum `QuestionProcessor`.

---

## Execução de RuleSets

Primeira execução real de uma norma pela Ribbon (Rule Engine, EPIC-0003).
Ao clicar em `NBR 5410` (grupo Engenharia), o BIMBrain executa todas as regras
do RuleSet `NBR 5410`.

Fluxo: `RunNBR5410Command` → `RuleCatalog` → `RuleRunner` → `TaskDialog`.

- **Localização** — `RuleCatalog.All.FirstOrDefault(rs => rs.Name == "NBR 5410")`
  (não instancia regras manualmente).
- **Execução** — `RuleRunner(ruleSet.Rules).RunAll(doc)` executa todas as regras
  do RuleSet sobre o `Document` ativo.
- **Relatório** — `TaskDialog` com: Regras executadas, Regras aprovadas, Warnings,
  Errors, resultado por regra (`✔` aprovada, `⚠` warning, `❌` erro) e Resultado
  Geral (`MODELO APROVADO` ou `MODELO COM INCONSISTÊNCIAS`).
- Sem IA, sem `QuestionProcessor`, sem Ollama; nenhuma regra foi alterada.

---

## Rule Results Window

Janela oficial de visualização de resultados de regras (Rule Engine, EPIC-0003).
Substitui o `TaskDialog` da execução do RuleSet por uma interface profissional,
reutilizável por qualquer execução de regras do BIMBrain.

- **Lista superior** — todas as `RuleResult`, com Ícone (✔/⚠/❌), Nome da regra,
  Status (Aprovada/Aviso/Erro) e Mensagem resumida.
- **Painel inferior** — ao selecionar uma regra, mostra Nome, Severidade,
  Mensagem completa e Quantidade de elementos afetados.
- **Selecionar** — `RuleActionService.Select(uiDoc, RuleResult)` (seleciona os
  `AffectedElements` no Revit; desabilitado quando não há elementos).
- **Navegar** — `RuleActionService.Navigate(uiDoc, RuleResult)` (zoom até os
  `AffectedElements`; desabilitado quando não há elementos).
- **Exportar** — placeholder ("Funcionalidade em desenvolvimento.").
- **Fechar** — fecha a janela.
- Utiliza `RuleResult.AffectedElements` diretamente; não recalcula nem consulta
  o modelo novamente. Sem IA, sem `QuestionProcessor`.

---

## Knowledge Viewer

Infraestrutura que conecta uma Rule ao seu documento oficial de conhecimento
(Knowledge Engine, EPIC-0005). Sem IA; carrega e exibe o Markdown como texto
simples (sem interpretação de Markdown nesta versão).

- `KnowledgeDocument` — `Title`, `RuleId`, `Standard`, `Markdown`, `Exists`.
- `KnowledgeRepository.GetByRuleName("NBR5410-001")` localiza
  `knowledge/standards/NBR5410/rules/NBR5410-001.md` (busca a pasta `knowledge`
  a partir do diretório do assembly, subindo até 8 níveis), carrega o conteúdo e
  preenche `KnowledgeDocument` (`Exists = false` quando não encontrado).
- `RuleResultsWindow` ganha o botão **Conhecimento**: ao clicar, carrega a
  documentação da regra selecionada e a abre em `KnowledgeWindow` (WPF com
  `ScrollViewer`, Markdown como texto puro). Se não houver documentação, exibe
  `"Documentação não encontrada."`.
- `KnowledgeWindow` — janela WPF simples (sem interpretação de Markdown).
- Não altera nenhuma Rule, não altera `RuleRunner`, não utiliza IA.

---

## Rule Inspector

Inspector visual dos elementos afetados por uma Rule (Rule Engine, EPIC-0003).
Sem IA, sem alteração nas Rules. Toda interação ocorre por janelas próprias do
BIMBrain (nenhum `TaskDialog`).

- `RuleInspectorWindow(UIDocument, RuleResult)` percorre `RuleResult.AffectedElements`
  e obtém, para cada `ElementId`, via `Document.GetElement`: Id, Nome, Categoria,
  Família (de `FamilyInstance.Symbol.Family`), Tipo (de `FamilyInstance.Symbol`),
  Nível (de `Element.LevelId`, com fallback `FAMILY_LEVEL_PARAM`).
- Lista WPF (`ListView` + `GridView`) com as colunas Id, Nome, Categoria,
  Família, Tipo, Nível.
- Selecionar um item → detalhes no painel inferior.
- **Selecionar** → `uiDoc.Selection.SetElementIds` apenas do elemento escolhido.
- **Navegar** → `uiDoc.ShowElements` apenas do elemento escolhido (zoom).
- **Fechar** → fecha a janela.
- `RuleResultsWindow` ganha o botão **Inspector**, que abre
  `RuleInspectorWindow(_uiDoc, regraSelecionada)`.
- Não altera `RuleRunner`, `RuleCatalog`, `EngineeringRule`, `QuestionProcessor`
  nem o Copilot.

---

## Classification Engine

Infraestrutura oficial de classificação de elementos (Classification Engine,
EPIC-0005). **Status: Category + Type + Family Classification.** Fluxo: (1)
Categoria Revit → (2) Alias do tipo (`Symbol.Name`) → (3) Alias da família
(`Family.Name`) → (4) Unknown. Sem Parâmetros nem IA.

- **ElementClassificationType** — Unknown, Outlet, Switch, LightingFixture, Panel,
  Circuit, Conduit, CableTray, Room, Level, Project, Model.
- **ElementClassification** — `ElementId`, `Classification`, `Confidence`, `Reason`.
- **Classify(Element)** — 1) `BuiltInCategory` com `Confidence = 100`,
  `Reason = "Classificado pela categoria Revit."`; 2) se Unknown e `FamilyInstance`,
  `Symbol.Name` via `FindByAlias` + contains (`GetAllAliases`) → `Confidence = 80`,
  `Reason = "Classificado pelo alias do tipo."`; 3) se ainda Unknown,
  `Symbol.Family.Name` via `FindByAlias` + contains → `Confidence = 70`,
  `Reason = "Classificado pelo alias da família."`; 4) senão Unknown,
  `Confidence = 0`, `Reason = "Categoria ainda não mapeada."`.
- Ex.: "Tomadas Hospital" → Outlet, "Interruptores" → Switch, "Luminárias" →
  LightingFixture (contains sobre os aliases); painéis e circuitos pela categoria.
- **ElementClassifier.Classify(IEnumerable<Element>)** aplica `Classify(Element)`.
- **ElementClassifier.Classify(ElementId)** permanece stub (sem `Document`).
- **ClassificationRepository** — carrega `knowledge/classification/aliases.json`;
  `GetAliases`, `GetAllAliases`, `HasAlias`, `FindByAlias`.
- Posiciona-se entre o Model Engine e o Project Graph; sem IA, sem Rules, sem
  alteração de comportamento existente.
- Observação: o `ElectricalCircuitAnalyzer` agora identifica tomada/interruptor/
  luminária exclusivamente via Classification Engine (não mais por
  `BuiltInCategory`); apenas a localização dos elementos usa categoria.
- As Rules expõem dados estruturados (`RuleResult.Data`) para reutilização
  interna; o `ModelIntegrityAnalyzer` consome esses dados em vez de interpretar
  mensagens (TASK-0078).
- Distribuição por níveis baseada em classificação: `ElementLevelAnalyzer` decide
  Outlet/Switch/LightingFixture/Panel exclusivamente via `ElementClassifier`
  (TASK-0079).
- Interpretação baseada em dicionário de engenharia: `QuestionProcessor`
  identifica elementos na pergunta via `ClassificationRepository.GetAllAliases`
  (`DetectClassification`); novos sinônimos passam a funcionar apenas editando
  `aliases.json`, sem alterar o código (TASK-0080).

---

## Engineering Dictionary

Primeiro dicionário oficial de engenharia do BIMBrain (Knowledge Engine +
Classification Engine, EPIC-0005). **Status: cadastrado (somente leitura).**
Ainda NÃO é usado para classificar elementos.

- **knowledge/classification/aliases.json** — mapa tipo → aliases:
  `Outlet` (tomada, tug, socket, outlet, power outlet, tomada dupla/simples/usb),
  `Switch` (interruptor, switch, three way, paralelo, intermediário),
  `LightingFixture` (luminária, luminaria, light fixture, lighting fixture, led
  panel, spot), `Panel` (painel, quadro, qd, qdl, qdc, distribution board),
  `Circuit` (circuito, circuit).
- **ClassificationRepository** carrega o JSON e expõe `GetAliases(type)`,
  `GetAllAliases()`, `HasAlias(type, alias)` e `FindByAlias(alias)`
  (case-insensitive).
- Nenhuma Rule alterada, nenhuma IA, nenhuma consulta, nenhuma classificação
  utiliza aliases ainda — apenas infraestrutura.

---

## Query Infrastructure

Infraestrutura oficial de handlers de consulta (Question Engine, EPIC-0006).
**Status: infraestrutura (sem handlers concretos).** Nenhum comportamento
existente foi alterado; o `QuestionProcessor` continua funcionando como hoje.

- `src/BIMBrain/Queries/IQueryHandler.cs` — contrato `CanHandle(QueryContext)`
  + `HandleAsync(QueryContext)` → `QueryResult`.
- `src/BIMBrain/Queries/QueryContext.cs` — contexto (`Question`, `CopilotContext`,
  `UIDocument`, `Document`); sem lógica.
- `src/BIMBrain/Queries/QueryResult.cs` — `Success`, `Response`, `Handled`.
- `src/BIMBrain/Queries/QueryRouter.cs` — recebe `IEnumerable<IQueryHandler>`,
  resolve o primeiro `CanHandle() == true`, executa `HandleAsync`; se nenhum
  aceitar, retorna `Handled = false`.
- Destinada a substituir gradualmente os blocos condicionais do `QuestionProcessor`.

---

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
