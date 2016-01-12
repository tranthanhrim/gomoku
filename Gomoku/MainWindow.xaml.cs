using System;
using System.Collections.Generic;
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

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*Process a = new Process();
            BitmapSource bmp = a.canvastoBitmap(cvsboard);
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = bmp;
            cvsboard.Children.Clear();
            cvsboard.Background = ib;*/
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            board.newGame();
        }

        private void m1_Click(object sender, RoutedEventArgs e)
        {
            m1.IsChecked = true;
            if (m2 != null)
                m2.IsChecked = false;
            if (Board.PropertyMode != 1 && board != null)
                board.newGame();
            Board.PropertyMode = 1;
        }

        private void m2_Click(object sender, RoutedEventArgs e)
        {
            m2.IsChecked = true;
            if (m1 != null)
                m1.IsChecked = false;
            if (Board.PropertyMode != 2 && board != null)
                board.newGame();
            Board.PropertyMode = 2;
        }

    }
}
