using ChessleGame.ChessLogic.Enums;
using ChessleGame.ChessLogic.Model;
using ChessleGame.UI.Enums;
using ChessleGame.UI.Model;

namespace ChessleGame.UI.Utils
{
    public static class FromVmConverter
    {
        public static PieceType GetPieceType(PieceTypeVm pieceTypeVm)
        {
            switch (pieceTypeVm)
            {
                case PieceTypeVm.WhiteKing: return PieceType.WhiteKing;
                case PieceTypeVm.WhiteQueen: return PieceType.WhiteQueen;
                case PieceTypeVm.WhiteBishop: return PieceType.WhiteBishop;
                case PieceTypeVm.WhiteKnight: return PieceType.WhiteKnight;
                case PieceTypeVm.WhiteRook: return PieceType.WhiteRook;
                case PieceTypeVm.WhitePawn: return PieceType.WhitePawn;
                case PieceTypeVm.BlackKing: return PieceType.BlackKing;
                case PieceTypeVm.BlackQueen: return PieceType.BlackQueen;
                case PieceTypeVm.BlackBishop: return PieceType.BlackBishop;
                case PieceTypeVm.BlackKnight: return PieceType.BlackKnight;
                case PieceTypeVm.BlackRook: return PieceType.BlackRook;
                case PieceTypeVm.BlackPawn: return PieceType.BlackPawn;
                default: return PieceType.Empty;
            }
        }

        public static PieceTypeVm GetPieceTypeVm(PieceType pieceType)
        {
            switch (pieceType)
            {
                case PieceType.WhiteKing: return PieceTypeVm.WhiteKing;
                case PieceType.WhiteQueen: return PieceTypeVm.WhiteQueen;
                case PieceType.WhiteBishop: return PieceTypeVm.WhiteBishop;
                case PieceType.WhiteKnight: return PieceTypeVm.WhiteKnight;
                case PieceType.WhiteRook: return PieceTypeVm.WhiteRook;
                case PieceType.WhitePawn: return PieceTypeVm.WhitePawn;
                case PieceType.BlackKing: return PieceTypeVm.BlackKing;
                case PieceType.BlackQueen: return PieceTypeVm.BlackQueen;
                case PieceType.BlackBishop: return PieceTypeVm.BlackBishop;
                case PieceType.BlackKnight: return PieceTypeVm.BlackKnight;
                case PieceType.BlackRook: return PieceTypeVm.BlackRook;
                case PieceType.BlackPawn: return PieceTypeVm.BlackPawn;
                default: return PieceTypeVm.Empty;
            }
        }

        public static MoveOrderType GetMoveOrderType(MoveOrderTypeVm moveOrderVm)
        {
            return moveOrderVm == MoveOrderTypeVm.White ? MoveOrderType.White : MoveOrderType.Black;
        }

        public static MoveOrderTypeVm GetMoveOrderTypeVm(MoveOrderType moveOrder)
        {
            return moveOrder == MoveOrderType.White ? MoveOrderTypeVm.White : MoveOrderTypeVm.Black;
        }

        public static Position GetPosition(PositionVm positionVm)
        {
            var position = new Position();

            for (int i = 0; i < ConstantsHelper.TotalSquaresCount; i++)
            {
                position.PiecesOnBoard[i / ConstantsHelper.SquaresInLineCount, i % ConstantsHelper.SquaresInLineCount] = GetPieceType(positionVm.PiecesOnBoard[i]);
            }

            position.MoveOrder = GetMoveOrderType(positionVm.MoveOrder);
            position.CanDoEnPassant = positionVm.CanDoEnPassant;
            position.EnPassantSquare = GetSquareFromId(positionVm.EnPassantSquareId);
            position.WhiteCastlingRules = new CastlingRules(positionVm.WhiteCastlingRules.CanDoLongCastling,
                positionVm.WhiteCastlingRules.CanDoShortCastling);
            position.BlackCastlingRules = new CastlingRules(positionVm.BlackCastlingRules.CanDoLongCastling,
                positionVm.BlackCastlingRules.CanDoShortCastling);

            return position;
        }

        public static Square GetSquareFromId(int squareId)
        {
            return new Square
            {
                X = squareId / ConstantsHelper.SquaresInLineCount,
                Y = squareId % ConstantsHelper.SquaresInLineCount
            };
        }

        public static int GetIdFromSquare(Square square)
        {
            return ConstantsHelper.SquaresInLineCount * square.X + square.Y;
        }

        public static PositionVm GetPositionVm(Position position)
        {
            var positionVm = new PositionVm();

            for (int i = 0; i < ConstantsHelper.TotalSquaresCount; i++)
            {
                positionVm.PiecesOnBoard[i] =
                    GetPieceTypeVm(position.PiecesOnBoard[i / ConstantsHelper.SquaresInLineCount,
                        i % ConstantsHelper.SquaresInLineCount]);
            }

            positionVm.MoveOrder = GetMoveOrderTypeVm(position.MoveOrder);
            positionVm.CanDoEnPassant = position.CanDoEnPassant;
            positionVm.EnPassantSquareId = GetIdFromSquare(position.EnPassantSquare);
            positionVm.WhiteCastlingRules = new CastlingRulesVm(position.WhiteCastlingRules.CanDoLongCastling,
                position.WhiteCastlingRules.CanDoShortCastling);
            positionVm.BlackCastlingRules = new CastlingRulesVm(position.BlackCastlingRules.CanDoLongCastling,
                position.BlackCastlingRules.CanDoShortCastling);

            return positionVm;
        }
    }
}
