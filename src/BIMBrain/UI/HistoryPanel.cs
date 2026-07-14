using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BIMBrain.UI
{
    public class HistoryItem
    {
        public string Time { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public long ElapsedMs { get; set; }
        public string Origin { get; set; }

        public override string ToString() => $"{Time} {Question}";
    }

    public class HistoryPanel
    {
        public Grid Panel { get; }
        public ListBox ListBox { get; }

        public event Action<HistoryItem> OnItemSelected;

        public HistoryPanel()
        {
            Panel = new Grid { Margin = new Thickness(5, 0, 0, 0) };
            Panel.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            Panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var header = new StackPanel { Orientation = Orientation.Horizontal };

            var lbl = new TextBlock
            {
                Text = "Histórico",
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center
            };

            var copyBtn = new Button
            {
                Content = "Copiar",
                Width = 60,
                Height = 24,
                Margin = new Thickness(8, 0, 4, 0),
                FontSize = 11
            };
            copyBtn.Click += OnCopyClick;

            var clearBtn = new Button
            {
                Content = "Limpar",
                Width = 60,
                Height = 24,
                FontSize = 11
            };
            clearBtn.Click += OnClearClick;

            header.Children.Add(lbl);
            header.Children.Add(copyBtn);
            header.Children.Add(clearBtn);

            ListBox = new ListBox
            {
                Margin = new Thickness(0, 5, 0, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            ScrollViewer.SetVerticalScrollBarVisibility(ListBox, ScrollBarVisibility.Visible);
            ListBox.SelectionChanged += OnSelectionChanged;

            Grid.SetRow(header, 0);
            Grid.SetRow(ListBox, 1);
            Panel.Children.Add(header);
            Panel.Children.Add(ListBox);
        }

        public void AddEntry(string question, string answer, long elapsedMs, string origin)
        {
            var now = DateTime.Now;
            var time = $"[{now:HH:mm:ss}]";

            var item = new HistoryItem
            {
                Time = time,
                Question = question,
                Answer = answer,
                ElapsedMs = elapsedMs,
                Origin = origin
            };
            ListBox.Items.Add(item);
            ListBox.ScrollIntoView(item);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem is HistoryItem item)
                OnItemSelected?.Invoke(item);
        }

        private void OnCopyClick(object sender, RoutedEventArgs e)
        {
            if (ListBox.Items.Count == 0)
                return;

            var text = string.Join(Environment.NewLine + Environment.NewLine,
                ListBox.Items.Cast<HistoryItem>().Select(i =>
                    $"{i.Question}{Environment.NewLine}{i.Answer}"));
            Clipboard.SetText(text);
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ListBox.Items.Clear();
        }

        public void Clear()
        {
            ListBox.Items.Clear();
        }
    }
}
