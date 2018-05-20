using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

namespace MLHelpers
{
    public static class SoftwareBitmapExtensions
    {
        public static SoftwareBitmap Resize(this SoftwareBitmap softwareBitmap, float newWidth, float newHeight)
        {
            var resourceCreator = CanvasDevice.GetSharedDevice();
            using (var canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(resourceCreator, softwareBitmap))
            using (var canvasRenderTarget = new CanvasRenderTarget(resourceCreator, newWidth, newHeight, canvasBitmap.Dpi))
            using (var drawingSession = canvasRenderTarget.CreateDrawingSession())
            using (var scaleEffect = new ScaleEffect())
            {
                drawingSession.Clear(Colors.White);

                scaleEffect.Source = canvasBitmap;
                scaleEffect.Scale = new System.Numerics.Vector2(newWidth / canvasBitmap.SizeInPixels.Width, newHeight / canvasBitmap.SizeInPixels.Height);
                drawingSession.DrawImage(scaleEffect);
                drawingSession.Flush();

                return SoftwareBitmap.CreateCopyFromBuffer(canvasRenderTarget.GetPixelBytes().AsBuffer(), BitmapPixelFormat.Bgra8, (int)newWidth, (int)newHeight, BitmapAlphaMode.Premultiplied);
            }
        }

        public static SoftwareBitmap Crop(this SoftwareBitmap softwareBitmap, Rect bounds)
        {
            var resourceCreator = CanvasDevice.GetSharedDevice();
            using (var canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(resourceCreator, softwareBitmap))
            using (var canvasRenderTarget = new CanvasRenderTarget(resourceCreator, (float)bounds.Width, (float)bounds.Width, canvasBitmap.Dpi))
            using (var drawingSession = canvasRenderTarget.CreateDrawingSession())
            using (var cropEffect = new CropEffect())
            using (var atlasEffect = new AtlasEffect())
            {
                drawingSession.Clear(Colors.White);

                cropEffect.SourceRectangle = bounds;
                cropEffect.Source = canvasBitmap;

                atlasEffect.SourceRectangle = bounds;
                atlasEffect.Source = cropEffect;
                
                drawingSession.DrawImage(atlasEffect);
                drawingSession.Flush();

                return SoftwareBitmap.CreateCopyFromBuffer(canvasRenderTarget.GetPixelBytes().AsBuffer(), BitmapPixelFormat.Bgra8, (int)bounds.Width, (int)bounds.Width, BitmapAlphaMode.Premultiplied);
            }

        }

        public static SoftwareBitmap CropAndResize(this SoftwareBitmap softwareBitmap, Rect bounds, float newWidth, float newHeight)
        {
            var resourceCreator = CanvasDevice.GetSharedDevice();
            using (var canvasBitmap = CanvasBitmap.CreateFromSoftwareBitmap(resourceCreator, softwareBitmap))
            using (var canvasRenderTarget = new CanvasRenderTarget(resourceCreator, newWidth, newHeight, canvasBitmap.Dpi))
            using (var drawingSession = canvasRenderTarget.CreateDrawingSession())
            using (var scaleEffect = new ScaleEffect())
            using (var cropEffect = new CropEffect())
            using (var atlasEffect = new AtlasEffect())
            {
                drawingSession.Clear(Colors.White);

                cropEffect.SourceRectangle = bounds;
                cropEffect.Source = canvasBitmap;

                atlasEffect.SourceRectangle = bounds;
                atlasEffect.Source = cropEffect;

                scaleEffect.Source = atlasEffect;
                scaleEffect.Scale = new System.Numerics.Vector2(newWidth / (float)bounds.Width, newHeight / (float)bounds.Height);
                drawingSession.DrawImage(scaleEffect);
                drawingSession.Flush();

                return SoftwareBitmap.CreateCopyFromBuffer(canvasRenderTarget.GetPixelBytes().AsBuffer(), BitmapPixelFormat.Bgra8, (int)newWidth, (int)newHeight, BitmapAlphaMode.Premultiplied);
            }
        }

        public static async Task<bool> SaveToFile(this SoftwareBitmap softwareBitmap, StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an encoder with the desired format
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

                // Set the software bitmap
                encoder.SetSoftwareBitmap(softwareBitmap);

                try
                {
                    await encoder.FlushAsync();
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
