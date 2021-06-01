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
        public const int N = 8;
        const int DIRECTION_UP = -1;
        const int DIRECTION_DOWN = 1;
        const int UPPER_HOME = 0;
        const int BOTTOM_HOME = N - 1;

        private int direction;
        private int homeRow;
        private bool isComputer;
        public Dictionary<int, Piece> Pieces { get => pieces; set => pieces = value; }
        public int Direction { get => direction; set => direction = value; }
        public int HomeRow { get => homeRow; set => homeRow = value; }

        public Player(int rowStart, int rowEnd, int color, bool isComputer = false)
        {
            this.color = color;
            this.isComputer = isComputer;
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

        public int getColor()
        {
            return this.color;
        }


        /// <summary>
        /// set player's move direction and home row according to his color
        /// </summary>
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

        /// <summary>
        /// Returns true if this player is a computer
        /// </summary>
        /// <returns></returns>
        public bool IsComputer()
        {
            return this.isComputer;
        }

        /// <summary>
        /// get current player color
        /// </summary>
        /// return current player color integer
        public int GetPlayerColor()
        {
            return this.color;
        }
        /// <summary>
        /// paint the piece
        /// </summary>
        /// <param name="graphics"></param>
        internal void Paint(Graphics graphics)
        {
            try
            {
                foreach (Piece piece in Pieces.Values)
                {
                    piece.Paint(graphics);
                }
            } catch(Exception e)
            {
                Console.WriteLine("paint error");
                this.Paint(graphics);
            }

            
        }
        /// <summary>
        /// get the piece on this location<row,col>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// return object of the piece on this cube if exist, or null if doesnt exist 
        internal Piece GetPiece(int row, int col)
        {
            int key = row * Board.N + col;
            return Pieces.ContainsKey(key) ? Pieces[key] : null;
        }
        /// <summary>
        /// get the piece on this location<key>
        /// </summary>
        /// <param name="key"></param>
        /// return object of the piece on this cube if exist, or null if doesnt exist 
        internal Piece GetPiece(int key)
        {
            return Pieces.ContainsKey(key) ? Pieces[key] : null;
        }


        /// <summary>
        /// check if the cube contain piece
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns>return TRUE if the cube contain piece, and FALSE if not</returns>
        internal bool Contains(int row, int col)
        {
            return this.pieces.ContainsKey(Piece.getKey(row, col));
        }


        /// <summary>
        /// Delete the piece at the given key place
        /// </summary>
        /// <param name="key"></param>
        public void Delete(int key)
        {
            this.Pieces.Remove(key);
        }

        /// <summary>
        /// Add a piece at the given position
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void Add(int row, int col)
        {
            try
            {
                int key = row * Board.N + col;
                Pieces.Add(key, new Piece(row, col, this.color));
            } catch(Exception e)
            {
                
            }
        }
        /// <summary>
        /// Turn a regular piece into a queen piece or virtual queen piece (temporarily for DoVirtualMove)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="isVirtual"></param>
        /// <param name="isOldPieceQueen"></param>
        internal void makeQueen(int row, int col, bool isVirtual = false, bool isOldPieceQueen = false)
        {
            int key = row * Board.N + col;

            Piece queen =  Pieces[key];
            if (!isVirtual || isOldPieceQueen)
            {
                queen.QUEEN = true;
            }else
            {
                queen.virtQUEEN = true;
            }

        }

        /// <summary>
        /// Change back a virtual/temp queen to a regular piece after Virtual move test
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        internal void undoVirtualQueen(int row, int col)
        {
            Console.WriteLine("virtual queen here");
            int key = row * Board.N + col;

            Piece queen = Pieces[key];

            queen.QUEEN = true;
            queen.virtQUEEN = false;
    
        }


        /// <summary>
        /// check if the move is regular (radius == 1)
        /// </summary>
        /// <param name="difRow"></param>
        /// <param name="difCol"></param>
        /// <returns>True if regular move false if not</returns>
        public bool isRegularMove(int difRow, int difCol)
        {
            return difRow == this.direction && Math.Abs(difCol) == 1;
        }


        /// <summary>
        /// check if the move is irregular ( radius > 1)
        /// </summary>
        /// <param name="difRow"></param>
        /// <param name="difCol"></param>
        /// <returns>True if irregular move false if not</returns>
        public bool isRegularEatMove(int difRow, int difCol)
        {
            return difRow == 2 * this.direction && Math.Abs(difCol) == 2;
        }
        
        
        /// <summary>
        /// Check that the direction of the move is in the right direction for current player
        /// </summary>
        /// <param name="difRow"></param>
        /// <returns>True if direction is valid, false iif not</returns>
        public bool isLegalDirection(int difRow)
        {
            if(this.direction == DIRECTION_DOWN)
                return difRow >= DIRECTION_DOWN || difRow == 0;
            return difRow <= DIRECTION_UP || difRow == 0;
        }
    }
}