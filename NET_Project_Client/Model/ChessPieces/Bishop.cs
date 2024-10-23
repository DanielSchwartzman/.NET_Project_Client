using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Bishop : Piece
    {
        public Bishop(bool color, Coordinate loc) : base(color, loc) { }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            int helpX, helpY;

            helpX = loc.x - 1; helpY = loc.y - 1;
            while(helpX >= 0 && helpY >= 0)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpX = helpX - 1;
                helpY = helpY - 1;
            }

            helpX = loc.x + 1; helpY = loc.y - 1;
            while (helpX <= 3 && helpY >= 0)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpX = helpX + 1;
                helpY = helpY - 1;
            }

            helpX = loc.x - 1; helpY = loc.y + 1;
            while (helpX >= 0 && helpY <= 7)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpX = helpX - 1;
                helpY = helpY + 1;
            }

            helpX = loc.x + 1; helpY = loc.y + 1;
            while (helpX <= 3 && helpY <= 7)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpX = helpX + 1;
                helpY = helpY + 1;
            }
        }
    }
}
