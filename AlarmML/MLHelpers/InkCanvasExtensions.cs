using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml.Controls;

namespace MLHelpers
{
    public static class InkCanvasExtensions
    {
        private static InkDrawingAttributes _defaultDrawingAttributes;

        private static InkDrawingAttributes GetDefaultInkDrawingAttributes()
        {
            if (_defaultDrawingAttributes == null)
            {
                _defaultDrawingAttributes = new InkDrawingAttributes();
                _defaultDrawingAttributes.Color = Colors.Black;
                _defaultDrawingAttributes.Size = new Size(4, 4);
            }

            return _defaultDrawingAttributes;
        }

        public static SoftwareBitmap GetSoftwareBitmap(this InkCanvas canvas, IEnumerable<InkStroke> strokes = null)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, (int)canvas.ActualWidth, (int)canvas.ActualHeight, 96);

            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.Clear(Colors.White);
                ds.DrawInk(strokes ?? canvas.InkPresenter.StrokeContainer.GetStrokes(), true);
            }

            return SoftwareBitmap.CreateCopyFromBuffer(renderTarget.GetPixelBytes().AsBuffer(), BitmapPixelFormat.Bgra8, (int)canvas.ActualWidth, (int)canvas.ActualHeight, BitmapAlphaMode.Premultiplied);
        }

        public static SoftwareBitmap GetCropedSoftwareBitmap(this InkCanvas canvas, IEnumerable<InkStroke> strokes = null, float newWidth = 0, float newHeight = 0, bool keepRelativeSize = false)
        {
            strokes = strokes ?? canvas.InkPresenter.StrokeContainer.GetStrokes();
            var bounds = strokes.GetBoundingBoxForInkStrokes();

            if (keepRelativeSize)
            {
                // copy strokes, resize them, but keep the stroke size the same, tranlsate to as close to 0, 0 as possible
                List<InkStroke> newStrokes = new List<InkStroke>();
                var scaleX = (float)(newWidth / bounds.Width);
                var scaleY = (float)(newHeight / bounds.Height);
                var translateX = 1 - (float)bounds.X * scaleX;
                var translateY = 1 - (float)bounds.Y * scaleY;

                foreach (var stroke in strokes)
                {
                    var newStroke = stroke.Clone();
                    newStroke.PointTransform = Matrix3x2.CreateScale(scaleX, scaleY) * Matrix3x2.CreateTranslation(translateX, translateY);
                    newStroke.DrawingAttributes = GetDefaultInkDrawingAttributes();
                    newStrokes.Add(newStroke);
                }

                strokes = newStrokes;
                bounds = strokes.GetBoundingBoxForInkStrokes();
            }

            bounds.X = Math.Max(bounds.X, 0);
            bounds.Y = Math.Max(bounds.Y, 0);
            var bitmap = canvas.GetSoftwareBitmap(strokes);

            if (newWidth > 0 && newHeight > 0)
            {
                return bitmap.CropAndResize(bounds, newWidth, newHeight);
            }

            return bitmap.Crop(bounds);
        }

        public static Rect GetBoundingBoxForInkStrokes(this IEnumerable<InkStroke> strokes)
        {
            double xMin = double.PositiveInfinity;
            double xMax = 0;
            double yMin = double.PositiveInfinity;
            double yMax = 0;

            foreach (var stroke in strokes)
            {
                xMin = Math.Min(xMin, stroke.BoundingRect.X);
                xMax = Math.Max(xMax, stroke.BoundingRect.X + stroke.BoundingRect.Width);

                yMin = Math.Min(yMin, stroke.BoundingRect.Y);
                yMax = Math.Max(yMax, stroke.BoundingRect.Y + stroke.BoundingRect.Height);


                // stroke.Selected = true;
            }

            var width = xMax - xMin;
            var height = yMax - yMin;

            if (width > height)
            {
                yMin = yMin - (width - height) / 2;
                height = width;
            }
            else if (height > width)
            {
                xMin = xMin - (height - width) / 2;
                width = height;
            }

            return new Rect(xMin, yMin, width, height);
        }
    }
}
