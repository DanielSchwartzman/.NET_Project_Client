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
        }

        public void MakeMove(Coordinate begin, Coordinate end)
        {
            chessBoard[end.x,end.y] = chessBoard[begin.x,begin.y];
            chessBoard[begin.x, begin.y] = null;

            SaveMoveToDB(begin, end);

            CalculateMovesForAllPieces();

            //Call Display

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
                    chessBoard[i, j].CalculateMoves(chessBoard);
            }
        }
    }
}
