using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;

using System.Collections.ObjectModel;

namespace DrawingViewPerf;

public class ViewWithDegradingPerformance
{
    int lineWidth = 5;

    public View GetView()
    {
        return new DrawingView
        {
            Lines = new ObservableCollection<IDrawingLine>(),
            LineColor = Colors.Red,
            BackgroundColor = Colors.Transparent,
            LineWidth = lineWidth,
            IsMultiLineModeEnabled = true,
        };
    }
}
