using System;

namespace yes_polish_draughts
{
    internal class Game
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Board gameBoard;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
            bool validStartCoordinate = false;
            bool validEndCoordinate = false;
            (int?, int?) validatedStartCoordinate = (null, null);
            (int?, int?) validatedEndCoordinate = (null, null);
            Console.Clear();
            Console.WriteLine(gameBoard);
            while (!validStartCoordinate)
            {
                Console.WriteLine("Enter coordinates for the piece you want to move:");
                (int, int) inputCoordinate = GetCoordinateInput();
                validStartCoordinate = ValidateStartCoordinate(inputCoordinate, player);
                if (validStartCoordinate)
                {
                    validatedStartCoordinate = inputCoordinate;
                }
            }

            while (!validEndCoordinate)
            {
                Console.WriteLine("Enter coordinates where you want to move that piece:");
                (int, int) inputCoordinate = GetCoordinateInput();
                validEndCoordinate = ValidateMove(validatedStartCoordinate, inputCoordinate);
                if (validEndCoordinate)
                {
                    validatedEndCoordinate = inputCoordinate;
                }
            }
            gameBoard.MovePawn(validatedStartCoordinate, validatedEndCoordinate);
            // Change roles
            player = (player + 1) % 2;
            opponent = (opponent + 1) % 2;
        }

        private bool ValidateStartCoordinate((int, int) inputCoordinate, int player)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return gameBoard.IsInBound(inputCoordinate) &&
                gameBoard.Fields[inputCoordinate.Item2, inputCoordinate.Item1] != null &&
                gameBoard.Fields[inputCoordinate.Item2, inputCoordinate.Item1].Color == player;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        private bool ValidateMove((int?, int?) inputCoordinate, (int?, int?) startCoordinate)
        {
            return false;
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
                
                input = Console.ReadLine() ?? String.Empty;
                if (input.Length >= 2 && Char.IsLetter(input[0]) && int.TryParse(input.Substring(1), out _))
                {
                    int coorX = Char.ToLower(input[0]) - 'a';
                    int coorY = int.Parse(input.Substring(1)) - 1;
                    coords = (coorX, coorY);
                }
            } while ( coords == null );
            return ((int, int))coords;
        }
    }
}
