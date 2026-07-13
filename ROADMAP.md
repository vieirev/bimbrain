# Roadmap

## Objetivo

Definir a sequência de desenvolvimento do MVP do BIMBrain. Cada Sprint entrega uma funcionalidade completa e funcional.

## Sprint 0 — Fundação

**Objetivo:** Configurar o ambiente de desenvolvimento e a estrutura do projeto.

**Entregas:**
- Repositório configurado
- Solução .NET 8 / C# 12 criada
- Projeto de testes configurado (xUnit + NSubstitute)
- Documentação inicial (README, PROJECT_MEMORY, ROADMAP)

**Critério de conclusão:** Projeto compila e `dotnet test` passa.

## Sprint 1 — Plugin Base

**Objetivo:** Criar o plugin Revit mínimo com interface WPF.

**Entregas:**
- Plugin Revit registrado no Add-in Manager
- Ribbon com aba BIMBrain
- Botão na ribbon
- Janela WPF funcional

**Critério de conclusão:** Plugin carrega no Revit e janela WPF abre ao clicar no botão.

## Sprint 2 — Leitura do Modelo

**Objetivo:** Ler elementos do modelo Revit e estruturar os dados para consulta.

**Entregas:**
- Ler modelo Revit aberto
- Listar elementos do projeto
- Estrutura interna para consultas

Sem IA.

**Critério de conclusão:** Plugin exibe lista de elementos do modelo carregado.

## Sprint 3 — Perguntas em Linguagem Natural

**Objetivo:** Integrar LLM para responder perguntas sobre o modelo.

**Entregas:**
- Conectar LLM Provider (OpenAI)
- Perguntas em linguagem natural
- Respostas utilizando o modelo carregado

**Critério de conclusão:** Usuário faz uma pergunta e recebe resposta baseada no modelo.
