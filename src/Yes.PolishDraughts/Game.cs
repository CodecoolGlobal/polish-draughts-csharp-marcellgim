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
        private bool isAI = false;
        int speed = 0;
        public void Start()
        {
            newGame:
            player = 0;
            opponent = 1;
            isAI = false;
            speed = 0;
            int boardSize;
            string input;
            do
            {
                Console.WriteLine("Enter board size (4-20):");
                input = Console.ReadLine() ?? String.Empty;

            } while (!int.TryParse(input, out boardSize) || boardSize > 20 || boardSize < 4);
            
            string inputVersion;
            int version;
            do
            {
                Console.WriteLine("Enter version:\n0 - Normal (Human vs Human)\n1 - Demo (AI vs AI)");
                inputVersion = Console.ReadLine() ?? String.Empty;

            } while (!int.TryParse(inputVersion, out version) || version > 1 || version < 0);
            if (version == 1) isAI = true;

            if (isAI)
            {
                string inputSpeedVersion;
                int speedVersion;
                do
                {
                    Console.WriteLine("Enter speed:\n0 - Normal (2 steps/second)\n1 - Fast (100 steps/second)");
                    inputSpeedVersion = Console.ReadLine() ?? String.Empty;

                } while (!int.TryParse(inputSpeedVersion, out speedVersion) || speedVersion > 1 || speedVersion < 0);
                speed = speedVersion == 0 ? 500 : 10;
            }
            
            gameBoard = new Board(boardSize);
            player = 0;
            do
            {
                Round();
            } while (!CheckForWinner());
            Console.Clear();
            Console.WriteLine(gameBoard.ToString(new List<(int, int)> { (-1, -1) }));
            string winner = opponent == 0 ? "White" : "Black";
            Console.WriteLine($"{winner} won the game!");
            Console.WriteLine("New game? (Y/N)");
            string answer = Console.ReadLine() ?? String.Empty;
            if (answer.ToLower() == "y")
            {
                Console.Clear();
                goto newGame;
            }
        }
        private void Round()
        {
            Undo:
            bool validStartCoordinate = false;
            bool validEndCoordinate = false;
            (int x, int y) validatedStartCoordinate = (-1, -1);
            (int x, int y) validatedEndCoordinate = (-1, -1);
            Pawn? movedPawn = null;
            

            List<(int, int)> possibleStarts;
            List<(int, int)> possibleEnds;
            List<List<(int x, int y)>> possibleJumps = PossibleJumpMoves();
            bool forceJump = CanPlayerJump();

            if (forceJump)
            {
                possibleStarts = StartPositionCoords(possibleJumps);
            }
            else
            {
                possibleStarts = AllMoveablePawnCoordinates();
            }
            Console.Clear();
            
            if (!isAI)
            {
                Console.WriteLine(gameBoard.ToString(possibleStarts));
                Console.WriteLine("You can move the following pawns:");
                OutputCoords(possibleStarts);
            }
            else
            {
                Console.WriteLine(gameBoard.ToString(new List<(int, int)> { (-1, -1) }));
            }

            while (!validStartCoordinate)
            {
                if (!isAI)
                {
                    Console.WriteLine("Enter coordinates for the piece you want to move:");
                }
                int randomStart = new Random().Next(possibleStarts.Count);
                (int, int) inputCoordinate = GetCoordinateInput(possibleStarts[randomStart]);
                if (inputCoordinate == (-1, -1)) goto Undo;
                if (possibleStarts.Contains(inputCoordinate))
                {
                    validStartCoordinate = true;
                    validatedStartCoordinate = inputCoordinate;
                    movedPawn = gameBoard.Fields[validatedStartCoordinate.x, validatedStartCoordinate.y];
                }
            }


            if (forceJump)
            {
                possibleEnds = 
                    (from sequence in possibleJumps
                    where sequence.First() == validatedStartCoordinate
                    select sequence.Last()).ToList();
                if (!isAI)
                {
                    Console.Clear();
                    Console.WriteLine(gameBoard.ToString(possibleEnds));
                    Console.WriteLine("You can jump with this pawn to:");
                    OutputCoords(possibleEnds);
                }
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                possibleEnds = PossibleMoves(movedPawn);
#pragma warning restore CS8604 // Possible null reference argument.
                if (!isAI)
                {
                    Console.Clear();
                    Console.WriteLine(gameBoard.ToString(possibleEnds));
                    Console.WriteLine("You can move this pawn to:");
                    OutputCoords(possibleEnds);
                }
            }
            

            while (!validEndCoordinate)
            {
                if (!isAI)
                {
                    Console.WriteLine("Enter coordinates where you want to move that piece:");
                }
                int randomEnd = new Random().Next(possibleEnds.Count);
                (int, int) inputCoordinate = GetCoordinateInput(possibleEnds[randomEnd]);
                if (inputCoordinate == (-1, -1)) goto Undo;
                if (possibleEnds.Contains(inputCoordinate))
                {
                    validEndCoordinate = true;
                    validatedEndCoordinate = inputCoordinate;
                    if (!forceJump)
#pragma warning disable CS8604 // Possible null reference argument.
                        gameBoard.MovePawn(movedPawn, validatedEndCoordinate);
#pragma warning restore CS8604 // Possible null reference argument.
                    else
                    {
                        var chosenSequence =
                            (from sequence in possibleJumps
                            where sequence.Last() == validatedEndCoordinate
                            select sequence).First().ToList();
                        ExecuteJumpSequence(chosenSequence);
                    }
                }
            }
            // Change roles
            player = (player + 1) % 2;
            opponent = (opponent + 1) % 2;
        }
        private bool CheckForWinner()
        {
            return !CanPlayerJump() && !CanPlayerMove();
        }
        private (int, int) GetCoordinateInput((int, int) possibleCoord)
        {
            (int, int)? coords = null;
            string input;
            do
            {
                if (isAI)
                {
                    Thread.Sleep(speed);
                    return possibleCoord;
                }
                input = Console.ReadLine() ?? String.Empty;
                if (input == String.Empty) {
                    return possibleCoord;
                }
                if (input.ToLower() == "quit")
                    Environment.Exit(0);
                if (input.ToLower() == "undo")
                    return (-1, -1);
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
        private List<List<(int, int)>> LongestJumpSequence(List<(int, int)> starterSequence, Pawn jumper)
        {
            (int x, int y) startPosition = starterSequence.Last();
            List<(int, int)> possibleJumps = PossibleJumps(startPosition, jumper);
            if (possibleJumps.Count > 0)
            {
                List<List<(int, int)>> runoffSequences = new List<List<(int, int)>>();
                for (int i = 0; i < possibleJumps.Count; i++)
                {
                    if (!starterSequence.Contains(possibleJumps[i]))
                    {
                        List<(int, int)> newSequence = new List<(int, int)>(starterSequence);
                        newSequence.Add(possibleJumps[i]);
                        List<List<(int, int)>> newJumps = LongestJumpSequence(newSequence, jumper);
                        runoffSequences.AddRange(newJumps);
                    }
                }
                if (runoffSequences.Count > 0)
                    return
                        (from sequence in runoffSequences
                        where sequence.Count == runoffSequences.Max(sequence => sequence.Count)
                        select sequence).ToList();
                else
                    return new List<List<(int, int)>>() { starterSequence };
            }
            else
            {
                return new List<List<(int, int)>>() { starterSequence };
            }
        }
        private bool CanPawnJump(Pawn pawn)
        {
            List<(int, int)> possibleJumps = PossibleJumps(pawn.Coordinates, pawn);
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
            if (!pawn.IsCrowned)
            {
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
            }
            else
            {
                (int x, int y)[] unitVectors = { (-1, -1), (1, -1), (-1, 1), (1, 1) };
                foreach ((int x, int y) vector in unitVectors)
                {
                    bool lineBlocked = false;
                    for (int moves = 1; !lineBlocked; moves++)
                    {
                        (int x, int y) currentCoord = (coordinate.x + (moves * vector.x), coordinate.y + (moves * vector.y));
                        if (!gameBoard.IsInBound(currentCoord))
                            break;
                        else if (gameBoard.Fields[currentCoord.x, currentCoord.y] is Pawn)
                            lineBlocked = true;
                        else if (gameBoard.Fields[currentCoord.x, currentCoord.y] == null)
                        {
                            result.Add(currentCoord);
                        }
                    }
                }
            }
            return result;
        }
        private bool CanPlayerJump()
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
        private bool CanPlayerMove()
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
        private List<(int, int)> AllMoveablePawnCoordinates()
        {
            List<Pawn> playerPawns = gameBoard.Pawns[player];
            var allMoveables = new List<(int, int)>();
            foreach (Pawn pawn in playerPawns)
            {
                if (CanPawnMove(pawn))
                {
                    allMoveables.Add(pawn.Coordinates);
                }
            }
            return allMoveables;
        }

        private List<(int, int)> PossibleJumps((int x, int y) coordinate, Pawn jumper)
        {
            List<(int, int)> result = new List<(int, int)>();
            if (!jumper.IsCrowned)
            {
                (int, int)[] possibleCoords = { (-2, -2), (2, -2), (-2, 2), (2, 2) };
                foreach ((int row, int col) possibleCoord in possibleCoords)
                {
                    (int, int) newCoord = (coordinate.x + possibleCoord.row, coordinate.y + possibleCoord.col);
                    (int, int) newJumpedCoord = (coordinate.x + (possibleCoord.row / 2), coordinate.y + (possibleCoord.col / 2));
                    if (gameBoard.IsInBound(newCoord))
                    {
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
            }
            else
            {
                (int, int)[] unitVectors = { (-1, -1), (1, -1), (-1, 1), (1, 1) };
                foreach ((int x, int y) vector in unitVectors)
                {
                    int pawnsEncountered = 0;
                    for (int moves = 1; pawnsEncountered < 2; moves++)
                    {
                        (int x, int y) currentCoord = (jumper.Coordinates.x + (moves * vector.x), jumper.Coordinates.y + (moves * vector.y));
                        if (!gameBoard.IsInBound(currentCoord))
                            break;
                        if (gameBoard.Fields[currentCoord.x, currentCoord.y] is Pawn)
                        {
                            pawnsEncountered++;
                            if (gameBoard.Fields[currentCoord.x, currentCoord.y].Color == player)
                                break;
                        }
                        if (pawnsEncountered == 1 && gameBoard.Fields[currentCoord.x, currentCoord.y] == null)
                            result.Add(currentCoord);
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
                outputString += (char)(coordinate.y + 'a');
                outputString += coordinate.x + 1;
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
            return result.Distinct().ToList();
        }

        private void ExecuteJumpSequence(List<(int x, int y)> jumpSequence)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Pawn movedPawn = gameBoard.Fields[jumpSequence[0].x, jumpSequence[0].y];
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            jumpSequence.RemoveAt(0);
            bool firstJumpFinished = false;
            foreach ((int x, int y) jump in jumpSequence)
            {

#pragma warning disable IDE0042 // Deconstruct variable declaration
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                (int x, int y) moveVector = (jump.x - movedPawn.Coordinates.x, jump.y - movedPawn.Coordinates.y);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore IDE0042 // Deconstruct variable declaration
                (int x, int y) unitVector = (moveVector.x / Math.Abs(moveVector.x), moveVector.y / Math.Abs(moveVector.y));
                // Since all moves are perfectly diagonal, any dimension can be used to determine jump length
                int jumpLength = Math.Abs(moveVector.x);
                for (int move = 1; move < jumpLength; move++)
                {
                    (int x, int y) currentField = (movedPawn.Coordinates.x + (unitVector.x * move),
                                                movedPawn.Coordinates.y + (unitVector.y * move));
                    if (gameBoard.Fields[currentField.x, currentField.y] is Pawn)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        gameBoard.RemovePawn(gameBoard.Fields[currentField.x, currentField.y]);
#pragma warning restore CS8604 // Possible null reference argument.
                        
                        break;
                    }
                }
                
                gameBoard.MovePawn(movedPawn, jump);
                int pause = speed;
                if (!isAI)
                {
                    pause = 1000;
                }
                if (firstJumpFinished) Thread.Sleep(pause);
                firstJumpFinished = true;
                Console.Clear();
                Console.WriteLine(gameBoard.ToString(new List<(int, int)> { (-1, -1) }));
            }

        }
        private List<List<(int x, int y)>> PossibleJumpMoves()
        {
            List<List<(int, int)>> longestSequences = new List<List<(int, int)>>();
            foreach (Pawn pawn in gameBoard.Pawns[player])
            {
                if (CanPawnJump(pawn))
                {
                    longestSequences.AddRange(LongestJumpSequence(new List<(int, int)> { (pawn.Coordinates.x, pawn.Coordinates.y) }, pawn));
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
