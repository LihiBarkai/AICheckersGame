using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;

namespace Damka
{
    public class Player
    {
         Dictionary<int, Piece> pieces;

        private int color;

        const int DIRECTION_UP = -1;
        const int DIRECTION_DOWN = 1;
        const int UPPER_HOME = 0;
        const int BOTTOM_HOME = Board.N - 1;

        private int direction;
        private int homeRow;
        public Dictionary<int, Piece> Pieces { get => pieces; set => pieces = value; }
        public int Direction { get => direction; set => direction = value; }
        public int HomeRow { get => homeRow; set => homeRow = value; }

        public Player(int rowStart, int rowEnd, int color)
        {
            this.color = color;
            this.initPlayer();
            Pieces = new Dictionary<int, Piece>();
            for (int i = rowStart; i <= rowEnd; i++)
            {
                for (int j = 0; j < Board.N; j++)
                {
                    if ((i+j)%2 != 0 )
                        Pieces.Add(i*Board.N+j, new Piece(i, j, color));
                }
            }
        }

        /* set player's move direction and home row according to his color */
        private void initPlayer()
        {
            if (this.color == Piece.BLACK_PIECE)
            {
                this.direction = DIRECTION_DOWN;
                this.homeRow = UPPER_HOME;
            }
            else
            {
                this.direction = DIRECTION_UP;
                this.homeRow = BOTTOM_HOME;
            }
        }

        public int GetColor()
        {
            return this.color;
        }

        internal void Paint(Graphics graphics)
        {
            foreach (Piece piece in Pieces.Values)
            {
                piece.Paint(graphics);
            }
        }

        internal Piece GetPiece(int row, int col)
        {
            int key = row * Board.N + col;
            return Pieces.ContainsKey(key) ? Pieces[key] : null;
        }

        internal Piece GetPiece(int key)
        {
            return Pieces.ContainsKey(key) ? Pieces[key] : null;
        }

        internal bool Contains(int row, int col)
        {
            return this.pieces.ContainsKey(Piece.getKey(row, col));
        }

        internal bool Contains(int key)
        {
            return this.pieces.ContainsKey(key);
        }


        public void Delete(int key)
        {
            this.Pieces.Remove(key);
        }
        public void Add(int row, int col)
        {
            int key = row * Board.N + col;
            Pieces.Add(key, new Piece(row, col, this.color));
        }

        internal void makeQueen(int row, int col)
        {
            int key = row * Board.N + col;

            Piece queen =  Pieces[key];
            queen.QUEEN = true;
        }


        internal bool isQueen(int row, int col)
        {
            int key = row * Board.N + col;

            Piece curPiece = Pieces[key];
            return curPiece.QUEEN;

        }

        public bool isRegularMove(int difRow, int difCol)
        {
            return difRow == this.direction && Math.Abs(difCol) == 1;
        }

        public bool isRegularEatMove(int difRow, int difCol)
        {
            return difRow == 2 * this.direction && Math.Abs(difCol) == 2;
        }


        public bool isLegalDirection(int difRow)
        {
            if(this.direction == DIRECTION_DOWN)
            {
                return difRow >= DIRECTION_DOWN || difRow == 0;
            } else
            {
                return difRow <= DIRECTION_UP || difRow == 0;
            }
        }
    }
}