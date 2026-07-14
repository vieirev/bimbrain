# Roadmap

## Visão Geral

O BIMBrain é uma plataforma de engenharia assistida por IA organizada em
**Engines** (motores) e **Disciplinas**. O roadmap é apresentado por capacidade,
mantendo o vínculo com os EPICs existentes. Cada item possui um dos seguintes
status:

- ✅ Concluído
- 🔄 Em andamento
- 📋 Planejado

Detalhes de arquitetura, motores e disciplinas estão em
[docs/ARCHITECTURE.md](docs/ARCHITECTURE.md), [docs/ENGINES.md](docs/ENGINES.md)
e [docs/DISCIPLINES.md](docs/DISCIPLINES.md).

---

## Foundation Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Estrutura do projeto e plugin Revit | ✅ Concluído | EPIC-0001 |
| Orquestração de consultas (QuestionProcessor) | ✅ Concluído | EPIC-0001 |
| UX WPF (4 regiões, histórico, status) | ✅ Concluído | EPIC-0001 |

## Model Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Leitura do modelo ativo (elementos, níveis, rooms) | ✅ Concluído | EPIC-0001 |
| Contexto do projeto e modelos vinculados | ✅ Concluído | EPIC-0001 / EPIC-0002 |
| Distribuição de elementos por nível | ✅ Concluído | EPIC-0002 |
| Análise estrutural de circuitos | ✅ Concluído | EPIC-0002 |
| Análise estrutural de painéis | ✅ Concluído | EPIC-0002 |
| Verificação de integridade do modelo | ✅ Concluído | EPIC-0002 |
| Relacionamentos avançados e performance | 🔄 Em andamento | EPIC-0002 |

## Rule Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Infraestrutura (EngineeringRule, RuleResult, RuleRunner) | ✅ Concluído | EPIC-0003 |
| Regras de consistência (5 regras) | ✅ Concluído | EPIC-0003 |
| Base normativa NBR 5410 (estrutura) | ✅ Concluído | EPIC-0003 |
| Regras normativas NBR5410-001 / NBR5410-002 | ✅ Concluído | EPIC-0003 |
| Catálogo de RuleSets | ✅ Concluído | EPIC-0003 |
| Demais regras NBR 5410 (003–005) | 📋 Planejado | EPIC-0003 |

## Knowledge Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Estrutura de conhecimento (knowledge/standards) | ✅ Concluído | EPIC-0003 |
| Índice de regras normativas (NBR 5410) | ✅ Concluído | EPIC-0003 |
| Expansão de normas e boas práticas | 📋 Planejado | EPIC-0005 |

## Action Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Seleção de elementos (RuleSelectionService) | ✅ Concluído | EPIC-0003 |
| Navegação até elementos (RuleNavigationService) | ✅ Concluído | EPIC-0003 |
| Orquestração de ações (RuleActionService) | ✅ Concluído | EPIC-0003 |
| Destaque visual (RuleHighlightService) | 📋 Planejado | EPIC-0003 |

## AI Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Normalização textual e consultas diretas | ✅ Concluído | EPIC-0001 |
| Integração Ollama (qwen3) e tool calling | ✅ Concluído | EPIC-0001 |
| Múltiplos LLMs e raciocínio avançado | 📋 Planejado | EPIC-0004 |
| AI Copilot (explicar, justificar, sugerir) | 📋 Planejado | EPIC-0004 |

## Document Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Interpretação de PDF (símbolos, textos, cotas) | 📋 Planejado | — |
| Leitura de DWG (layers, blocos) | 📋 Planejado | — |

## Coordination Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Detecção de clashes | 📋 Planejado | — |
| Sugestão e correção de conflitos | 📋 Planejado | — |

## Automation Engine

| Capacidade | Status | EPIC |
|------------|--------|------|
| Criação automática de circuitos/infraestrutura | 📋 Planejado | — |
| Autotag, autopreenchimento, autocorreções | 📋 Planejado | — |

## Disciplines

| Disciplina | Status | EPIC |
|------------|--------|------|
| Core (independente de disciplina) | ✅ Concluído | EPIC-0001 / EPIC-0003 |
| Electrical (piloto) | ✅ Concluído | EPIC-0001 / EPIC-0002 / EPIC-0003 |
| Hydraulic | 📋 Planejado | — |
| HVAC | 📋 Planejado | — |
| Fire Protection | 📋 Planejado | — |
| Architecture | 📋 Planejado | — |
| Structure | 📋 Planejado | — |

---

## Vínculo com EPICs

| EPIC | Título | Status |
|------|--------|--------|
| EPIC-0001 | Foundation | ✅ Concluído |
| EPIC-0002 | BIM Engine | 🔄 Em andamento |
| EPIC-0003 | Rules Engine | 🔄 Em andamento |
| EPIC-0004 | AI Engine | 📋 Planejado |
| EPIC-0005 | Engineering Knowledge | 📋 Planejado |
| EPIC-0006 | Production | 📋 Planejado |
