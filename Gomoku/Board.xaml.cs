using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Threading;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public Board()
        {
            InitializeComponent();
            myWorker.DoWork += myWorker_DoWork;
            myWorker.RunWorkerCompleted += myWorker_RunWorkerCompleted;
        }

        //vùng khai báo
        #region declare
        static int mode = 1; //chế độ chơi

        static int height = 12;
        static int width = 12;

        int AI_x, AI_y; //tọa độ x, y máy
        bool isWin = false;
        public static int PropertyMode
        {
            get { return mode; }
            set { mode = value; }
        }

        public static int PropertyHeight
        {
            get { return height; }
            set { height = value; }
        }

        public static int PropertyWidth
        {
            get { return width; }
            set { width = value; }
        }

        Player player1 = new Player();
        Player player2 = new Player();
        BackgroundWorker myWorker = new BackgroundWorker();

        int turn = 1;

        String name0 = "images/background.png";
        String name1 = "images/background1.png";
        String p1 = "images/player1.png";
        String p2 = "images/player2.png";
        String p1_1 = "images/player1_1.png";
        String p2_1 = "images/player2_1.png";
        String p1_h = "images/highlight1.png";
        String p2_h = "images/highlight2.png";
        #endregion

        private void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
            Button b = (Button)grid.Children[12 * AI_x + AI_y];

            if (AI_x % 2 == 0 && AI_y % 2 == 0 || AI_x % 2 != 0 && AI_y % 2 != 0)
            {
                setBackground(b, p2);
            }
            else
            {
                setBackground(b, p2_1);
            }
            player2.mark(AI_x, AI_y);

            int result2 = player2.check(AI_x, AI_y);
            if (result2 != 0)
            {
                MessageBox.Show("AI win!");
                //highlight dong win
                highlight(player2, result2, p2_h);
                isWin = true;
            }

            //if (!e.Cancelled && e.Error == null)//Check if the worker has been canceled or if an error occurred
            //{}
            //else if (e.Cancelled)
            //{
            //    MessageBox.Show("User Canceled");
            //}
            //else
            //{
            //     MessageBox.Show("An error has occurred");
            //}

        }

        private void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1000);
            Random ran = new Random();
            do
            {
                AI_x = ran.Next(12);
                AI_y = ran.Next(12);
            }
            while (isblank(player1, AI_x, AI_y) == false || isblank(player2, AI_x, AI_y) == false);
        }

        //highlight player win
        private void highlight(Player player, int result, String path)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (player.play[i, j] == result)
                    {
                        Button k = (Button)grid.Children[12 * i + j];
                        setBackground(k, path);
                    }
                }
            }
        }

        //kiểm tra ô trống bàn cờ
        bool isblank(Player player, int x, int y)
        {
            if (player.play[x, y] == 0)
                return true;
            return false;
        }

        //cài background cho nút
        void setBackground(Button a, String path)
        {
            BitmapImage bitimg = new BitmapImage();
            bitimg.BeginInit();
            bitimg.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
            bitimg.EndInit();

            Image img = new Image();
            img.Stretch = Stretch.Fill;
            img.Source = bitimg;

            a.Content = img;
            a.Background = new ImageBrush(bitimg);
        }

        //ván mới
        public void newGame()
        {
            isWin = false;
            turn = 1;
            player1 = new Player();
            player2 = new Player();
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Button a = (Button)grid.Children[12 * i + j];
                    if (i % 2 == 0 && j % 2 == 0 || i % 2 != 0 && j % 2 != 0)
                    {
                        setBackground(a, name0);
                    }
                    else if (i % 2 == 0 && j % 2 != 0 || i % 2 != 0 && j % 2 == 0)
                    {
                        setBackground(a, name1);
                    }
                }
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            if (isWin)
                return;

            #region Two players
            if (mode == 1)
            {
                if (turn == 1)
                {
                    Button a = (Button)sender;
                    int x = (int)a.GetValue(Grid.RowProperty);
                    int y = (int)a.GetValue(Grid.ColumnProperty);

                    if (isblank(player1, x, y) == false || isblank(player2, x, y) == false)
                        return;
                
                    turn = 2;

                    if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                    {
                        setBackground(a, p1);
                    }
                    else
                    {
                        setBackground(a, p1_1);
                    }

                    player1.mark(x, y);

                    int result = player1.check(x, y);

                    if (result != 0)
                    {
                        MessageBox.Show("Player 1 win!");
                        //highlight dong win
                        highlight(player1, result, p1_h);
                        isWin = true;
                    }             
                }
                else
                {
                    Button a = (Button)sender;
                    int x = (int)a.GetValue(Grid.RowProperty);
                    int y = (int)a.GetValue(Grid.ColumnProperty);

                    if (isblank(player1, x, y) == false || isblank(player2, x, y) == false)
                        return;

                    turn = 1;

                    if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                    {
                        setBackground(a, p2);
                    }
                    else
                    {
                        setBackground(a, p2_1);
                    }
                    player2.mark(x, y);

                    int result = player2.check(x, y);
                    if (result != 0)
                    {
                        MessageBox.Show("Player 2 win!");
                        //highlight dong win
                        highlight(player2, result, p2_h);
                        isWin = true;
                        
                    }             
                }
            }
            #endregion

            #region One player
            else if (mode == 2)
            {
                if (myWorker.IsBusy)
                    return;
                #region player
                Button a = (Button)sender;

                int x = (int)a.GetValue(Grid.RowProperty);
                int y = (int)a.GetValue(Grid.ColumnProperty);

                if (isblank(player1, x, y) == false || isblank(player2, x, y) == false)
                    return;

                if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                {
                    setBackground(a, p1);
                }
                else
                {
                    setBackground(a, p1_1);
                }

                player1.mark(x, y);

                int result = player1.check(x, y);
                if (result != 0)
                {
                    MessageBox.Show("Player 1 win!");
                    //highlight dong win
                    highlight(player1, result, p1_h);
                    isWin = true;
                    return;
                }             
                #endregion

                #region AI

                myWorker.RunWorkerAsync();

                #endregion
            }
            #endregion
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < height; i++)
            {
                ColumnDefinition c = new ColumnDefinition();
                c.Name = "c" + i.ToString();
                c.Width = new GridLength(1, GridUnitType.Star);

                RowDefinition r = new RowDefinition();
                r.Name = "r" + i.ToString();
                r.Height = new GridLength(1, GridUnitType.Star);

                grid.ColumnDefinitions.Add(c);
                grid.RowDefinitions.Add(r);
            }

            for (int i = 0; i < height; i++)
            {

                for (int j = 0; j < width; j++)
                {
                    Button a = new Button();
                    a.SetValue(Grid.RowProperty, i);
                    a.SetValue(Grid.ColumnProperty, j);
                    a.Click += btn_Click;
                    if (i % 2 == 0 && j % 2 == 0 || i % 2 != 0 && j % 2 != 0)
                    {
                        setBackground(a, name0);
                    }
                    else if (i % 2 == 0 && j % 2 != 0 || i % 2 != 0 && j % 2 == 0)
                    {
                        setBackground(a, name1);
                    }
                    grid.Children.Add(a);
                }
            }
        }
    }
}
