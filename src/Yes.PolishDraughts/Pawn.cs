using System;
using System.Drawing;

namespace yes_polish_draughts
{
    internal class Pawn
    {
        public int Color { get; }
        public (int x, int y) Coordinates { get; set; }
        public bool IsCrowned { get; private set; }


        public Pawn(int color, (int, int) coordinates)
        {
            Color = color;
            Coordinates = coordinates;
        }

        public bool TryToMakeMove((int x, int y) targetCoords)
        {
            (int x, int y) moveVector = (targetCoords.x - Coordinates.x, targetCoords.y - Coordinates.y);
            // All single moves are diagonal
            if (Math.Abs(moveVector.x) != Math.Abs(moveVector.y))
                return false;
            // Uncrowned pieces cannot move more than 2 spaces vertically
            else if (!IsCrowned && Math.Abs(moveVector.x) > 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
