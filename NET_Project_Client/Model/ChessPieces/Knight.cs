using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Knight : Piece
    {
        public Knight(bool color, Coordinate loc) : base(color, loc)
        {
            if (color)
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/knight_black.png");
            else
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/knight_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            if(loc.y - 2 >= 0)
            {
                if (loc.x - 1 >= 0)
                    if (GameBoard[loc.y - 2, loc.x - 1] != null && GameBoard[loc.y - 2, loc.x - 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y - 2, loc.x - 1));
                if (loc.x + 1 <= 3)
                    if (GameBoard[loc.y - 2, loc.x + 1] != null && GameBoard[loc.y - 2, loc.x + 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y - 2, loc.x + 1));
            }
            if(loc.y + 2 <= 7)
            {
                if (loc.x - 1 >= 0)
                    if (GameBoard[loc.y + 2, loc.x - 1] != null && GameBoard[loc.y + 2, loc.x - 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y + 2, loc.x - 1));
                if (loc.x + 1 <= 3)
                    if (GameBoard[loc.y + 2, loc.x + 1] != null && GameBoard[loc.y + 2, loc.x + 1].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y + 2, loc.x + 1));
            }
            if(loc.x + 2 <= 3)
            {
                if (loc.y - 1 >= 0)
                    if (GameBoard[loc.y - 1, loc.x + 2] != null && GameBoard[loc.y - 1, loc.x + 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y - 1, loc.x + 2));
                if (loc.y + 1 <= 7)
                    if (GameBoard[loc.y + 1, loc.x + 1] != null && GameBoard[loc.y + 1, loc.x + 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y + 1, loc.x + 2));
            }
            if(loc.x - 2 >= 0)
            {
                if (loc.y - 1 >= 0)
                    if (GameBoard[loc.y - 1, loc.x - 2] != null && GameBoard[loc.y - 1, loc.x - 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y - 1, loc.x - 2));
                if (loc.y + 1 <= 7)
                    if (GameBoard[loc.y + 1, loc.x - 2] != null && GameBoard[loc.y + 1, loc.x - 2].getColor() != color)
                        AvailableMoves.Add(new Coordinate(loc.y + 1, loc.x - 2));
            }
        }
    }
}
