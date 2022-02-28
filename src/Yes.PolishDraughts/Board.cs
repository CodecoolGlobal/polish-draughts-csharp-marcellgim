using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yes_polish_draughts
{
    internal class Board
    {
        public Pawn[,] Fields { get; set; }
        public Board(int size)
        {
            Fields = new Pawn[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if ((row*size+col) % 2 == 1)
                    {
                        if (row < 4)
                        {
                            Fields[row, col] = new Pawn(1, (row, col));
                        }
                        else if (row >= size - 4) {
                            Fields[row, col] = new Pawn(0, (row, col));
                        }
                    }
                    
                }
            }
        }
    }
}
