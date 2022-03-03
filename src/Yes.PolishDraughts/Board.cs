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
                        int rowCount = size < 10 ? 1 : 4;
                        if (row < rowCount)
                        {
                            Pawn newPawn = new Pawn(1, (row, col));
                            board[row, col] = newPawn;
                            Pawns[1].Add(newPawn);
                        }
                        else if (row >= size - rowCount)
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
        public string ToString(List<(int, int)>? possibleCoordinates)
        {
            // ◆◇◌▒◔◕
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
                            if (possibleCoordinates.Contains((row, col)))
                            {
                                if (Fields[row, col].IsCrowned)
                                {
                                    boardString += "◈";
                                }
                                else
                                {
                                    boardString += "◉";
                                }
                            }
                            else
                            {
                                if (Fields[row, col].IsCrowned)
                                {
                                    boardString += "◆";
                                }
                                else
                                {
                                    boardString += "●";
                                }
                            }
                            
                        }
                        if (Fields[row, col].Color == 1)
                        {
                            if (possibleCoordinates.Contains((row, col)))
                            {
                                if (Fields[row, col].IsCrowned)
                                {
                                    boardString += "⟐";
                                }
                                else
                                {
                                    boardString += "◎";
                                }
                            }
                            else
                            {
                                if (Fields[row, col].IsCrowned)
                                {
                                    boardString += "◇";
                                }
                                else
                                {
                                    boardString += "○";
                                }
                            }
                        }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    }
                    else
                    {
                        if (possibleCoordinates.Contains((row, col)))
                        {
                            boardString += "◌";
                        }
                        else
                        {
                            boardString += " ";
                        }
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
            Pawns[movedPawn.Color].Remove(movedPawn);
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
            if (movedPawn.Coordinates.x == 0 && movedPawn.Color == 0)
            {
                movedPawn.IsCrowned = true;
            }
            if (movedPawn.Coordinates.x == Fields.GetLength(0) - 1 && movedPawn.Color == 1)
            {
                movedPawn.IsCrowned = true;
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
