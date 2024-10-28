using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal abstract class Piece
    {
        protected bool color; // 0-White 1-Black
        protected Coordinate loc;
        public List<Coordinate> AvailableMoves;
        public Image img;

        public Piece(bool color, Coordinate loc)
        {
            this.color = color;
            this.loc = loc;
            AvailableMoves = new List<Coordinate>();
        }

        public abstract void CalculateMoves(Piece[,] GameBoard);

        public void setLoc(int row, int col)
        {
            loc = new Coordinate(row, col);
        }

        public int getRow()
        {
            return loc.y;
        }

        public int getCol()
        {
            return loc.x;
        }

        public bool getColor()
            { return color; }

        public string AvailableMovesToString()
        {
            string sb = "";

            for (int i = 0; i < AvailableMoves.Count; i++)
            {
                sb += " " + AvailableMoves[i].ToString();
            }
            return sb;
        }
    }
}
