# AGENTS

## Objetivo

Definir o comportamento esperado de agentes de IA que trabalham no projeto BIMBrain.

## Antes de iniciar qualquer tarefa

Ler obrigatoriamente:

- README.md
- PROJECT_MEMORY.md
- ROADMAP.md
- TASK.md

## Durante a implementação

- Não alterar arquitetura sem autorização
- Não implementar funcionalidades fora da Sprint
- Manter simplicidade
- Evitar dependências desnecessárias
- Respeitar a documentação existente
- Manter alterações mínimas
- Toda nova regra normativa deverá possuir documentação correspondente em knowledge/standards antes de sua implementação
- Seguir o fluxo de trabalho definido no PROJECT_MEMORY
- Todo novo desenvolvimento deve pertencer obrigatoriamente a um EPIC antes da criação de uma TASK
- Toda nova funcionalidade deve pertencer a um Engine
- Toda nova TASK deverá informar: Engine, Disciplina, EPIC e User Story
- Nenhuma implementação poderá ser iniciada sem pertencer a uma dessas categorias

## Ao finalizar

- Resumir alterações realizadas
- Não realizar commit
- Aguardar revisão humana

## Princípios de Engenharia

- Simplicidade
- Clareza
- Baixo acoplamento
- Alta legibilidade
- Evitar overengineering
- Documentação antes de código quando necessário

## O que evitar

- Criar funcionalidades não solicitadas
- Mudar arquitetura sem autorização
- Adicionar bibliotecas sem necessidade
- Criar arquivos desnecessários
- Modificar documentos fora da tarefa
- Repetir informações já documentadas

## Anchored Summary
### TASK-0022 / TASK-0022.1 — Ollama connectivity + JSON deserialization
- "Testar IA" button: HttpClient POST to localhost:11434/api/chat (qwen3)
- Switched from manual string parsing to JsonSerializer.Deserialize<OllamaChatResponse>()
- DTOs: OllamaChatResponse, OllamaMessage, OllamaToolCall, OllamaFunctionCall (PropertyNameCaseInsensitive)
- Added System.Text.Json NuGet v10.0.9
- OnConsultarClick became async void
- Fixed function schemas: potential_total_instalada -> potencia_total_instalada; removed eletrodutos from categoria_com_mais_elementos

### TASK-0023 — README fix
- Stack table: "C# 12 / .NET 8" -> "C# / .NET Framework 4.8 (net48)" + note

### TASK-0024 / TASK-0024.1 / TASK-0024.2 — Tool calling + timeout
- Unmatched questions → Ollama tools param with 14 function schemas
- ExecuteToolByName routes tool_calls[0].function.name to same handlers
- No tool match: "IA não encontrou função correspondente"
- Timeout root cause: 30s too short (~37-82s); fixed to 120s
- TaskCanceledException caught separately: "A IA demorou demais para responder"
- Other exceptions: "Pergunta ainda não suportada pelo BIMBrain."

### TASK-0025 — Known limitation
- PROJECT_MEMORY.md "Limitações Conhecidas": qwen3 may select wrong function

### TASK-0026 / TASK-0026.1 — Retroactive task files
- 25 files in .tasks/ (TASK-0013 through TASK-0025)
- Corrections: TASK-0013, TASK-0013.1, TASK-0017

### TASK-0027 — QuestionProcessor refactoring
- New QuestionProcessor.cs (BIMBrain namespace): ProcessQuestionAsync, Normalize, CountByCategory, SumParameter, SumPowerParameter, FindCategoryWithMostElements, ListLoadedFamilies, TryResolveWithOllamaAsync, GetToolSchemas, ExecuteToolByName, HttpClient, all DTOs — extracted from MainWindow
- MainWindow.xaml.cs (BIMBrain.UI): delegates to QuestionProcessor, keeps UI only
- Window 520×600, Stopwatch timing, structured response panel (status/project/question/result/elapsed ms)
- SetInitialState / SetResponseSuccess / SetResponseError / SetResponseSimple
- Error prefix detection → "Consulta não realizada"
- History: [HH:mm:ss] + ms
- docs/US-0004.md (response UX), docs/US-0005.md (QuestionProcessor refactoring), AUDITORIA.txt created
- Build: 0 errors, 12 warnings (Revit DLL conflicts)

### TASK-0028 — Resumo do projeto (composite query)
- New "resumo do projeto" / "faça um resumo do projeto" query in ProcessQuestionAsync
- Reuses existing methods: CountByCategory, SumParameter, ListLoadedFamilies, inline collectors
- Composite executive summary: Projeto, Níveis, Ambientes, Tomadas, Interruptores, Luminárias, Quadros, Circuitos, Comprimento de conduítes, Área construída, Famílias carregadas
- docs/US-0006.md
- Build: 0 errors

### TASK-0029 — ModelContext + contexto do projeto
- New ModelContext.cs (BIMBrain namespace): discovers RevitLinkInstance, gets LinkDocument, read-only collection
- New LinkedDocumentInfo: Name, LinkDocument, IsLoaded flag
- New query in ProcessQuestionAsync: "modelos carregados", "informacoes dos modelos", "contexto do projeto"
- Lists active project, all loaded/unloaded links, rooms per linked document
- knowledge/revit/API.md: added RevitLinkInstance
- docs/US-0007.md
- Build: 0 errors

### TASK-0030 — UX overhaul
- Window resized to 900x700, resizable, min 700x500
- Four-region layout: Project info, Query input, Response+History side-by-side, Status bar
- Response replaced with read-only TextBox (scrollable, word wrap)
- History: Copy, Clear buttons, click-to-recall answers, 280px wide
- Status bar: Status / Tempo / Modelos carregados
- Enter key triggers consultation
- New files: ResponsePanel.cs, HistoryPanel.cs, StatusBar.cs (prevent MainWindow growth)
- ModelContext deduplicates links by document name
- docs/US-0008.md
- Build: 0 errors

### TASK-0031 — UX polish
- History: shows only question title, vertical scrollbar always visible
- History click-to-recall: reloads question, answer, time, and origin
- "Testar IA" button removed from interface
- "Consulta" field: label renamed from "Faça uma pergunta", added placeholder (GotFocus/LostFocus)
- "Consultar" button renamed to "Executar"
- StatusBar: added "Origem" field (BIMBrain or IA) detected via answer prefix
- ResponsePanel: cleaner formatting with spacing, origin in footer
- New files: none (updated existing 4 files)
- docs/US-0009.md
- Build: 0 errors
- CAPABILITIES.md is the official reference of implemented capabilities

### TASK-0033 — Diagnóstico automático
- New query: "diagnóstico", "analisar projeto", "analisar modelo", "saúde do projeto", "health check"
- BuildProjectDiagnosis() reuses CountByCategory, SumParameter, ModelContext — no duplicated collectors
- Report: summary (10 fields) + analysis (8 checks ✔/⚠) + status (OK/ATENÇÃO)
- No IA used — pure engine analysis
- Tool schema added for Ollama routing
- docs/US-0010.md, CAPABILITIES.md updated
- Build: 0 errors

### TASK-0035 — Diagnóstico avançado dos modelos vinculados
- ModelContext.cs: LinkedDocumentInfo → DocumentInfo (Name, Type, IsLoaded, IsMainDocument, RoomCount, LevelCount, ElementCount, FilePath); ModelContext gains MainDocument + HasDuplicateNames
- QuestionProcessor.cs: "contexto do projeto" rewrite — Resumo quantitativo (7 fields), listagem por modelo, Diagnóstico (3 checks ✔/⚠)
- Helper methods CountRooms, CountLevels, CountElements em DocumentInfo (evita duplicação)
- docs/US-0012.md, CAPABILITIES.md atualizados
- Build: 0 errors

### TASK-0036 — Distribuição de elementos elétricos por nível
- New ElementLevelAnalyzer.cs (BIMBrain namespace): Analyze(BuiltInCategory) → Dictionary<string,int> grouped by level name; GetLevelName via LEVEL_PARAM + FamilyInstance fallback
- QuestionProcessor.cs: new query branch for "por nivel" / "distribuicao" patterns; DetectDistributionCategory() extracts 4 categories; FormatDistribution() formats structured response with per-level detail + resumo
- Engine-only — no IA used
- docs/US-0013.md, CAPABILITIES.md atualizados
- Build: 0 errors

### TASK-0037 — Análise estrutural dos circuitos elétricos
- New ElectricalCircuitAnalyzer.cs (BIMBrain namespace): CircuitInfo class (Name, PanelName, TomadaCount, LuminariaCount, InterruptorCount); Analyze() collects all ElectricalSystem with per-circuit breakdown by category
- QuestionProcessor.cs: new query branch for "listar circuitos", "mostrar circuitos", "informacoes dos circuitos", "resumo dos circuitos", "estrutura dos circuitos"; FormatCircuitSummary() formats structured response with per-circuit detail + resumo
- Engine-only — no IA used; no duplicated collectors
- docs/US-0014.md, CAPABILITIES.md atualizados
- Build: 0 errors

### TASK-0038 — Análise estrutural dos painéis elétricos
- New PanelAnalyzer.cs (BIMBrain namespace): PanelInfo class (Name, CircuitCount, TomadaCount, LuminariaCount, InterruptorCount); Analyze() finds ElectricalEquipment panels and reuses ElectricalCircuitAnalyzer to match circuits by BaseEquipment name
- QuestionProcessor.cs: new query branch for "listar paineis", "resumo dos paineis", "informacoes dos paineis", "estrutura dos paineis", "circuitos por painel"; FormatPanelSummary() formats structured response with per-panel detail + resumo
- Engine-only — no IA used; no duplicated collectors
- docs/US-0015.md, CAPABILITIES.md atualizados
- Build: 0 errors

### TASK-0039 — Verificação de integridade do modelo elétrico
- New ModelIntegrityAnalyzer.cs (BIMBrain namespace): ModelIntegrityResult (TomadasSemCircuito, LuminariasSemCircuito, InterruptoresSemCircuito, PaineisSemCircuitos); Analyze() reuses ElectricalCircuitAnalyzer.GetConnectedElementIds + PanelAnalyzer
- ElectricalCircuitAnalyzer.cs: added GetConnectedElementIds() helper (HashSet<ElementId>)
- QuestionProcessor.cs: new query branch for "verificar modelo", "verificar integridade", "analisar modelo", "existem elementos sem circuito", "diagnostico de integridade"; FormatIntegrityReport() formats OK/issues response
- Engine-only — no IA used; reuses 3 existing analyzers
- docs/US-0016.md, CAPABILITIES.md atualizados
- Build: 0 errors

### TASK-0040 — Infraestrutura da Rules Engine
- New files under src/BIMBrain/Rules/: EngineeringRule.cs (abstract base), RuleResult.cs, RuleSeverity.cs (enum), RuleRunner.cs (executor)
- Architecture: any future engineering rule inherits EngineeringRule and is executed by RuleRunner
- No real rules implemented yet — infrastructure only
- docs/US-0017.md, CAPABILITIES.md atualizados
- Build: 0 errors

### TASK-0041 — Primeira regra de consistência: circuitos sem painel
- New src/BIMBrain/Rules/Consistency/CircuitWithoutPanelRule.cs — first real rule
- Herda EngineeringRule, implementa Execute(Document), percorre ElectricalSystem, checa BaseEquipment == null
- EngineeringRule.Execute retorna List<RuleResult> (não mais scalar) para suportar múltiplos resultados por regra
- RuleRunner.RunAll usa SelectMany para achatar resultados
- Nenhuma consulta existente alterada, nenhuma interface modificada, nenhuma IA utilizada
- docs/US-0018.md, CAPABILITIES.md, AGENTS.md atualizados
- Build: 0 errors

### TASK-0042 — Segunda regra de consistência: painéis sem circuitos
- New src/BIMBrain/Rules/Consistency/PanelWithoutCircuitRule.cs — reusa PanelAnalyzer existente
- Filtra painéis com CircuitCount == 0, gera RuleResult por painel inconsistente
- Nenhuma lógica duplicada, nenhuma consulta alterada, nenhuma IA utilizada
- docs/US-0019.md, CAPABILITIES.md, AGENTS.md atualizados
- Build: 0 errors

### TASK-0043 — Terceira regra de consistência: elementos sem circuito
- New src/BIMBrain/Rules/Consistency/ElementsWithoutCircuitRule.cs — reusa ElectricalCircuitAnalyzer.GetConnectedElementIds
- Um RuleResult por categoria (tomadas, luminárias, interruptores) com Warning + contagem
- ModelIntegrityAnalyzer.cs refatorado: lógica própria de elementos removida, passa a executar a regra via RuleRunner e extrair counts das mensagens
- Painéis detection no ModelIntegrityAnalyzer permanece inalterado (PanelAnalyzer direto)
- Comportamento externo de FormatIntegrityReport preservado
- Nenhuma lógica duplicada, nenhuma consulta alterada, nenhuma IA utilizada
- docs/US-0020.md, CAPABILITIES.md, AGENTS.md atualizados
- Build: 0 errors

### TASK-0044 — Quarta regra de consistência: modelos vinculados descarregados
- New src/BIMBrain/Rules/Consistency/UnloadedLinksRule.cs — reusa ModelContext existente
- Itera ctx.Links, filtra !IsLoaded, gera RuleResult por modelo descarregado
- Nenhuma lógica duplicada, nenhuma consulta alterada, nenhuma IA utilizada
- docs/US-0021.md, CAPABILITIES.md, AGENTS.md atualizados
- Build: 0 errors

### TASK-0045 — Quinta regra de consistência: painéis com nomes duplicados
- New src/BIMBrain/Rules/Consistency/DuplicatePanelNameRule.cs — reusa PanelAnalyzer existente
- Agrupa painéis por nome, ignora nomes vazios, gera RuleResult por grupo com mais de 1
- Nenhuma lógica duplicada, nenhuma consulta alterada, nenhuma IA utilizada
- docs/US-0022.md, CAPABILITIES.md, AGENTS.md atualizados
- Build: 0 errors

### TASK-0046 — Base normativa estruturada (NBR 5410)
- knowledge/standards/ structure created: README.md, NBR5410/README.md, NBR5410/INDEX.md
- Documents separation between knowledge (INDEX.md) and implementation (src/BIMBrain/Rules/)
- NBR5410 INDEX with 5 planned rules (NBR5410-001 through NBR5410-005)
- ROADMAP.md: EPIC-0003 status updated with normative phase note
- CAPABILITIES.md: new "Base Normativa" section added
- AGENTS.md: new rule added — "Toda nova regra normativa deverá possuir documentação correspondente em knowledge/standards antes de sua implementação"
- No code changed — documentation only

### TASK-0047 — Primeira regra normativa (NBR5410-001)
- knowledge/standards/NBR5410/rules/NBR5410-001.md created with full documentation
- INDEX.md: NBR5410-001 status changed to "Em implementação"
- New src/BIMBrain/Rules/Standards/NBR5410/NBR5410_001_ConnectedOutletsRule.cs
- Reuses ElementsWithoutCircuitRule — filters results for outlet-specific messages
- docs/US-0023.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0048 — Segunda regra normativa (NBR5410-002)
- knowledge/standards/NBR5410/rules/NBR5410-002.md created with full documentation
- INDEX.md: NBR5410-002 status changed to "Em implementação"
- New src/BIMBrain/Rules/Standards/NBR5410/NBR5410_002_IdentifiedPanelsRule.cs
- Reuses DuplicatePanelNameRule (duplicate names) + PanelAnalyzer (empty names)
- docs/US-0024.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0049 — Catálogo de RuleSets
- New src/BIMBrain/Rules/RuleSet.cs (Name, Description, Rules list)
- New src/BIMBrain/Rules/RuleCatalog.cs (static catalog, NBR 5410 RuleSet with NBR5410-001 and NBR5410-002)
- knowledge/standards/NBR5410/README.md updated with 8-section normative structure
- No existing rules changed, no behavior changed
- docs/US-0025.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0050 — AffectedElements nas regras
- RuleResult.cs: added `List<ElementId> AffectedElements { get; set; } = new List<ElementId>()`
- All 7 rules updated to populate AffectedElements with the specific ElementIds that originated each inconsistency
- CircuitWithoutPanelRule → circuit.Id; PanelWithoutCircuitRule → panel Ids via supplemental collector; ElementsWithoutCircuitRule → unconnected element Ids per category; UnloadedLinksRule → empty list; DuplicatePanelNameRule → duplicate panel Ids; NBR5410-001 → pass-through from ElementsWithoutCircuitRule; NBR5410-002 → pass-through from DuplicatePanelNameRule + empty-name collector
- No functionality changed, no Message/Severity/Success/RuleName altered
- docs/US-0026.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0051 — RuleSelectionService
- New RuleSelectionService.cs (BIMBrain namespace): Select(UIDocument, RuleResult) + ClearSelection(UIDocument)
- Select: if AffectedElements is empty → false; otherwise uiDoc.Selection.SetElementIds → true
- ClearSelection: sets empty ElementId list
- No transaction, no model modification, no rules changed, no queries changed
- docs/US-0027.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0052 — RuleNavigationService
- New RuleNavigationService.cs (BIMBrain namespace): ZoomTo(UIDocument, RuleResult) + ZoomToFirst(UIDocument, RuleResult)
- ZoomTo: if AffectedElements is empty → false; otherwise uiDoc.ShowElements → true
- ZoomToFirst: navigates only to the first element in AffectedElements
- No transaction, no model modification, no rules changed, no queries changed
- docs/US-0028.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0053 — RuleHighlightService (skeleton)
- New RuleHighlightService.cs (BIMBrain namespace): Highlight(UIDocument, View, RuleResult) + Clear(UIDocument, View)
- Both methods throw NotImplementedException (skeleton for future OverrideGraphicSettings implementation)
- No element modified, no model data written
- docs/US-0029.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors

### TASK-0054 — RuleActionService
- New RuleActionService.cs (BIMBrain namespace): orchestrates Selection, Navigation, Highlight
- Select, Navigate, Highlight, ClearHighlight — each delegates to corresponding service
- No additional logic, no model modification
- docs/US-0030.md, CAPABILITIES.md, AGENTS.md updated
- Build: 0 errors
