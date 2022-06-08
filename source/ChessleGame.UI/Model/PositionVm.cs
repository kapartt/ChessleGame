using ChessleGame.ChessLogic;
using ChessleGame.UI.Enums;
using ChessleGame.UI.Utils;
using System.Collections.Generic;
using System.Linq;

namespace ChessleGame.UI.Model
{
    public class PositionVm
    {
        public PositionVm()
        {
            SetEmptyBoard();
            MoveOrder = MoveOrderTypeVm.White;
            WhiteCastlingRules = new CastlingRulesVm();
            BlackCastlingRules = new CastlingRulesVm();
            LastMoveMotation = string.Empty;
        }

        public PieceTypeVm[] PiecesOnBoard { get; private set; }
        public MoveOrderTypeVm MoveOrder { get; set; }
        public CastlingRulesVm WhiteCastlingRules { get; set; }
        public CastlingRulesVm BlackCastlingRules { get; set; }
        public bool CanDoEnPassant { get; set; }
        public int EnPassantSquareId { get; set; }
        public string LastMoveMotation { get; private set; }

        public PositionVm(PositionVm position)
        {
            SetEmptyBoard();

            for (int i = 0; i < ConstantsHelper.TotalSquaresCount; i++)
            {
                PiecesOnBoard[i] = position.PiecesOnBoard[i];
            }

            MoveOrder = position.MoveOrder;
            WhiteCastlingRules = new CastlingRulesVm(position.WhiteCastlingRules);
            BlackCastlingRules = new CastlingRulesVm(position.BlackCastlingRules);
            CanDoEnPassant = position.CanDoEnPassant;
            EnPassantSquareId = position.EnPassantSquareId;
            LastMoveMotation = position.LastMoveMotation;
        }

        public void SetEmptyBoard()
        {
            PiecesOnBoard = new PieceTypeVm[ConstantsHelper.TotalSquaresCount];

            for (int i = 0; i < ConstantsHelper.TotalSquaresCount; i++)
            {
                PiecesOnBoard[i] = PieceTypeVm.Empty;
            }
        }

        public static PositionVm GetStartPosition()
        {
            var position = new PositionVm
            {
                PiecesOnBoard =
                {
                    [0] = PieceTypeVm.BlackRook,
                    [1] = PieceTypeVm.BlackKnight,
                    [2] = PieceTypeVm.BlackBishop,
                    [3] = PieceTypeVm.BlackQueen,
                    [4] = PieceTypeVm.BlackKing,
                    [5] = PieceTypeVm.BlackBishop,
                    [6] = PieceTypeVm.BlackKnight,
                    [7] = PieceTypeVm.BlackRook,
                    [8] = PieceTypeVm.BlackPawn,
                    [9] = PieceTypeVm.BlackPawn,
                    [10] = PieceTypeVm.BlackPawn,
                    [11] = PieceTypeVm.BlackPawn,
                    [12] = PieceTypeVm.BlackPawn,
                    [13] = PieceTypeVm.BlackPawn,
                    [14] = PieceTypeVm.BlackPawn,
                    [15] = PieceTypeVm.BlackPawn,
                    [48] = PieceTypeVm.WhitePawn,
                    [49] = PieceTypeVm.WhitePawn,
                    [50] = PieceTypeVm.WhitePawn,
                    [51] = PieceTypeVm.WhitePawn,
                    [52] = PieceTypeVm.WhitePawn,
                    [53] = PieceTypeVm.WhitePawn,
                    [54] = PieceTypeVm.WhitePawn,
                    [55] = PieceTypeVm.WhitePawn,
                    [56] = PieceTypeVm.WhiteRook,
                    [57] = PieceTypeVm.WhiteKnight,
                    [58] = PieceTypeVm.WhiteBishop,
                    [59] = PieceTypeVm.WhiteQueen,
                    [60] = PieceTypeVm.WhiteKing,
                    [61] = PieceTypeVm.WhiteBishop,
                    [62] = PieceTypeVm.WhiteKnight,
                    [63] = PieceTypeVm.WhiteRook
                },
                MoveOrder = MoveOrderTypeVm.White
            };

            position.LastMoveMotation = string.Empty;

            return position;
        }

        public bool IsLegalMove(int beginSquareId, int endSquareId)
        {
            var movesList = GetPossibleMovesEndSquareIds(beginSquareId);

            return movesList.Contains(endSquareId);
        }

        public PositionVm GetNewPositionAfterMove(int beginSquareId, int endSquareId)
        {
            var position = FromVmConverter.GetPosition(this);
            var square = FromVmConverter.GetSquareFromId(beginSquareId);

            var move = ChessLogicHelper.GetPossibleMovesForSelectedStartSquare(position, square)
                .First(x => endSquareId == FromVmConverter.GetIdFromSquare(x.EndSquare));

            var nextPosition = ChessLogicHelper.GetPositionAfterMove(position, move);

            var nextPositionVm = FromVmConverter.GetPositionVm(nextPosition);
            nextPositionVm.LastMoveMotation = NotationHelper.GetMoveNotation(position, move);

            return nextPositionVm;
        }

        public List<int> GetPossibleMovesEndSquareIds(int beginSquareId)
        {
            var position = FromVmConverter.GetPosition(this);
            var square = FromVmConverter.GetSquareFromId(beginSquareId);

            return ChessLogicHelper.GetPossibleMovesForSelectedStartSquare(position, square).Select(x => FromVmConverter.GetIdFromSquare(x.EndSquare)).ToList();
        }
    }
}
