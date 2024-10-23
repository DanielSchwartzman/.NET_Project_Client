using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model.ChessPieces
{
    internal abstract class Piece
    {
        protected bool color; // 0-White 1-Black
        protected Coordinate loc;
        protected List<Coordinate> AvailableMoves;

        public Piece(bool color, Coordinate loc)
        {
            this.color = color;
            this.loc = loc;
            AvailableMoves = new List<Coordinate>();
        }

        public abstract void CalculateMoves(Piece[,] GameBoard);

        public bool getColor()
            { return color; }
    }
}
