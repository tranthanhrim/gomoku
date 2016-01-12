using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{
    public class Player
    {
        static int height = 12;
        static int width = 12;

        public int PropertyHeight
        {
            get { return height; }
            set { height = value; }
        }

        public int PropertyWidth
        {
            get { return width; }
            set { width = value; }
        }

        public int[,] play = new int[12,12];

        public void mark(int x, int y)
        {
            play[x, y] = 1;
        }

        void backup()
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (play[i, j] >= 1)
                        play[i, j] = 1;
        }


        public int check(int x, int y)
        {
            int countVer = 0;
            int countHor = 0;
            int countDiag1 = 0;
            int countDiag2 = 0;

            int row = x;
            while (true)
            {
                if (row > height - 1)
                    break;
                if (play[row, y] == 1)
                {
                    countVer++;
                    play[row, y] = 2;
                    row++;
                }
                else
                    break;               
            }

            row = x - 1;
            while(true)
            {
                if (row < 0)
                    break;

                if (play[row, y] == 1)
                {
                    countVer++;
                    play[row, y] = 2;
                    row--;                   
                }
                else
                    break;      
            }

            if (countVer >= 5)
                return 2;
            backup();

            int col = y;
            while (true)
            {
                if (col > width - 1)
                    break;
                if (play[x, col] == 1)
                {
                    countHor++;
                    play[x, col] = 3;
                    col++;
                    
                }
                else
                    break;
            }

            col = y - 1;
            while (true)
            {
                if (col < 0)
                    break;
                if (play[x, col] == 1)
                {
                    countHor++;
                    play[x, col] = 3;
                    col--;                  
                }
                else
                    break;
            }
            if (countHor >= 5)
                return 3;
            backup();

            row = x;
            col = y;
            while (true)
            {
                if (row < 0 || col > width - 1)
                    break;
                if (play[row, col] == 1)
                {
                    countDiag1++;
                    play[row, col] = 4;
                    row--;
                    col++;                  
                }
                else
                    break;
            }

            row = x + 1;
            col = y - 1;
            while (true)
            {
                if (row > height - 1 || col < 0)
                    break;
                if (play[row, col] == 1)
                {
                    countDiag1++;
                    play[row, col] = 4;
                    row++;
                    col--;                   
                }
                else
                    break;
            }
            if (countDiag1 >= 5)
                return 4;
            backup();


            row = x;
            col = y;
            while (true)
            {
                if (row < 0 || col < 0)
                    break;
                if (play[row, col] == 1)
                {
                    countDiag2++;
                    play[row, col] = 5;
                    row--;
                    col--;
                }
                else
                    break;
            }

            row = x + 1;
            col = y + 1;
            while (true)
            {
                if (row > height - 1 || col > width - 1)
                    break;
                if (play[row, col] == 1)
                {
                    countDiag2++;
                     play[row, col] = 5;
                    row++;
                    col++;

                }
                else
                    break;
            }
            if (countDiag2 >= 5)
                return 5;
            backup();

            return 0;
        }     
    }
}
