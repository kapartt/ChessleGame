using ChessleGame.UI.Enums;

namespace ChessleGame.UI.Utils
{
    public static class PicturesDirectories
    {
        public const string Chessboard = "\\Pictures\\board.png";
        public const string WhiteKing = "\\Pictures\\wk.png";
        public const string WhiteQueen = "\\Pictures\\wq.png";
        public const string WhiteBishop = "\\Pictures\\wb.png";
        public const string WhiteKnight = "\\Pictures\\wn.png";
        public const string WhiteRook = "\\Pictures\\wr.png";
        public const string WhitePawn = "\\Pictures\\wp.png";
        public const string BlackKing = "\\Pictures\\bk.png";
        public const string BlackQueen = "\\Pictures\\bq.png";
        public const string BlackBishop = "\\Pictures\\bb.png";
        public const string BlackKnight = "\\Pictures\\bn.png";
        public const string BlackRook = "\\Pictures\\br.png";
        public const string BlackPawn = "\\Pictures\\bp.png";

        public static string GetDirectoryFromPieceName(PieceTypeVm pieceType)
        {
            switch (pieceType)
            {
                case PieceTypeVm.WhiteKing: return WhiteKing;
                case PieceTypeVm.WhiteQueen: return WhiteQueen;
                case PieceTypeVm.WhiteBishop: return WhiteBishop;
                case PieceTypeVm.WhiteKnight: return WhiteKnight;
                case PieceTypeVm.WhiteRook: return WhiteRook;
                case PieceTypeVm.WhitePawn: return WhitePawn;
                case PieceTypeVm.BlackKing: return BlackKing;
                case PieceTypeVm.BlackQueen: return BlackQueen;
                case PieceTypeVm.BlackBishop: return BlackBishop;
                case PieceTypeVm.BlackKnight: return BlackKnight;
                case PieceTypeVm.BlackRook: return BlackRook;
                case PieceTypeVm.BlackPawn: return BlackPawn;
                default: return string.Empty;
            }
        }
    }
}
