using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Knight : Piece
    {
        public Knight(bool color, Coordinate loc) : base(color, loc) { }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            if(loc.y - 2 >= 0)
            {
                if (loc.x - 1 >= 0)
                    if (GameBoard[loc.x - 1, loc.y - 2] != null && GameBoard[loc.x - 1, loc.y - 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x - 1, loc.y - 2));
                if (loc.x + 1 <= 3)
                    if (GameBoard[loc.x + 1, loc.y - 2] != null && GameBoard[loc.x + 1, loc.y - 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x + 1, loc.y - 2));
            }
            if(loc.y + 2 <= 7)
            {
                if (loc.x - 1 >= 0)
                    if (GameBoard[loc.x - 1, loc.y + 2] != null && GameBoard[loc.x - 1, loc.y + 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x - 1, loc.y + 2));
                if (loc.x + 1 <= 3)
                    if (GameBoard[loc.x + 1, loc.y + 2] != null && GameBoard[loc.x + 1, loc.y + 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x + 1, loc.y + 2));
            }
            if(loc.x + 2 <= 3)
            {
                if (loc.y - 1 >= 0)
                    if (GameBoard[loc.x + 2, loc.y - 1] != null && GameBoard[loc.x + 2, loc.y - 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x + 2, loc.y - 1));
                if (loc.y + 1 <= 7)
                    if (GameBoard[loc.x + 1, loc.y + 1] != null && GameBoard[loc.x + 2, loc.y + 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x + 2, loc.y + 1));
            }
            if(loc.x - 2 >= 0)
            {
                if (loc.y - 1 >= 0)
                    if (GameBoard[loc.x - 2, loc.y - 1] != null && GameBoard[loc.x - 2, loc.y - 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x - 2, loc.y - 1));
                if (loc.y + 1 <= 7)
                    if (GameBoard[loc.x - 2, loc.y + 1] != null && GameBoard[loc.x - 2, loc.y + 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.x - 2, loc.y + 1));
            }
        }
    }
}
