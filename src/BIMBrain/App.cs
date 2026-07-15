using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BIMBrain
{
    public class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication app)
        {
            RegisterAssemblyRedirects();

            try
            {
                app.CreateRibbonTab("BIMBrain");
            }
            catch
            {
            }

            BuildMainPanel(app);
            BuildCopilotPanel(app);
            BuildEngineeringPanel(app);
            BuildToolsPanel(app);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Result.Succeeded;
        }

        private static readonly HashSet<string> RedirectedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "System.Text.Json",
            "System.Memory",
            "System.Buffers",
            "System.Numerics.Vectors",
            "System.Runtime.CompilerServices.Unsafe",
            "System.Text.Encodings.Web",
            "System.IO.Pipelines",
            "System.Threading.Tasks.Extensions",
            "Microsoft.Bcl.AsyncInterfaces"
        };

        private static void RegisterAssemblyRedirects()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                try
                {
                    var requested = new AssemblyName(args.Name);
                    if (!RedirectedAssemblies.Contains(requested.Name))
                    {
                        return null;
                    }

                    var exact = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().FullName == requested.FullName);
                    if (exact != null)
                    {
                        return exact;
                    }

                    var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var path = Path.Combine(dir ?? string.Empty, requested.Name + ".dll");
                    if (File.Exists(path))
                    {
                        return Assembly.Load(File.ReadAllBytes(path));
                    }
                }
                catch
                {
                }

                return null;
            };
        }

        private static void BuildMainPanel(UIControlledApplication app)
        {
            var panel = app.CreateRibbonPanel("BIMBrain", "BIMBrain");

            var main = new PushButtonData(
                "BIMBrain.Open",
                "BIMBrain",
                typeof(Command).Assembly.Location,
                typeof(Command).FullName);

            main.Image = LoadIcon("Icons16.bimbrain.png");
            main.LargeImage = LoadIcon("Icons32.bimbrain.png");
            main.ToolTip = "Abre o Copilot BIMBrain.";
            main.LongDescription =
                "Abre a janela principal do BIMBrain, permitindo consultar o modelo, " +
                "executar regras e utilizar o copiloto.";

            panel.AddItem(main);
        }

        private static void BuildCopilotPanel(UIControlledApplication app)
        {
            var panel = app.CreateRibbonPanel("BIMBrain", "Copilot");

            var explicar = MakeButton(
                "BIMBrain.Explain",
                "Explicar",
                typeof(ExplainSelectionCommand),
                "explicar",
                "Explica o elemento selecionado.",
                "Apresenta o contexto, as dependências e o impacto do elemento " +
                "selecionado com base no Project Graph do BIMBrain.");

            var diagnostico = MakePlaceholder(
                "BIMBrain.Diagnostico",
                "Diagnóstico",
                "diagnostico",
                "Diagnóstico do modelo.",
                "Executa uma análise completa do projeto elétrico e dos modelos " +
                "vinculados, destacando inconsistências.");

            var ia = MakePlaceholder(
                "BIMBrain.IA",
                "IA",
                "ia",
                "Assistente de IA.",
                "Interface conversacional com o copiloto do BIMBrain para responder " +
                "perguntas sobre o modelo.");

            panel.AddStackedItems(explicar, diagnostico, ia);
        }

        private static void BuildEngineeringPanel(UIControlledApplication app)
        {
            var panel = app.CreateRibbonPanel("BIMBrain", "Engenharia");

            var nbr = MakeButton(
                "BIMBrain.NBR5410",
                "NBR 5410",
                typeof(RunNBR5410Command),
                "nbr5410",
                "Executa as regras NBR 5410.",
                "Valida o modelo elétrico contra as regras normativas da NBR 5410 " +
                "implementadas no BIMBrain.");

            var integridade = MakePlaceholder(
                "BIMBrain.Integridade",
                "Integridade",
                "integridade",
                "Verifica a integridade do modelo.",
                "Identifica elementos sem circuito, circuitos sem painel e demais " +
                "inconsistências de consistência.");

            var coordenacao = MakePlaceholder(
                "BIMBrain.Coordenacao",
                "Coordenação",
                "coordenacao",
                "Coordenação de modelos.",
                "Gerencia modelos vinculados, clash detection e navegação para " +
                "problemas de coordenação.");

            panel.AddStackedItems(nbr, integridade, coordenacao);
        }

        private static void BuildToolsPanel(UIControlledApplication app)
        {
            var panel = app.CreateRibbonPanel("BIMBrain", "Ferramentas");

            var automacao = MakePlaceholder(
                "BIMBrain.Automacao",
                "Automação",
                "automacao",
                "Automação de modelagem.",
                "Ferramentas de automação para modelagem, circuitos e tags do " +
                "projeto elétrico.");

            var configuracoes = MakePlaceholder(
                "BIMBrain.Configuracoes",
                "Configurações",
                "configuracoes",
                "Configurações do BIMBrain.",
                "Abre as configurações gerais do plugin, incluindo parâmetros de " +
                "IA e regras.");

            var sobre = MakePlaceholder(
                "BIMBrain.Sobre",
                "Sobre",
                "sobre",
                "Sobre o BIMBrain.",
                "Exibe informações de versão, créditos e documentação do BIMBrain.");

            panel.AddStackedItems(automacao, configuracoes, sobre);
        }

        private static PushButtonData MakePlaceholder(
            string name,
            string text,
            string iconKey,
            string tooltip,
            string longDescription)
        {
            return MakeButton(
                name,
                text,
                typeof(PlaceholderCommand),
                iconKey,
                tooltip,
                longDescription);
        }

        private static PushButtonData MakeButton(
            string name,
            string text,
            System.Type commandType,
            string iconKey,
            string tooltip,
            string longDescription)
        {
            var data = new PushButtonData(
                name,
                text,
                commandType.Assembly.Location,
                commandType.FullName);

            data.Image = LoadIcon("Icons16." + iconKey + ".png");
            data.LargeImage = LoadIcon("Icons32." + iconKey + ".png");
            data.ToolTip = tooltip;
            data.LongDescription = longDescription;

            return data;
        }

        private static ImageSource LoadIcon(string resourceName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var stream = asm.GetManifestResourceStream("BIMBrain.Resources." + resourceName);
                if (stream != null)
                {
                    var decoder = new PngBitmapDecoder(
                        stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                    return decoder.Frames[0];
                }
            }
            catch
            {
            }

            return null;
        }
    }
}
