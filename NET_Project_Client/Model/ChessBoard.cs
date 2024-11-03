using NET_Project_Client.Model.ChessPieces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NET_Project_Client.Model
{
    internal class ChessBoard
    {
        public Piece[,] chessBoard;
        bool turn;
        public bool check;
        public List<Coordinate> coordinateList;
        Form view;
        int move = 1;
        bool type;
        int isPromo = 0;
        int promoteTo = 9;

        public ChessBoard(Form f, bool type)
        {
            view = f;
            turn = false;
            chessBoard = new Piece[8, 4];
            check = false;
            InitializeBoard();
            CalculateMovesForAllPieces(chessBoard, true);
            this.type = type;
        }

        private bool IsPawnPromoting(Piece p, int row, int col)
        {
            if (p is Pawn)
            {
                if (p.getRow() == 1 && row == 0 && p.getColor() == turn)
                {
                    return true;
                }
                if (p.getRow() == 6 && row == 7 && p.getColor() == turn)
                {
                    return true;
                }
            }
            return false;
        }

        public bool MakeMove(Coordinate begin, Coordinate end, string PieceType)
        {
            bool victory = false;
            int kingRow = -1, kingCol = -1, trRow = -1, trCol = -1;

            chessBoard[end.y, end.x] = chessBoard[begin.y, begin.x];
            chessBoard[end.y, end.x].setLoc(end.y, end.x);
            chessBoard[begin.y, begin.x] = null;

            if (type)
            {
                 SaveMoveToDB(begin, end, isPromo, promoteTo);
            }

            if(PieceType != "")
            {
                ChangePiece(end, PieceType);
            }            
                
                
            CalculateMovesForAllPieces(chessBoard, true);
            
            check = checkIfCheck(chessBoard,out kingRow, out kingCol, out trRow, out trCol);
            if (check)
            {
                setFocus(kingRow, kingCol, trRow, trCol);
                ((King)chessBoard[kingRow,kingCol]).setPicture(true);
                victory = checkVictory();
            }
            else
            {
                if(chessBoard[end.y,end.x] is King)
                {
                    ((King)chessBoard[end.y, end.x]).setPicture(false);
                }
            }

            if (!turn && type)
                ((Form1)view).switchTurn();

            turn = !turn;
            move++;
            return victory;
        }

        private void ChangePiece(Coordinate p, string PieceType)
        {
            switch (PieceType)
            {
                case "Rook":
                    {
                        chessBoard[p.y, p.x] = new Rook(chessBoard[p.y, p.x].getColor(), new Coordinate(p.y, p.x));
                        ((Form1)view).promotions.Add(move, 3);
                        break;
                    }
                case "Bishop":
                    {
                        chessBoard[p.y, p.x] = new Bishop(chessBoard[p.y, p.x].getColor(), new Coordinate(p.y, p.x));
                        ((Form1)view).promotions.Add(move, 1);
                        break;
                    }
                case "Knight":
                    {
                        chessBoard[p.y, p.x] = new Knight(chessBoard[p.y, p.x].getColor(), new Coordinate(p.y, p.x));
                        ((Form1)view).promotions.Add(move, 2);
                        break;
                    }
            }
        }

        public bool checkVictory()
        {
            bool victory = true;
            for (int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 4; j++)
                {
                    if (chessBoard[i, j] != null)
                    {
                        if (chessBoard[i,j].getColor() != turn && chessBoard[i,j].AvailableMoves.Count() > 0)
                            victory = false;
                    }
                }
            }
            return victory;
        }

        private bool checkIfCheck(Piece[,] Board, out int kingRow, out int kingCol, out int trRow, out int trCol)
        {
            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for(int j = 0; j < Board.GetLength(1); j++)
                {
                    if (Board[i,j] != null)
                    {
                        int size = Board[i, j].AvailableMoves.Count();
                        for (int k = 0; k < size ; k++)
                        {
                            if(Board[Board[i, j].AvailableMoves[k].y, Board[i, j].AvailableMoves[k].x] is King)
                            {
                                kingRow = Board[i, j].AvailableMoves[k].y;
                                kingCol = Board[i, j].AvailableMoves[k].x;
                                trRow = i; trCol = j;
                                return true;
                            }
                        }
                        if (Board[i, j] is Pawn)
                        {
                            size = ((Pawn)Board[i, j]).Threatening.Count();
                            for (int k = 0; k < size; k++)
                            {
                                if (Board[((Pawn)Board[i, j]).Threatening[k].y, ((Pawn)Board[i, j]).Threatening[k].x] is King && (Board[i, j].getColor() != Board[((Pawn)Board[i, j]).Threatening[k].y, ((Pawn)Board[i, j]).Threatening[k].x].getColor()))
                                {
                                    kingRow = ((Pawn)Board[i, j]).Threatening[k].y;
                                    kingCol = ((Pawn)Board[i, j]).Threatening[k].x;
                                    trRow = i; trCol = j;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            kingRow = -1; kingCol = -1; trRow = -1; trCol = -1; 
            return false;
        }

        private void removeIllegalMovesForKing()
        {
            int kingRow = -1, kingCol = -1, trRow = -1, trCol = -1;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (chessBoard[i, j] != null)
                    {
                        if(chessBoard[i, j] is King)
                            ((King)chessBoard[i, j]).removeMoves(chessBoard);
                        for (int k = 0; k < chessBoard[i, j].AvailableMoves.Count(); k++)
                        {
                            Piece[,] copy = getCopyChessboard();
                            copy[chessBoard[i, j].AvailableMoves[k].y, chessBoard[i, j].AvailableMoves[k].x] = copy[i,j];
                            copy[chessBoard[i, j].getRow(), chessBoard[i, j].getCol()] = null;
                            CalculateMovesForAllPieces(copy, false);
                            if (checkIfCheck(copy, out kingRow, out kingCol, out trRow, out trCol))
                            {
                                chessBoard[i, j].AvailableMoves.RemoveAt(k);
                                k--;
                            }
                            chessBoard[i, j].setLoc(i, j);
                        }
                    }
                }
            }
        }

        private Piece[,] getCopyChessboard()
        {
            Piece[,] copy = new Piece[8, 4];
            for (int i = 0;i < 8;i++)
            {
                for(int j = 0;j < 4;j++)
                {
                    if(chessBoard[i, j] != null)
                        if(chessBoard[i, j] is Pawn)
                            copy[i,j] = new Pawn(chessBoard[i, j].getColor(), new Coordinate(i, j));
                        if (chessBoard[i, j] is King)
                            copy[i, j] = new King(chessBoard[i, j].getColor(), new Coordinate(i, j));
                        if (chessBoard[i, j] is Rook)
                            copy[i, j] = new Rook(chessBoard[i, j].getColor(), new Coordinate(i, j));
                        if (chessBoard[i, j] is Bishop)
                            copy[i, j] = new Bishop(chessBoard[i, j].getColor(), new Coordinate(i, j));
                        if (chessBoard[i, j] is Knight)
                            copy[i, j] = new Knight(chessBoard[i, j].getColor(), new Coordinate(i, j));
                }
            }
            return copy;
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

        public List<List<int>> CalculateMovesForServer()
        {
            List<List<int>> availableMoves = new List<List<int>>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Piece p = chessBoard[i,j];
                    if (p != null)
                        if (p.getColor())
                        {
                            List<int> numbers = new List<int>();
                            for (int k = 0; k < p.AvailableMoves.Count(); k++)
                            {

                                int number = (p.getRow() * 1000) + (p.getCol() * 100) + (p.AvailableMoves[k].y * 10) + p.AvailableMoves[k].x;
                                numbers.Add(number);
                            }
                            availableMoves.Add(numbers);
                        }
                }
            }
            return availableMoves;
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
            if (result.Count() > 0)
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



        private void SaveMoveToDB(Coordinate begin, Coordinate end,int isPromo, int promoteTo)
        {
            ((Form1)view).InsertMoveIntoDB(move, begin.y, begin.x, end.y, end.x,isPromo,promoteTo);
        }
        public int getMoveId()
        {
            return move;
        }

        public void CalculateMovesForAllPieces(Piece[,] Board, bool recursion)
        {
            for (int i = 0; i <= 7; i++)
            {
                for (int j = 0; j <= 3; j++)
                    if (Board[i,j] != null)
                        Board[i, j].CalculateMoves(Board);
            }
            if(recursion)
                removeIllegalMovesForKing();
        }

        public void RefreshMoveForPiece(int row, int col)
        {
            CalculateMovesForAllPieces(chessBoard, true);
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

        public void SetPieceAt(Piece newpiece,int row, int col)
        {
            this.chessBoard[row, col] = newpiece;
        }
    }
}
