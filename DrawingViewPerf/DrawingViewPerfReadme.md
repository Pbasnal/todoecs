Below is the question that was posted on [StackOverFlow](https://stackoverflow.com/questions/78051566/improving-performance-of-the-draw-calls-of-maui-graphicsview)

I'm writing a handwriting app in MAUI using DrawingView from the communityToolKit. Below is the sample code of how I'm creating the DrawingView

```
public MainPage()
{
    InitializeComponent();

    Content = new DrawingView
    {
        Lines = new ObservableCollection<IDrawingLine>(),
        LineColor = Colors.Red,
        LineWidth = 5,
        IsMultiLineModeEnabled = true,

    };
}
```
This code runs perfectly fine on windows machine but when executed on the android device or emulator, the app starts lagging after drawing a few lines. To be specific, the lag between registering consecutive points increases.

I started looking at the code of community toolkit to figure out what is happening and I found out that

canvas draws all the lines every time a new point is registered.
The Draw method of IDrawable is called by internal library when the Invalidate() is called on the GraphicsView.
Canvas also has State but it is only storing Stroke properties and not the actual lines.
The draw call code of DrawingView from community toolkit. The call DrawCurrentLines(canvas, drawingView); draws all the lines every time an input is registered.

```
class DrawingViewDrawable : IDrawable
{
    readonly MauiDrawingView drawingView;
    Stopwatch watch = new();

    public DrawingViewDrawable(MauiDrawingView drawingView)
    {
        this.drawingView = drawingView;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.SetFillPaint(drawingView.Paint, dirtyRect);
        canvas.FillRectangle(dirtyRect);

        drawingView.DrawAction?.Invoke(canvas, dirtyRect);
        DrawCurrentLines(canvas, drawingView);

        SetStroke(canvas, drawingView.LineWidth, drawingView.LineColor);
        canvas.DrawPath(drawingView.currentPath);
    }
}
```
I'm not able to figure out how to improve performance of this Draw call. How can I avoid redrawing all the lines every time. Or I should check some other package?


I'm using 
CommunityToolkit.Maui 7.0.1 
.NET 8 
emulator: Pixel 5 - API 34

Tested on device: Samsung Tab S6 FE

Video Showing Issue Shows the issue. It is specially visible in the last circle which is drawn. Note that while the circle is being drawn, there is significant lag between where the mouse pointer and the line. But more important is the time delay between recording mouse input. That makes the line very jagged. This video is from the sample code which I'm using to debug performance issue.

Recording on Android Device of the app. This recording is of the app that I'm building and it's running on Samsung Tab S6 FE. The drop in performance is visible and it keeps degrading the more strokes a user makes.