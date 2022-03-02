using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yes_polish_draughts
{
    internal class Board
    {
        public Pawn?[,] Fields { get; set; }
        public List<Pawn>[] Pawns { get; set; } = new List<Pawn>[2] { new List<Pawn>(), new List<Pawn>() };
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
                            Pawn newPawn = new Pawn(1, (row, col));
                            board[row, col] = newPawn;
                            Pawns[1].Add(newPawn);
                        }
                        else if (row >= size - 4)
                        {
                            Pawn newPawn = new Pawn(0, (row, col));
                            board[row, col] = newPawn;
                            Pawns[0].Add(newPawn);
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
            boardString += "   ┌";
            for (int i = 0; i < Fields.GetLength(1)-1; i++)
            {
                boardString += "───┬";
            }
            boardString += "───┐";
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
                    boardString += "│ ";
                    if (Fields[row, col] != null)
                    {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        if (Fields[row, col].Color == 0)
                        {
                            boardString += "●";
                        }
                        if (Fields[row, col].Color == 1)
                        {
                            boardString += "○";
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    else
                    {
                        boardString += " ";
                    }
                    boardString += " ";
                }
                boardString += "│";
                boardString += "\n";
                boardString += "   ";
                
                if (row < Fields.GetLength(0)-1)
                {
                    boardString += "├";
                    for (int i = 0; i < Fields.GetLength(1) - 1; i++)
                    {
                        boardString += "───┼";
                    }
                    boardString += "───┤";
                    boardString += "\n";
                }
            }
            boardString += "└";
            for (int i = 0; i < Fields.GetLength(1) - 1; i++)
            {
                boardString += "───┴";
            }
            boardString += "───┘";
            return boardString;
        }
        public void RemovePawn(Pawn movedPawn)
        {
            Fields[movedPawn.Coordinates.x, movedPawn.Coordinates.y] = null;
        }
        public void MovePawn(Pawn movedPawn, (int? x , int? y) endPosition)
        {
            if (endPosition.Item1 != null && endPosition.Item2 != null)
            {
                (int x, int y) startPos = movedPawn.Coordinates;
                (int x, int y) endPos = ((int)endPosition.x, (int)endPosition.y);
                if (Fields[startPos.Item1, startPos.Item2] != null)
                {
                    Fields[startPos.x, startPos.y] = null;
                    Fields[endPos.x, endPos.y] = movedPawn;
                    movedPawn.Coordinates = endPos;
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
    }
}
