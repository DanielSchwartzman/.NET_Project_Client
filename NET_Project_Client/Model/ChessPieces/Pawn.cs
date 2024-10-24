using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Pawn : Piece
    {
        public Pawn(bool color, Coordinate loc) : base(color, loc)
        {
            if(color)
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/pawn_black.png");
            else
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/pawn_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            AvailableMoves = new List<Coordinate>();
            if (color == false)
            {
                if(loc.y - 1 >= 0)
                    if (GameBoard[loc.y - 1, loc.x] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y - 1, loc.x));
                    }
                if(loc.x - 1 >= 0 && loc.y - 1 >= 0)
                {
                    if (GameBoard[loc.y - 1, loc.x - 1] != null && GameBoard[loc.y - 1, loc.x - 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y - 1, loc.x - 1));
                    }
                }
                if (loc.x + 1 <= 3 && loc.y - 1 >= 0)
                {
                    if (GameBoard[loc.y - 1, loc.x + 1] != null && GameBoard[loc.y - 1, loc.x + 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y - 1, loc.x + 1));
                    }
                }
            }
            else
            {
                if(loc.y + 1 <= 7)
                    if (GameBoard[loc.y + 1, loc.x] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y + 1, loc.x));
                    }
                if (loc.x - 1 >= 0 && loc.y + 1 >= 0)
                {
                    if (GameBoard[loc.y + 1, loc.x - 1] != null && GameBoard[loc.y + 1, loc.x - 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y + 1, loc.x - 1));
                    }
                }
                if (loc.x + 1 <= 3 && loc.y + 1 >= 0)
                {
                    if (GameBoard[loc.y + 1, loc.x + 1] != null && GameBoard[loc.y + 1, loc.x + 1].getColor() != color)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y + 1, loc.x + 1));
                    }
                }
            }
            if (loc.x + 1 <= 3)
                if (GameBoard[loc.y, loc.x + 1] == null)
                {
                    AvailableMoves.Add(new Coordinate(loc.y, loc.x + 1));
                }
            if (loc.x - 1 >= 0)
                if (GameBoard[loc.y, loc.x - 1] == null)
                {
                    AvailableMoves.Add(new Coordinate(loc.y, loc.x - 1));
                }
        }
    }
}
