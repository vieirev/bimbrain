# TASK-HOTFIX-002

## Título

Carga de System.Text.Json no Revit (AssemblyResolve)

## Engine

Foundation Engine

## Disciplina

Core

## EPIC

EPIC-0006

## Objetivo

Resolver FileLoadException de System.Text.Json/System.* no Revit 2025 (versões próprias do Revit em conflito).

## Status

Done

## Resultado

- `App.cs`: `AppDomain.AssemblyResolve` redireciona os assemblies `System.*` empacotados para as versões já carregadas pelo Revit, com fallback `Assembly.Load(File.ReadAllBytes(path))`.
- Tratado em 05d3f2f.

## Commit

05d3f2f (feat: Classification Engine, Query Handlers e correção de carga de assemblies no Revit)

## Observações

Crítico para deploy; exigir fechar o Revit antes de copiar a DLL.
