using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Gomoku
{
    public class Process
    {
        public BitmapImage canvastoBitmap(Canvas canvas)
        {
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96d, 96d, PixelFormats.Pbgra32);
            bitmap.Render(canvas);
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            BitmapImage bmp = new BitmapImage() { CacheOption = BitmapCacheOption.OnLoad };
            MemoryStream outStream = new MemoryStream();
            encoder.Save(outStream);
            outStream.Seek(0, SeekOrigin.Begin);
            bmp.BeginInit();
            bmp.StreamSource = outStream;
            bmp.EndInit();
            return bmp;
        }
    }
}
