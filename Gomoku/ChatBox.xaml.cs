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

        public void send()
        {
            if (typebox.Text == null)
                return;

            String user = "YOU" + "\t" + DateTime.Now.ToString() + "\n";

            String content = chatbox.Text.ToString() + "\n";

            content += user;
            content += typebox.Text.ToString();
            content += "\n..............................................................................\n";
            chatbox.Text = content;
            typebox.Clear();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            send();
        }
    }
}
