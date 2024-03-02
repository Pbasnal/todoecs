using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;

using SkiaSharp;

using System.Collections.ObjectModel;

namespace DrawingViewPerf
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        int lineWidth = 5;

        DrawingLine borderLine;
        Image[] images;
        int currentImageIndex = 0;

        DrawingView drawingView;

        byte[] imageBuffer;
        bool isImageBufferEmpty = true;

        public MainPage()
        {
            InitializeComponent();

            images = new Image[] {
                new Image(),
                new Image()
            };

            imageBuffer = new byte[0];

            drawingView = new()
            {
                Lines = new ObservableCollection<IDrawingLine>(),
                LineColor = Colors.Red,
                BackgroundColor = Colors.Transparent,
                LineWidth = lineWidth,
                IsMultiLineModeEnabled = true,
            };

            // Uncomment below line to use the experimental optimisation
            WritingLinesToImageForPerformance();

            Grid grid = new Grid
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
            grid.Add(images[0], 0, 0);
            grid.Add(images[1], 0, 0);
            grid.Add(drawingView, 0, 0);

            Content = grid;
        }

        public void WritingLinesToImageForPerformance()
        {
            drawingView.SizeChanged += (s, e) =>
            {
                InitialiseBoundaryLine();
                DrawBorder();
            };

            drawingView.DrawingLineCompleted += async (s, e) =>
            {
                int indexOfImageToUpdate = (currentImageIndex + 1) % 2;

                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(200));
                Stream newImageStream = await DrawingView.GetImageStream(
                    drawingView.Lines,
                    new Size(drawingView.Width, drawingView.Height),
                    SolidColorBrush.Transparent,
                    cts.Token);

                if (isImageBufferEmpty)
                {
                    imageBuffer = CreateImagePng(newImageStream);
                    isImageBufferEmpty = false;
                }
                else
                {
                    byte[] newImageBuffer = CreateImagePng(newImageStream);
                    imageBuffer = MergeImages(new MemoryStream(imageBuffer), new MemoryStream(newImageBuffer));
                }

                images[currentImageIndex].Source = ImageSource.FromStream(() => new MemoryStream(imageBuffer));
                //DrawBorder();
                drawingView.Lines.RemoveAt(0);
            };
        }

        public static byte[] CreateImagePng(Stream imageStream)
        {
            // Decode the PNG images from memory streams
            SKBitmap bitmap1 = SKBitmap.Decode(imageStream);

            //Encode the merged bitmap to a PNG format
            byte[] imageBytes;
            using (SKImage image = SKImage.FromBitmap(bitmap1))
            {
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    imageBytes = data.ToArray();
                }
            }
            return imageBytes;
        }

        public static byte[] MergeImages(Stream imageStream1, Stream imageStream2)
        {
            // Decode the PNG images from memory streams
            SKBitmap bitmap1 = SKBitmap.Decode(imageStream1);
            SKBitmap bitmap2 = SKBitmap.Decode(imageStream2);

            // Calculate the maximum width and height of the merged image
            int maxWidth = Math.Max(bitmap1.Width, bitmap2.Width);
            int maxHeight = Math.Max(bitmap1.Height, bitmap2.Height);

            // Create a new bitmap to hold the merged image
            SKBitmap mergedBitmap = new SKBitmap(maxWidth, maxHeight);

            // Create a canvas from the merged bitmap
            using (SKCanvas canvas = new SKCanvas(mergedBitmap))
            {
                // Draw the first image on the canvas
                canvas.DrawBitmap(bitmap1, 0, 0);
                // Draw the second image on top of the first image with alpha blending
                using (var paint = new SKPaint { BlendMode = SKBlendMode.SrcOver })
                {
                    canvas.DrawBitmap(bitmap2, SKRect.Create(maxWidth, maxHeight), paint);
                }
            }

            //Encode the merged bitmap to a PNG format
            byte[] mergedBytes;
            using (SKImage image = SKImage.FromBitmap(mergedBitmap))
            {
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    mergedBytes = data.ToArray();
                }
            }
            return mergedBytes;
        }

        private void InitialiseBoundaryLine()
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

        void DrawBorder()
        {
            //drawingView.Lines.Clear();
            drawingView.Lines.Add(borderLine);
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
