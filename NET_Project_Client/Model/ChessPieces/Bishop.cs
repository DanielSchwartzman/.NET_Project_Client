using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class Bishop : Piece
    {
        public Bishop(bool color, Coordinate loc) : base(color, loc)
        {
            if (color)
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/bishop_black.png");
            else
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/bishop_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            int helpX, helpY;

            helpX = loc.x - 1; helpY = loc.y - 1;
            while(helpX >= 0 && helpY >= 0)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpX = helpX - 1;
                helpY = helpY - 1;
            }

            helpX = loc.x + 1; helpY = loc.y - 1;
            while (helpX <= 3 && helpY >= 0)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpX = helpX + 1;
                helpY = helpY - 1;
            }

            helpX = loc.x - 1; helpY = loc.y + 1;
            while (helpX >= 0 && helpY <= 7)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpX = helpX - 1;
                helpY = helpY + 1;
            }

            helpX = loc.x + 1; helpY = loc.y + 1;
            while (helpX <= 3 && helpY <= 7)
            {
                if (GameBoard[helpY, helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                helpX = helpX + 1;
                helpY = helpY + 1;
            }
        }
    }
}
