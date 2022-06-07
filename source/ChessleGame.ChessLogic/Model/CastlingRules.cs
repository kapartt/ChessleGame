namespace ChessleGame.ChessLogic.Model
{
    public class CastlingRules
    {
        public CastlingRules(bool canDoLongCastling = true, bool canDoShortCastling = true)
        {
            CanDoLongCastling = canDoLongCastling;
            CanDoShortCastling = canDoShortCastling;
        }

        public CastlingRules(CastlingRules castlingRules)
        {
            CanDoLongCastling = castlingRules.CanDoLongCastling;
            CanDoShortCastling = castlingRules.CanDoShortCastling;
        }

        public bool CanDoLongCastling { get; set; }
        public bool CanDoShortCastling { get; set; }
    }
}
