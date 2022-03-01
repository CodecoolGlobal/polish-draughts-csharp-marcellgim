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
                validEndCoordinate = ValidateMove(((int, int))validatedStartCoordinate, inputCoordinate);
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
                gameBoard.Fields[inputCoordinate.Item1, inputCoordinate.Item2] != null &&
                gameBoard.Fields[inputCoordinate.Item1, inputCoordinate.Item2].Color == player;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
                    int coorY = Char.ToLower(input[0]) - 'a';
                    int coorX = int.Parse(input.Substring(1)) - 1;

                    coords = (coorX, coorY);
                }
                if (coords == null)
                {
                    Console.WriteLine("Invalid coordinate format. Please try again:");
                }
            } while ( coords == null );
            return ((int, int))coords;
        }
        private bool ValidateMove((int x, int y)validStartPos, (int x, int y)endPos)
        {
            // End position is in bounds
            if (!gameBoard.IsInBounds(endPos))
                return false;
            // All single moves are diagonal
            (int x, int y) moveVector = (validStartPos.x - endPos.x, validStartPos.y - endPos.y);
            if (Math.Abs(moveVector.x) != Math.Abs(moveVector.y))
                return false;
            // Pawn cannot move to an already occupied field
            if (gameBoard.Fields[endPos.x, endPos.y] != null)
                return false;
            // Uncrowned pieces cannot move more than 2 spaces vertically
            Pawn piece = gameBoard.Fields[validStartPos.x, validStartPos.y];
            if (!piece.IsCrowned && Math.Abs(moveVector.x) > 2)
                return false;
            // Uncrowned pieces can only move forward
            if (!piece.IsCrowned &&
                ((piece.Color == 0 && moveVector.x <= -1) || (piece.Color == 1 && moveVector.x >= 1)))
            {
                return false;
            }
            // Check other pieces traversed - one enemy piece and zero own pieces can be traversed
            (int x, int y) unitVector = (moveVector.x / Math.Abs(moveVector.x), moveVector.y / Math.Abs(moveVector.y));
            bool pieceTraversed = false;
            for (int moved = 1; moved < moveVector.x; moved++)
            {
                Pawn? targetField = gameBoard.Fields[validStartPos.x + moved * unitVector.x, validStartPos.y + moved * unitVector.y];
                if (targetField != null)
                {
                    if (targetField.Color == player || pieceTraversed)
                        return false;

                    pieceTraversed = true;
                }
            }
            // Move complies with all rules, return true
            return true;
        }
    }
}
