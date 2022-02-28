using System;

namespace yes_polish_draughts
{
    internal class Game
    {
        private Board gameBoard;
        private int player = 0;
        public void Start()
        {
            int boardSize;
            string input;
            do
            {
                Console.WriteLine("Enter board size (10-20):");
                input = Console.ReadLine() ?? String.Empty;

            } while (!int.TryParse(input, out boardSize) || boardSize > 20 || boardSize < 10);

            gameBoard = new Board(boardSize);
            player = 0;
            do
            {
                Round();
            } while (!CheckForWinner());
        }
        private void Round()
        {
            // Next player
            player = (player + 1) % 2;
        }
        private bool CheckForWinner()
        {
            return false;
        }
    }
}
