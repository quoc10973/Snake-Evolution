



namespace Snake
{
    public class Direction
    {
        public readonly static Direction Up = new Direction(-1, 0); //đi lên
        public readonly static Direction Down = new Direction(1, 0); //đi xuống
        public readonly static Direction Left = new Direction(0, -1); //qua trái
        public readonly static Direction Right = new Direction(0, 1); //qua phải

        public int RowOffset { get;}
        public int ColumnOffset { get;}

        public Direction(int rowOffset, int columnOffset)
        {
            RowOffset = rowOffset;
            ColumnOffset = columnOffset;
        }

        public Direction Opposite()
        {
            return new Direction(-RowOffset, -ColumnOffset);
        }

        public override bool Equals(object? obj)
        {
            return obj is Direction direction &&
                   RowOffset == direction.RowOffset &&
                   ColumnOffset == direction.ColumnOffset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowOffset, ColumnOffset);
        }

        public static bool operator ==(Direction? left, Direction? right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction? left, Direction? right)
        {
            return !(left == right);
        }
    }
}
