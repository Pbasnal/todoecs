using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;

using System.Collections.ObjectModel;

namespace DrawingViewPerf;

public class PerformantViewCorrectedAlignmentButFlickering
{
    int lineWidth = 5;

    DrawingLine borderLine;
    Image image;

    DrawingView drawingView;
    Grid gridView;

    byte[] imageBuffer;
    bool isImageBufferEmpty = true;

    public PerformantViewCorrectedAlignmentButFlickering()
    {
        image = new Image();

        drawingView = new()
        {
            Lines = new ObservableCollection<IDrawingLine>(),
            LineColor = Colors.Red,
            BackgroundColor = Colors.Transparent,
            LineWidth = lineWidth,
            IsMultiLineModeEnabled = true,
        };

        gridView = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = new GridLength(2, GridUnitType.Star) },
            },
            ColumnDefinitions =
            {
                new ColumnDefinition()
            }
        };
    }

    public View GetView()
    {
        drawingView.SizeChanged += (s, e) =>
        {
            InitialiseBoundaryLineWhithCausesAlignmentIssue();
            DrawBorder();
        };

        drawingView.DrawingLineCompleted += async (s, e) =>
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(200));
            Stream newImageStream = await DrawingView.GetImageStream(
                drawingView.Lines,
                new Size(drawingView.Width, drawingView.Height),
                SolidColorBrush.Transparent,
                cts.Token);

            if (isImageBufferEmpty)
            {
                imageBuffer = ImageUtils.CreateImagePng(newImageStream);
                isImageBufferEmpty = false;
            }
            else
            {
                byte[] newImageBuffer = ImageUtils.CreateImagePng(newImageStream);
                imageBuffer = ImageUtils.MergeImages(new MemoryStream(imageBuffer), new MemoryStream(newImageBuffer));
            }

            image.Source = ImageSource.FromStream(() => new MemoryStream(imageBuffer));

            // remove the last drawn line
            // So that the boundary remains and the latency of Draw call stays consistent
            drawingView.Lines.RemoveAt(1);
        };

        gridView.Add(image, 0, 0);
        gridView.Add(drawingView, 0, 0);

        return gridView;
    }

    private void InitialiseBoundaryLineWhithCausesAlignmentIssue()
    {
        double width = drawingView.Width;
        double height = drawingView.Height;

        double originX = lineWidth;
        double originY = lineWidth;
        double endX = width - lineWidth;
        double endY = height - lineWidth;

        borderLine = new();
        borderLine.Points.Add(new Point(originX, originY));
        borderLine.Points.Add(new Point(originX, endY));
        borderLine.Points.Add(new Point(endX, endY));
        borderLine.Points.Add(new Point(endX, originY));
        borderLine.Points.Add(new Point(originX, originY));

        borderLine.LineColor = Colors.Red;
        borderLine.LineWidth = 5;
    }

    private void DrawBorder()
    {
        drawingView.Lines.Clear();
        drawingView.Lines.Add(borderLine);
    }
}
