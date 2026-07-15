# Arquitetura do BIMBrain

Este documento descreve a arquitetura atual do BIMBrain e a responsabilidade de
cada componente existente. Nenhuma alteração de código é descrita aqui — apenas
a estrutura vigente.

## Fluxo principal

```
UI (WPF)
   ↓
CopilotExecutor
   ↓
CopilotContextBuilder
   ↓
CopilotOrchestrator
   ↓
QuestionProcessor
   ↓
Analyzers
   ↓
Project Graph
   ↓
ProjectGraphQuery
   ↓
ProjectImpactAnalyzer
   ↓
ElementExplanationService
   ↓
Rules Engine
   ↓
Knowledge
   ↓
Revit API
```

Fluxo oficial integrado (TASK-0064): a UI não chama `QuestionProcessor`
diretamente. Ela invoca `CopilotExecutor`, que monta o `CopilotContext` e resolve
o `CopilotRoute`; o `CopilotContext` é então entregue a `QuestionProcessor`, que
segue responsável por todas as consultas BIM existentes (nenhum branch removido,
nenhuma resposta alterada). `SelectionContextService` é usado internamente por
`CopilotContextBuilder`.

O `CopilotExecutor` é o ponto único de entrada: constrói o contexto e resolve a
rota, preparando o pipeline para os motores consumidores (não executa nenhum
Engine):

```
UI
   ↓
CopilotExecutor
   ↓
CopilotContextBuilder
   ↓
CopilotOrchestrator
   ↓
Question Engine / Selection Engine / Rule Engine / Knowledge / AI / Automation / Coordination / Document
```

1. A **UI (WPF)** recebe a pergunta do usuário e dispara a consulta.
2. O **QuestionProcessor** normaliza o texto, decide se a pergunta é resolvida
   diretamente ou via IA (Ollama), e formata a resposta.
3. Consultas diretas utilizam os **Analyzers** ou a leitura do **ModelContext**.
4. Análises de conformidade utilizam a **Rules Engine** (RuleRunner + catálogo).
5. Regras normativas apoiam-se na **Knowledge** documentada.
6. Toda leitura de dados passa pela **Revit API**.

## Principais componentes

### Orquestração

- **QuestionProcessor** — Ponto de entrada das consultas. Normaliza o texto,
  roteia para analisadores ou para a IA (Ollama), executa tool calling e formata
  a resposta estruturada. A identificação de elementos de engenharia na pergunta
  (tomada/interruptor/luminária/quadro/etc.) é feita via `DetectClassification`,
  que consulta o Engineering Dictionary (`ClassificationRepository.GetAllAliases`)
  — não há palavras fixas no código.

### Query Handlers (infraestrutura — TASK-0081)

Camada oficial de handlers de consulta, destinada a substituir gradualmente os
blocos condicionais do `QuestionProcessor`. Nesta fase existem apenas os tipos
base; nenhum handler concreto foi criado e nenhum `if` foi removido.

- **Question Engine → Query Router → Query Handlers**:
  - `IQueryHandler` — contrato `CanHandle(QueryContext)` +
    `HandleAsync(QueryContext)` (retorna `QueryResult`).
  - `QueryContext` — contexto da consulta (`Question`, `CopilotContext`,
    `UIDocument`, `Document`); sem lógica.
  - `QueryResult` — `Success`, `Response`, `Handled`.
  - `QueryRouter` — recebe `IEnumerable<IQueryHandler>`, resolve o primeiro
    `CanHandle() == true`, executa `HandleAsync` e retorna o `QueryResult`; se
    nenhum aceitar, retorna `Handled = false`.

### Contexto do modelo

- **ModelContext** — Descobre o documento ativo e os modelos vinculados
  (`RevitLinkInstance`), expondo uma coleção somente leitura de informações por
  documento (nome, tipo, status, níveis, rooms, elementos, caminho).

### Classification (Classification Engine)

Infraestrutura oficial de classificação de elementos (TASK-0072, EPIC-0005).
Nesta versão é apenas arquitetura: nenhum elemento é classificado.

- **ElementClassificationType** — Enumeração dos tipos possíveis (Unknown, Outlet,
  Switch, LightingFixture, Panel, Circuit, Conduit, CableTray, Room, Level,
  Project, Model).
- **ElementClassification** — Resultado de uma classificação (ElementId,
  Classification, Confidence, Reason).
- **ElementClassifier** — `Classify(Element)`, `Classify(ElementId)` e
  `Classify(IEnumerable<Element>)`; nesta versão retornam `Unknown`,
  `Confidence = 0` e `Reason = "Classification engine not implemented."`.
- **ClassificationRepository** — Base de conhecimento futura; `GetAliases(type)`
  retorna lista vazia (sem implementação).

O Classification Engine posiciona-se entre o Model Engine e o Project Graph:
no futuro alimentará o grafo com classificações enriquecidas dos elementos.

### Engineering Dictionary (Knowledge Engine)

Primeiro dicionário oficial de engenharia do BIMBrain (TASK-0074, EPIC-0005).
Nesta task é apenas cadastrado — ainda NÃO é usado para classificar elementos.

- **knowledge/classification/aliases.json** — mapa `ElementClassificationType`
  (string) → lista de aliases (ex.: `Outlet` → tomada, tug, socket, outlet…;
  `Switch`, `LightingFixture`, `Panel`, `Circuit`).
- **ClassificationRepository** — carrega `aliases.json` (buscando a pasta
  `knowledge` a partir do assembly) e expõe:
  - `GetAliases(ElementClassificationType)` — aliases de um tipo.
  - `GetAllAliases()` — dicionário completo.
  - `HasAlias(ElementClassificationType, string)` — pertence ao tipo?
  - `FindByAlias(string)` — resolve o tipo a partir de um alias (case-insensitive).
- Apenas leitura; nenhuma classificação utiliza aliases ainda.

### Graph (Model Engine)

- **GraphNode** — Representa um elemento do modelo (ElementId, Category, Name,
  DocumentName). Sem lógica de negócio.
- **GraphEdge** — Representa um relacionamento entre nós (Source, Target,
  Relation).
- **GraphRelation** — Enumeração dos tipos de relacionamento (Contains,
  ConnectedTo, FedBy, LocatedIn, BelongsTo, LinkedTo).
- **ProjectGraph** — Armazena nós e arestas e expõe navegação/busca
  (AddNode, AddEdge, GetNode, GetNeighbors, FindByCategory, FindByName).
- **ProjectGraphBuilder** — Constrói o grafo inicial a partir de um `Document`,
  reutilizando ModelContext, PanelAnalyzer e ElectricalCircuitAnalyzer. Cria nós
  para projeto, modelos vinculados carregados, níveis, painéis e circuitos, e
  relacionamentos simples (Projeto Contains Modelo, Modelo Contains Nível, Nível
  Contains Painel, Painel FedBy Circuito).
- **ProjectGraphQuery** — Camada de navegação sobre o `ProjectGraph`. Recebe um
  grafo e nunca acessa o Revit diretamente. Expõe busca (FindNode,
  FindNodesByCategory, FindNodesByName), navegação direcional (GetNeighbors,
  GetChildren, GetParents, GetDescendants, GetAncestors), caminhos (FindPath via
  BFS, ShortestPath) e traversia BFS (Traverse → TraversalResult). É genérica e
  reutilizável por qualquer disciplina.
- **ProjectImpactAnalyzer** — Infraestrutura de análise de impacto sobre o grafo.
  Recebe um `ProjectGraph`, utiliza exclusivamente `ProjectGraphQuery` (nunca
  acessa o Revit, `Document` ou `FilteredElementCollector`) e calcula
  dependências (upstream), elementos afetados (downstream), resumo de impacto e
  predicados `HasImpact`, `IsLeaf`, `IsRoot`. Genérico e reutilizável por
  qualquer disciplina.
- **ElementExplanationService** — Primeiro caso de uso completo sobre o grafo,
  combinando `ProjectGraph`, `ProjectGraphQuery` e `ProjectImpactAnalyzer`. Recebe
  os três e entrega uma explicação completa de um elemento (`ElementExplanation`):
  pais, filhos, dependências (upstream), elementos afetados (downstream), cadeia
  hierárquica e resumo textual. Não acessa Revit, `Document` ou
  `FilteredElementCollector`. Genérico e reutilizável por qualquer disciplina.

### Foundation — Selection Context

- **SelectionContext** — Estado do contexto da seleção do usuário: elemento
  selecionado (`SelectedElementId`, `SelectedNode`), categoria, nome, modelo,
  nível, pais, filhos, explicação automática e impacto (`ImpactAnalysisResult`).
- **SelectionContextService** — Infraestrutura oficial de contexto da seleção
  (Foundation Engine). Recebe `UIDocument`, `ProjectGraph`, `ProjectGraphQuery`,
  `ProjectImpactAnalyzer` e `ElementExplanationService`. A seleção apenas obtém o
  `ElementId` inicial; toda descoberta de relações usa o grafo (sem acesso
  direto ao Revit para navegação). `Create()`/`Create(ElementId)` montam o
  contexto, `HasSelection()` indica seleção ativa e `Clear()` limpa a seleção.
  Será consumido futuramente por IA, Rules, Automation, Coordination e Document
  Engine. Reutiliza os serviços existentes, sem duplicar código.
- **CopilotContext** — Objeto único de contexto com `ProjectContext`,
  `SelectionContext`, `Question`, `Timestamp`, `HasSelection`, `HasQuestion` e
  `ProjectGraph`.
- **CopilotContextBuilder** — Camada oficial de resolução de contexto (Foundation
  Engine). Recebe `UIDocument`, `ProjectGraph`, `ProjectGraphQuery`,
  `ProjectImpactAnalyzer`, `ElementExplanationService` e `SelectionContextService`.
  Agrega pergunta, seleção (via `SelectionContextService`), projeto e grafo em um
  único `CopilotContext`. Não responde perguntas, não usa IA, não executa Rules e
  não faz consultas — apenas agrega contexto. Overloads: `Build()`,
  `Build(string question)`, `Build(ElementId)`, `Build(string question, ElementId)`.
- **CopilotRequestType** — Enumeração dos tipos de solicitação roteáveis
  (Unknown, Question, Selection, RuleCheck, Knowledge, AI, Automation,
  Coordination, Document).
- **CopilotRoute** — Decisão de roteamento: `RequestType`, `EngineName`, `Reason`,
  `CanExecute`.
- **CopilotOrchestrator** — Infraestrutura oficial de orquestração (Foundation
  Engine). Recebe `CopilotContext` e, em `Resolve()`/`Resolve(CopilotContext)`,
  decide apenas qual Engine atende a solicitação: pergunta → Question Engine;
  pergunta + seleção → Question Engine (a IA usa a seleção depois); apenas seleção
  → Selection Engine; nada → Unknown. Não executa QuestionProcessor, Rules, IA,
  Automation, Coordination ou Document Engine — apenas decide.
- **CopilotExecutionResult** — Resultado padronizado do pipeline: `Context`,
  `Route`, `Success`, `Message`, `Timestamp`.
- **CopilotExecutor** — Ponto único de entrada do BIMBrain (Foundation Engine).
  Recebe `UIDocument`, `CopilotContextBuilder` e `CopilotOrchestrator`. Em
  `Execute()` / `Execute(string)` / `Execute(ElementId)` /
  `Execute(string, ElementId)` monta o `CopilotContext`, resolve o `CopilotRoute`
  e retorna `CopilotExecutionResult`. Não executa nenhum Engine — apenas prepara o
  pipeline.

### Analisadores (Model Engine)

- **ElementLevelAnalyzer** — Agrupa elementos elétricos por nível, retornando a
  distribuição por pavimento. Localiza candidatos via `BuiltInCategory` e decide
  o tipo (Outlet/Switch/LightingFixture/Panel) exclusivamente via `ElementClassifier`.
- **ElectricalCircuitAnalyzer** — Coleta todos os `ElectricalSystem` e produz a
  contagem de tomadas, luminárias e interruptores por circuito, além do conjunto
  de elementos conectados. A identificação de tomada/interruptor/luminária passa
  obrigatoriamente pelo **Classification Engine** (`ElementClassifier`):
  `BuiltInCategory` ainda localiza os elementos, mas nunca decide o que o
  elemento representa.
- **PanelAnalyzer** — Identifica painéis elétricos (`ElectricalEquipment`) e
  associa seus circuitos e contagens por categoria.
- **ModelIntegrityAnalyzer** — Verifica a integridade do modelo elétrico
  (elementos e painéis sem circuito), reutilizando os analisadores acima. Consome
  os dados estruturados de `RuleResult.Data` (produzidos pelas Rules) — não
  interpreta `Message` nem usa Regex.

### Rules Engine

- **RuleRunner** — Executa todas as regras de engenharia registradas e aplaina
  os resultados.
- **RuleCatalog** — Catálogo estático de RuleSets disponíveis (ex.: NBR 5410).
- **RuleSet** — Agrupamento lógico de regras (nome, descrição, lista de regras).
- **EngineeringRule** — Classe base abstrata de toda regra.
- **RuleResult** — Resultado de uma regra, com severidade, mensagem e elementos
  afetados.
- **RuleSeverity** — Enumeração de severidade do resultado.

### Action Engine

- **RuleActionService** — Orquestra as ações sobre um `RuleResult`
  (selecionar, navegar, destacar).
- **RuleSelectionService** — Seleciona os elementos afetados no Revit.
- **RuleNavigationService** — Navega (zoom) até os elementos afetados.
- **RuleHighlightService** — API de destaque visual (esqueleto; implementação
  futura com OverrideGraphicSettings).

### Knowledge

- Documentada em `knowledge/standards/` (separada da implementação em
  `src/BIMBrain/Rules/`), com índice de regras normativas por norma (NBR 5410).

### UI

- Janela WPF code-only (sem XAML) em 4 regiões: informações do projeto,
  consulta, resposta + histórico e barra de status. Apoia-se em painéis
  auxiliares (`ResponsePanel`, `HistoryPanel`, `StatusBar`) para evitar
  crescimento do `MainWindow`.

## Princípios arquiteturais aplicados

- Núcleo independente das disciplinas.
- Conhecimento separado da implementação.
- Componentes reutilizáveis entre consultas e regras.
- Sem dependências desnecessárias.
- Sem modificação do modelo sem autorização.
