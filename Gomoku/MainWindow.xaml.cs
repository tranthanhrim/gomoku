using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System.ComponentModel;
using System.Configuration;

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
            myWorker.DoWork += myWorker_DoWork;
            myWorker.RunWorkerCompleted += myWorker_RunWorkerCompleted;
            this.DataContext = binding;
        }

        #region declare

        DataBinding binding = new DataBinding();//data binding

        BackgroundWorker myWorker = new BackgroundWorker();

        bool connected = false;//kiểm tra kết nối

        bool isCheckTurn = false;//kiểm tra khởi tạo lượt đi đầu tiên

        public static Socket socket;

        string message;//chứa tin nhắn hệ thống gửi tới

        string name = "SERVER"; //Tên đối tượng gửi tin nhắn

        string namePlayer = "Player";//Tên của mình

        #endregion

        #region BackgroundWoker
        private void myWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
    
        private void myWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            String ServerName = ConfigurationManager.ConnectionStrings["ServerName"].ConnectionString;
            socket = IO.Socket(ServerName);
            while(connected == false)
            {
                socket.On(Socket.EVENT_CONNECT, () =>
                {
                    connected = true;
                });
            }

            socket.On("ChatMessage", (data) =>
            {
                //var jobject = data as JToken;
                //message = jobject.Value<String>("message");
                message = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();

                if (binding.TextChat == message)
                {
                    binding.TextChat = message + "<next>";
                }
                else
                {
                    binding.TextChat = message;
                }
                
                //MessageBox.Show(message);
                //OnMessage(message);
                //chatbox.set("aa");

                if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                {
                    socket.Emit("MyNameIs", namePlayer);
                    socket.Emit("ConnectToOtherPlayer");
                    //MessageBox.Show("ConnecttoOtherPlayer");
                    //Console.ReadKey(intercept: true);
                }

                if (((Newtonsoft.Json.Linq.JObject)data).Count == 2)
                {
                    name = ((Newtonsoft.Json.Linq.JObject)data)["from"].ToString() ;
                }
                else
                {
                    name = "SERVER";
                }

                if (isCheckTurn == false)
                {
                    string[] tokens = message.Split(new[] { "<br />" }, StringSplitOptions.None);

                    if (tokens.Length < 2)
                        return;

                    /*for (int i = 0; i < tokens.Length; i++)
                        MessageBox.Show(tokens[i]);*/


                    if (tokens[1] == "You are the first player!")
                    {
                        board.Onlineturn = 1;
                        isCheckTurn = true;
                        MessageBox.Show("You are the FIRST player!");
                        if (Board.PropertyMode == 4)
                        {
                            Point temp = new Point(-2, -2);
                            binding.Pos = temp;
                        }
                    }
                    else if (tokens[1] == "You are the second player!")
                    {
                        board.Onlineturn = 2;
                        isCheckTurn = true;
                        MessageBox.Show("You are the SECOND player!");
                    }
                }
            });

            socket.On("NextStepIs", (data) =>
            {
                //Console.WriteLine("NextStepIs: " + data);
                var nextStep = data as JToken;
                //int x = nextStep.Value<int>("row");
                //int y = nextStep.Value<int>("col");
                if (nextStep.Value<int>("player") == 1)
                {
                    Point temp = new Point();
                    temp.X = nextStep.Value<int>("row");
                    temp.Y = nextStep.Value<int>("col");
                    binding.Pos = temp;
                    //btnplay_Click((object)btnplay, RoutedEventArgs.Empty);
                    //pos.Pos = temp;
                    //MessageBox.Show(Pos.ToString());
                }
            });

            socket.On("EndGame", (data) =>
            {
                binding.IsOnlineWin = 1;
                message = ((Newtonsoft.Json.Linq.JObject)data)["message"].ToString();
                binding.TextChat = message;
            });
        }
        #endregion

        #region Menu Item
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            board.newGame();
        }

        private void m1_Click(object sender, RoutedEventArgs e)
        {
            m1.IsChecked = true;

            if (m2 != null)
                m2.IsChecked = false;
            if (m3 != null)
                m3.IsChecked = false;
            if (m4 != null)
                m4.IsChecked = false;

            if (Board.PropertyMode != 1 && board != null)
                board.newGame();
            Board.PropertyMode = 1;

            if (socket != null)
                socket.Close();

            btnchatbox.IsEnabled = false;
            btnconnect.IsEnabled = false;
            btnusername.IsEnabled = false;
            menu_namegame.IsEnabled = true;
        }

        private void m2_Click(object sender, RoutedEventArgs e)
        {
            m2.IsChecked = true;
            if (m1 != null)
                m1.IsChecked = false;
            if (m3 != null)
                m3.IsChecked = false;
            if (m4 != null)
                m4.IsChecked = false;

            if (Board.PropertyMode != 2 && board != null)
                board.newGame();
            Board.PropertyMode = 2;
            if (socket != null)
                socket.Close();

            btnchatbox.IsEnabled = false;
            btnconnect.IsEnabled = false;
            btnusername.IsEnabled = false;
            menu_namegame.IsEnabled = true;
        }

        private void m3_Click(object sender, RoutedEventArgs e)
        {
            m3.IsChecked = true;
            if (m1 != null)
                m1.IsChecked = false;
            if (m2 != null)
                m2.IsChecked = false;
            if (m4 != null)
                m4.IsChecked = false;

            if (Board.PropertyMode != 3 && board != null)
            {
                myWorker.RunWorkerAsync();
                board.newGame();
            }
                            
            Board.PropertyMode = 3;
            btnchatbox.IsEnabled = true;
            btnconnect.IsEnabled = true;
            btndisconnect.IsEnabled = true;
            btnusername.IsEnabled = true;
            menu_namegame.IsEnabled = false;
        }

        private void m4_Click(object sender, RoutedEventArgs e)
        {
            m4.IsChecked = true;
            if (m1 != null)
                m1.IsChecked = false;
            if (m2 != null)
                m2.IsChecked = false;
            if (m3 != null)
                m3.IsChecked = false;

            if (Board.PropertyMode != 4 && board != null)
            {
                myWorker.RunWorkerAsync();
                board.newGame();
            }

            Board.PropertyMode = 4;
            btnchatbox.IsEnabled = true;
            btnconnect.IsEnabled = true;
            btndisconnect.IsEnabled = true;
            btnusername.IsEnabled = true;
            menu_namegame.IsEnabled = false;
        }
        #endregion

        #region text_changed
        private void txtpos_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((int)binding.Pos.X == -1)
                 return;

            board.setStep((int)binding.Pos.X, (int)binding.Pos.Y);

            if (Board.PropertyMode == 4)
            {
                board.AIplay();
                return;
            }          
        }

        private void txtwin_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (binding.IsOnlineWin == 0)
                return;

            board.onlineHighLight();

            if (binding.IsOnlineWin == 1)
                btnnewgame.IsEnabled = true;
        }

        private void txtchat_TextChanged(object sender, TextChangedEventArgs e)
        {
            string[] tokens = txtchat.Text.Split(new[] { "<next>" }, StringSplitOptions.None);
            chatbox.set(name, tokens[0]);         
        }
        #endregion

        #region button
        private void btnchatbox_Click(object sender, RoutedEventArgs e)
        {
            string text = chatbox.TextChat;
            //((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!"
            //socket.Emit("ChatMessage", JObject.FromObject());
            //MainWindow.socket.Emit("MyStepIs", JObject.FromObject(new { row = x, col = y }));
            socket.Emit("ChatMessage", message = text);
            chatbox.typebox.Clear();
        }

        private void btnusername_Click(object sender, RoutedEventArgs e)
        {
            namePlayer = chatbox.txtusername.Text;
            socket.Emit("MyNameIs", namePlayer);
        }

        private void btnconnect_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
            {
                socket.Emit("message", "Waiting!");
                btnconnect.IsEnabled = false;
            }          
            //btndisconnect.IsEnabled = true;
        }

        private void btnnewgame_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
            {
                socket.Disconnect();
                socket.Emit("MyNameIs", namePlayer);
                socket.Emit("ConnectToOtherPlayer");
            }

            board.newGame();
            btnconnect.IsEnabled = true;
            setDefaultParameter();            
            myWorker.RunWorkerAsync();
            btnnewgame.IsEnabled = false;
        }

        private void btndisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (socket != null)
            {
                socket.Disconnect();
                btnconnect.IsEnabled = false;
                btndisconnect.IsEnabled = false;
                btnusername.IsEnabled = false;
                btnnewgame.IsEnabled = false;
                btnchatbox.IsEnabled = false;
                m3.IsChecked = false;
                m4.IsChecked = false;

                Board.PropertyMode = -1;

                setDefaultParameter();
             
                socket = null;
            }
        }
        #endregion

        void setDefaultParameter()
        {
            Point temp = new Point(-1, -1);
            binding.Pos = temp;
            binding.IsOnlineWin = 0;
            binding.TextChat = String.Empty;
            isCheckTurn = false;
            connected = false;
        }

    }
}
