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

namespace Damka
{
    public class Board
    {
        public const int N = 8;
        public static int MAX_PIECES = 12;
        int oldKey; //המיקום הישן
                    // bool isBlack = true;

        Piece curPiece = null; // בדיקה אם הלחיצה הקודמת הייתה על חייל או לא
        int difCol, difRow;
        public Player currentPlayer;
        public Player otherPlayer;
        bool isSecClick = false;
        GameForm gameForm;
        bool isQueen = false;

        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

        public Board(GameForm gameForm)
        {
            this.gameForm = gameForm;
            this.currentPlayer = new Player(0, 2, Piece.BLACK_PIECE);      // 2 - black;
            this.otherPlayer = new Player(N - 3, N - 1, Piece.WHITE_PIECE);   // 3 - white;
        }

        private void restartGame()
        {
            this.currentPlayer = new Player(0, 2, Piece.BLACK_PIECE);      // 2 - black;
            this.otherPlayer = new Player(N - 3, N - 1, Piece.WHITE_PIECE);   // 3 - white;
            this.gameForm.Refresh();
        }

        internal void Paint(Graphics graphics)
        {
            this.currentPlayer.Paint(graphics);
            this.otherPlayer.Paint(graphics);
        }

        public void deselect() //deselect
        {
            curPiece.deselectPiece();
            curPiece = null;
        }

        public void secondClick(int row, int col, bool isQueen) //second click
        {
            difCol = col - oldKey % N;
            difRow = row - oldKey / N;

            if (this.isValidCube(row, col))
            {

                if (isQueen)
                {
                    secondClickQueen(currentPlayer, row, col, otherPlayer);
                }
                else
                {
                    if (this.currentPlayer.isRegularMove(difRow, difCol)) //1 radius move forward - black
                    {
                        move(row, col);
                    }
                    else if (this.currentPlayer.isRegularEatMove(difRow, difCol)) //2 radius - black
                    {
                        moveAndEat(row, col, difCol);
                    }
                    else
                    {
                        playIrregularMove(row, col);
                    }
                }

                isSecClick = false;
            }
        }

        private void playIrregularMove(int row, int col)
        {
            if (this.currentPlayer.isLegalDirection(this.difRow)) // check the direction
            {
                int currentCol = oldKey % N;
                int currentRow = oldKey / N;
                this.tryMove(row, col, currentRow, currentCol, this.currentPlayer.GetPiece(oldKey).IsQueen(), new List<Piece>(), new List<int>());
            }
        }

        public bool tryMove(int targetRow, int targetCol, int currentRow, int currentCol, bool isQueen, List<Piece> enemyPiecesToDelete, List<int> visitedSpots)
        {
            if (areLocationsEqual(targetRow, targetCol, currentRow, currentCol)) // תנאי עצירה
            {
                foreach (Piece piece in enemyPiecesToDelete)
                {
                    int key = piece.getKey();
                    this.otherPlayer.Delete(piece.getKey());
                }
                this.move(targetRow, targetCol); // it also changes turn!!!!!
                return true;
            }

            List<int> nextPossibleMoves = this.getPossibleMoves(currentRow, currentCol, enemyPiecesToDelete.Count() > 0, isQueen);
            foreach (int possibleMove in nextPossibleMoves)
            {
                if (visitedSpots.IndexOf(possibleMove) != -1)
                {
                    continue;
                }
                Piece enemyPieceOnKey = this.otherPlayer.GetPiece(possibleMove);
                if (enemyPieceOnKey != null)
                {
                    int enemyPieceCol = enemyPieceOnKey.getKey() % N;
                    int enemyPieceRow = enemyPieceOnKey.getKey() / N; 

                    int addToCol = (enemyPieceCol - currentCol);
                    int addToRow = (enemyPieceRow - currentRow);
                    addToCol = addToCol >= 1 ? 1 : -1;
                    addToRow = addToRow >= 1 ? 1 : -1;

                    int nextCol = enemyPieceCol + addToCol;
                    int nextRow = enemyPieceRow + addToRow;

                    if (this.isCubeEmpty(nextRow, nextCol))
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

        public bool isCubeEmpty(int row, int col)
        {
            return this.isLocationInsideBoard(row, col) && !this.currentPlayer.Contains(row, col) && !this.otherPlayer.Contains(row, col);
        }

        public List<int> getPossibleMoves(int currentRow, int currentCol, bool alreadyEatOne, bool isQueen)
        {
            List<int> nextPossibleMoves = new List<int>();
            if (isQueen && !alreadyEatOne)
            {
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotsOnLine(currentRow, currentCol, -1, -1)); // top left
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotsOnLine(currentRow, currentCol, -1, 1)); // top right
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotsOnLine(currentRow, currentCol, 1, -1)); // bottom left
                nextPossibleMoves.AddRange(this.getPossibleMoveSpotsOnLine(currentRow, currentCol, 1, 1)); // bottom right
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

        public List<int> getPossibleMoveSpotsOnLine(int fromRow, int fromCol, int directionRow, int directionCol)
        {
            int row = fromRow + directionRow;
            int col = fromCol + directionCol;
            List<int> possibleSpots = new List<int>();
            while (this.isLocationInsideBoard(row, col))
            {
                if (this.otherPlayer.GetPiece(row, col) != null)
                {
                    possibleSpots.Add(Piece.getKey(row, col));
                    break;
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

        public bool isValidMoveLocation(int row, int col, int directionRow, int directionCol)
        {
            return this.otherPlayer.GetPiece(row, col) == null || // location empty or
                    (this.otherPlayer.GetPiece(row, col) != null && this.isLocationInsideBoard(row + directionRow, col + directionCol)); // location with enemy that can get eat
        }

        public bool isLocationEmpty(int row, int col)
        {
            return this.otherPlayer.GetPiece(row, col) == null && this.currentPlayer.GetPiece(row, col) == null;
        }

        public static bool areLocationsEqual(int row, int col, int row1, int col1)
        {
            return row == row1 && col == col1;
        }

        private bool isValidCube(int row, int col)

        {
            return (row + col) % 2 != 0;
        }

        private void secondClickQueen(Player player, int row, int col, Player otherPlayer)
        {
            int deleteKey = 0;
            int count = 0;
            int oldCol = oldKey % N;
            int oldRow = oldKey / N;
            int newCol = 0;
            int newRow = 0;

            if (Math.Abs(oldCol - col) == Math.Abs(oldRow - row)) // check if new location is on the same line
            {

                for (int i = 1; i <= Math.Abs(oldRow - row); i++)
                {
                    newCol = oldCol > col ? oldCol - (i) : oldCol + (i);
                    newRow = oldRow > row ? oldRow - (i) : oldRow + (i);

                    if (otherPlayer.GetPiece(newRow, newCol) != null)//check my player doesn't contain
                    {
                        count++;
                        deleteKey = newRow * N + newCol;
                    }
                }

                if (count == 0)
                {
                    moveQueen(player, row, col, otherPlayer);
                }

                else if (count == 1)
                {
                    moveAndEatQueen(player, otherPlayer, row, col, deleteKey);
                }
                else
                {
                    this.irregularQueenMove(row, col);
                }

            } else
            {
                this.irregularQueenMove(row, col);
            }
        }

        private void irregularQueenMove(int row, int col)
        {
            int currentCol = oldKey % N;
            int currentRow = oldKey / N;
            this.tryMove(row, col, currentRow, currentCol, this.currentPlayer.GetPiece(oldKey).IsQueen(), new List<Piece>(), new List<int>());
        }

        private void moveAndEatQueen(Player player, Player otherPlayer, int row, int col, int deleteKey)
        {
            otherPlayer.Delete(deleteKey);
            moveQueen(player, row, col, otherPlayer);
            checkWinner(false);
        }


        private void moveQueen(Player player, int row, int col, Player otherPlayer)
        {
            player.Delete(oldKey);
            player.Add(row, col);
            player.makeQueen(row, col);
            this.changeTurn();
        }

        public void moveAndEat(int row, int col, int difCol) //eat
        {
            int pieceToEatRow = row - this.currentPlayer.Direction;
            int pieceToEatCol = col - (difCol / 2);
            if (this.otherPlayer.GetPiece(pieceToEatRow, pieceToEatCol) != null)
            {
                this.otherPlayer.Delete((pieceToEatRow) * N + (pieceToEatCol));

                move(row, col);

                checkWinner(false);
            }
        }

        private void checkWinner(bool forceEndGame)
        {
            Player wonPlayer = null;
            if (this.currentPlayer.Pieces.Count == 0)
            {
                wonPlayer = this.otherPlayer;
            } else if(this.otherPlayer.Pieces.Count == 0)
            {
                wonPlayer = this.currentPlayer;
            } 

            if(forceEndGame)
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
                string wonColor = wonPlayer != null && wonPlayer.GetColor() == Piece.BLACK_PIECE ? "Black" : "White";
                string message = wonPlayer != null ? "Game over! " + wonColor +  " player won!" : "Game over, its a draw!";
                message += "\n would you like to play another game?";
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

        private bool isLocationInsideBoard(int row, int col)
        {
            return row >= 0 && row < Board.N && col >= 0 && col < Board.N;
        }

        public void move(int row, int col)//move piece 
        {
            bool isOldPieceQueen = this.currentPlayer.GetPiece(oldKey).IsQueen();
            this.currentPlayer.Delete(oldKey);
            if (row == otherPlayer.HomeRow || isOldPieceQueen) //is queen
            {
                this.currentPlayer.Add(row, col);
                this.currentPlayer.makeQueen(row, col);
            }

            else //not queen
            {
                this.currentPlayer.Add(row, col);
            }

            //// התור משתנה
            this.changeTurn();
        }

        private void changeTurn()
        {
            Console.WriteLine("! going to change turns, current: " + this.currentPlayer.GetColor());
            Player tempPlayer = currentPlayer;
            currentPlayer = otherPlayer;
            otherPlayer = tempPlayer;
            this.updateGUI();

        }

        private void updateGUI()
        {
            gameForm.labelTurn.Text = currentPlayer.GetColor() == Piece.BLACK_PIECE ? "Black turn (Player 2)" : "White turn (Player 1)";
            gameForm.labelTurn.ForeColor = currentPlayer.GetColor() == Piece.BLACK_PIECE ? Color.Black : Color.White;
        }

        public void endGame()
        {
            this.checkWinner(true);
        }

        public void select(int row, int col, Piece piece) 
        {
            oldKey = row * N + col;
            curPiece = piece;
            piece.selectPiece();
        }

        internal void click(Point location)
        {
            int col = (location.X - Piece.PIECESIZE / 2) / Piece.PIECESIZE;
            int row = (location.Y - Piece.PIECESIZE / 2) / Piece.PIECESIZE;

            Piece selectedPiece = this.currentPlayer.GetPiece(row, col) != null ?
                        this.currentPlayer.GetPiece(row, col) :
                        this.otherPlayer.GetPiece(row, col);

            if (selectedPiece == null)
            {
                if (isSecClick)
                {
                    secondClick(row, col, isQueen);
                    isQueen = false;
                }
            }
            else
            {
                if (currentPlayer.GetColor() == selectedPiece.GetColor())
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
