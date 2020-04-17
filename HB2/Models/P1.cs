using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HB2.Models
{
    public class P1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Data { get; set; }
        public string NameAct { get; set; } // доход расход
        public string Coment { get; set; }
        public int Sum { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }
    }
}
