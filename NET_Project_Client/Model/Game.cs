using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model
{
    internal class Game
    {
        public int? Gid { get; set; }
        public int? Pid { get; set; }
        public bool Winner { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }

        public Game(int? pid, bool winner, DateTime date, int duration)
        {
            Pid = pid;
            Winner = winner;
            Date = date;
            Duration = duration;
        }
    }
}
