using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model
{
    internal class Coordinate
    {
        public int x { get; set; }
        public int y { get; set; }

        public Coordinate(int y, int x)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Coordinate other)
        {
            return this.x == other.x && this.y == other.y;
        }
    }
}
