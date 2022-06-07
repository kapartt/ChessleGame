using ChessleGame.ChessLogic.Enums;

namespace ChessleGame.ChessLogic.Model
{
    public class Position
    {
        public Position()
        {
            SetEmptyBoard();
            MoveOrder = MoveOrderType.White;
            WhiteCastlingRules = new CastlingRules();
            BlackCastlingRules = new CastlingRules();
            EnPassantSquare = new Square();
        }

        public PieceType[,] PiecesOnBoard { get; private set; }
        public MoveOrderType MoveOrder { get; set; }
        public CastlingRules WhiteCastlingRules { get; set; }
        public CastlingRules BlackCastlingRules { get; set; }
        public bool CanDoEnPassant { get; set; }
        public Square EnPassantSquare { get; set; }

        public Position(Position position)
        {
            SetEmptyBoard();

            for (int i = 0; i < ChessLogicHelper.SquaresInLineCount; i++)
                for (int j = 0; j < ChessLogicHelper.SquaresInLineCount; j++)
                {
                    PiecesOnBoard[i, j] = position.PiecesOnBoard[i, j];
                }

            MoveOrder = position.MoveOrder;

            WhiteCastlingRules = new CastlingRules(position.WhiteCastlingRules);
            BlackCastlingRules = new CastlingRules(position.BlackCastlingRules);
            CanDoEnPassant = position.CanDoEnPassant;

            EnPassantSquare = new Square(position.EnPassantSquare);
        }

        public void SetEmptyBoard()
        {
            PiecesOnBoard = new PieceType[ChessLogicHelper.SquaresInLineCount, ChessLogicHelper.SquaresInLineCount];

            for (int i = 0; i < ChessLogicHelper.SquaresInLineCount; i++)
                for (int j = 0; j < ChessLogicHelper.SquaresInLineCount; j++)
                {
                    PiecesOnBoard[i, j] = PieceType.Empty;
                }
        }
    }
}
