# Engines do BIMBrain

O BIMBrain é organizado em motores (Engines). Cada Engine representa uma
capacidade fundamental da plataforma. Esta seção documenta oficialmente os
motores existentes e os planejados.

> Legenda: ✅ Existente · 📋 Planejado

## Foundation Engine ✅

Responsabilidade: estrutura do projeto, plugin Revit, orquestração de consultas,
UX e infraestrutura básica. É a base sobre a qual todos os demais motores
operam.

- Componentes: plugin Revit, WPF UI, QuestionProcessor.
- Status: **Existente** (EPIC-0001).

## Model Engine ✅

Responsabilidade: leitura e compreensão do modelo BIM — elementos, níveis,
rooms, famílias, modelos vinculados e relacionamentos estruturais.

- Componentes: ModelContext, ElementLevelAnalyzer, ElectricalCircuitAnalyzer,
  PanelAnalyzer, ModelIntegrityAnalyzer.
- Status: **Existente** (EPIC-0001 / EPIC-0002).

## Rule Engine ✅

Responsabilidade: execução de regras de engenharia e normas técnicas, produzindo
resultados rastreáveis e elementos afetados.

- Componentes: EngineeringRule, RuleResult, RuleSeverity, RuleRunner, RuleSet,
  RuleCatalog, regras de consistência e normativas (NBR 5410).
- Status: **Existente** (EPIC-0003).

## Knowledge Engine ✅

Responsabilidade: base de conhecimento técnico documentada (normas, boas
práticas), separada da implementação.

- Componentes: `knowledge/standards/` (NBR 5410), índice de regras normativas.
- Status: **Existente** (EPIC-0003, fase inicial).

## Action Engine ✅

Responsabilidade: ações sobre os resultados das regras no Revit — seleção,
navegação e destaque visual dos elementos afetados.

- Componentes: RuleActionService, RuleSelectionService, RuleNavigationService,
  RuleHighlightService (esqueleto).
- Status: **Existente** (EPIC-0003).

## AI Engine ✅

Responsabilidade: integração com modelos de linguagem (LLM) para reconhecimento
de perguntas, tool calling e explicações em linguagem natural.

- Componentes: integração Ollama (qwen3), tool calling com 14 schemas,
  normalização textual.
- Status: **Existente** (EPIC-0001); evolução planejada em EPIC-0004.

## Document Engine 📋

Responsabilidade: interpretação de documentos externos (PDF, DWG), reconhecimento
de símbolos, textos e cotas.

- Status: **Planejado** (ver FUTURE.md).

## Coordination Engine 📋

Responsabilidade: detecção de clashes (conflitos), sugestão de solução e correção
automática mediante aprovação.

- Status: **Planejado** (ver FUTURE.md).

## Automation Engine 📋

Responsabilidade: criação e autocorreção automática de elementos do modelo
(circuitos, infraestrutura, autotag, autopreenchimento) mediante autorização.

- Status: **Planejado** (ver FUTURE.md).

## Resumo

| Engine | Status |
|--------|--------|
| Foundation Engine | ✅ Existente |
| Model Engine | ✅ Existente |
| Rule Engine | ✅ Existente |
| Knowledge Engine | ✅ Existente |
| Action Engine | ✅ Existente |
| AI Engine | ✅ Existente |
| Document Engine | 📋 Planejado |
| Coordination Engine | 📋 Planejado |
| Automation Engine | 📋 Planejado |
