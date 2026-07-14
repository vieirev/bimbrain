# Disciplinas do BIMBrain

O BIMBrain separa o núcleo (Core) da implementação por disciplina. O Core é
independente das disciplinas: cada disciplina pluga-se nele utilizando a mesma
infraestrutura de motores, analisadores, regras e serviços de ação.

```
Core
  ↓
Electrical
  ↓
Hydraulic
  ↓
HVAC
  ↓
Fire Protection
  ↓
Architecture
  ↓
Structure
```

## Core

Núcleo agnóstico de domínio. Contém a Foundation Engine, a orquestração de
consultas, a Rules Engine, a Knowledge Engine e a Action Engine. Não contém
lógica específica de nenhuma disciplina.

## Disciplinas

- **Electrical** — Disciplina piloto. Todas as capacidades atuais (consultas,
  analisadores, regras de consistência e normativas NBR 5410) foram construídas
  aqui como prova de conceito da plataforma.
- **Hydraulic** — Futura. Reutilizará exatamente a mesma infraestrutura do Core.
- **HVAC** — Futura. Reutilizará exatamente a mesma infraestrutura do Core.
- **Fire Protection** — Futura. Reutilizará exatamente a mesma infraestrutura do
  Core.
- **Architecture** — Futura. Reutilizará exatamente a mesma infraestrutura do
  Core.
- **Structure** — Futura. Reutilizará exatamente a mesma infraestrutura do Core.

## Regras de integração

- **Electrical é a disciplina piloto.** Ela define o padrão a ser seguido pelas
  demais.
- **Todas as demais disciplinas utilizarão exatamente a mesma infraestrutura**
  (analyzers, Rule Engine, Action Engine, Knowledge Engine).
- **Nenhuma regra elétrica deverá ficar acoplada ao Core.** As regras de
  Electrical residem em `src/BIMBrain/Rules/` e consomem os motores do Core, sem
  modificá-lo.
- **Uma disciplina nunca deve quebrar outra.** O acoplamento é sempre da
  disciplina para o Core, nunca o contrário.
