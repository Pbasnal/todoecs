namespace DrawingViewPerf
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            //var viewGenerator = new ViewWithDegradingPerformance();
            //var viewGenerator = new ViewWithBetterPerfButWrongAlignment();
            var viewGenerator = new PerformantViewCorrectedAlignmentButFlickering();

            Content = viewGenerator.GetView();
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
