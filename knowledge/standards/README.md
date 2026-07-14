# Base Normativa BIMBrain

## Objetivo

Organizar o conhecimento técnico-normativo que alimenta a Rules Engine. Cada norma técnica relevante para o BIMBrain possui sua própria pasta dentro de `knowledge/standards/`, contendo documentação estruturada sobre quais regras verificáveis podem ser implementadas.

## Como adicionar novas normas

1. Criar uma nova pasta em `knowledge/standards/<NORMA>/`
2. Adicionar um `README.md` explicando o escopo da norma dentro do BIMBrain
3. Criar ou atualizar `INDEX.md` com a lista de regras candidatas (ID, Nome, Status, Task relacionada)
4. Sempre que uma regra for implementada via Rules Engine, atualizar seu status no índice

## Diferença entre conhecimento e implementação

| Conceito | Onde vive | O que contém |
|---|---|---|
| Conhecimento normativo | `knowledge/standards/<NORMA>/INDEX.md` | ID, descrição, justificativa técnica, referência da norma |
| Implementação | `src/BIMBrain/Rules/` | Código C# herdando de `EngineeringRule`, executado via `RuleRunner` |

O conhecimento documenta *o que* a norma exige. A implementação codifica *como* verificar essa exigência no modelo Revit. Um pode existir sem o outro (regra planejada vs. regra implementada), mas toda implementação deve ter sua contraparte documentada no índice.
