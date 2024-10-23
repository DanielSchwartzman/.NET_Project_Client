using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Rook : Piece
    {
        public Rook(bool color, Coordinate loc) : base(color, loc) { }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            int helpX, helpY;

            helpX = loc.x; helpY = loc.y - 1;
            while (helpY >= 0)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpY = helpY - 1;
            }

            helpX = loc.x; helpY = loc.y + 1;
            while (helpY <= 7)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpY = helpY + 1;
            }

            helpX = loc.x - 1; helpY = loc.y;
            while (helpX >= 0)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpX = helpX - 1;
            }

            helpX = loc.x + 1; helpY = loc.y;
            while (helpX <= 3)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpX, helpY].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpX, helpY));
                helpX = helpX + 1;
            }
        }
    }
}
