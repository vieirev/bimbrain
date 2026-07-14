# NBR 5410 — Instalações Elétricas de Baixa Tensão

## Objetivo no BIMBrain

A NBR 5410 é a norma brasileira que define as condições mínimas para projetos de instalações elétricas de baixa tensão. No BIMBrain, apenas regras **verificáveis via modelo Revit** serão implementadas — ou seja, aquelas que podem ser validadas consultando parâmetros, categorias e relacionamentos do projeto BIM.

## Escopo

- Regras que dependem exclusivamente de dados presentes no modelo Revit
- Verificações dimensionais, de quantidade, de agrupamento e de nomenclatura
- Relações entre elementos (ex.: circuito → painel, tomada → ambiente)

## Fora de escopo

- Regras que exigem cálculos elétricos externos (queda de tensão, corrente de curto-circuito, seção de condutores)
- Dimensionamento que depende de fatores não modelados no Revit (tipo de isolamento, temperatura ambiente, método de instalação)
- Qualquer validação que dependa de dados fora do arquivo .rvt ou de seus links

## Estrutura da norma

| Seção | Conteúdo | Regras |
|-------|----------|--------|
| Instalação Geral | Disposições gerais, definições, condições ambientais | — |
| Tomadas | Quantidade mínima, localização, circuitos dedicados | NBR5410-001 |
| Iluminação | Pontos de luz, interruptores, comandos | — |
| Circuitos | Divisão de circuitos, proteção, identificação | — |
| Quadros | Identificação, localização, aterramento | NBR5410-002 |
| Motores | Proteção, partida, dimensionamento | — |
| Proteção | Sobrecorrente, curto-circuito, DR, DPS | — |
| Aterramento | Condutores de proteção, equipotencialização | — |
