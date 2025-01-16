using System.Runtime.CompilerServices;

namespace Chess_Backend.Models
{
    public abstract class Piece
    {
        // ganti color menjadi enum
        public Color Color { get; set; }
        public Position Position { get; set; }

        protected Piece(Color color, Position position)
        {
            Color = color;
            Position = position;
        }

        public abstract bool IsValidMove(Position newPosition, Piece?[,] board);

        // public bool IsValidMove(Position newPosition, Piece[,] board);
        // public bool IsLegalMove(Position newPosition);
        // return collection of position yang legal
        // siapa yang akan ngecek legal move? board
        // board membatasi langkahnya ditaruh di board
        // public void Move(Position newPosition);
        // memanggil isLegalMove dulu agar tau legal atau tidak
        // tambah untuk check sudah ada gerakan belum
    }
}
