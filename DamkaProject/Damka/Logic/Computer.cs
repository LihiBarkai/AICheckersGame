using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System;
using System.Drawing;
using System.Windows.Media.Media3D;
using System.Threading;

namespace Damka
{
    internal class Computer : Player 
    {
        Piece pieceToEat = new Piece();
        Player human;
        int DEPTH = 3;
        Board board;

        public Computer(int rowStart, int rowEnd, int color) : base(rowStart, rowEnd, color, true) 
        {
        }

        /// <summary>
        /// Call a new thread for starting the computer "thinking" process for executing its next best move
        /// </summary>
        /// <param name="board"></param>
        public void doMove(Board board)
        {
            this.board = board;
            human = board.otherPlayer;
            Thread threadThink = new Thread(new ThreadStart(Think));
            threadThink.Start();
        }

        /// <summary>
        /// The main function for finding out the best move for the computer by virtually running all the moves and grading them
        /// and then executing the best move.
        /// Using a short Sleep before the move to make it more user friendly
        /// </summary>
        private void Think()
        {
            Thread.Sleep(500);
    
            List<Move> validMoves = this.getValidMoves(true);
            Move bestMove= null;
            int maxGrade = int.MinValue;
            int moveIndex = 0;
            foreach (Move move in validMoves)
            {
                bool wasPieceAQueen = move.PieceToMove.IsQueen(); // check if piece was queen before virtual turns
                DoVirtualMove(move, true);
                Piece realPiece = move.PieceToMove;
                int grade = AlphaBeta(DEPTH - 1, maxGrade, int.MaxValue, false);

                if (grade > maxGrade)
                {
                    maxGrade = grade;
                    bestMove = move;
                }
                board.currentPlayer = this;
                board.otherPlayer = human;
                UndoVirtualMove(move, true);
                if(wasPieceAQueen != board.currentPlayer.GetPiece(move.PieceToMove.getKey()).IsQueen()) // if some how virtual turns affected queen state, change it back
                {
                    board.currentPlayer.GetPiece(move.PieceToMove.getKey()).toggleQueen();
                }
                moveIndex++;
                try
                {
                    Console.WriteLine("piece: " + board.currentPlayer.GetPiece(33));
                } catch (Exception e) {
                    Console.WriteLine("exception: " + e);
                }
            }

            Console.WriteLine("---------------------------------");

            board.gameForm.Invoke((MethodInvoker) delegate
            {
                if (bestMove != null)
                {
                    board.click(bestMove.PieceToMove.ROW, bestMove.PieceToMove.COL); //first computer click - piece to move
                    board.click(bestMove.Dest.X, bestMove.Dest.Y); //second click - to destination
                    board.gameForm.pictureBox1.Invalidate();
                }
            });               
        }



        /// <summary>
        /// The main algorithm for choosing the computer's best move out of all possible moves
        ///  by using AlphaBeta cut with depth
        /// </summary>
        /// <param name="depth"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="comp"></param>
        /// <returns>returns the best grade</returns>
        private int AlphaBeta(int depth, int alpha, int beta, bool comp)
        {
            if (depth == 0) //|| board.IsGameOver() ) }
            {
                board.currentPlayer = comp ? this : human;
                board.otherPlayer = comp ? human : this;
                return HeuristicFunction();
            }

            List<Move> validMoves = this.getValidMoves(comp);
            int bestGrade = comp ? int.MinValue : int.MaxValue;
            foreach (Move move in validMoves)
            {
                board.currentPlayer = comp ? this : human;
                board.otherPlayer = comp ? human : this;
                DoVirtualMove(move,comp);
                if (comp)
                {
                    bestGrade = Math.Max(bestGrade, AlphaBeta(depth - 1, alpha, beta, !comp));
                    alpha = Math.Max(bestGrade, alpha);
                }
                else
                {
                    bestGrade = Math.Min(bestGrade, AlphaBeta(depth - 1, alpha, beta, !comp));
                    beta = Math.Min(bestGrade, beta);
                }
               
               board.currentPlayer = comp ? this : human;
               board.otherPlayer = comp ? human : this;
               UndoVirtualMove(move, comp);

                //the following if statement constitutes alpha-beta pruning
                if (alpha >= beta)
                    break;
            }
            return bestGrade;
        }


        /// <summary>
        /// Calculates the actual grade of the move
        /// </summary>
        /// <returns>the move's final grade</returns>
        private int HeuristicFunction()
        { 
            int gradeComp = 0;
            foreach (Piece piece in this.Pieces.Values)
            {
                gradeComp += (piece.virtQUEEN || piece.QUEEN) ? 16 : 10;
                gradeComp += checkEnemyProx(piece, this,human);
                gradeComp += checkForTrap(piece, this, human);
                if ((piece.COL == 0)|| (piece.COL == 7))
                {
                    gradeComp += 3;
                }
            
            }

            int gradeHuman = 0;
            foreach (Piece piece in human.Pieces.Values)
            {
                gradeHuman += piece.QUEEN ? 16 : 10;
                gradeComp += checkEnemyProx(piece, human, this);
                gradeComp += checkForTrap(piece, human, this);
                if ((piece.COL == 0) || (piece.COL == 7))
                {
                    gradeComp += 3;
                }
            }
            return gradeComp - gradeHuman;
        }


        /// <summary>
        /// check if the move is close to enemy piece but safe and give it a better grade if so
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns>the move's grade</returns>
        private int checkEnemyProx(Piece piece,Player player, Player enemy)
        {
            int grade = 0;
            if ((enemy.GetPiece(piece.ROW + player.Direction , piece.COL + 1) !=null)&&
                (enemy.GetPiece(piece.ROW - player.Direction, piece.COL - 1) != null)) 
            {
                grade = 2;
            }
            if ((enemy.GetPiece(piece.ROW + player.Direction, piece.COL - 1) != null)&&
                 (enemy.GetPiece(piece.ROW - player.Direction, piece.COL + 1) != null))
            {
                grade += 2;
            }
            return grade;
        }

        /// <summary>
        /// Lower the grade of the move if the piece could be eaten
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="player"></param>
        /// <param name="enemy"></param>
        /// <returns>the move's grade</returns>
        private int checkForTrap(Piece piece, Player player, Player enemy)
        {
            int grade = 0;
            if ((enemy.GetPiece(piece.ROW + player.Direction, piece.COL + 1) != null) && (enemy.GetPiece(piece.ROW - player.Direction, piece.COL - 1) == null))
            {
                grade = -4;
            }
            if ((enemy.GetPiece(piece.ROW + player.Direction, piece.COL - 1) != null)&& (enemy.GetPiece(piece.ROW - player.Direction, piece.COL + 1) == null))
            {
                grade += -4;
            }
            return grade;
        }


        /// <summary>
        /// Cancel a  virtual move
        /// </summary>
        /// <param name="move"></param>
        /// <param name="comp"></param>
        private void UndoVirtualMove(Move move, bool comp)
        {
            board.currentPlayer = comp ? this : human;
            board.otherPlayer = comp ? human : this;
            board.oldKey = move.Dest.X * N + move.Dest.Y;
            int curKey = move.PieceToMove.ROW * N + move.PieceToMove.COL;
            //board.oldKey = move.PieceToMove.getKey();
            board.move(move.PieceToMove.ROW, move.PieceToMove.COL, true, move.PieceToMove.IsQueen());
            Piece curPiece = board.currentPlayer.GetPiece(curKey);
            if (curPiece != null && curPiece.virtQUEEN)
            {
                undoVirtualQueen(move.PieceToMove.ROW, move.PieceToMove.COL);
            }
            if (move.Eat.Count == 1)// regular eat undo
            {
                if (move.Eat[0].COLOR == board.otherPlayer.getColor())
                {
                    board.otherPlayer.Add(move.Eat[0].ROW, move.Eat[0].COL);
                    // if some how virtual turns affected queen state, change it back
                    if (move.Eat[0] != null && move.Eat[0].IsQueen() != board.otherPlayer.GetPiece(move.Eat[0].getKey()).IsQueen())
                    {
                        board.otherPlayer.GetPiece(move.Eat[0].getKey()).toggleQueen();
                    }
                } else
                {
                    board.currentPlayer.Add(move.Eat[0].ROW, move.Eat[0].COL);
                    // if some how virtual turns affected queen state, change it back
                    if (move.Eat[0] != null && move.Eat[0].IsQueen() != board.currentPlayer.GetPiece(move.Eat[0].getKey()).IsQueen())
                    {
                        board.currentPlayer.GetPiece(move.Eat[0].getKey()).toggleQueen();
                    }
                }
                Console.WriteLine("added because of eat?");
            }
            else if (move.Eat.Count > 1) //irregular eat undo
            {
                foreach (Piece piece in move.Eat)
                {
                    if(piece.COLOR == board.otherPlayer.getColor())
                    {
                        board.otherPlayer.Add(piece.ROW, piece.COL);
                        // if some how virtual turns affected queen state, change it back
                        if (piece != null &&  piece.IsQueen() != board.otherPlayer.GetPiece(piece.getKey()).IsQueen())
                        {
                            board.otherPlayer.GetPiece(piece.getKey()).toggleQueen();
                        }
                        Console.WriteLine("added because of eat? 2" + piece.ROW + " " + piece.COL);
                    } else
                    {
                        board.currentPlayer.Add(piece.ROW, piece.COL);
                        // if some how virtual turns affected queen state, change it back
                        if (piece != null && piece.IsQueen() != board.currentPlayer.GetPiece(piece.getKey()).IsQueen())
                        {
                            board.currentPlayer.GetPiece(piece.getKey()).toggleQueen();
                        }
                        Console.WriteLine("added because of eat? 2" + piece.ROW + " " + piece.COL);
                    }
                }
            }
        }


        /// <summary>
        /// Test a given move without actually committing to it (will be cancelled by the UndoVirtualMove)
        /// </summary>
        /// <param name="move"></param>
        /// <param name="comp"></param>
        private void DoVirtualMove(Move move, bool comp)
        {
            int difcol;
            board.currentPlayer = comp ? this : human;
            board.otherPlayer = comp ? human : this;
            board.oldKey = move.PieceToMove.ROW * N + move.PieceToMove.COL;
            ///  Piece oldPiece = board.currentPlayer.GetPiece(board.oldKey); 
            Piece realPiece = move.PieceToMove;
            if (move.Eat.Count == 1) // regular eat
            {
                difcol = move.Dest.Y - board.oldKey % Board.N;
                pieceToEat.ROW = move.Dest.X - board.currentPlayer.Direction;
                pieceToEat.COL = (move.Dest.Y - difcol / 2);
                pieceToEat.COLOR = board.otherPlayer.GetPlayerColor();
                board.oldKey = move.PieceToMove.getKey();
                board.moveAndEat(move.Dest.X, move.Dest.Y, difcol, true, move.PieceToMove.IsQueen());
            }
            else if (move.Eat.Count > 1) //irregular eat
            {
                 foreach (Piece piece in move.Eat)
                    {
                        board.otherPlayer.Delete(piece.getKey());
                    }
                board.oldKey = move.PieceToMove.getKey();
                board.move(move.Dest.X, move.Dest.Y, true, move.PieceToMove.IsQueen()); // it also changes turn*/
            }
            else //count = 0, not eating
            {
                board.oldKey = move.PieceToMove.getKey();
                board.move(move.Dest.X, move.Dest.Y, true, move.PieceToMove.IsQueen());
            }                  
        }


        /// <summary>
        /// Get current Player's valid moves
        /// </summary>
        /// <param name="comp"></param>
        /// <returns>List of valid moves</returns>
        private List<Move> getValidMoves(bool comp)
        {
            List<Move> validMoves = new List<Move>();
   
            Player player = comp ? this : human;
            foreach (Piece piece in player.Pieces.Values)
            {
                validMoves.AddRange(this.getPieceValidMoves(piece, comp));

                validMoves.AddRange(this.getIrregularValidMoves(piece, comp));

            }


            return validMoves;
        }

        /// <summary>
        /// Get the selected piece's valid moves
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="comp"></param>
        /// <returns>List of valid moves</returns>
        private List<Move> getPieceValidMoves(Piece piece,bool comp) 
        {
            Player player = comp ? this : human;
            Player enemy = comp ? human : this;
            List<Move> validMoves = new List<Move>();

            if (piece.IsQueen() || piece.virtQUEEN)
            {
                validMoves = getQueenValidMoves(piece, comp);
            }
            else
            {
                List<Move> checkEatMoves = new List<Move>();
                List<Point> possiblePoints = new List<Point> { new Point(piece.ROW + player.Direction,piece.COL + 1),
                                                           new Point(piece.ROW + player.Direction, piece.COL - 1) };

                List<Point> validPoints = possiblePoints.Where(possiblePoint => board.isCubeEmptyAndValid(possiblePoint.X, possiblePoint.Y)).ToList();

                foreach (Point validPoint in validPoints)
                {
                    validMoves.Add(new Move(piece, validPoint));
                }
                List<Point3D> possiblePointsEat = new List<Point3D> { new Point3D(piece.ROW +2*player.Direction, piece.COL + 2,2),
                                                                  new Point3D(piece.ROW +2*player.Direction, piece.COL - 2,-2) };

                foreach (Point3D possiblePointEat in possiblePointsEat)
                {
                    Piece enemyPiece = board.moveAndEatCheck((int)possiblePointEat.X, (int)possiblePointEat.Y, (int)possiblePointEat.Z, player, enemy);
                    if (enemyPiece != null && board.isCubeEmptyAndValid((int)possiblePointEat.X, (int)possiblePointEat.Y))
                    {
                        validMoves.Add(new Move(piece, new Point((int)possiblePointEat.X, (int)possiblePointEat.Y), enemyPiece));
                        //  checkEatMoves.Add(new Move(piece, new Point((int)possiblePointEat.X, (int)possiblePointEat.Y), enemyPiece));
                        if (board.getPossibleMoves((int)possiblePointEat.X, (int)possiblePointEat.Y, false, false).Equals
                            ((piece, new Point((int)possiblePointEat.X, (int)possiblePointEat.Y), enemyPiece)))
                        {
                            checkEatMoves.Add(new Move(piece, new Point((int)possiblePointEat.X, (int)possiblePointEat.Y), enemyPiece));
                        }
                    }
                }
            }
            

            return validMoves;
        }


        /// <summary>
        /// gets the list of irregular valid moves
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="comp"></param>
        /// <returns>List of all valid irregular moves</returns>
       public List<Move> getIrregularValidMoves(Piece piece, bool comp)
        {
 
             List<int> visitedSpots = new List<int>(piece.getKey());
             List<Piece> enemyPiecesToDelete = new List<Piece>();
             List<Move> possibleIrregularMoves = new List<Move>();
             Piece movingTargertPiece = new Piece(piece);
             getPossibleIrregularMoves(piece, movingTargertPiece, visitedSpots, false /*end*/, enemyPiecesToDelete, possibleIrregularMoves,comp);
   
 
            return possibleIrregularMoves;



        }


        /// <summary>
        /// Get all the combinations of irregular moves recursively
        /// </summary>
        /// <param name="orgPiece"></param>
        /// <param name="movingTargertPiece"></param>
        /// <param name="visitedSpots"></param>
        /// <param name="end"></param>
        /// <param name="enemyPiecesToDelete"></param>
        /// <param name="possibleIrregularMoves"></param>
        /// <returns>false for continue true for end (exit condition)</returns>
        public bool getPossibleIrregularMoves(Piece orgPiece, Piece movingTargertPiece, List<int> visitedSpots, bool end, List<Piece> enemyPiecesToDelete, List<Move> possibleIrregularMoves,bool comp)
        {
            Player player = comp ? this : human;
            Player enemy = comp ? human : this;
            if (end) // תנאי עצירה
            {
                possibleIrregularMoves.Add(new Move(orgPiece, new Point(movingTargertPiece.ROW, movingTargertPiece.COL), enemyPiecesToDelete)); //check if redundent
                return true;
            }

            List<int> nextPossibleMoves = board.getPossibleMoves(movingTargertPiece.ROW, movingTargertPiece.COL, enemyPiecesToDelete.Count() > 0, orgPiece.QUEEN); 
            foreach (int possibleMove in nextPossibleMoves)
            {
                if (visitedSpots.IndexOf(possibleMove) != -1) //כל עוד לא הגעתי למקום הנבחר - ז"א הוא לא נמצא ברשימת המקומות שעברתי
                {
                    continue;
                }
                Piece enemyPieceOnKey = board.otherPlayer.GetPiece(possibleMove);
                if (enemyPieceOnKey != null) //המשך אכילה אחרי אכילה אחת
                {
                    int enemyPieceCol = enemyPieceOnKey.getKey() % N;
                    int enemyPieceRow = enemyPieceOnKey.getKey() / N;

                    int colDirMove = (enemyPieceCol - movingTargertPiece.COL); //הכיוון של ההתקדמות בטור האכילה הקודמת 
                    int rowDirMove = (enemyPieceRow - movingTargertPiece.ROW);//הכיוון של ההתקדמות בשורת האכילה הקודמת
                    colDirMove = colDirMove >= 1 ? 1 : -1;
                    rowDirMove = rowDirMove >= 1 ? 1 : -1;

                    int nextCol = enemyPieceCol + colDirMove;
                    int nextRow = enemyPieceRow + rowDirMove;

                    if (board.isCubeEmptyAndValid(nextRow, nextCol))
                    {
                        visitedSpots.Add(enemyPieceOnKey.getKey());
                        enemyPiecesToDelete.Add(enemyPieceOnKey);
                        movingTargertPiece.ROW = nextRow;
                        movingTargertPiece.COL = nextCol;
                        end = getPossibleIrregularMoves(orgPiece, movingTargertPiece, visitedSpots, end, enemyPiecesToDelete, possibleIrregularMoves,comp);
                        if (end)
                        {
                            if (enemyPiecesToDelete.Count() > 1)
                            { //we have an irregular eat
                                possibleIrregularMoves.Add(new Move(orgPiece, new Point(movingTargertPiece.ROW, movingTargertPiece.COL), enemyPiecesToDelete));
                            }
                            return false;
                        }
                    } 
                 
                }
            }
            return true;
        }

        /// <summary>
        /// calculate all the possible moves for the queen and then add to list only the valid moves
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="comp"></param>
        /// <returns>List of valid moves</returns>
        public List<Move> getQueenValidMoves(Piece piece, bool comp)
        {
            Player player = comp ? this : human;
            Player enemy = comp ? human : this;
            List<Move> validMovesQueen = new List<Move>();
            List<Point> possiblePointsQueen = new List<Point>();
            board.currentPlayer = comp ? this : human;
            board.otherPlayer = comp ? human : this;

            for (int i=1;i<N;i++)
            {
                possiblePointsQueen.Add(new Point(piece.ROW + (player.Direction * i), piece.COL + i));
                possiblePointsQueen.Add(new Point(piece.ROW + (player.Direction * i), piece.COL - i));
                possiblePointsQueen.Add(new Point(piece.ROW - (player.Direction * i), piece.COL + i));
                possiblePointsQueen.Add(new Point(piece.ROW - (player.Direction * i), piece.COL - i));
            }
            List<Point> validPointsQueen = possiblePointsQueen.Where
                (possiblePoint => board.isCubeEmptyAndValid(possiblePoint.X, possiblePoint.Y)).ToList();
            foreach (Point validPoint in validPointsQueen)
            {
                if (secondClickQueenCheck(piece.getKey(),player,validPoint.X, validPoint.Y, enemy))
                {
                    validMovesQueen.Add(new Move(piece, validPoint));
                }
                
            }
            return validMovesQueen;
        }


        /// <summary>
        /// check if the queen's move is valid
        /// </summary>
        /// <param name="oldKey"></param>
        /// <param name="currentPlayer"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="otherPlayer"></param>
        /// <returns> return true if the queen's move is valid else return false </returns>
        private bool secondClickQueenCheck(int oldKey, Player currentPlayer, int row, int col, Player otherPlayer)
        {

            int deleteKey = 0;
            int count = 0;
            int oldCol = oldKey % N;
            int oldRow = oldKey / N;
            int newCol = 0;
            int newRow = 0;
            bool isValid = false;

            if (Math.Abs(oldCol - col) == Math.Abs(oldRow - row)) // check if new location is on the same diag.
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
                    else if (currentPlayer.GetPiece(newRow, newCol) != null)//check that my player doesn't have a piece in the new spot
                    {
                        count = -1;
                    }
                }

                if (count == 0 || count == 1) //can move with or withot eating one piece
                {
                    isValid = true;
                }

            }

            return isValid;
        }

 
    }
}