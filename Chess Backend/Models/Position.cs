namespace Chess_Backend.Models
{
    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsValid()
        {
            return X >= 0 && X < 8 && Y >= 0 && Y < 8;
        }
        
    }
}
