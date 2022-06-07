namespace ChessleGame.UI.Model
{
    public class CastlingRulesVm
    {
        public CastlingRulesVm(bool canDoLongCastling = true, bool canDoShortCastling = true)
        {
            CanDoLongCastling = canDoLongCastling;
            CanDoShortCastling = canDoShortCastling;
        }

        public CastlingRulesVm(CastlingRulesVm castlingRules)
        {
            CanDoLongCastling = castlingRules.CanDoLongCastling;
            CanDoShortCastling = castlingRules.CanDoShortCastling;
        }

        public bool CanDoLongCastling { get; set; }
        public bool CanDoShortCastling { get; set; }
    }
}
