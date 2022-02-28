﻿using System;
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
            Fields = CreateBoard(size);
        }

        private Pawn[,] CreateBoard(int size)
        {
            var board = new Pawn[size, size];
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    if ((row + col) % 2 == 1)
                    {
                        if (row < 4)
                        {
                            board[row, col] = new Pawn(1, (row, col));
                        }
                        else if (row >= size - 4)
                        {
                            board[row, col] = new Pawn(0, (row, col));
                        }
                    }

                }
            }
            return board;
        }
        public override string ToString()
        {
            string boardString = "  ";
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int col = 0; col < Fields.GetLength(1); col++)
            {
                boardString += $" {alpha[col].ToString()}";
            }
            boardString += "\n";
            for (int row = 0; row < Fields.GetLength(0); row++)
            {
                boardString += (row+1).ToString();
                boardString += " ";
                if (row < 9)
                {
                    boardString += " ";
                }
                for (int col = 0; col < Fields.GetLength(1); col++)
                {
                    if (Fields[row, col] == null)
                    {
                        boardString += "  ";
                    }
                    else
                    {
                        if (Fields[row, col].Color == 0)
                        {
                            boardString += "><";
                        }
                        if (Fields[row, col].Color == 1)
                        {
                            boardString += "()";
                        }
                    }
                }
                boardString += "\n";
            }
            return boardString;
        }
    }
}
