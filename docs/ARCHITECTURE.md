# Arquitetura do BIMBrain

Este documento descreve a arquitetura atual do BIMBrain e a responsabilidade de
cada componente existente. Nenhuma alteração de código é descrita aqui — apenas
a estrutura vigente.

## Fluxo principal

```
UI (WPF)
   ↓
QuestionProcessor
   ↓
Analyzers / Rules Engine
   ↓
Knowledge
   ↓
Revit API
```

1. A **UI (WPF)** recebe a pergunta do usuário e dispara a consulta.
2. O **QuestionProcessor** normaliza o texto, decide se a pergunta é resolvida
   diretamente ou via IA (Ollama), e formata a resposta.
3. Consultas diretas utilizam os **Analyzers** ou a leitura do **ModelContext**.
4. Análises de conformidade utilizam a **Rules Engine** (RuleRunner + catálogo).
5. Regras normativas apoiam-se na **Knowledge** documentada.
6. Toda leitura de dados passa pela **Revit API**.

## Principais componentes

### Orquestração

- **QuestionProcessor** — Ponto de entrada das consultas. Normaliza o texto,
  roteia para analisadores ou para a IA (Ollama), executa tool calling e formata
  a resposta estruturada.

### Contexto do modelo

- **ModelContext** — Descobre o documento ativo e os modelos vinculados
  (`RevitLinkInstance`), expondo uma coleção somente leitura de informações por
  documento (nome, tipo, status, níveis, rooms, elementos, caminho).

### Analisadores (Model Engine)

- **ElementLevelAnalyzer** — Agrupa elementos elétricos por nível, retornando a
  distribuição por pavimento.
- **ElectricalCircuitAnalyzer** — Coleta todos os `ElectricalSystem` e produz a
  contagem de tomadas, luminárias e interruptores por circuito, além do conjunto
  de elementos conectados.
- **PanelAnalyzer** — Identifica painéis elétricos (`ElectricalEquipment`) e
  associa seus circuitos e contagens por categoria.
- **ModelIntegrityAnalyzer** — Verifica a integridade do modelo elétrico
  (elementos e painéis sem circuito), reutilizando os analisadores acima.

### Rules Engine

- **RuleRunner** — Executa todas as regras de engenharia registradas e aplaina
  os resultados.
- **RuleCatalog** — Catálogo estático de RuleSets disponíveis (ex.: NBR 5410).
- **RuleSet** — Agrupamento lógico de regras (nome, descrição, lista de regras).
- **EngineeringRule** — Classe base abstrata de toda regra.
- **RuleResult** — Resultado de uma regra, com severidade, mensagem e elementos
  afetados.
- **RuleSeverity** — Enumeração de severidade do resultado.

### Action Engine

- **RuleActionService** — Orquestra as ações sobre um `RuleResult`
  (selecionar, navegar, destacar).
- **RuleSelectionService** — Seleciona os elementos afetados no Revit.
- **RuleNavigationService** — Navega (zoom) até os elementos afetados.
- **RuleHighlightService** — API de destaque visual (esqueleto; implementação
  futura com OverrideGraphicSettings).

### Knowledge

- Documentada em `knowledge/standards/` (separada da implementação em
  `src/BIMBrain/Rules/`), com índice de regras normativas por norma (NBR 5410).

### UI

- Janela WPF code-only (sem XAML) em 4 regiões: informações do projeto,
  consulta, resposta + histórico e barra de status. Apoia-se em painéis
  auxiliares (`ResponsePanel`, `HistoryPanel`, `StatusBar`) para evitar
  crescimento do `MainWindow`.

## Princípios arquiteturais aplicados

- Núcleo independente das disciplinas.
- Conhecimento separado da implementação.
- Componentes reutilizáveis entre consultas e regras.
- Sem dependências desnecessárias.
- Sem modificação do modelo sem autorização.
