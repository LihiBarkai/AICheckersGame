using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

namespace Damka
{
    public class Board
    {
        public const int N = 8;
        public static int MAX_PIECES = 12;
        public const int PLAYER_VS_PLAYER = 0;
        public const int PLAYER_VS_COMPUTER = 1;
        Move currentMove;
        Piece curSelectedPiece;
        public int oldKey; //המיקום הישן
        Piece curPiece = null; // בדיקה אם הלחיצה הקודמת הייתה על חייל או לא
        int difCol, difRow;
        public Player currentPlayer;
        public Player otherPlayer;
        bool isSecClick = false;
        public GameForm gameForm;
        bool isQueen = false;
        int mode = PLAYER_VS_PLAYER;
       
        public Player CurrentPlayer { get => currentPlayer;  set => currentPlayer = value; }
     
     /// <summary>
     /// creates the appropriate type of player according to the user's choice
     /// </summary>
     /// <param name="gameForm"></param>
     /// <param name="mode"></param>
     public Board(GameForm gameForm, int mode)
        {
            this.gameForm = gameForm;
            this.currentPlayer = new Player(0, 2, Piece.BLACK_PIECE);      // 1 - black;

            if(mode == Board.PLAYER_VS_PLAYER)
            {
                this.otherPlayer = new Player(N - 3, N - 1, Piece.WHITE_PIECE);   // 0 - white;
            } else
            {
                this.otherPlayer = new Computer(N - 3, N - 1, Piece.WHITE_PIECE);   // 0 - white;
            }
            this.mode = mode;

        }

        /// <summary>
        /// new game
        /// </summary>
        private void restartGame()
        {
            this.currentPlayer = new Player(0, 2, Piece.BLACK_PIECE);      // 1 - black;
            if (mode == Board.PLAYER_VS_PLAYER)
            {
                this.otherPlayer = new Player(N - 3, N - 1, Piece.WHITE_PIECE);   // 0 - white, player;
            }
            else
            {
                this.otherPlayer = new Computer(N - 3, N - 1, Piece.WHITE_PIECE);   // 0 - white, computer;
            }


            this.gameForm.Refresh();
        }

        internal void Paint(Graphics graphics)
        {
            this.currentPlayer.Paint(graphics);
            this.otherPlayer.Paint(graphics);
        }

        /// <summary>
        /// deselect the selected piece
        /// </summary>
        public void deselect() 
        {
            curPiece.deselectPiece();
            curPiece = null;
        }

        /// <summary>
        /// second click
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="isQueen"></param>
        /// 
        public bool secondClick(int row, int col, bool isQueen) 
        {
            difCol = col - oldKey % N;
            difRow = row - oldKey / N;
            bool isTurnDone = false;
            if (this.isValidCube(row, col))
            {

                if (isQueen)
                {
                    isTurnDone = secondClickQueen(currentPlayer, row, col, otherPlayer);
                }
                else
                {
                    if (this.currentPlayer.isRegularMove(difRow, difCol)) //1 radius move forward - black
                    {
                        move(row, col, false);
                        currentMove = new Move(curSelectedPiece, new Point(row,col));
                        isTurnDone = true;
                    }
                    else if (this.currentPlayer.isRegularEatMove(difRow, difCol)) //2 radius - black
                    {
                        isTurnDone = moveAndEat(row, col, difCol, false, isQueen);
                    }
                    else
                    {
                       isTurnDone = playIrregularMove(row, col);
                    }
                }

                isSecClick = false;

                if(isTurnDone)
                {
                    checkWinner();
                    this.changeTurn();
                }
            }

            return isTurnDone;
        }

        private bool playIrregularMove(int row, int col)
        {
            if (this.currentPlayer.isLegalDirection(this.difRow)) // check the direction
            {
                int currentCol = oldKey % N;
                int currentRow = oldKey / N;
                this.oldKey = currentRow * N + currentCol;
                return this.tryMove(row, col, currentRow, currentCol, this.currentPlayer.GetPiece(oldKey).IsQueen(), new List<Piece>(), new List<int>());
            }

            return false; // illegal move => false
        }
        /// <summary>
        /// try Continuous eat
        /// </summary>
        /// <param name="targetRow"></param>
        /// <param name="targetCol"></param>
        /// <param name="currentRow"></param>
        /// <param name="currentCol"></param>
        /// <param name="isQueen"></param>
        /// <param name="enemyPiecesToDelete"></param>
        /// <param name="visitedSpots"></param>
        /// returns true if the move is possible, false if not possible
        public bool tryMove(int targetRow, int targetCol, int currentRow, int currentCol, bool isQueen, List<Piece> enemyPiecesToDelete, List<int> visitedSpots)
        {
            if (areLocationsEqual(targetRow, targetCol, currentRow, currentCol)) // תנאי עצירה
            {
                foreach (Piece piece in enemyPiecesToDelete)
                {
                    int key = piece.getKey();
                    this.otherPlayer.Delete(piece.getKey());
                }
                this.move(targetRow, targetCol, false, isQueen); 
                return true;
            }

            List<int> nextPossibleMoves = this.getPossibleMoves(currentRow, currentCol, enemyPiecesToDelete.Count() > 0, isQueen);
            foreach (int possibleMove in nextPossibleMoves)
            {
                if (visitedSpots.IndexOf(possibleMove) != -1) //כל עוד לא הגעתי למקום הנבחר - ז"א הוא לא נמצא ברשימת המקומות שעברתי
                {
                    continue;
                }
                Piece enemyPieceOnKey = this.otherPlayer.GetPiece(possibleMove);
                if (enemyPieceOnKey != null) //המשך אכילה אחרי אכילה אחת
                {
                    int enemyPieceCol = enemyPieceOnKey.getKey() % N;
                    int enemyPieceRow = enemyPieceOnKey.getKey() / N; 

                    int colDirMove = (enemyPieceCol - currentCol); //הכיוון של ההתקדמות בטור האכילה הקודמת 
                    int rowDirMove = (enemyPieceRow - currentRow);//הכיוון של ההתקדמות בשורת האכילה הקודמת
                    colDirMove = colDirMove >= 1 ? 1 : -1;
                    rowDirMove = rowDirMove >= 1 ? 1 : -1;

                    int nextCol = enemyPieceCol + colDirMove;
                    int nextRow = enemyPieceRow + rowDirMove;

                    if (this.isCubeEmptyAndValid(nextRow, nextCol))
                    {
                        visitedSpots.Add(enemyPieceOnKey.getKey());
                        enemyPiecesToDelete.Add(enemyPieceOnKey);
                        if (tryMove(targetRow, targetCol, nextRow, nextCol, isQueen, enemyPiecesToDelete, visitedSpots))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// check if the cube is empty and on the board
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// return TRUE if the cube on (row,col) is on the board and empty and FALSE if not  
        public bool isCubeEmptyAndValid(int row, int col)
        {
            return this.isLocationInsideBoard(row, col) && !this.currentPlayer.Contains(row, col) && !this.otherPlayer.Contains(row, col);
        }

        /// get the locations of the possible enemy
        /// <param name="currentRow"></param>
        /// <param name="currentCol"></param>
        /// <param name="alreadyEatOne"></param>
        /// <param name="isQueen"></param>
        /// return list of integers - the key's (location) of the possible moves 
        public List<int> getPossibleMoves(int currentRow, int currentCol, bool alreadyEatOne, bool isQueen)
        {
            List<int> nextPossibleMoves = new List<int>();
            if (isQueen && !alreadyEatOne)
            {
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotOnDiag(currentRow, currentCol, -1, -1)); // top left
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotOnDiag(currentRow, currentCol, -1, 1)); // top right
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotOnDiag(currentRow, currentCol, 1, -1)); // bottom left
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotOnDiag(currentRow, currentCol, 1, 1)); // bottom right
            }
            else if (alreadyEatOne || isQueen)
            {
                nextPossibleMoves.Add(Piece.getKey(currentRow + this.currentPlayer.Direction, currentCol + 1));
                nextPossibleMoves.Add(Piece.getKey(currentRow + this.currentPlayer.Direction, currentCol - 1));
                nextPossibleMoves.Add(Piece.getKey(currentRow - this.currentPlayer.Direction, currentCol + 1));
                nextPossibleMoves.Add(Piece.getKey(currentRow - this.currentPlayer.Direction, currentCol - 1));
            }
            else
            {
                nextPossibleMoves.Add(Piece.getKey(currentRow + this.currentPlayer.Direction, currentCol + 1));
                nextPossibleMoves.Add(Piece.getKey(currentRow + this.currentPlayer.Direction, currentCol - 1));
            }

            return nextPossibleMoves;
        }


        /// <summary>
        /// check where is the location for the queen to eat on the diag if it exists 
        /// </summary>
        /// <param name="fromRow"></param>
        /// <param name="fromCol"></param>
        /// <param name="directionRow"></param>
        /// <param name="directionCol"></param>
        /// return
        public List<int> getPossibleMoveSpotOnDiag(int fromRow, int fromCol, int directionRow, int directionCol)
        {
            int row = fromRow + directionRow;
            int col = fromCol + directionCol;
            List<int> possibleSpots = new List<int>();
            while (this.isLocationInsideBoard(row, col))
            {
                if (this.otherPlayer.GetPiece(row, col) != null) //אם יש שם כלי אויב
                {
                    possibleSpots.Add(Piece.getKey(row, col));
                    break; // יוצא מהלולאת while
                }
                else if (this.currentPlayer.GetPiece(row, col) != null)
                {
                    break;
                }
                row += directionRow;
                col += directionCol;
            }

            return possibleSpots;
        }


        /// <summary>
        /// check if the locations are equal
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="row1"></param>
        /// <param name="col1"></param>
        /// return TRUE if the location are eequal, and FALSE if not
        public static bool areLocationsEqual(int row, int col, int row1, int col1)
        {
            return row == row1 && col == col1;
        }

        /// <summary>
        /// check if the cube is valid (brown cube)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// return TRUE if the cube is valid, and FALSE if not
        private bool isValidCube(int row, int col)

        {
            return (row + col) % 2 != 0;
        }

       /// <summary>
       /// second click with queen piece
       /// </summary>
       /// <param name="player"></param>
       /// <param name="row"></param>
       /// <param name="col"></param>
       /// <param name="otherPlayer"></param>
        private bool secondClickQueen(Player player, int row, int col, Player otherPlayer)
        {
            // TODO : make check winner and switch turns only at the end of this function
            int deleteKey = 0;
            int count = 0;
            int oldCol = oldKey % N;
            int oldRow = oldKey / N;
            int newCol = 0;
            int newRow = 0;
            bool isTurnDone = false;

            if (Math.Abs(oldCol - col) == Math.Abs(oldRow - row)) // check if new location is on the same line
            {

                for (int i = 1; i <= Math.Abs(oldRow - row); i++)
                {
                    newCol = oldCol > col ? oldCol - (i) : oldCol + (i);
                    newRow = oldRow > row ? oldRow - (i) : oldRow + (i);

                    if (otherPlayer.GetPiece(newRow, newCol) != null)//check if there is enemy to eat
                    {
                        count++;
                        deleteKey = newRow * N + newCol;
                    }
                    if (currentPlayer.GetPiece(newRow, newCol)!=null)//check that my player doesn't contain
                    {
                        count = -1;
                    }
                }

                if (count == 0)
                {
                    moveQueen(player, row, col, otherPlayer);
                    isTurnDone = true;   
                }

                else if (count == 1)
                {
                    moveAndEatQueen(player, otherPlayer, row, col, deleteKey);
                    isTurnDone = true;
                }
                else if (count!=-1)
                {
                    isTurnDone = this.irregularQueenMove(row, col);
                }

            } else
            {
                isTurnDone = this.irregularQueenMove(row, col);
            }

            return isTurnDone;
        }

        private bool irregularQueenMove(int row, int col)
        {
            int currentCol = oldKey % N;
            int currentRow = oldKey / N;
            return this.tryMove(row, col, currentRow, currentCol, this.currentPlayer.GetPiece(oldKey).IsQueen(), new List<Piece>(), new List<int>());
        }

        /// <summary>
        /// eat oyher player by a queen piece
        /// </summary>
        /// <param name="player"></param>
        /// <param name="otherPlayer"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="deleteKey"></param>
        private void moveAndEatQueen(Player player, Player otherPlayer, int row, int col, int deleteKey)
        {
            otherPlayer.Delete(deleteKey);
            moveQueen(player, row, col, otherPlayer);
        }

        /// <summary>
        /// move queen piece
        /// </summary>
        /// <param name="player"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="otherPlayer"></param>
        private void moveQueen(Player player, int row, int col, Player otherPlayer)
        {
            player.Delete(oldKey);
            player.Add(row, col);
            player.makeQueen(row, col);
        }


        /// <summary>
        /// eat other player
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="difCol"></param>
        public bool moveAndEat(int row, int col, int difCol, bool isVirtual = false, bool is_queen = false) 
        {
            int pieceToEatRow = row - this.currentPlayer.Direction;
            int pieceToEatCol = col - (difCol / 2);
            if (this.otherPlayer.GetPiece(pieceToEatRow, pieceToEatCol) != null)
            {
                this.otherPlayer.Delete(Piece.getKey(pieceToEatRow, pieceToEatCol));
                move(row, col, isVirtual, is_queen);
                
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if can perform an eating move
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="difCol"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns>returns the Piece to eat if exists, else return null</returns>
        public Piece moveAndEatCheck(int row, int col, int difCol, Player player, Player enemy)
        {
            int pieceToEatRow = row - player.Direction;
            int pieceToEatCol = col - (difCol/2);
            if (isLocationInsideBoard(pieceToEatRow, pieceToEatCol))
                 return enemy.GetPiece(pieceToEatRow, pieceToEatCol);
            return null;
        }


        /// <summary>
        /// check and announce who is the winner and end the game by force if requested
        /// suggest another game
        /// </summary>
        /// <param name="forceEndGame"></param>
        public void checkWinner(bool forceEndGame = false)
        {
            Player wonPlayer = null;
            if (this.currentPlayer.Pieces.Count == 0) 
            {
                wonPlayer = this.otherPlayer;
            } else if(this.otherPlayer.Pieces.Count == 0)
            {
                wonPlayer = this.currentPlayer;
            } 

            if(forceEndGame) // End botton clicked
            {
                if (this.currentPlayer.Pieces.Count > this.otherPlayer.Pieces.Count)
                {
                    wonPlayer = this.currentPlayer;
                }
                else if (this.currentPlayer.Pieces.Count < this.otherPlayer.Pieces.Count)
                {
                    wonPlayer = this.otherPlayer;
                }
            }
            
            if(wonPlayer != null || forceEndGame)
            {
                string wonColor = wonPlayer != null && wonPlayer.GetPlayerColor() == Piece.BLACK_PIECE ? "Black" : "White";
                string message = wonPlayer != null ? "Game over! " + wonColor +  " player won!" : "Game over, its a draw!";
                message += "\n would you like to play again?";
                string caption = "Game over!";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    this.restartGame();
                }
            }
        }

        /// <summary>
        /// check that the cube is inside the board
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// return TRUE if the cube's location is inside the board, and FALSE if not
        public bool isLocationInsideBoard(int row, int col)
        {
            return row >= 0 && row < Board.N && col >= 0 && col < Board.N;
        }

        /// <summary>
        /// Undo the last move
        /// </summary>
        internal void Undo()
        { 
            oldKey = currentMove.Dest.X * N + currentMove.Dest.Y;
            Player temp = currentPlayer;
            currentPlayer = otherPlayer;
            move(currentMove.PieceToMove.ROW, currentMove.PieceToMove.COL);
            currentPlayer = temp;
            changeTurn(); ///////שיניתי עכשיו
        }


        /// <summary>
        /// Actually move the piece by deleting the old piece and adding a new piece in the new location and updating queen status
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void move(int row, int col, bool isVirtual = false, bool isQueen = false)
        {
            bool isOldPieceQueen = isQueen;
            Piece oldPiece = this.currentPlayer.GetPiece(oldKey);
            if (oldPiece != null)
            {
                isOldPieceQueen = isQueen || this.currentPlayer.GetPiece(oldKey).IsQueen();
            }
            this.currentPlayer.Delete(oldKey);
            this.currentPlayer.Add(row, col);
            if (row == otherPlayer.HomeRow || isOldPieceQueen) //is queen
            {
                this.currentPlayer.makeQueen(row, col, isVirtual, isOldPieceQueen);
            }
        }

        public void changePlayer()
        {
            Player tempPlayer = currentPlayer;
            currentPlayer = otherPlayer;
            otherPlayer = tempPlayer;
        }
        /// <summary>
        /// change turn
        /// </summary>
        private void changeTurn()
        { 
            changePlayer();
            this.updateGUI();
            computerTurn();
        }

        /// <summary>
        /// check if this is a computer turn and do the move
        /// </summary>
        public void computerTurn()
        {
            if (this.currentPlayer.IsComputer())
            {              
                ((Computer)this.currentPlayer).doMove(this); 
            }
        }

        /// <summary>
        /// Set lableTurn to who is the current player turn
        /// </summary>
        private void updateGUI()
        {
            gameForm.labelTurn.Text = currentPlayer.GetPlayerColor() == Piece.BLACK_PIECE ? "Black turn " : "White turn";
            gameForm.labelTurn.ForeColor = currentPlayer.GetPlayerColor() == Piece.BLACK_PIECE ? Color.Black : Color.White;
        }

        /// <summary>
        /// select piece and save its data
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="piece"></param>
        public void select(int row, int col, Piece piece) 
        {
            oldKey = row * N + col;
            curPiece = piece;
            piece.selectPiece();
        }

        /// <summary>
        /// Recieves a location Point and translates it into column and row, then calls Click function
        /// </summary>
        /// <param name="location"></param>
        /// 
        internal void clickOnScreen(Point location)
        {
            int col = (location.X - Piece.PIECESIZE / 2) / Piece.PIECESIZE;
            int row = (location.Y - Piece.PIECESIZE / 2) / Piece.PIECESIZE;
            click(row, col);
        }
        /// <summary>
        /// The function handles both the first click and the 2nd click (the move) and saves its data
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        internal void click(int row, int col)
        {
            Piece selectedPiece = this.currentPlayer.GetPiece(row, col) != null ?
                        this.currentPlayer.GetPiece(row, col) :
                        this.otherPlayer.GetPiece(row, col);

 
            if (selectedPiece == null) // empty cube
            {
                if (isSecClick)
                {
                    secondClick(row, col, isQueen);
                    isQueen = false;
                }
            }
            else 
            {
                curSelectedPiece = selectedPiece;
                if (currentPlayer.GetPlayerColor() == selectedPiece.COLOR) // if this is the current player's piece
                {
                    if (curPiece != null) //deselect
                    {
                        deselect();
                    }

                    select(row, col, selectedPiece);
                    isQueen = selectedPiece.QUEEN;
                    isSecClick = true;
                }
            }
        }
    }
}