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
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;

namespace Gomoku
{
    public class Connect
    {
        public Socket connect()
        {
            var socket = IO.Socket("ws://gomoku-lajosveres.rhcloud.com:8000");
            bool _connected = false;
            while (_connected == false)
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                //Console.WriteLine("Connected");
                _connected = true;
            });

            socket.On("ChatMessage", (data) =>
            {
                Console.WriteLine(data);
                if (((Newtonsoft.Json.Linq.JObject)data)["message"].ToString() == "Welcome!")
                {
                    socket.Emit("MyNameIs", "my_PC");
                    socket.Emit("ConnectToOtherPlayer");
                    //Console.ReadKey(intercept: true);         
                }

            });

            return socket;
        }
    }
}
