using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal class King : Piece
    {
        public King(bool color, Coordinate loc) : base(color, loc)
        {
            if (color)
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/king_black.png");
            else
                img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/king_white.png");
        }

        public override void CalculateMoves(Piece[,] GameBoard)
        {
            AvailableMoves = new List<Coordinate>();
            int helpX, helpY;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    helpY = loc.y + i;
                    helpX = loc.x + j;
                    if(helpY >= 0 && helpY <= 7 && helpX >= 0 && helpX <= 3)
                    {
                        if (GameBoard[helpY,helpX] == null || GameBoard[helpY, helpX].getColor() != color)
                            AvailableMoves.Add(new Coordinate(helpY, helpX));
                    }
                }
            }
        }
    }
}
