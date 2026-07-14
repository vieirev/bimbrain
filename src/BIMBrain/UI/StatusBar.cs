using System.Windows;
using System.Windows.Controls;

namespace BIMBrain.UI
{
    public class StatusBar
    {
        public StackPanel Panel { get; }
        private readonly TextBlock _statusText;
        private readonly TextBlock _originText;
        private readonly TextBlock _timeText;
        private readonly TextBlock _modelText;

        public StatusBar()
        {
            Panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 8, 0, 0)
            };

            _statusText = CreateField();
            _originText = CreateField();
            _timeText = CreateField();
            _modelText = CreateField();

            Panel.Children.Add(_statusText);
            Panel.Children.Add(CreateSeparator());
            Panel.Children.Add(_originText);
            Panel.Children.Add(CreateSeparator());
            Panel.Children.Add(_timeText);
            Panel.Children.Add(CreateSeparator());
            Panel.Children.Add(_modelText);

            SetStatus("Pronto");
            SetOrigin("BIMBrain");
            SetTime(0);
            SetModelCount(1);
        }

        private static TextBlock CreateField()
        {
            return new TextBlock
            {
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            };
        }

        private static Separator CreateSeparator()
        {
            return new Separator
            {
                Width = 1,
                Height = 14,
                Margin = new Thickness(8, 0, 8, 0)
            };
        }

        public void SetStatus(string status)
        {
            _statusText.Text = $"Status: {status}";
        }

        public void SetOrigin(string origin)
        {
            _originText.Text = $"Origem: {origin}";
        }

        public void SetTime(long elapsedMs)
        {
            _timeText.Text = $"Tempo: {elapsedMs} ms";
        }

        public void SetModelCount(int count)
        {
            _modelText.Text = $"Modelos: {count}";
        }
    }
}
