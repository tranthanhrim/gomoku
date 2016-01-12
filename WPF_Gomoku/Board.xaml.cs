using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Gomoku
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public Board()
        {
            InitializeComponent();
        }

        int row, col;
        double width = 360;
        double height = 360;

        public double PropertyWidth
        {
            get { return width; }
            set { width = value; }
        }

        public double PropertyHeight
        {
            get { return height; }
            set { height = value; }
        }
        public int PropertyRow
        {
            get{return row;}
            set { row = value; }
        }

        public int PropertyCol
        {
            get { return col; }
            set { col = value; }
        }

        BitmapImage canvastoBitmap(Canvas canvas)
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapSource bmp = canvastoBitmap(cvsboard);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = bmp;
            cvsboard.Children.Clear();
            cvsboard.Background = ib;
        }

        Point pos;
        int x, y;
        private void cvsboard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pos = e.GetPosition(cvsboard);
            x = (int)(pos.X / (width / 12));
            y = (int)(pos.Y / (height / 12));
            //MessageBox.Show(pos.X.ToString() + " " + pos.Y.ToString());
            MessageBox.Show("Tọa độ: (" + x.ToString() + " , " + y.ToString() + ")");
        }
    }
}
