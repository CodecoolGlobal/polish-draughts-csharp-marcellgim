using System;

namespace yes_polish_draughts
{
    internal class Game
    {
        private Board gameBoard;
        private int player = 0;
        private int opponent = 1;
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
            Console.Clear();
            Console.WriteLine(gameBoard);
            GetCoordinateInput();
            // Change roles
            player = (player + 1) % 2;
            opponent = (opponent + 1) % 2;
        }
        private bool CheckForWinner()
        {
            return false;
        }
        private (int, int) GetCoordinateInput()
        {
            (int, int)? coords = null;
            string input;
            do
            {
                Console.WriteLine("Enter coordinates for the piece you want to move:");
                input = Console.ReadLine() ?? String.Empty;
                if (input.Length >= 2 && Char.IsLetter(input[0]) && int.TryParse(input.Substring(1), out _))
                {
                    int coorX = Char.ToLower(input[0]) - 'a';
                    int coorY = int.Parse(input.Substring(1));
                    coords = (coorX, coorY);
                }
            } while ( coords == null );
            return ((int, int))coords;
        }
    }
}
