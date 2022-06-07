using ChessleGame.Algo.Entities;
using ChessleGame.UI.Model;

namespace ChessleGame.UI.Utils
{
    public static class BullsAndCowsCounter
    {
        public const char WrongMove = '0';
        public const char Cow = '1';
        public const char Bull = '2';

        public static char[] GetBullsAndCows(ChessleSubmissionVm submission, PgnVariant answer)
        {
            var bullsCows = new char[ChessleSubmissionVm.MovesCount];
            if (bullsCows.Length != answer.Moves.Length) return bullsCows;

            var isFoundMove = new bool[ChessleSubmissionVm.MovesCount];

            for (int i = 0; i < bullsCows.Length; i++)
            {
                bullsCows[i] = WrongMove;
            }

            for (int i = 0; i < bullsCows.Length; i++)
            {
                if (submission.MovesNotation[i] == answer.Moves[i])
                {
                    isFoundMove[i] = true;
                    bullsCows[i] = Bull;
                }
            }

            for (int i = 0; i < bullsCows.Length; i++)
            {
                for (int j = 0; j < bullsCows.Length; j++)
                {
                    if (i == j) continue;
                    if (submission.MovesNotation[i] != answer.Moves[j] || isFoundMove[j]) continue;

                    isFoundMove[j] = true;
                    bullsCows[i] = Cow;
                    break;
                }
            }

            return bullsCows;
        }
    }
}
