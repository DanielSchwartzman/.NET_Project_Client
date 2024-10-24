using NET_Project_Client.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model
{
    internal class ChessBoard
    {
        public Piece[,] chessBoard;
        public ChessBoard()
        {
            chessBoard = new Piece[8, 4];
            InitializeBoard();
            CalculateMovesForAllPieces();
        }

        public void MakeMove(Coordinate begin, Coordinate end)
        {
            chessBoard[end.y, end.x] = chessBoard[begin.y, begin.x];
            chessBoard[end.y, end.x].setLoc(end.y, end.x);
            chessBoard[begin.y, begin.x] = null;

            SaveMoveToDB(begin, end);

            CalculateMovesForAllPieces();

            //SEND FOR RESPONSE HEREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
            ///////////////////////////////////////
            ///////////////////
            ////////////////////////
            ///

        }

        private void SaveMoveToDB(Coordinate begin, Coordinate end)
        {

        }

        private void CalculateMovesForAllPieces()
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 3; j++)
                    if (chessBoard[i,j] != null)
                        chessBoard[i, j].CalculateMoves(chessBoard);
            }
        }

        private void InitializeBoard()
        {
            chessBoard[0, 3] = new Rook(true, new Coordinate(0, 3));
            chessBoard[7, 3] = new Rook(false, new Coordinate(7, 3));

            chessBoard[0, 1] = new Bishop(true, new Coordinate(0, 1));
            chessBoard[7, 1] = new Bishop(false, new Coordinate(7, 1));

            chessBoard[0, 2] = new Knight(true, new Coordinate(0, 2));
            chessBoard[7, 2] = new Knight(false, new Coordinate(7, 2));


            chessBoard[1, 0] = new Pawn(true, new Coordinate(1, 0));
            chessBoard[1, 1] = new Pawn(true, new Coordinate(1, 1));
            chessBoard[1, 2] = new Pawn(true, new Coordinate(1, 2));
            chessBoard[1, 3] = new Pawn(true, new Coordinate(1, 3));

            chessBoard[6, 0] = new Pawn(false, new Coordinate(6, 0));
            chessBoard[6, 1] = new Pawn(false, new Coordinate(6, 1));
            chessBoard[6, 2] = new Pawn(false, new Coordinate(6, 2));
            chessBoard[6, 3] = new Pawn(false, new Coordinate(6, 3));

            chessBoard[0, 0] = new King(true, new Coordinate(0, 0));
            chessBoard[7, 0] = new King(false, new Coordinate(7, 0));
        }

        public Piece GetPieceAt(int row, int col)
        {
            return chessBoard[row, col];
        }
    }
}
