# BIMBrain — Memória de Sessão (Memory)

Arquivo de memória mantido entre sessões. **Sempre atualizar ao finalizar uma TASK.**
Referências detalhadas: `docs/CAPABILITIES.md` (estado atual) e `docs/ARCHITECTURE.md` (estrutura).

Última atualização: 2026-07-14 (TASK-0081 + hotfixes de deploy concluídos; catálogo de tasks 0051-0081 atualizado)

---

## O que é

Plugin Revit 2025 (C# / .NET Framework 4.8, `net48`) que transforma o BIMBrain de
MVP em plataforma de engenharia assistida por IA. Add-in em
`%APPDATA%\Autodesk\Revit\Addins\2025\` (DLL + `BIMBrain.addin` + `knowledge/`).

## Engines e status

- **Foundation Engine** ✅ — CopilotContext, CopilotContextBuilder, CopilotOrchestrator, CopilotExecutor, SelectionContext(Service).
- **Model Engine** ✅ — ProjectGraph, ProjectGraphQuery, ProjectImpactAnalyzer, ElementExplanation(Service), ModelContext, Analyzers (ElectricalCircuit, Panel, ElementLevel, ElementIntegrity), QuestionProcessor.
- **Rule Engine** ✅ — EngineeringRule, RuleResult, RuleSeverity, RuleRunner, RuleSet, RuleCatalog, regras de consistência + NBR 5410. UI: RuleResultsWindow, RuleInspectorWindow.
- **Knowledge Engine** ✅ — KnowledgeDocument, KnowledgeRepository, RecommendationRepository, RuleRecommendation. UI: KnowledgeWindow.
- **Classification Engine** ✅ — 3 camadas (categoria → tipo → família) via Engineering Dictionary (`aliases.json`); usado por `ElectricalCircuitAnalyzer`, `ElementLevelAnalyzer`, `ModelIntegrityAnalyzer` e `QuestionProcessor.DetectClassification`.
- **AI Engine** 🟡 — Explicar Seleção implementado (100% Graph, sem IA); tool calling Ollama existente.
- Automation / Coordination / Document — ❌ não implementados (botões placeholder).

## Ribbon (aba BIMBrain, 4 grupos)

- **BIMBrain** (grande) → `Command` (janela de consultas).
- **Copilot** (stacked): Explicar (`ExplainSelectionCommand`), Diagnóstico, IA (placeholders).
- **Engenharia** (stacked): NBR 5410 (`RunNBR5410Command`), Integridade, Coordenação (placeholders).
- **Ferramentas** (stacked): Automação, Configurações, Sobre (placeholders).
- Ícones em `Resources/Icons16|Icons32/*.png` (embutidos como EmbeddedResource).
- `PlaceholderCommand` = única classe reutilizável para botões não implementados.

## Pipelines principais

- Consulta: UI → CopilotExecutor → CopilotContextBuilder → CopilotOrchestrator → QuestionProcessor (sem branch removido).
- Explicar: ExplainSelectionCommand → ProjectGraphBuilder → CopilotContextBuilder → SelectionContextService → ElementExplanationService → TaskDialog.
- Regra: RunNBR5410Command → RuleCatalog → RuleRunner → RuleResultsWindow → (Inspector / Knowledge / Selecionar / Navegar via RuleActionService).

## Mapa de arquivos chave

- `src/BIMBrain/App.cs` — Ribbon (4 grupos, `MakeButton`/`MakePlaceholder`).
- `src/BIMBrain/Command.cs` — abre MainWindow.
- `src/BIMBrain/PlaceholderCommand.cs` — botões não implementados.
- `src/BIMBrain/ExplainSelectionCommand.cs` — Explicar Seleção.
- `src/BIMBrain/RunNBR5410Command.cs` — executa RuleSet NBR 5410.
- `src/BIMBrain/UI/MainWindow.xaml.cs` + ResponsePanel/HistoryPanel/StatusBar.
- `src/BIMBrain/UI/RuleResultsWindow.cs`, `RuleInspectorWindow.cs`, `KnowledgeWindow.cs`.
- `src/BIMBrain/Graph/*` — ProjectGraph, Query, Impact, Explanation.
- `src/BIMBrain/Rules/*` — Engine de regras + Standards/NBR5410.
- `src/BIMBrain/Knowledge/*` — KnowledgeDocument, KnowledgeRepository, RuleRecommendation, RecommendationRepository.
- `src/BIMBrain/Classification/*` — ElementClassification(Type), ElementClassifier, ClassificationRepository.
- `knowledge/standards/NBR5410/rules/*.md` — docs das regras (copiados p/ Addins/2025/knowledge).

## Build / Deploy

- Build: `"C:\Program Files\dotnet\dotnet.exe" build src/BIMBrain/BIMBrain.csproj -c Release`
- Copiar `src/BIMBrain/bin/Release/net48/*` → `%APPDATA%\Autodesk\Revit\Addins\2025\`.
- **O Revit trava o BIMBrain.dll**: fechar o Revit antes de copiar.
- Opcional: copiar `knowledge/` para `Addins/2025/knowledge` (para o botão Conhecimento funcionar).

## Convenções (AGENTS.md)

- Toda TASK precisa Engine, Disciplina, EPIC, User Story.
- Toda funcionalidade em um Engine; não alterar arquitetura sem autorização.
- Não commitar; aguardar revisão humana.
- Documentar antes de código quando necessário; manter alterações mínimas.

## TASKs concluídas recentes

- 0065 Ribbon por Engines (5 painéis → depois 4 grupos em UX-0001).
- UX-0001 Redesenho comercial da Ribbon (4 grupos, ícones, tooltips).
- 0066 ExplainSelectionCommand (Explicar, 100% Graph).
- 0067 RunNBR5410Command (executa RuleSet NBR 5410).
- 0068 RuleResultsWindow (substitui TaskDialog).
- 0069 Knowledge Viewer (KnowledgeDocument, KnowledgeRepository, KnowledgeWindow).
- 0070 RuleInspectorWindow (inspector de elementos afetados).
- 0071 Recommendation Engine (RuleRecommendation, RecommendationRepository).
- 0072 Classification Engine — infraestrutura.
- 0073 Classification Engine — Classify(Element) por BuiltInCategory (Status: Category Classification).
- 0074 Engineering Dictionary — aliases.json + ClassificationRepository.GetAliases/GetAllAliases/HasAlias/FindByAlias (somente leitura, ainda não usado p/ classificar).
- 0075 Classification Engine — 2ª camada: alias do tipo (Symbol.Name via Engineering Dictionary), Confidence 80 (Status: Category + Type Alias Classification).
- 0076 Classification Engine — 3ª camada: alias da família (Family.Name via Engineering Dictionary), Confidence 70 (Status: Category + Type + Family Classification).
- 0077 ElectricalCircuitAnalyzer usa Classification Engine p/ identificar tomada/interruptor/luminária (sem BuiltInCategory p/ decidir tipo).
- 0078 RuleResult ganha Data (Dictionary<string,object>); ElementsWithoutCircuitRule popula Tipo/Count; ModelIntegrityAnalyzer consome Data (Regex removido); NBR5410-001 filtra por Data.
- 0079 ElementLevelAnalyzer.Analyze(ElementClassificationType) usa ElementClassifier p/ decidir Outlet/Switch/LightingFixture/Panel; QuestionProcessor mapeia palavras p/ tipo.
- 0080 QuestionProcessor.DetectClassification usa ClassificationRepository.GetAllAliases (sem palavras fixas); DetectDistributionCategory removido.
- 0081 Infraestrutura Query Handlers: IQueryHandler, QueryContext, QueryResult, QueryRouter em src/BIMBrain/Queries/ (sem handlers concretos).
- HOTFIX pós-0081: ClassificationRepository.FindKnowledgeRoot/LoadAliases endurecidos — BaseDirectory pode ser null no Revit e Path.Combine lançava TypeInitializationException; agora busca a partir de Assembly.Location (Addins/2025, onde knowledge/ vive) e nunca lança.
- HOTFIX crítico (deploy Revit): System.Text.Json v10.0.0.9 (e System.* empacotados) falham ao carregar no Revit 2025 porque o Revit já carregou versões próprias no contexto padrão → FileLoadException → TypeInitializationException em ClassificationRepository. Fixo em App.cs com AppDomain.AssemblyResolve que redireciona essos assemblies para os já carregados pelo Revit (fallback LoadFrom da pasta do addin). SEMPRE fechar o Revit antes de copiar a DLL (trava BIMBrain.dll).

## Pendências / próximos passos prováveis (espelho do ROADMAP)

- Conectar Classification Engine ao Project Graph (enriquecer nós com o tipo classificado).
- Implementar botões placeholder ainda pendentes: Diagnóstico (já há consulta, falta botão), IA (conversacional), Integridade, Coordenação, Automação, Configurações, Sobre.
- RuleHighlightService: implementar destaque visual real (OverrideGraphicSettings) — hoje lança NotImplementedException.
- Regras NBR 5410 003–005 (ROADMAP: Planejado).
- Query Handlers concretos (TASK-0081 criou só a infraestrutura) para substituir os `if` do QuestionProcessor.
- Exportar resultados em RuleResultsWindow.
- Demais disciplinas (Hydraulic, HVAC, Fire Protection, Architecture, Structure).
- EPIC-0004 (AI Engine avançado) e EPIC-0006 (Production) ainda Planejados.
