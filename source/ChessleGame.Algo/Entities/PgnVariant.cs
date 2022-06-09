using System;

namespace ChessleGame.Algo.Entities
{
    public class PgnVariant
    {
        public static char WrongMove = '0';
        public static char Cow = '1';
        public static char Bull = '2';

        public PgnVariant(string pgnMoves = "", int usingsCount = 0, string gameInfo = "")
        {
            var pgnArray = pgnMoves.Split();
            Moves = new string[10];

            var j = 1;
            var isWhiteMove = true;

            if (pgnMoves == String.Empty) return;

            for (int i = 0; i < 10; i++)
            {
                Moves[i] = pgnArray[j];
                isWhiteMove = !isWhiteMove;

                if (isWhiteMove)
                {
                    j += 2;
                }
                else
                {
                    j++;
                }
            }

            UsingsCount = usingsCount;
            GameInfo = gameInfo;
        }

        public string[] Moves { get; set; }
        public int UsingsCount { get; set; }
        public string GameInfo { get; set; }

        public bool CanBeSolutionFor(PgnVariant submission, char[] bullsCows)
        {
            if (Moves.Length != submission.Moves.Length || Moves.Length != bullsCows.Length)
            {
                return false;
            }

            var used = new bool[bullsCows.Length];

            // check for bulls
            for (int i = 0; i < bullsCows.Length; i++)
            {
                if (bullsCows[i] == Bull)
                {
                    if (Moves[i] != submission.Moves[i])
                    {
                        return false;
                    }

                    used[i] = true;
                }
            }

            // check for cows
            for (int i = 0; i < bullsCows.Length; i++)
            {
                if (bullsCows[i] == Cow)
                {
                    var isFoundCow = false;

                    for (int j = 0; j < Moves.Length; j++)
                    {
                        if (!used[j] && submission.Moves[i] == Moves[j] && i != j)
                        {
                            used[j] = true;
                            isFoundCow = true;
                            break;
                        }
                    }

                    if (!isFoundCow)
                    {
                        return false;
                    }
                }
            }

            // check for wrong moves
            for (int i = 0; i < bullsCows.Length; i++)
            {
                if (bullsCows[i] == WrongMove)
                {
                    var isFoundWrongMove = false;

                    for (int j = 0; j < Moves.Length; j++)
                    {
                        if (!used[j] && submission.Moves[i] == Moves[j])
                        {
                            isFoundWrongMove = true;
                            break;
                        }
                    }

                    if (isFoundWrongMove)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override string ToString()
        {
            return string.Join(" ", Moves);
        }
    }
}
