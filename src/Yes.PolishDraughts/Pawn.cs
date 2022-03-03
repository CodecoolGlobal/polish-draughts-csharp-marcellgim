using System;
using System.Drawing;

namespace yes_polish_draughts
{
    internal class Pawn
    {
        public int Color { get; }
        public (int x, int y) Coordinates { get; set; }
        public bool IsCrowned { get; set; }


        public Pawn(int color, (int, int) coordinates)
        {
            Color = color;
            Coordinates = coordinates;
            IsCrowned = false;
        }
    }
}
