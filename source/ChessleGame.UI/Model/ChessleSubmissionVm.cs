using ChessleGame.UI.Utils;

namespace ChessleGame.UI.Model
{
    public class ChessleSubmissionVm
    {
        public static int MovesCount = 10;
        public static int DefaultFontSize = 12;
        public static int SmallerFontSize = 9;
        public static int MaxNotationLengthForDefaultFontSize = 5;

        public ChessleSubmissionVm()
        {
            MovesNotation = new string[MovesCount];
            MoveColors = new string[MovesCount];
            FontSize = new int[MovesCount];

            for (int i = 0; i < MovesCount; i++)
            {
                MovesNotation[i] = string.Empty;
                MoveColors[i] = ChessleColors.Transparent;
                CurrentMove = 0;
                FontSize[i] = DefaultFontSize;
            }

            IsSolved = false;
        }

        public string[] MovesNotation { get; set; }

        public string[] MoveColors { get; set; }

        public int[] FontSize { get; set; }

        public int CurrentMove { get; set; }

        public bool IsSolved { get; private set; }

        public void FillTransparent()
        {
            for (int i = 0; i < MovesCount; i++)
            {
                MoveColors[i] = ChessleColors.Transparent;
            }
        }

        public void FillColorsCheckSubmission(char[] bullsCows)
        {
            var solved = true;

            for (int i = 0; i < MovesCount; i++)
            {
                MoveColors[i] = bullsCows[i] == BullsAndCowsCounter.Bull
                    ? ChessleColors.GreenMove
                    : bullsCows[i] == BullsAndCowsCounter.Cow ? ChessleColors.YellowMove : ChessleColors.GrayMove;

                if (bullsCows[i] != BullsAndCowsCounter.Bull)
                {
                    solved = false;
                }
            }

            IsSolved = solved;
        }
    }
}
