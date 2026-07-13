# Projeto

BIMBrain — Plataforma de Inteligência Artificial para escritórios BIM.

# Objetivo do MVP

Entregar um plugin para Autodesk Revit capaz de compreender projetos de engenharia elétrica e responder perguntas em linguagem natural sobre o modelo aberto.

# Escopo Atual

- Plugin Revit conectado ao projeto ativo
- Leitura de elementos do modelo
- Perguntas e respostas em linguagem natural via LLM
- Exibição de respostas dentro do Revit (WPF)

# Fora do Escopo

- PDF
- DWG
- IFC
- Dashboard
- Analytics
- QA (quality assurance automatizado)
- Multiusuário
- Licenciamento

# Stack Tecnológica

| Camada           | Tecnologia                            |
|------------------|---------------------------------------|
| Linguagem        | C# 12 / .NET 8                        |
| Plataforma       | Autodesk Revit (Revit API)            |
| Interface        | WPF (Windows Presentation Foundation) |
| IA               | LLM Provider (OpenAI)                 |
| Testes           | xUnit + NSubstitute                   |
| Build            | MSBuild / dotnet CLI                  |

# Arquitetura

Plugin Revit
↓
Camada de domínio
↓
LLM Provider

# Princípios

- Simplicidade primeiro
- Uma tarefa por vez
- Documentação antes do código
- Não implementar funcionalidades fora da Sprint
- Foco em validar o MVP antes de expandir

# Decisões Arquiteturais

001. O BIMBrain será desenvolvido como um MVP.
002. O foco inicial será projetos elétricos em Revit.
003. A primeira funcionalidade será responder perguntas sobre um modelo aberto.
004. Toda alteração relevante deve ser documentada.

# Fluxo de Trabalho

Issue
↓
TASK
↓
Implementação
↓
Revisão
↓
Commit
