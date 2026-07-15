# TASK INDEX

A partir de TASK-0007, cada task possui um arquivo próprio dentro da pasta `.tasks/`.

Novas tasks devem ser criadas como `.tasks/TASK-NNNN.md` seguindo o formato padrão (Título, Objetivo, Status, Resultado, Commit relacionado, Observações).

Toda nova task deve obrigatoriamente pertencer a um EPIC.

## Tasks

| Task | Título | EPIC | Status |
|------|--------|------|--------|
| TASK-0001 | Inicializar estrutura do projeto | EPIC-0001 | Done |
| TASK-0002 | Criar README.md | EPIC-0001 | Done |
| TASK-0003 | Criar PROJECT_MEMORY.md | EPIC-0001 | Done |
| TASK-0004 | Criar ROADMAP.md | EPIC-0001 | Done |
| TASK-0005 | Criar AGENTS.md | EPIC-0001 | Done |
| TASK-0006 | Criar primeiro plugin Revit funcional | EPIC-0001 | Done |
| TASK-0007 a TASK-0012 | (não catalogadas individualmente — ver .tasks/) | EPIC-0001 | Done |
| TASK-0013 | 5 perguntas básicas (tomadas, interruptores, luminárias, quadros, níveis) | EPIC-0001 | Done |
| TASK-0013.1 | Correção de categoria de interruptores | EPIC-0001 | Done |
| TASK-0014 | MILESTONES M5 concluído | EPIC-0001 | Done |
| TASK-0015 | Histórico de perguntas em memória (M4) | EPIC-0001 | Done |
| TASK-0016 | MILESTONES M4 concluído | EPIC-0001 | Done |
| TASK-0017 | Tolerância a variações de escrita (Normalize) | EPIC-0001 | Done |
| TASK-0017.1 | Correção de regressão nas strings canônicas | EPIC-0001 | Done |
| TASK-0018 | 5 perguntas novas (ambientes, circuitos, área, eletroduto, potência) | EPIC-0001 | Done |
| TASK-0018.1 | Mensagens contextuais para resultados zerados | EPIC-0001 | Done |
| TASK-0019 | MILESTONES M7 (10 perguntas) | EPIC-0001 | Done |
| TASK-0020 | 4 perguntas novas (vistas, folhas, categoria líder, famílias) | EPIC-0001 | Done |
| TASK-0021 | MILESTONES M8 (14 perguntas) | EPIC-0001 | Done |
| TASK-0022 | Conexão básica com Ollama (qwen3) | EPIC-0001 | Done |
| TASK-0022.1 | Correção: parsing manual → JSON deserialization | EPIC-0001 | Done |
| TASK-0023 | Correção de documentação (README: net48, não .NET 8) | EPIC-0001 | Done |
| TASK-0024 | Tool calling: IA resolve perguntas fora dos 14 padrões | EPIC-0001 | Done |
| TASK-0024.1 | Investigação: causa raiz do timeout mascarando erro | EPIC-0001 | Done |
| TASK-0024.2 | Correção: timeout 120s + mensagens diferenciadas | EPIC-0001 | Done |
| TASK-0025 | Registro de limitação conhecida do tool calling | EPIC-0001 | Done |
| TASK-0026 | Retroactive task files (25 arquivos em .tasks/) | EPIC-0001 | Done |
| TASK-0026.1 | Correções nos arquivos retroativos | EPIC-0001 | Done |
| TASK-0027 | QuestionProcessor refactoring | EPIC-0001 | Done |
| TASK-0028 | Resumo do projeto (consulta composta) | EPIC-0001 | Done |
| TASK-0029 | ModelContext + contexto do projeto | EPIC-0001 | Done |
| TASK-0030 | UX overhaul (janela 900x700, 4 regiões) | EPIC-0001 | Done |
| TASK-0031 | UX polish (placeholder, origem, histórico limpo) | EPIC-0001 | Done |
| TASK-0032 | CAPABILITIES.md (catálogo de capacidades) | EPIC-0001 | Done |
| TASK-0033 | Diagnóstico automático | EPIC-0001 | Done |
| TASK-0034 | (pulado) | — | — |
| TASK-0035 | Diagnóstico avançado dos modelos vinculados | EPIC-0002 | Done |
| TASK-0036 | Distribuição de elementos elétricos por nível | EPIC-0002 | Done |
| TASK-0037 | Análise estrutural dos circuitos elétricos | EPIC-0002 | Done |
| TASK-0038 | Análise estrutural dos painéis elétricos | EPIC-0002 | Done |
| TASK-0039 | Verificação de integridade do modelo elétrico | EPIC-0002 | Done |
| TASK-0040 | Infraestrutura da Rules Engine | EPIC-0003 | Done |
| TASK-0041 | Primeira regra de consistência: circuitos sem painel | EPIC-0003 | Done |
| TASK-0042 | Segunda regra de consistência: painéis sem circuitos | EPIC-0003 | Done |
| TASK-0043 | Terceira regra de consistência: elementos sem circuito | EPIC-0003 | Done |
| TASK-0044 | Quarta regra de consistência: modelos vinculados descarregados | EPIC-0003 | Done |
| TASK-0045 | Quinta regra de consistência: painéis com nomes duplicados | EPIC-0003 | Done |
| TASK-0046 | Base normativa estruturada (NBR 5410) | EPIC-0003 | Done |
| TASK-0047 | Primeira regra normativa (NBR5410-001) | EPIC-0003 | Done |
| TASK-0048 | Segunda regra normativa (NBR5410-002) | EPIC-0003 | Done |
| TASK-0049 | Catálogo de RuleSets | EPIC-0003 | Done |
| TASK-0050 | AffectedElements nas regras | EPIC-0003 | Done |
| TASK-0051 | RuleSelectionService (selecionar elementos afetados) | EPIC-0003 | Done |
| TASK-0052 | RuleNavigationService (zoom/navegar até elementos) | EPIC-0003 | Done |
| TASK-0053 | RuleHighlightService (esqueleto de destaque visual) | EPIC-0003 | Done |
| TASK-0054 | RuleActionService (orquestra ações de regra) | EPIC-0003 | Done |
| TASK-0055 | RuleResultsWindow (substitui TaskDialog) | EPIC-0003 | Done |
| TASK-0056 | Knowledge Viewer (KnowledgeDocument/Repository/Window) | EPIC-0003 | Done |
| TASK-0057 | RuleInspectorWindow (inspetor de elementos afetados) | EPIC-0003 | Done |
| TASK-0058 | Recommendation Engine (RuleRecommendation/Repository) | EPIC-0003 | Done |
| TASK-0059 | Classification Engine — infraestrutura | EPIC-0005 | Done |
| TASK-0060 | Classification Engine — classificação por categoria | EPIC-0005 | Done |
| TASK-0061 | Engineering Dictionary (aliases.json) | EPIC-0005 | Done |
| TASK-0062 | Classification Engine — classificação por alias do tipo | EPIC-0005 | Done |
| TASK-0063 | Classification Engine — classificação por alias da família | EPIC-0005 | Done |
| TASK-0064 | Pipeline de integração (Copilot/Graph/Query/Impact/Explanation/Selection) | EPIC-0002 | Done |
| TASK-0065 | Ribbon por Engines (5 painéis) | EPIC-0003 | Done |
| TASK-UX-0001 | Redesenho comercial da Ribbon (4 grupos, ícones) | EPIC-0003 | Done |
| TASK-0066 | ExplainSelectionCommand (Explicar Seleção) | EPIC-0002 | Done |
| TASK-0067 | RunNBR5410Command (executa RuleSet NBR 5410) | EPIC-0003 | Done |
| TASK-0068 | RuleResultsWindow (consolidação da UI de regras) | EPIC-0003 | Done |
| TASK-0069 | Knowledge Viewer (janela de conhecimento) | EPIC-0003 | Done |
| TASK-0070 | RuleInspectorWindow (inspetor de elementos afetados) | EPIC-0003 | Done |
| TASK-0071 | Recommendation Engine (recomendações por inconsistência) | EPIC-0003 | Done |
| TASK-0072 | Classification Engine — infraestrutura (canônica) | EPIC-0005 | Done |
| TASK-0073 | Classification Engine — Classify(Element) por BuiltInCategory | EPIC-0005 | Done |
| TASK-0074 | Engineering Dictionary (aliases.json) — camada de consulta | EPIC-0005 | Done |
| TASK-0075 | Classification Engine — classificação por alias do tipo | EPIC-0005 | Done |
| TASK-0076 | Classification Engine — classificação por alias da família | EPIC-0005 | Done |
| TASK-0077 | ElectricalCircuitAnalyzer usa Classification Engine | EPIC-0002 | Done |
| TASK-0078 | ModelIntegrityAnalyzer consome RuleResult estruturado (sem Regex) | EPIC-0003 | Done |
| TASK-0079 | ElementLevelAnalyzer baseado em classificação | EPIC-0005 | Done |
| TASK-0080 | QuestionProcessor interpreta via Engineering Dictionary | EPIC-0005 | Done |
| TASK-0081 | Infraestrutura de Query Handlers | EPIC-0006 | Done |
| TASK-HOTFIX-001 | ClassificationRepository robusto a BaseDirectory nulo | EPIC-0005 | Done |
| TASK-HOTFIX-002 | Carga de System.Text.Json no Revit (AssemblyResolve) | EPIC-0006 | Done |
