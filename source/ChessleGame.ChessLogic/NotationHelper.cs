using ChessleGame.ChessLogic.Enums;
using ChessleGame.ChessLogic.Model;
using System.Linq;

namespace ChessleGame.ChessLogic
{
    public static class NotationHelper
    {
        public static string GetMoveNotation(Position position, Move move)
        {
            if (move.IsLongCastling) return "O-O-O";
            if (move.IsShortCastling) return "O-O";

            var pieceType = position.PiecesOnBoard[move.BeginSquare.X, move.BeginSquare.Y];
            var pieceName = GetPieceName(pieceType);

            var beginSquare = string.Empty;

            if (IsPawn(pieceType))
            {
                beginSquare = GetVertical(move.BeginSquare);
            }
            else
            {
                var possibleMovesForSelectedPieceType =
                    ChessLogicHelper.GetPossibleMovesForSelectedPiece(position, pieceType);

                var beginSquaresWithSelectedPieceType = possibleMovesForSelectedPieceType
                    .Where(x => x.EndSquare.Equals(move.EndSquare) && !x.BeginSquare.Equals(move.BeginSquare)).Select(x => x.BeginSquare).ToList();

                if (beginSquaresWithSelectedPieceType.Count > 0)
                {
                    if (beginSquaresWithSelectedPieceType.All(x => x.Y != move.BeginSquare.Y))
                    {
                        beginSquare = GetVertical(move.BeginSquare);
                    }
                    else if (beginSquaresWithSelectedPieceType.All(x => x.X != move.BeginSquare.X))
                    {
                        beginSquare = GetHorizontal(move.BeginSquare);
                    }
                    else
                    {
                        beginSquare = GetVertical(move.BeginSquare) + GetHorizontal(move.BeginSquare);
                    }
                }

            }

            var capture = move.IsEnPassantCapture ||
                          position.PiecesOnBoard[move.EndSquare.X, move.EndSquare.Y] != PieceType.Empty ? "x" : string.Empty;

            var endVertical = string.Empty;

            if (!IsPawn(pieceType) || capture != string.Empty)
            {
                endVertical = GetVertical(move.EndSquare);
            }

            var endHorizontal = GetHorizontal(move.EndSquare);
            var promotion = move.IsPromotion ? "=Q" : string.Empty;

            var check = string.Empty;

            var nextPosition = ChessLogicHelper.GetPositionAfterMove(position, move);
            if (ChessLogicHelper.IsCheckmate(nextPosition))
            {
                check = "#";
            }
            else if (ChessLogicHelper.IsCheck(nextPosition))
            {
                check = "+";
            }

            return pieceName + beginSquare + capture + endVertical + endHorizontal + promotion + check;
        }

        private static string GetPieceName(PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.WhiteKing => "K",
                PieceType.WhiteQueen => "Q",
                PieceType.WhiteBishop => "B",
                PieceType.WhiteKnight => "N",
                PieceType.WhiteRook => "R",
                PieceType.BlackKing => "K",
                PieceType.BlackQueen => "Q",
                PieceType.BlackBishop => "B",
                PieceType.BlackKnight => "N",
                PieceType.BlackRook => "R",
                _ => string.Empty
            };
        }

        private static string GetVertical(Square square)
        {
            return ((char)('a' + square.Y)).ToString();
        }

        private static string GetHorizontal(Square square)
        {
            return (ChessLogicHelper.SquaresInLineCount - square.X).ToString();
        }

        private static bool IsPawn(PieceType pieceType)
        {
            return pieceType == PieceType.WhitePawn || pieceType == PieceType.BlackPawn;
        }
    }
}
