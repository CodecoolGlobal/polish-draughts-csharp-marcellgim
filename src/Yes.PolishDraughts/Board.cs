﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yes_polish_draughts
{
    internal class Board
    {
        public Pawn?[,] Fields { get; set; }
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
            string boardString = "   ";
            char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
            for (int col = 0; col < Fields.GetLength(1); col++)
            {
                boardString += $"  {alpha[col].ToString()} ";
            }
            boardString += "\n";
            boardString += "   ";
            boardString += new string('-', (Fields.GetLength(1) * 4) + 1);
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
                    boardString += "| ";
                    if (Fields[row, col] != null)
                    {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (Fields[row, col].Color == 0)
                        {
                            boardString += "X";
                        }
                        if (Fields[row, col].Color == 1)
                        {
                            boardString += "O";
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    else
                    {
                        boardString += ".";
                    }
                    boardString += " ";
                }
                boardString += "|";
                boardString += "\n";
                boardString += "   ";
                boardString += new string('-', (Fields.GetLength(1)*4)+1);
                boardString += "\n";
            }
            return boardString;
        }
        public void RemovePawn((int, int) position)
        {
            Fields[position.Item1, position.Item2] = null;
        }
        public void MovePawn((int?, int?) startPosition, (int?, int?) endPosition)
        {
            if (startPosition.Item1 != null && startPosition.Item2 != null && endPosition.Item1 != null && endPosition.Item2 != null)
            {
                (int, int) startPos = ((int)startPosition.Item1, (int)startPosition.Item2);
                (int, int) endPos = ((int)endPosition.Item1, (int)endPosition.Item2);
                if (Fields[startPos.Item1, startPos.Item2] != null)
                {
                    Pawn? movedPawn = Fields[startPos.Item1, startPos.Item2];
                    Fields[startPos.Item1, startPos.Item2] = null;
                    Fields[endPos.Item1, endPos.Item2] = movedPawn;
                }
            }
        }
        public bool IsInBound((int, int)inputCoordinate)
        {
            return inputCoordinate.Item1 >= 0 &&
                inputCoordinate.Item2 >= 0 &&
                Fields.GetLength(0) > inputCoordinate.Item1 &&
                Fields.GetLength(1) > inputCoordinate.Item2;
        }
        public bool IsInBounds((int x, int y)coords)
        {
            return coords.x < Fields.GetLength(0) && coords.y < Fields.GetLength(1);
        }
    }
}
