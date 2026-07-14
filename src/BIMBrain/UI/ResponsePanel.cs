using System.Windows;
using System.Windows.Controls;

namespace BIMBrain.UI
{
    public class ResponsePanel
    {
        public TextBox TextBox { get; }

        private static readonly string Separator = new string('─', 48);

        public ResponsePanel()
        {
            TextBox = new TextBox
            {
                IsReadOnly = true,
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Margin = new Thickness(0, 0, 5, 0),
                FontSize = 13,
                Padding = new Thickness(8)
            };

            SetInitialState();
        }

        public void SetInitialState()
        {
            TextBox.Text = $"  Aguardando consulta...{Separator}\n\n" +
                "Nenhuma pergunta informada.\nDigite uma pergunta antes de consultar.";
        }

        public void ShowSimple(string status, string message)
        {
            TextBox.Text = $"  {status}\n{Separator}\n\n{message}";
        }

        public void ShowSuccess(string projectName, string question, string answer, long elapsedMs, string origin)
        {
            TextBox.Text = $"  {question}\n{Separator}\n\n" +
                $"Projeto: {projectName}\n\n" +
                $"{answer}\n\n" +
                $"{Separator}\n" +
                $"  Tempo: {elapsedMs} ms   |   Origem: {origin}";
        }

        public void ShowError(string message)
        {
            TextBox.Text = $"  Consulta não realizada\n{Separator}\n\n{message}";
        }

        public void Clear()
        {
            SetInitialState();
        }
    }
}
