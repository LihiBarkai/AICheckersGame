using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Damka
{
    internal class Move
    {
        Piece pieceToMove; //chosen player
        Point dest;
        List<Piece>  eat = new List<Piece>();

        public Move(Piece pieceToMove, Point dest)
        {
            this.PieceToMove = pieceToMove;
            this.Dest = dest;
        }

        public Move(Piece pieceToMove, Point dest, Piece enemy) : this(pieceToMove, dest)
        {
            eat.Add(enemy);
        }

        public Move(Piece pieceToMove, Point dest, List<Piece> enemys) : this(pieceToMove, dest)
        {
            eat.AddRange(enemys);
        }

        public Piece PieceToMove { get => pieceToMove; set => pieceToMove = value; }
        public Point Dest { get => dest; set => dest = value; }
        public List<Piece> Eat { get => eat; set => eat = value; }
    }
}