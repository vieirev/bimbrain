# TASK-UX-0001

## Título

Redesenho comercial da Ribbon (4 grupos, ícones)

## Engine

Foundation Engine

## Disciplina

Core

## EPIC

EPIC-0003

## Objetivo

Redesenhar a Ribbon em 4 grupos comerciais (BIMBrain, Copilot, Engenharia, Ferramentas) com ícones e tooltips.

## Status

Done

## Resultado

- `App.cs`: 4 grupos; `MakeButton`/`MakePlaceholder`; `LoadIcon` de `Resources/Icons16|Icons32` (EmbeddedResource).
- `PlaceholderCommand` reutilizado para botões não implementados.
- docs/US-0042.md (US-UX-0001) criado; CAPABILITIES.md, AGENTS.md, TASK.md atualizados.

## Commit

d7f9cc0 (feat(platform): complete BIMBrain platform foundation)

## Observações

Ícones embutidos como recurso; sem alteração de funcionalidade.
