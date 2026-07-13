# BIMBrain

Plataforma de Inteligência Artificial para escritórios BIM.

## MVP

O MVP do BIMBrain é um plugin para Autodesk Revit capaz de compreender um projeto arquitetônico e responder perguntas em linguagem natural sobre o modelo.

### Funcionalidades do MVP

- Conectar-se a um projeto Revit aberto
- Extrair e interpretar o modelo BIM (famílias, parâmetros, tipos, instâncias, materiais)
- Responder perguntas em português sobre o projeto
- Exibir respostas diretamente na interface do Revit via WPF
- Suporte a perguntas contextuais baseadas no modelo carregado

## Stack

| Camada           | Tecnologia                               |
|------------------|------------------------------------------|
| Linguagem        | C# 12 / .NET 8                           |
| Plataforma       | Autodesk Revit (Revit API)               |
| Interface        | WPF (Windows Presentation Foundation)    |
| IA               | OpenAI API / Azure OpenAI Service        |
| Testes           | xUnit + NSubstitute                      |
| Build            | MSBuild / dotnet CLI                     |

## Pré-requisitos

- Windows 10 ou superior
- Autodesk Revit 2024 ou 2025
- .NET 8 SDK
- Acesso à API OpenAI (chave de API)

## Instalação

1. Clone o repositório:

   ```bash
   git clone https://github.com/vieirev/bimbrain.git
   ```

2. Compile o projeto:

   ```bash
   cd bimbrain
   dotnet build src/BIMBrain.sln
   ```

3. Copie a DLL gerada para a pasta de Add-ins do Revit:

   ```
   %APPDATA%\Autodesk\Revit\Addins\<versão>\
   ```

4. Configure a chave da API OpenAI nas configurações do plugin.

5. Inicie o Revit e carregue um projeto.

## Uso

1. No Revit, acesse a aba **BIMBrain** na ribbon.
2. Clique em **Conectar** para vincular ao modelo ativo.
3. Digite sua pergunta em linguagem natural no painel lateral.
4. O plugin processa a consulta via IA e exibe a resposta.

### Exemplos de perguntas

- "Quantas portas existem no pavimento térreo?"
- "Qual a área total de paredes externas?"
- "Liste todos os ambientes com área superior a 50 m²"
- "Quais famílias de janelas estão sendo usadas?"

## Estrutura do repositório

```
bimbrain
├── src/                  # Código fonte do plugin
│   ├── BIMBrain.Core/    # Lógica principal e domínio
│   ├── BIMBrain.Infra/   # Integrações externas (IA, Revit API)
│   └── BIMBrain.UI/      # Interface WPF
├── tests/                # Testes automatizados
│   └── BIMBrain.Tests/   # Testes unitários e de integração
├── docs/                 # Documentação técnica
├── .tasks/               # Tarefas do projeto
├── AGENTS.md             # Configuração de agentes
├── ICEBOX.md             # Ideias futuras
├── PROJECT_MEMORY.md     # Memória do projeto
├── ROADMAP.md            # Roadmap
└── TASK.md               # Tarefa atual
```

## Como contribuir

1. Faça um fork do repositório
2. Crie uma branch descritiva (`git checkout -b feature/nome-da-feature`)
3. Commit suas alterações com mensagens claras (`git commit -m "descrição concisa do que foi feito"`)
4. Mantenha o padrão de código existente (StyleCop, análise estática)
5. Execute os testes antes de abrir o PR (`dotnet test`)
6. Faça push (`git push origin feature/nome-da-feature`)
7. Abra um Pull Request descrevendo a mudança

## Roadmap resumido

- **MVP** — Plugin Revit com perguntas em linguagem natural
- **Expansão** — Suporte a PDF, DWG e IFC
- **Plataforma** — Dashboard, Analytics e QA

## Licença

Este projeto está licenciado sob a licença MIT — veja o arquivo [LICENSE](LICENSE) para detalhes.
