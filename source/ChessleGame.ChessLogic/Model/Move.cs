namespace ChessleGame.ChessLogic.Model
{
    public class Move
    {
        public Move(Square beginSquare = null, Square endSquare = null,
            bool isPromotion = false, bool isShortCastling = false,
            bool isLongCastling = false, bool isEnPassantCapture = false,
            bool isEnPassantActivated = false, string notation = "")
        {
            BeginSquare = beginSquare;
            EndSquare = endSquare;
            IsPromotion = isPromotion;
            IsShortCastling = isShortCastling;
            IsLongCastling = isLongCastling;
            IsEnPassantCapture = isEnPassantCapture;
            IsEnPassantActivated = isEnPassantActivated;
            Notation = notation;
        }

        public Square BeginSquare { get; set; }
        public Square EndSquare { get; set; }
        public bool IsPromotion { get; set; }
        public bool IsShortCastling { get; set; }
        public bool IsLongCastling { get; set; }
        public bool IsEnPassantCapture { get; set; }
        public bool IsEnPassantActivated { get; set; }
        public string Notation { get; set; }
    }
}
