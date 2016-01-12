using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace Gomoku
{
    public class DataBinding : INotifyPropertyChanged
    {
        Point _pos;

        int _isOnlineWin;

        string _textChat;

        string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public string TextChat
        {
            get { return _textChat; }
            set { _textChat = value; OnPropertyChanged("TextChat"); }
        }



        public int IsOnlineWin
        {
            get { return _isOnlineWin; }
            set { _isOnlineWin = value; OnPropertyChanged("IsOnlineWin"); }
        }


        public Point Pos
        {
            get { return _pos; }
            set { _pos = value; OnPropertyChanged("Pos"); }
        }

        public DataBinding()
        {
            _pos = new Point();
            _pos.X = -1;
            _pos.Y = -1;
            _isOnlineWin = 0;
            _textChat = String.Empty;
            _name = String.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
