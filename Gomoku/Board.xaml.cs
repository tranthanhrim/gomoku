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
using Newtonsoft.Json.Linq;

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
            AIOnlineWoker.DoWork += AIOnlineWoker_DoWork;
            AIOnlineWoker.RunWorkerCompleted += AIOnlineWoker_RunWorkerCompleted;
        }

        #region declare
        static int mode = 1; //chế độ chơi

        static int height = 12;
        static int width = 12;

        int AI_x, AI_y; //tọa độ x, y máy offline
        int AI_online_x, AI_online_y; //tọa độ x, y máy online

        Point AI_online;
        Point AI;

        bool isWin = false;
        bool isOnlineWin = false;
        bool isAIOnlineWin = false;

        Point onlineStep = new Point();
        Point online_me = new Point();


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
        BackgroundWorker AIOnlineWoker = new BackgroundWorker();

        int[,] BoardMerge = new int[12, 12];

        int turn = 1;

        int _onlineturn = -1;
        
        public int Onlineturn
        {
            get { return _onlineturn; }
            set { _onlineturn = value; }
        }

        String name0 = "images/background.png";
        String name1 = "images/background1.png";
        String p1 = "images/player1.png";
        String p2 = "images/player2.png";
        String p1_1 = "images/player1_1.png";
        String p2_1 = "images/player2_1.png";
        String p1_h = "images/highlight1.png";
        String p2_h = "images/highlight2.png";
        #endregion

        #region BackgroundWoker
        private void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
           
            Button b = (Button)grid.Children[12 * (int)AI.X + (int)AI.Y];

            if ((int)AI.X % 2 == 0 && (int)AI.Y % 2 == 0 || (int)AI.X % 2 != 0 && (int)AI.Y % 2 != 0)
            {
                setBackground(b, p2);
            }
            else
            {
                setBackground(b, p2_1);
            }

            player2.mark((int)AI.X, (int)AI.Y);

            BoardMerge[(int)AI.X, (int)AI.Y] = 2;

            int result2 = player2.check((int)AI.X, (int)AI.Y);
            if (result2 != 0)
            {
                MessageBox.Show("AI win!");
                //highlight dong win
                highlight(player2, result2, p2_h);
                isWin = true;
            }
        }

        private void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Thread.Sleep(1000);
            //Random ran = new Random();
            //do
            //{
            //    AI_x = ran.Next(12);
            //    AI_y = ran.Next(12);
            //}
            //while (isblank(player1, AI_x, AI_y) == false || isblank(player2, AI_x, AI_y) == false);
            AI = ai_FindWay();
        }

        private void AIOnlineWoker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isAIOnlineWin == true)
            {
                isAIOnlineWin = false;
                return;
            }

            Button b = (Button)grid.Children[12 * (int)AI_online.X + (int)AI_online.Y];

            if ((int)AI_online.X % 2 == 0 && (int)AI_online.Y % 2 == 0 || (int)AI_online.X % 2 != 0 && (int)AI_online.Y % 2 != 0)
            {
                setBackground(b, p1);
            }
            else
            {
                setBackground(b, p1_1);
            }

            player1.mark((int)AI_online.X, (int)AI_online.Y);

            BoardMerge[(int)AI_online.X, (int)AI_online.Y] = 2;

            online_me.X = (int)AI_online.X;
            online_me.Y = (int)AI_online.Y;

            MainWindow.socket.Emit("MyStepIs", JObject.FromObject(new { row = (int)AI_online.X, col = (int)AI_online.Y }));
        }

        private void AIOnlineWoker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Random ran = new Random();
            //do
            //{
            //    AI_online_x = ran.Next(12);
            //    AI_online_y = ran.Next(12);
            //}
            //while (isblank(player1, AI_online_x, AI_online_y) == false || isblank(player2, AI_online_x, AI_online_y) == false);
            AI_online = ai_FindWay();
        }

        #endregion

        //highlight dòng win offline
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

        //highlight dòng win online
        public void onlineHighLight()
        {
            int result1 = player1.check((int)online_me.X, (int)online_me.Y);
            if (result1 != 0)
            {
                //highlight dong win
                highlight(player1, result1, p1_h);
                isAIOnlineWin = true;
                
                return;
            }

            int result2 = player2.check((int)onlineStep.X, (int)onlineStep.Y);
            if (result2 != 0)
            {
                //highlight dong win
                highlight(player2, result2, p2_h);
                isAIOnlineWin = true;
                
                return;
            }
        }

        //kiểm tra ô trống trên bàn cờ
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
            isAIOnlineWin = false;
            turn = 1;
            _onlineturn = -1;
            player1 = new Player();
            player2 = new Player();
            BoardMerge = new int[12, 12];
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

        //Đánh dấu nước đi kế tiếp của đối thủ
        public void setStep(int x, int y)
        {
            if (x < 0)
                return;
            Button a = (Button)grid.Children[12 * x + y];

            if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
            {
                setBackground(a, p2);
            }
            else
            {
                setBackground(a, p2_1);
            }

            player2.mark(x, y);
            BoardMerge[x, y] = 1;

            onlineStep.X = x;
            onlineStep.Y = y;

            Onlineturn = 1;               
        }

        public void AIplay()
        {
            AIOnlineWoker.RunWorkerAsync();
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            #region Two players
            if (mode == 1)
            {
                if (isWin)
                    return;

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
                if (isWin)
                    return;

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

                BoardMerge[x, y] = 1;

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

            #region play Online
            else if (mode == 3)
            {            
                if (isOnlineWin == true)
                    return;
                    
                Button a = (Button)sender;
                int x = (int)a.GetValue(Grid.RowProperty);
                int y = (int)a.GetValue(Grid.ColumnProperty);

                MainWindow.socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));

                if (Onlineturn == 2 || Onlineturn == -1)
                    return;

                if (isblank(player1, x, y) == false || isblank(player2, x, y) == false)
                    return;

                player1.mark(x, y);
                online_me.X = x;
                online_me.Y = y;

                if (x % 2 == 0 && y % 2 == 0 || x % 2 != 0 && y % 2 != 0)
                {
                    setBackground(a, p1);
                }
                else
                {
                    setBackground(a, p1_1);
                }

                Onlineturn = 2;
            }
            #endregion
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //OnNextStep += new OnNextStepHandler(setStep);
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


        #region Tìm nước đi AI
        public long[] aScore = new long[7] { 0, 3, 24, 192, 1536, 12288, 98304 };
        public long[] dScore = new long[7] { 0, 1, 9, 81, 729, 6561, 59849 };
      
        private Point ai_FindWay()
        {
            Point res = new Point();
            long max_Mark = 0; //điểm để xác định nước đi

            for (int i = 1; i < width; i++)
            {
                for (int j = 1; j < height; j++)
                {
                    if (BoardMerge[i, j] == 0)
                    {
                        long Attackscore = DiemTC_DuyetDoc(i, j) + DiemTC_DuyetNgang(i, j) + DiemTC_DuyetCheoNguoc(i, j) + DiemTC_DuyetCheoXuoi(i, j);
                        long Defensescore = DiemPN_DuyetDoc(i, j) + DiemPN_DuyetNgang(i, j) + DiemPN_DuyetCheoNguoc(i, j) + DiemPN_DuyetCheoXuoi(i, j); ;
                        long tempMark = Attackscore > Defensescore ? Attackscore : Defensescore;
                        if (max_Mark < tempMark)
                        {
                            max_Mark = tempMark;
                            res.X = i;
                            res.Y = j;
                        }
                    }
                }
            }
            return res;
        }
        private long DiemTC_DuyetDoc(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyệt từ dưới lên 
            for (int count = 1; count < 6 && currRow - count >= 0; count++)
            {
                if (BoardMerge[currRow - count, currCol] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow - count, currCol] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            //duyet từ trên xuống
            for (int count = 1; count < 6 && currRow + count < width; count++)
            {
                if (BoardMerge[currRow + count, currCol] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow + count, currCol] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (SoQuanDich == 2)
                return 0;
            Sum -= dScore[SoQuanDich + 1];
            Sum += aScore[SoQuanTa];
            return Sum;
        }
        private long DiemTC_DuyetNgang(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyệt từ phải sang trái
            for (int count = 1; count < 6 && currCol - count >= 0; count++)
            {
                if (BoardMerge[currRow, currCol - count] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow, currCol - count] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            //duyet từ trái sang phải
            for (int count = 1; count < 6 && currCol + count < width; count++)
            {
                if (BoardMerge[currRow, currCol + count] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow, currCol + count] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (SoQuanDich == 2)
                return 0;
            Sum -= dScore[SoQuanDich + 1];
            Sum += aScore[SoQuanTa];
            return Sum;
        }
        private long DiemTC_DuyetCheoXuoi(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyệt góc phải trên
            for (int count = 1; count < 6 && currRow - count >= 0 && currCol + count < width; count++)
            {
                if (BoardMerge[currRow - count, currCol + count] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow - count, currCol + count] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            //duyet goc trái dưới
            for (int count = 1; count < 6 && currRow + count < width && currCol - count >= 0; count++)
            {
                if (BoardMerge[currRow + count, currCol - count] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow + count, currCol - count] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (SoQuanDich == 2)
                return 0;
            Sum -= dScore[SoQuanDich + 1];
            Sum += aScore[SoQuanTa];
            return Sum;
        }
        private long DiemTC_DuyetCheoNguoc(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyệt góc trái trên
            for (int count = 1; count < 6 && currRow - count >= 0 && currCol - count >= 0; count++)
            {
                if (BoardMerge[currRow - count, currCol - count] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow - count, currCol - count] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }

            //duyet goc phải dưới
            for (int count = 1; count < 6 && currRow + count < width && currCol + count < width; count++)
            {
                if (BoardMerge[currRow + count, currCol + count] == 2)
                    SoQuanTa++;
                else if (BoardMerge[currRow + count, currCol + count] == 1)
                {
                    SoQuanDich++;
                    break;
                }
                else
                    break;
            }
            if (SoQuanDich == 2)
                return 0;
            Sum -= dScore[SoQuanDich + 1];
            Sum += aScore[SoQuanTa];
            return Sum;
        }
        private long DiemPN_DuyetDoc(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyệt từ dưới lên 
            for (int count = 1; count < 6 && currRow - count >= 0; count++)
            {
                if (BoardMerge[currRow - count, currCol] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow - count, currCol] == 1)
                    SoQuanDich++;
                else
                    break;
            }

            //duyet từ trên xuống
            for (int count = 1; count < 6 && currRow + count < width; count++)
            {
                if (BoardMerge[currRow + count, currCol] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow + count, currCol] == 1)
                    SoQuanDich++;
                else
                    break;
            }
            if (SoQuanTa == 2)
                return 0;
            Sum += dScore[SoQuanDich];
            return Sum;
        }
        private long DiemPN_DuyetNgang(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyệt từ phải sang trái
            for (int count = 1; count < 6 && currCol - count >= 0; count++)
            {
                if (BoardMerge[currRow, currCol - count] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow, currCol - count] == 1)
                    SoQuanDich++;
                else
                    break;
            }

            //duyet từ trái sang phải
            for (int count = 1; count < 6 && currCol + count < width; count++)
            {
                if (BoardMerge[currRow, currCol + count] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow, currCol + count] == 1)
                    SoQuanDich++;
                else
                    break;
            }
            if (SoQuanTa == 2)
                return 0;
            Sum += dScore[SoQuanDich];
            return Sum;
        }
        private long DiemPN_DuyetCheoXuoi(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyet góc phải trên
            for (int count = 1; count < 6 && currRow - count >= 0 && currCol + count < width; count++)
            {
                if (BoardMerge[currRow - count, currCol + count] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow - count, currCol + count] == 1)
                    SoQuanDich++;
                else
                    break;
            }

            //duyet góc trái dưới
            for (int count = 1; count < 6 && currRow + count < width && currCol - count >= 0; count++)
            {
                if (BoardMerge[currRow + count, currCol - count] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow + count, currCol - count] == 1)
                    SoQuanDich++;
                else
                    break;
            }
            if (SoQuanTa == 2)
                return 0;
            Sum += dScore[SoQuanDich];
            return Sum;
        }
        private long DiemPN_DuyetCheoNguoc(int currRow, int currCol)
        {
            int SoQuanTa = 0;
            int SoQuanDich = 0;
            long Sum = 0;

            //duyet góc trái trên
            for (int count = 1; count < 6 && currRow - count >= 0 && currCol - count >= 0; count++)
            {
                if (BoardMerge[currRow - count, currCol - count] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow - count, currCol - count] == 1)
                    SoQuanDich++;
                else
                    break;
            }

            //duyet góc phải dưới
            for (int count = 1; count < 6 && currRow + count < width && currCol + count < width; count++)
            {
                if (BoardMerge[currRow + count, currCol + count] == 2)
                {
                    SoQuanTa++;
                    break;
                }
                else if (BoardMerge[currRow + count, currCol + count] == 1)
                    SoQuanDich++;
                else
                    break;
            }
            if (SoQuanTa == 2)
                return 0;
            Sum += dScore[SoQuanDich];
            return Sum;
        }
        #endregion
    }
}
