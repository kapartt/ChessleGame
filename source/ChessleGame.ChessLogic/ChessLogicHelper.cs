using ChessleGame.ChessLogic.Enums;
using ChessleGame.ChessLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessleGame.ChessLogic
{
    public static class ChessLogicHelper
    {
        public const int SquaresInLineCount = 8;
        public const int TotalSquaresCount = 64;

        public const int WhiteFirstLine = 7;
        public const int WhiteSecondLine = 6;
        public const int WhiteFifthLine = 3;
        public const int WhiteSeventhLine = 1;

        public const int VerticalA = 0;
        public const int VerticalB = 1;
        public const int VerticalC = 2;
        public const int VerticalD = 3;
        public const int VerticalE = 4;
        public const int VerticalF = 5;
        public const int VerticalG = 6;
        public const int VerticalH = 7;

        public static List<Move> GetPossibleMovesForSelectedStartSquare(Position position, Square square)
        {
            var whiteMovePosition = position.MoveOrder == MoveOrderType.White
                ? position
                : GetFlippedBoardPosition(position);
            var x = position.MoveOrder == MoveOrderType.White ? square.X : SquaresInLineCount - square.X - 1;
            var y = square.Y;

            var isSelectedWhitePiece = IsWhitePiece(whiteMovePosition.PiecesOnBoard[x, y]);

            if (!isSelectedWhitePiece) return new List<Move>();

            var candidateMoves = GetMovesForWhitePiece(whiteMovePosition, new Square(x, y));

            var possibleMoves = new List<Move>();

            foreach (var candidateMove in candidateMoves)
            {
                var newPosition = GetPositionAfterWhiteMove(whiteMovePosition, candidateMove);

                if (!AreBlackPiecesAttackingSquare(newPosition, GetWhiteKingSquare(newPosition)))
                {
                    possibleMoves.Add(candidateMove);
                }
            }

            return position.MoveOrder == MoveOrderType.White ? possibleMoves : possibleMoves.Select(GetFlippedMove).ToList();
        }

        public static Position GetPositionAfterMove(Position position, Move move)
        {
            var whiteMovePosition = position.MoveOrder == MoveOrderType.White
                ? position
                : GetFlippedBoardPosition(position);
            var whiteMove = position.MoveOrder == MoveOrderType.White ? move : GetFlippedMove(move);

            var newPosition = GetPositionAfterWhiteMove(whiteMovePosition, whiteMove);

            return position.MoveOrder == MoveOrderType.White ? newPosition : GetFlippedBoardPosition(newPosition);
        }

        public static List<Move> GetPossibleMovesForSelectedPiece(Position position, PieceType pieceType)
        {
            var moves = new List<Move>();

            for (int i = 0; i < SquaresInLineCount; i++)
                for (int j = 0; j < SquaresInLineCount; j++)
                {
                    if (position.PiecesOnBoard[i, j] == pieceType)
                    {
                        moves.AddRange(GetPossibleMovesForSelectedStartSquare(position, new Square(i, j)));
                    }
                }

            return moves;
        }

        public static bool IsCheck(Position position)
        {
            if (position.MoveOrder == MoveOrderType.White)
                return AreBlackPiecesAttackingSquare(position, GetWhiteKingSquare(position));

            var flippedPosition = GetFlippedBoardPosition(position);
            return AreBlackPiecesAttackingSquare(flippedPosition, GetWhiteKingSquare(flippedPosition));
        }

        public static bool IsCheckmate(Position position)
        {
            for (int i = 0; i < SquaresInLineCount; i++)
                for (int j = 0; j < SquaresInLineCount; j++)
                {
                    if (GetPossibleMovesForSelectedStartSquare(position, new Square(i, j)).Count > 0)
                    {
                        return false;
                    }
                }

            return IsCheck(position);
        }

        #region Private methods

        private static Position GetPositionAfterWhiteMove(Position position, Move move)
        {
            var newPosition = new Position(position)
            {
                MoveOrder = MoveOrderType.Black,
                CanDoEnPassant = move.IsEnPassantActivated
            };

            if (move.IsEnPassantActivated)
            {
                newPosition.EnPassantSquare = new Square(move.EndSquare);
            }

            if (move.IsEnPassantCapture)
            {
                newPosition.PiecesOnBoard[move.BeginSquare.X, move.BeginSquare.Y] = PieceType.Empty;
                newPosition.PiecesOnBoard[position.EnPassantSquare.X, position.EnPassantSquare.Y] = PieceType.Empty;
                newPosition.PiecesOnBoard[move.EndSquare.X, move.EndSquare.Y] = PieceType.WhitePawn;

                return newPosition;
            }

            if (position.PiecesOnBoard[move.BeginSquare.X, move.BeginSquare.Y] == PieceType.WhiteKing)
            {
                newPosition.WhiteCastlingRules = new CastlingRules(false, false);
            }

            if (position.PiecesOnBoard[move.BeginSquare.X, move.BeginSquare.Y] == PieceType.WhiteRook)
            {
                switch (move.BeginSquare.Y)
                {
                    case VerticalA:
                        newPosition.WhiteCastlingRules.CanDoLongCastling = false;
                        break;
                    case VerticalH:
                        newPosition.WhiteCastlingRules.CanDoShortCastling = false;
                        break;
                }
            }

            if (move.IsShortCastling)
            {
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalE] = PieceType.Empty;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalF] = PieceType.WhiteRook;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalG] = PieceType.WhiteKing;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalH] = PieceType.Empty;

                return newPosition;
            }

            if (move.IsLongCastling)
            {
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalA] = PieceType.Empty;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalB] = PieceType.Empty;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalC] = PieceType.WhiteKing;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalD] = PieceType.WhiteRook;
                newPosition.PiecesOnBoard[WhiteFirstLine, VerticalE] = PieceType.Empty;

                return newPosition;
            }

            newPosition.PiecesOnBoard[move.EndSquare.X, move.EndSquare.Y] = newPosition.PiecesOnBoard[move.BeginSquare.X, move.BeginSquare.Y];
            newPosition.PiecesOnBoard[move.BeginSquare.X, move.BeginSquare.Y] = PieceType.Empty;

            if (move.IsPromotion)
            {
                newPosition.PiecesOnBoard[move.EndSquare.X, move.EndSquare.Y] = PieceType.WhiteQueen;
            }

            return newPosition;
        }

        private static Position GetFlippedBoardPosition(Position position)
        {
            var newPosition = new Position();
            newPosition.SetEmptyBoard();

            for (int i = 0; i < SquaresInLineCount; i++)
                for (int j = 0; j < SquaresInLineCount; j++)
                {
                    newPosition.PiecesOnBoard[i, j] = GetSamePieceChangedColor(position.PiecesOnBoard[SquaresInLineCount - i - 1, j]);
                }

            newPosition.WhiteCastlingRules = new CastlingRules(position.BlackCastlingRules);
            newPosition.BlackCastlingRules = new CastlingRules(position.WhiteCastlingRules);

            newPosition.MoveOrder = position.MoveOrder == MoveOrderType.White ? MoveOrderType.Black : MoveOrderType.White;
            newPosition.CanDoEnPassant = position.CanDoEnPassant;
            newPosition.EnPassantSquare = new Square(SquaresInLineCount - position.EnPassantSquare.X - 1, position.EnPassantSquare.Y);

            return newPosition;
        }

        private static PieceType GetSamePieceChangedColor(PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.WhiteKing => PieceType.BlackKing,
                PieceType.WhiteQueen => PieceType.BlackQueen,
                PieceType.WhiteBishop => PieceType.BlackBishop,
                PieceType.WhiteKnight => PieceType.BlackKnight,
                PieceType.WhiteRook => PieceType.BlackRook,
                PieceType.WhitePawn => PieceType.BlackPawn,
                PieceType.BlackKing => PieceType.WhiteKing,
                PieceType.BlackQueen => PieceType.WhiteQueen,
                PieceType.BlackBishop => PieceType.WhiteBishop,
                PieceType.BlackKnight => PieceType.WhiteKnight,
                PieceType.BlackRook => PieceType.WhiteRook,
                PieceType.BlackPawn => PieceType.WhitePawn,
                _ => PieceType.Empty
            };
        }

        private static Move GetFlippedMove(Move move)
        {
            return new Move
            {
                BeginSquare = GetFlippedSquare(move.BeginSquare),
                EndSquare = GetFlippedSquare(move.EndSquare),
                IsEnPassantActivated = move.IsEnPassantActivated,
                IsEnPassantCapture = move.IsEnPassantCapture,
                IsLongCastling = move.IsLongCastling,
                IsPromotion = move.IsPromotion,
                IsShortCastling = move.IsShortCastling
            };
        }

        private static Square GetFlippedSquare(Square square)
        {
            return new Square
            {
                X = SquaresInLineCount - square.X - 1,
                Y = square.Y
            };
        }

        private static List<Move> GetMovesForWhitePiece(Position position, Square square)
        {
            return position.PiecesOnBoard[square.X, square.Y] switch
            {
                PieceType.WhiteKing => GetWhiteKingMoves(position, square),
                PieceType.WhiteQueen => GetQueenMoves(position, square),
                PieceType.WhiteBishop => GetBishopMoves(position, square),
                PieceType.WhiteKnight => GetKnightMoves(position, square),
                PieceType.WhiteRook => GetRookMoves(position, square),
                PieceType.WhitePawn => GetWhitePawnMoves(position, square),
                _ => new List<Move>()
            };
        }

        private static List<Move> GetWhiteKingMoves(Position position, Square square)
        {
            var moves = new List<Move>();

            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    if ((dx != 0 || dy != 0) && CanDoSingleMoveForDirection(position, square, dx, dy))
                    {
                        moves.Add(new Move(square, new Square(square.X + dx, square.Y + dy)));
                    }
                }

            if (position.WhiteCastlingRules.CanDoShortCastling &&
                position.PiecesOnBoard[WhiteFirstLine, VerticalF] == PieceType.Empty &&
                position.PiecesOnBoard[WhiteFirstLine, VerticalG] == PieceType.Empty &&
                !AreBlackPiecesAttackingSquare(position, new Square(WhiteFirstLine, VerticalE)) &&
                !AreBlackPiecesAttackingSquare(position, new Square(WhiteFirstLine, VerticalF)) &&
                !AreBlackPiecesAttackingSquare(position, new Square(WhiteFirstLine, VerticalG)))
            {
                moves.Add(new Move(square, new Square(WhiteFirstLine, VerticalG), isShortCastling: true));
            }

            if (position.WhiteCastlingRules.CanDoLongCastling &&
                position.PiecesOnBoard[WhiteFirstLine, VerticalB] == PieceType.Empty &&
                position.PiecesOnBoard[WhiteFirstLine, VerticalC] == PieceType.Empty &&
                position.PiecesOnBoard[WhiteFirstLine, VerticalD] == PieceType.Empty &&
                !AreBlackPiecesAttackingSquare(position, new Square(WhiteFirstLine, VerticalC)) &&
                !AreBlackPiecesAttackingSquare(position, new Square(WhiteFirstLine, VerticalD)) &&
                !AreBlackPiecesAttackingSquare(position, new Square(WhiteFirstLine, VerticalE)))
            {
                moves.Add(new Move(square, new Square(WhiteFirstLine, VerticalC), isLongCastling: true));
            }

            return moves;
        }

        private static List<Move> GetQueenMoves(Position position, Square square)
        {
            var bishopMoves = GetBishopMoves(position, square);
            var rookMoves = GetRookMoves(position, square);
            bishopMoves.AddRange(rookMoves);

            return bishopMoves;
        }

        private static List<Move> GetBishopMoves(Position position, Square square)
        {
            var moves = new List<Move>();

            GetAllMovesForDirection(position, square, square, 1, 1, ref moves);
            GetAllMovesForDirection(position, square, square, 1, -1, ref moves);
            GetAllMovesForDirection(position, square, square, -1, 1, ref moves);
            GetAllMovesForDirection(position, square, square, -1, -1, ref moves);

            return moves;
        }

        private static List<Move> GetKnightMoves(Position position, Square square)
        {
            var moves = new List<Move>();

            for (int dx = -2; dx <= 2; dx++)
                for (int dy = -2; dy <= 2; dy++)
                {
                    if (Math.Abs(dx * dy) == 2 && CanDoSingleMoveForDirection(position, square, dx, dy))
                    {
                        moves.Add(new Move(square, new Square(square.X + dx, square.Y + dy)));
                    }
                }

            return moves;
        }

        private static List<Move> GetRookMoves(Position position, Square square)
        {
            var moves = new List<Move>();

            GetAllMovesForDirection(position, square, square, 0, 1, ref moves);
            GetAllMovesForDirection(position, square, square, 0, -1, ref moves);
            GetAllMovesForDirection(position, square, square, 1, 0, ref moves);
            GetAllMovesForDirection(position, square, square, -1, 0, ref moves);

            return moves;
        }

        private static void GetAllMovesForDirection(Position position, Square startSquare, Square currentSquare,
            int dx, int dy, ref List<Move> prevMoves)
        {
            var x = currentSquare.X + dx;
            var y = currentSquare.Y + dy;

            if (x < 0 || y < 0 || x >= SquaresInLineCount || y >= SquaresInLineCount) return;

            if (position.PiecesOnBoard[x, y] == PieceType.Empty)
            {
                prevMoves.Add(new Move(startSquare, new Square(x, y)));
                GetAllMovesForDirection(position, startSquare, new Square(x, y), dx, dy, ref prevMoves);
                return;
            }

            if (IsWhitePiece(position.PiecesOnBoard[startSquare.X, startSquare.Y]) ^ IsWhitePiece(position.PiecesOnBoard[x, y]))
            {
                prevMoves.Add(new Move(startSquare, new Square(x, y)));
            }
        }

        private static bool CanDoSingleMoveForDirection(Position position, Square startSquare, int dx, int dy)
        {
            var x = startSquare.X + dx;
            var y = startSquare.Y + dy;

            if (x < 0 || y < 0 || x >= SquaresInLineCount || y >= SquaresInLineCount) return false;

            return position.PiecesOnBoard[x, y] == PieceType.Empty ||
                   IsWhitePiece(position.PiecesOnBoard[startSquare.X, startSquare.Y]) ^ IsWhitePiece(position.PiecesOnBoard[x, y]);
        }

        private static List<Move> GetWhitePawnMoves(Position position, Square square)
        {
            var moves = new List<Move>();

            switch (square.X)
            {
                case WhiteSecondLine:
                    {
                        if (position.PiecesOnBoard[square.X - 1, square.Y] == PieceType.Empty &&
                            position.PiecesOnBoard[square.X - 2, square.Y] == PieceType.Empty)
                        {
                            moves.Add(new Move(square, new Square(square.X - 2, square.Y), isEnPassantActivated: true));
                        }
                        break;
                    }
                case WhiteFifthLine:
                    {
                        if (position.CanDoEnPassant && position.EnPassantSquare.X == square.X &&
                            Math.Abs(position.EnPassantSquare.Y - square.Y) == 1)
                        {
                            moves.Add(new Move(square, new Square(square.X - 1, position.EnPassantSquare.Y), isEnPassantCapture: true));
                        }
                        break;
                    }
            }

            if (position.PiecesOnBoard[square.X - 1, square.Y] == PieceType.Empty)
            {
                moves.Add(new Move(square, new Square(square.X - 1, square.Y), square.X == WhiteSeventhLine));
            }

            if (square.Y > 0 &&
                !IsWhitePiece(position.PiecesOnBoard[square.X - 1, square.Y - 1]) &&
                position.PiecesOnBoard[square.X - 1, square.Y - 1] != PieceType.Empty)
            {
                moves.Add(new Move(square, new Square(square.X - 1, square.Y - 1), square.X == WhiteSeventhLine));
            }

            if (square.Y < SquaresInLineCount - 1 &&
                !IsWhitePiece(position.PiecesOnBoard[square.X - 1, square.Y + 1]) &&
                position.PiecesOnBoard[square.X - 1, square.Y + 1] != PieceType.Empty)
            {
                moves.Add(new Move(square, new Square(square.X - 1, square.Y + 1), square.X == WhiteSeventhLine));
            }

            return moves;
        }

        private static bool IsWhitePiece(PieceType pieceType)
        {
            return pieceType switch
            {
                PieceType.WhiteKing => true,
                PieceType.WhiteQueen => true,
                PieceType.WhiteBishop => true,
                PieceType.WhiteKnight => true,
                PieceType.WhiteRook => true,
                PieceType.WhitePawn => true,
                _ => false
            };
        }

        private static Square GetWhiteKingSquare(Position position)
        {
            for (int i = 0; i < SquaresInLineCount; i++)
                for (int j = 0; j < SquaresInLineCount; j++)
                {
                    if (position.PiecesOnBoard[i, j] != PieceType.WhiteKing) continue;

                    return new Square(i, j);
                }

            return null;
        }

        private static bool AreBlackPiecesAttackingSquare(Position position, Square square)
        {
            for (int i = 0; i < SquaresInLineCount; i++)
                for (int j = 0; j < SquaresInLineCount; j++)
                {
                    switch (position.PiecesOnBoard[i, j])
                    {
                        case PieceType.BlackKing:
                            if (IsKingAttackingSquare(new Square(i, j), square)) return true;
                            else break;
                        case PieceType.BlackQueen:
                            if (IsQueenAttackingSquare(position, new Square(i, j), square)) return true;
                            else break;
                        case PieceType.BlackBishop:
                            if (IsBishopAttackingSquare(position, new Square(i, j), square)) return true;
                            else break;
                        case PieceType.BlackKnight:
                            if (IsKnightAttackingSquare(position, new Square(i, j), square)) return true;
                            else break;
                        case PieceType.BlackRook:
                            if (IsRookAttackingSquare(position, new Square(i, j), square)) return true;
                            else break;
                        case PieceType.BlackPawn:
                            if (IsPawnAttackingSquare(position, new Square(i, j), square)) return true;
                            else break;
                    }
                }

            return false;
        }

        private static bool IsKingAttackingSquare(Square beginSquare, Square targetSquare)
        {
            return !Equals(beginSquare, targetSquare) && Math.Abs(beginSquare.X - targetSquare.X) <= 1 && Math.Abs(beginSquare.Y - targetSquare.Y) <= 1;
        }

        private static bool IsQueenAttackingSquare(Position position, Square beginSquare, Square targetSquare)
        {
            return GetQueenMoves(position, beginSquare).Any(x => Equals(x.EndSquare, targetSquare));
        }

        private static bool IsBishopAttackingSquare(Position position, Square beginSquare, Square targetSquare)
        {
            return GetBishopMoves(position, beginSquare).Any(x => Equals(x.EndSquare, targetSquare));
        }

        private static bool IsKnightAttackingSquare(Position position, Square beginSquare, Square targetSquare)
        {
            return GetKnightMoves(position, beginSquare).Any(x => Equals(x.EndSquare, targetSquare));
        }

        private static bool IsRookAttackingSquare(Position position, Square beginSquare, Square targetSquare)
        {
            return GetRookMoves(position, beginSquare).Any(x => Equals(x.EndSquare, targetSquare));
        }

        private static bool IsPawnAttackingSquare(Position position, Square beginSquare, Square targetSquare)
        {
            var dx = position.PiecesOnBoard[beginSquare.X, beginSquare.Y] == PieceType.WhitePawn ? 1 : -1;
            return targetSquare.X + dx == beginSquare.X && Math.Abs(targetSquare.Y - beginSquare.Y) == 1;
        }

        #endregion
    }
}
