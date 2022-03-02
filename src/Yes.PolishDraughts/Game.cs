﻿using System;

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
            (int x, int y) validatedStartCoordinate;
            (int x, int y) validatedEndCoordinate;
            Pawn? movedPawn = null;
            Console.Clear();
            Console.WriteLine(gameBoard);
            List<List<(int x, int y)>> possibleSequences;

            if (CanPlayerJump(player))
            {
                possibleSequences = PossibleJumpMoves();
            }
            else
            {
                possibleSequences = AllPawnMoves(player);
            }

            List<(int,int)> possibleStarts = StartPositionCoords(possibleSequences);
            Console.WriteLine("You can move the following pawns:");
            OutputCoords(possibleStarts);

            while (!validStartCoordinate)
            {
                Console.WriteLine("Enter coordinates for the piece you want to move:");
                (int, int) inputCoordinate = GetCoordinateInput();
                if (possibleStarts.Contains(inputCoordinate))
                {
                    validStartCoordinate = true;
                    validatedStartCoordinate = inputCoordinate;
                    movedPawn = gameBoard.Fields[validatedStartCoordinate.x, validatedStartCoordinate.y];
                }
            }


            while (!validEndCoordinate)
            {
                Console.WriteLine("Enter coordinates where you want to move that piece:");
                (int, int) inputCoordinate = GetCoordinateInput();
                validEndCoordinate = ValidateMove(movedPawn.Coordinates, inputCoordinate);
                if (validEndCoordinate)
                {
                    validatedEndCoordinate = inputCoordinate;
                    gameBoard.MovePawn(movedPawn, validatedEndCoordinate);
                }
            }
            // Change roles
            player = (player + 1) % 2;
            opponent = (opponent + 1) % 2;
        }

        private bool ValidateStartCoordinate((int, int) inputCoordinate, int player)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            bool isOwn = gameBoard.IsInBound(inputCoordinate) &&
                gameBoard.Fields[inputCoordinate.Item1, inputCoordinate.Item2] != null &&
                gameBoard.Fields[inputCoordinate.Item1, inputCoordinate.Item2].Color == player;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (!isOwn)
            {
                return false;
            }
            else
            {
                Pawn movedPawn = gameBoard.Fields[inputCoordinate.Item1, inputCoordinate.Item2];
                if (CanPlayerJump(player))
                {
                    if (!CanPawnJump(movedPawn))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else if (CanPawnMove(movedPawn))
                {
                    return true;
                }
            }
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
            if (!gameBoard.IsInBound(endPos))
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
        private List<(int, int)> LongestJumpSequence(List<(int, int)> starterSequence)
        {
            (int x, int y) startPosition = starterSequence.Last();
            List<(int, int)> possibleJumps = PossibleJumps(startPosition);
            if (possibleJumps.Count > 0)
            {
                List<(int, int)>[] runoffSequences = new List<(int, int)>[possibleJumps.Count];
                for (int i = 0; i < possibleJumps.Count; i++)
                {
                    if (!starterSequence.Contains(possibleJumps[i]))
                    {
                        List<(int, int)> newSequence = new List<(int, int)>(starterSequence);
                        newSequence.Add(possibleJumps[i]);
                        runoffSequences[i] = LongestJumpSequence(newSequence);
                    }
                }
                if (Array.TrueForAll(runoffSequences, sequence => sequence != null))
                    return runoffSequences.OrderByDescending(sequence => sequence.Count).First();
                else
                    return starterSequence;
            }
            else
            {
                return starterSequence;
            }
        }
        private bool CanPawnJump(Pawn pawn)
        {
            List<(int, int)> possibleJumps = PossibleJumps(pawn.Coordinates);
            return possibleJumps.Count != 0;
        }

        private bool CanPawnMove(Pawn pawn)
        {
            List<(int, int)> possibleMoves = PossibleMoves(pawn);
            return possibleMoves.Count != 0;
        }

        private List<(int, int)> PossibleMoves (Pawn pawn)
        {
            (int x, int y) coordinate = pawn.Coordinates;
            List<(int, int)> result = new List<(int, int)>();
            (int, int)[] possibleCoords = { ((player * 2 - 1), 1), ((player * 2 - 1), -1) };
            foreach ((int row, int col) possibleCoord in possibleCoords)
            {
                (int, int) newCoord = (coordinate.x + possibleCoord.row, coordinate.y + possibleCoord.col);
                if (gameBoard.IsInBound(newCoord))
                {
                    Pawn? newField = gameBoard.Fields[newCoord.Item1, newCoord.Item2];
                    if (newField == null)
                    {
                        result.Add(newCoord);
                    }
                }
            }
            return result;
        }
        private bool CanPlayerJump(int player)
        {
            List<Pawn> playerPawns = gameBoard.Pawns[player];
            foreach ( Pawn pawn in playerPawns)
            {
                if (CanPawnJump(pawn))
                {
                    return true;
                }
            }
            return false;
        }
        private bool CanPlayerMove(int player)
        {
            List<Pawn> playerPawns = gameBoard.Pawns[player];
            foreach (Pawn pawn in playerPawns)
            {
                if (CanPawnMove(pawn))
                {
                    return true;
                }
            }
            return false;
        }
        private List<List<(int, int)>> AllPawnMoves(int player)
        {
            List<Pawn> playerPawns = gameBoard.Pawns[player];
            var allMoves = new List<List<(int, int)>>();
            foreach (Pawn pawn in playerPawns)
            {
                if (CanPawnMove(pawn))
                {
                    allMoves.Add(PossibleMoves(pawn));
                }
            }
            return allMoves;
        }

        private List<(int, int)> PossibleJumps((int x, int y) coordinate)
        {
            List<(int, int)> result = new List<(int, int)>();
            (int, int)[] possibleCoords = { (-2, -2), (2, -2), (-2, 2), (2, 2)};
            foreach ((int row, int col) possibleCoord in possibleCoords)
            {
                (int, int) newCoord = (coordinate.x + possibleCoord.row, coordinate.y + possibleCoord.col);
                (int, int) newJumpedCoord = (coordinate.x + (possibleCoord.row / 2), coordinate.y + (possibleCoord.col / 2));
                if (gameBoard.IsInBound(newCoord)) {
                    Pawn? newjumpedField = gameBoard.Fields[newJumpedCoord.Item1, newJumpedCoord.Item2];
                    Pawn? newField = gameBoard.Fields[newCoord.Item1, newCoord.Item2];
                    if (newjumpedField != null &&
                        newjumpedField.Color == opponent &&
                        newField == null)
                    {
                        result.Add(newCoord);
                    }
                }
            }
            return result;
        }

        private void OutputCoords (List<(int, int)> coordinates)
        {
            string outputString = "";
            foreach ((int x, int y) coordinate in coordinates)
            {
                outputString += (char)(coordinate.x + 'a');
                outputString += coordinate.y + 1;
                outputString += " ";
            }
            Console.WriteLine(outputString);
        }

        private List<(int, int)> StartPositionCoords(List<List<(int, int)>> sequences)
        {
            List<(int, int)> result = new List<(int, int)> ();
            foreach (List<(int, int)> sequence in sequences)
            {
                result.Add(sequence[0]);
            }
            return result;
        }

        private List<(int, int)> EndPositionCoords(List<List<(int, int)>> sequences)
        {
            List<(int, int)> result = new List<(int, int)>();
            foreach (List<(int, int)> sequence in sequences)
            {
                result.Add(sequence[sequence.Count - 1]);
            }
            return result;
        }
        private void ExecuteJumpSequence(List<(int x, int y)> jumpSequence)
        {
            Pawn movedPawn = gameBoard.Fields[jumpSequence[0].x, jumpSequence[0].y];
            jumpSequence.RemoveAt(0);
            foreach ((int x, int y) jump in jumpSequence)
            {
                gameBoard.MovePawn(movedPawn, jump);

                (int x, int y) moveVector = (jump.x - movedPawn.Coordinates.x, jump.y - movedPawn.Coordinates.y);
                (int x, int y)unitVector = (moveVector.x / Math.Abs(moveVector.x), moveVector.y / Math.Abs(moveVector.y));
                // Since all moves are perfectly diagonal, any dimension can be used to determine jump length
                int jumpLength = Math.Abs(moveVector.x);
                for (int move = 1; move < jumpLength; move++)
                {
                    (int x, int y) currentField = (movedPawn.Coordinates.x + (unitVector.x * move),
                                                movedPawn.Coordinates.y + (unitVector.y * move));
                    if (gameBoard.Fields[currentField.x, currentField.y] is Pawn)
                    {
                        gameBoard.RemovePawn(gameBoard.Fields[currentField.x, currentField.y]);
                        break;
                    }
                }
            }
        }
        private List<List<(int x, int y)>> PossibleJumpMoves()
        {
            List<List<(int, int)>> longestSequences = new List<List<(int, int)>>();
            foreach (Pawn pawn in gameBoard.Pawns[player])
            {
                if (CanPawnJump(pawn))
                {
                    longestSequences.Add(LongestJumpSequence(new List<(int, int)> { (pawn.Coordinates.x, pawn.Coordinates.y) }));
                }
            }
            var validMoves =
                    (from sequence in longestSequences
                    where sequence.Count == longestSequences.Max(sequence => sequence.Count)
                    select sequence).ToList();
            return validMoves;
        }
    }
}
