using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET_Project_Client.Model
{
    public class Client
    {
        public string Name { get; set; }

        public int? ID { get; set; }

        public string Phone { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return $"{Name} (ID: {ID})";
        }
    }
}
