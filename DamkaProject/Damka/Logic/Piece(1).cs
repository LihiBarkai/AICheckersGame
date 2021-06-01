using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;

namespace Damka
{
    public class Piece
    {
        public static int PIECESIZE = 50;
        int row, col;
        int color;
        bool isQueen = false;
        bool isSelected = false;

        public const int WHITE_PIECE = 0;
        public const int BLACK_PIECE = 1;
        public const int QUEEN_PIECE = 2;
        public const int SELECTED_PIECE = 3;

        public int ROW { get { return row; } set { row = value; } }
        public int COL { get { return col; } set { col = value; } }
        public bool QUEEN { get { return isQueen; } set { isQueen = value; } }


        public Piece(int row, int col,int color)
        {
            this.row = row;
            this.col = col;
            this.color = color;
        }

        internal void Paint(Graphics graphics)
        {
            Image image;
            if(this.isSelected)
            {
                // image for selected queen or slected piece
                image = this.isQueen ? Properties.Resources.dam : Properties.Resources.dam; // change one of the queens to other resource
            } else
            {
                if(this.isQueen)
                {
                    // image for black queen or white queen
                    image = this.color == BLACK_PIECE ? Properties.Resources.queen : Properties.Resources.queen; 
                } else
                {
                    // image for black piece or white piece
                    image = this.color == BLACK_PIECE ? Properties.Resources.black : Properties.Resources.white1;
                }
            }

            graphics.DrawImage(image, col * PIECESIZE+ PIECESIZE/2, row * PIECESIZE+ PIECESIZE/2,
                                      PIECESIZE, PIECESIZE);
        }

        public bool IsQueen()
        {
            return this.isQueen;
        }

        public int GetColor()
        {
            return this.color;
        }

        public void selectPiece()
        {
            this.isSelected = true;
        }

        public void deselectPiece()
        {
            this.isSelected = false;
        }
   
        public void makeQueen()
        {
            this.isSelected = true;
        }

        public int getKey()
        {
            return this.ROW * Board.N + this.COL;
        }

        public static int getKey(int row, int col)
        {
            return (row * Board.N) + col;
        }

    }
}