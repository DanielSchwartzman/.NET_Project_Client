using NET_Project_Client.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model
{
    internal class ChessBoard
    {
        public Piece[,] chessBoard;
        bool turn;
        public bool check;
        public List<Coordinate> coordinateList;

        public ChessBoard()
        {
            turn = false;
            chessBoard = new Piece[8, 4];
            check = false;
            InitializeBoard();
            CalculateMovesForAllPieces();
        }

        public void MakeMove(Coordinate begin, Coordinate end)
        {
            int kingRow = -1, kingCol = -1, trRow = -1, trCol = -1;
            chessBoard[end.y, end.x] = chessBoard[begin.y, begin.x];
            chessBoard[end.y, end.x].setLoc(end.y, end.x);
            chessBoard[begin.y, begin.x] = null;

            SaveMoveToDB(begin, end);

            CalculateMovesForAllPieces();
            check = checkIfCheck(out kingRow, out kingCol, out trRow, out trCol);
            if (check)
            {
                setFocus(kingRow, kingCol, trRow, trCol);
            }

            //SEND FOR RESPONSE HEREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
            ///////////////////////////////////////
            ///////////////////
            ////////////////////////
            ///
            turn = !turn;
        }

        private bool checkIfCheck(out int kingRow, out int kingCol, out int trRow, out int trCol)
        {
            for (int i = 0; i < chessBoard.GetLength(0); i++)
            {
                for(int j = 0; j < chessBoard.GetLength(1); j++)
                {
                    if (chessBoard[i,j] != null)
                    {
                        int size = chessBoard[i, j].AvailableMoves.Count();
                        for (int k = 0; k < size ; k++)
                        {
                            if(chessBoard[chessBoard[i, j].AvailableMoves[k].y, chessBoard[i, j].AvailableMoves[k].x] is King)
                            {
                                kingRow = chessBoard[i, j].AvailableMoves[k].y;
                                kingCol = chessBoard[i, j].AvailableMoves[k].x;
                                trRow = i; trCol = j;
                                return true;
                            }
                        }
                    }
                }
            }
            kingRow = -1; kingCol = -1; trRow = -1; trCol = -1; 
            return false;
        }

        private void setFocus(int kingRow, int kingCol, int trRow, int trCol)
        {
            coordinateList = getBlockLocation(kingRow, kingCol, trRow, trCol);
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (chessBoard[i,j] != null)
                        if (chessBoard[i,j].getColor() != turn && !(chessBoard[i,j] is King))
                        {
                            for (int k = 0; k < chessBoard[i, j].AvailableMoves.Count(); k++)
                            {
                                if(!checkIfContained(chessBoard[i, j].AvailableMoves[k]))
                                {
                                    chessBoard[i, j].AvailableMoves.RemoveAt(k);
                                    k--;
                                }
                            }
                        }
                }
            }

        }

        private bool checkIfContained(Coordinate c)
        {
            int size2 = coordinateList.Count();
            for (int i = 0; i < size2; i++)
            {
                if (coordinateList[i].Equals(c))
                    return true;
            }
            return false;
        }

        private List<Coordinate> getBlockLocation(int kingRow, int kingCol, int trRow, int trCol)
        {
            List<Coordinate> result = new List<Coordinate>();

            int rowDirection = Math.Sign(trRow - kingRow);
            int colDirection = Math.Sign(trCol - kingCol);

            if (rowDirection != 0 && colDirection != 0 && Math.Abs(trRow - kingRow) != Math.Abs(trCol - kingCol))
            {
                return result;
            }

            result.Add(new Coordinate(kingRow, kingCol));

            int currentRow = kingRow;
            int currentCol = kingCol;

            while (currentRow != trRow || currentCol != trCol)
            {
                currentRow += rowDirection;
                currentCol += colDirection;

                result.Add(new Coordinate(currentRow, currentCol));
            }
            if (result.Count() > 0) ;
                result.RemoveAt(0);

            return result;
        }

        public string coordinatecoordinateListString()
        {
            string sb = string.Empty;
            for (int i = 0; i < coordinateList.Count(); i++) 
            {
                sb += coordinateList[i].ToString();
            }
            return sb;
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
