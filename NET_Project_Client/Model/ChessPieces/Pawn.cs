using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Pawn : Piece
    {
        public Pawn(bool color, Coordinate loc) : base(color, loc){}

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            if(color == false)
            {
                if(loc.y - 1 >= 0)
                    if (GameBoard[loc.x, loc.y - 1] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.x, loc.y - 1));
                    }
                if(loc.x - 1 >= 0 && loc.y - 1 >= 0)
                {
                    if (GameBoard[loc.x - 1, loc.y - 1] != null && GameBoard[loc.x - 1, loc.y - 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.x - 1, loc.y - 1));
                    }
                }
                if (loc.x + 1 <= 3 && loc.y - 1 >= 0)
                {
                    if (GameBoard[loc.x + 1, loc.y - 1] != null && GameBoard[loc.x + 1, loc.y - 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.x + 1, loc.y - 1));
                    }
                }
            }
            else
            {
                if(loc.y + 1 <= 7)
                    if (GameBoard[loc.x, loc.y + 1] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.x, loc.y + 1));
                    }
                if (loc.x - 1 >= 0 && loc.y + 1 >= 0)
                {
                    if (GameBoard[loc.x - 1, loc.y + 1] != null && GameBoard[loc.x - 1, loc.y + 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.x - 1, loc.y + 1));
                    }
                }
                if (loc.x + 1 <= 3 && loc.y + 1 >= 0)
                {
                    if (GameBoard[loc.x + 1, loc.y + 1] != null && GameBoard[loc.x + 1, loc.y + 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.x + 1, loc.y + 1));
                    }
                }
            }
            if (loc.x + 1 <= 3)
                if (GameBoard[loc.x + 1, loc.y] == null)
                {
                    AvailableMoves.Add(new Coordinate(loc.x + 1, loc.y));
                }
            if (loc.x - 1 >= 0)
                if (GameBoard[loc.x - 1, loc.y] == null)
                {
                    AvailableMoves.Add(new Coordinate(loc.x - 1, loc.y));
                }
        }
    }
}
