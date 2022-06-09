using ChessleGame.Algo.Entities;
using System.Collections.Generic;

namespace ChessleGame.UI.Model
{
    public class GameState
    {
        public GameState()
        {
            PlayerSubmissions = new List<ChessleSubmissionVm>();
            EngineSubmissions = new List<ChessleSubmissionVm>();
            CurrentMove = 1;
            IsUserMove = true;
            RightAnswer = new PgnVariant();
            GuessedPositions = new Dictionary<int, PositionVm>();
            GameInfo = string.Empty;
            EndingPositionVm = PositionVm.GetStartPosition();
        }

        public List<ChessleSubmissionVm> PlayerSubmissions { get; set; }
        public List<ChessleSubmissionVm> EngineSubmissions { get; set; }
        public int CurrentMove { get; set; }
        public bool IsUserMove { get; set; }
        public PgnVariant RightAnswer { get; set; }
        public Dictionary<int, PositionVm> GuessedPositions { get; set; }
        public string GameInfo { get; set; }
        public PositionVm EndingPositionVm { get; set; }
    }
}
