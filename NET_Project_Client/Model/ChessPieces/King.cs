using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class King : Piece
    {
        public King(bool color, Coordinate loc) : base(color, loc) { }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            int helpX, helpY;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    helpX = loc.x + i;
                    helpY = loc.y + j;
                    if(helpY >= 0 && helpY <= 7 && helpX >= 0 && helpX <= 3 && (helpX != loc.x && helpY != loc.y))
                    {
                        if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                            AvailableMoves.Add(new Coordinate(helpX, helpY));
                    }
                }
            }
        }
    }
}
