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

        public void removeMoves(Piece[,] GameBoard)
        {
            int size;
            bool rm;
            for (int i = 0; i < AvailableMoves.Count(); i++)
            {
                rm = false;
                for (int j = 0; j < 8; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        if (GameBoard[j, k] != null)
                            if (GameBoard[j, k].getColor() != color)
                            {
                                size = GameBoard[j, k].AvailableMoves.Count();
                                for (int l = 0; l < size; l++)
                                {
                                    if(GameBoard[j, k].AvailableMoves[l].Equals(GameBoard[loc.y, loc.x].AvailableMoves[i]))
                                    {
                                        AvailableMoves.RemoveAt(i);
                                        rm = true;
                                        break;
                                    }
                                }
                            }
                        if (rm)
                            break;
                    }
                    if (rm)
                        break;
                }
                if (rm)
                    i--;
            }
        }

        public void setPicture(bool tr)
        {
            if (tr)
            {
                if (color == false)
                    img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/king_white_check.png");
                else
                    img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/king_black_check.png");
            }
            else
            {
                if (color == false)
                    img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/king_white.png");
                else
                    img = Image.FromFile("C:/GitRep/.NET_Project_Client/Resources/king_black.png");
            }
        }
    }
}
