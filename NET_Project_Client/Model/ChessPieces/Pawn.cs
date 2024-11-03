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
        public List<Coordinate> Threatening;
        public Pawn(bool color, Coordinate loc) : base(color, loc)
        {
            if(color)
                img = Image.FromFile("./../../../Resources/pawn_black.png");
            else
                img = Image.FromFile("./../../../Resources/pawn_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            AvailableMoves = new List<Coordinate>();
            Threatening = new List<Coordinate>();

            if (color == false)//white
            {
                if(loc.y - 1 >= 0)
                    if (GameBoard[loc.y - 1, loc.x] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y - 1, loc.x));
                    }
                if (loc.x - 1 >= 0 && loc.y - 1 >= 0)
                {
                    Threatening.Add(new Coordinate(loc.y - 1, loc.x - 1));
                }
                if (loc.x + 1 <= 3 && loc.y - 1 >= 0)
                {
                    Threatening.Add(new Coordinate(loc.y - 1, loc.x + 1));
                }
                if (loc.y - 2 >= 0 && loc.y == 6)
                    if (GameBoard[loc.y - 2, loc.x] == null && GameBoard[loc.y - 1, loc.x] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y - 2, loc.x));
                    }
            }
            else//black
            {
                if(loc.y + 1 <= 7)
                    if (GameBoard[loc.y + 1, loc.x] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y + 1, loc.x));
                    }
                if (loc.x - 1 >= 0 && loc.y + 1 >= 0 && loc.y + 1 <= 7)
                {
                    Threatening.Add(new Coordinate(loc.y + 1, loc.x - 1));
                }
                if (loc.x + 1 <= 3 && loc.y + 1 >= 0 && loc.y + 1 <= 7)
                {
                    Threatening.Add(new Coordinate(loc.y + 1, loc.x + 1));
                }
                if (loc.y + 2 <= 7 && loc.y == 1)
                    if (GameBoard[loc.y + 2, loc.x] == null && GameBoard[loc.y + 1, loc.x] == null)
                    {
                        AvailableMoves.Add(new Coordinate(loc.y + 2, loc.x));
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
