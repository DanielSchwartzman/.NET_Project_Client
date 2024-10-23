using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Rook : Piece
    {
        public Rook(bool color, Coordinate loc) : base(color, loc) 
        {
            if (color)
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/rook_black.png");
            else
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/rook_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            int helpX, helpY;

            helpX = loc.x; helpY = loc.y - 1;
            while (helpY >= 0)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpY = helpY - 1;
            }

            helpX = loc.x; helpY = loc.y + 1;
            while (helpY <= 7)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpY = helpY + 1;
            }

            helpX = loc.x - 1; helpY = loc.y;
            while (helpX >= 0)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpX = helpX - 1;
            }

            helpX = loc.x + 1; helpY = loc.y;
            while (helpX <= 3)
            {
                if (GameBoard[helpX, helpY] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpX = helpX + 1;
            }
        }
    }
}
