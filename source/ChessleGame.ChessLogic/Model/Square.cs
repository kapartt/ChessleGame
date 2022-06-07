using System;

namespace ChessleGame.ChessLogic.Model
{
    public class Square
    {
        protected bool Equals(Square other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Square)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public Square(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        public Square(Square square)
        {
            X = square.X;
            Y = square.Y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
