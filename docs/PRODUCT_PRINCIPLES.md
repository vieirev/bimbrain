# Princípios do Produto

Estes são os princípios oficiais do BIMBrain. Eles guiam toda decisão de
produto, arquitetura e implementação.

- **O usuário sempre mantém o controle.** O engenheiro decide quando e como
  agir. A ferramenta nunca toma decisões por ele.

- **Nunca modificar o modelo sem autorização explícita.** Nenhuma alteração no
  modelo Revit é realizada sem consentimento direto do usuário.

- **Toda sugestão deve ser explicável.** Cada resposta ou recomendação deve
  vir acompanhada da fundamentação que a originou.

- **Toda decisão deve ser rastreável.** É possível identificar a origem de cada
  análise, regra e dado utilizado.

- **Engenharia antes da IA.** A base técnica (regras, normas, cálculos) vem
  antes e independente da camada de inteligência artificial.

- **Conhecimento separado da implementação.** A base de conhecimento (normas,
  boas práticas) é documentada à parte do código que a implementa.

- **Componentes reutilizáveis.** Analisadores, regras e serviços são construídos
  para serem reaproveitados entre disciplinas e funcionalidades.

- **Uma disciplina nunca deve quebrar outra.** O desenvolvimento de uma
  disciplina não pode comprometer o funcionamento das demais.

- **O núcleo deve permanecer independente das disciplinas.** O Core é agnóstico
  quanto à disciplina; cada disciplina pluga-se nele.

- **Todo comportamento deve ser previsível.** O resultado de uma consulta ou
  regra deve ser determinístico e reproduzível para as mesmas entradas.
