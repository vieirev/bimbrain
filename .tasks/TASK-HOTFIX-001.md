# TASK-HOTFIX-001

## Título

Classificação do ClassificationRepository robusta a BaseDirectory nulo

## Engine

Knowledge Engine

## Disciplina

Core

## EPIC

EPIC-0005

## Objetivo

Evitar TypeInitializationException no Revit quando AppDomain.BaseDirectory é nulo.

## Status

Done

## Resultado

- `ClassificationRepository.FindKnowledgeRoot`/`LoadAliases` buscam a partir de `Assembly.Location` e never lançam.
- Tratado em 05d3f2f.

## Commit

05d3f2f (feat: Classification Engine, Query Handlers e correção de carga de assemblies no Revit)

## Observações

Hotfix pós-0081; não altera comportamento das consultas.
