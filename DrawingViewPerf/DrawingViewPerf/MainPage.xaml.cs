using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

using System.Collections.ObjectModel;

namespace DrawingViewPerf
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            DrawingView pageContent = new()
            {
                Lines = new ObservableCollection<IDrawingLine>(),
                LineColor = Colors.Red,
                BackgroundColor = Colors.Transparent,
                LineWidth = 5,
                IsMultiLineModeEnabled = true,
            };

            Content = pageContent;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
