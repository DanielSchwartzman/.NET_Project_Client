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
                img = Image.FromFile("./../../../Resources/bishop_black.png");
            else
                img = Image.FromFile("./../../../Resources/bishop_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            AvailableMoves = new List<Coordinate>();
            bool stop = false;
            int helpX, helpY;

            helpX = loc.x - 1; helpY = loc.y - 1;
            while(helpX >= 0 && helpY >= 0 && !stop)
            {
                if (GameBoard[helpY, helpX] == null)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                else if (GameBoard[helpY, helpX].getColor() != color)
                {
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                    stop = true;
                }
                else
                {
                    stop = true;
                }
                helpX = helpX - 1;
                helpY = helpY - 1;
            }

            stop = false;
            helpX = loc.x + 1; helpY = loc.y - 1;
            while (helpX <= 3 && helpY >= 0 && !stop)
            {
                if (GameBoard[helpY, helpX] == null)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                else if (GameBoard[helpY, helpX].getColor() != color)
                {
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                    stop = true;
                }
                else
                {
                    stop = true;
                }
                helpX = helpX + 1;
                helpY = helpY - 1;
            }

            stop = false;
            helpX = loc.x - 1; helpY = loc.y + 1;
            while (helpX >= 0 && helpY <= 7 && !stop)
            {
                if (GameBoard[helpY, helpX] == null)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                else if (GameBoard[helpY, helpX].getColor() != color)
                {
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                    stop = true;
                }
                else
                {
                    stop = true;
                }
                helpX = helpX - 1;
                helpY = helpY + 1;
            }

            stop = false;
            helpX = loc.x + 1; helpY = loc.y + 1;
            while (helpX <= 3 && helpY <= 7 && !stop)
            {
                if (GameBoard[helpY, helpX] == null)
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                else if (GameBoard[helpY, helpX].getColor() != color)
                {
                    AvailableMoves.Add(new Coordinate(helpY, helpX));
                    stop = true;
                }
                else
                {
                    stop = true;
                }
                helpX = helpX + 1;
                helpY = helpY + 1;
            }
        }
    }
}
