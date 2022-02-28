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
            if (Math.Abs(moveVector.x) != Math.Abs(moveVector.y))
                return false;

        }
    }
}
