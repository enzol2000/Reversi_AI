using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;
using FullSailAFI.GamePlaying.CoreAI;

namespace FullSailAFI.GamePlaying
{
    public class StudentAI : Behavior
    {
        TreeVisLib treeVisLib;  // lib functions to communicate with TreeVisualization
        bool visualizationFlag = false;  // turn this on to use tree visualization (which you will have to implement via the TreeVisLib API)
                                         // WARNING: Will hang program if the TreeVisualization project is not loaded!

        public StudentAI()
        {
            if (visualizationFlag == true)
            {
                if (treeVisLib == null)  // should always be null, but just in case
                    treeVisLib = TreeVisLib.getTreeVisLib();  // WARNING: Creation of this object will hang if the TreeVisualization project is not loaded!
            }
        }


        //
        // This function starts the look ahead process to find the best move
        // for this player color.
        //
        public ComputerMove Run(int _nextColor, Board _board, int _lookAheadDepth)
        {
            double Alpha = Double.NegativeInfinity;
            double Beta = Double.PositiveInfinity;

            ComputerMove nextMove = GetBestMove(_nextColor, _board, 0, _lookAheadDepth, Alpha, Beta);

            return nextMove;
        }


        private List<ComputerMove> GetAllValidMoves(Board board, int color)
        {
            List<ComputerMove> moves = new List<ComputerMove>();

            // Check all board positions.
            int i, j;
            for (i = 0; i < Board.Height; i++)
                for (j = 0; j < Board.Width; j++)
                    // If the move is valid for the color, bump the count.
                    if (board.IsValidMove(color, i, j))
                    {
                        ComputerMove move = new ComputerMove(i, j);
                        moves.Add(move);
                    }

            return moves;
        }

        //
        // This function uses look ahead to evaluate all valid moves for a
        // given player color and returns the best move it can find. This
        // method will only be called if there is at least one valid move
        // for the player of the designated color.
        //
        private ComputerMove GetBestMove(int color, Board board, int depth, int depthLimit, double _alpha, double _beta)
        {
            //TODO: the lab
            
            ComputerMove BestMove = null;
            Board newState = new Board();
            double Alpha = _alpha;
            double Beta = _beta;

            List<ComputerMove> moves = GetAllValidMoves(board, color);

            for (int i = 0; i < moves.Count; i++)
            {
                newState.Copy(board);
                newState.MakeMove(color, moves[i].row, moves[i].col);
                // newState no board down 
                if (newState.IsTerminalState() || depth == depthLimit)
                {
                    moves[i].rank = Evaluate(newState);
                }
                else
                {
                    if (newState.HasAnyValidMove(GetNextPlayer(color, newState)))
                    {
                        moves[i].rank = GetBestMove(GetNextPlayer(color, newState), newState, depth + 1, depthLimit, Alpha, Beta).rank;
                    }
                }
                // black or white can change rank 
                if (BestMove == null ||
                    moves[i].rank > BestMove.rank && color > 0 ||
                    moves[i].rank < BestMove.rank && color < 0)
                {
                    BestMove = moves[i];
                    #region alpha and beta
                    if ( color > 0 && BestMove.rank > Alpha)
                    {
                        Alpha = BestMove.rank;
                    }
                    else if ( color < 0 && BestMove.rank < Beta)
                    {
                        Beta = BestMove.rank;
                    }
                    if (Alpha >= Beta)
                    {
                        break;
                    }





                    /*
                    heuristic to make path faster 
                    given cost is to start to that point
                    take cheapies - most effcient resul,

                    weight change 

                    decision tree	
                    */





                    #endregion

                }

            }

            return BestMove;
        }

        private int GetNextPlayer(int player, Board gameState)
        {
            if (gameState.HasAnyValidMove(-player))
                return -player;
            else
                return player;
        }

        #region Recommended Helper Functions

        private int Evaluate(Board _board)
        {
            //determine score based on position of pieces

            
            int[,] squares = new int[Board.Height, Board.Width]; ;


            // Check all board positions.
            int i, j;
            for (i = 0; i < Board.Height; i++)
            {
                for (j = 0; j < Board.Width; j++)
                {
                    if ((j == 0 || j == Board.Width - 1) && (i == 0 || i == Board.Height - 1))
                    {
                        squares[i, j] = _board.GetSquareContents(i, j) * 100;
                    }
                    else if (j == 0 || j == Board.Width - 1 || i == 0 || i == Board.Height - 1)
                    {
                        squares[i, j] = _board.GetSquareContents(i, j) * 10;
                    }
                    else
                    {
                        squares[i, j] = _board.GetSquareContents(i, j);
                    }
                } 

            }
            int result = 0;

            for (i = 0; i < Board.Height; i++)
            {
                for (j = 0; j < Board.Width; j++)
                {
                    result += squares[i, j];
                }

            }
            
            if (_board.IsTerminalState())
            {
                if (result >= 0)
                {
                    result += 10000;
                }
                else
                {
                    result -= 10000;
                }
            }

            return result; 
        }

        #endregion

    }
}
