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
    /// Interaction logic for ChatBox.xaml
    /// </summary>
    public partial class ChatBox : UserControl
    {
        public ChatBox()
        {
            InitializeComponent();
        }

        string _textChat;
        public string TextChat
        {
            get { return _textChat; }
            set { _textChat = value; }
        }

        /*public void send()
        {
            if (typebox.Text == null)
                return;

            String user = "YOU" + "\t\t\t"  + DateTime.Now.ToString("hh:mm:ss") + "\n";

            String content = chatbox.Text.ToString() + "\n";

            content += user;
            content += typebox.Text.ToString();
            content += "\n................................................................................\n";
            chatbox.Text = content;
            typebox.Clear();
            
        }*/

        public void set(string name, string text)
        {
            string user = name;
            for (int i = name.Length; i < 45; i++)
            {
                user += ' ';
            }
            user += DateTime.Now.ToString("hh:mm:ss") + "\n";

            string content = chatbox.Text.ToString() + "\n";

            content += user;
            content += text;
            content += "\n................................................................................\n";
            chatbox.Text = content;
        }

        private void typebox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _textChat = typebox.Text;
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (sv.VerticalOffset == sv.ScrollableHeight)
            {
                sv.ScrollToEnd();//debug breakpoint
            }
        }
    }
}
