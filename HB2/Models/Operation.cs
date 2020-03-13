using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HB2.Models
{
    public class Operation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAct { get; set; } // доход расход
        public string Coment { get; set; }
        public int Sum { get; set; }

        public int PlanId { get; set; }
        public Plan Plan { get; set; }

        //public List<P> p { get; set; }
    }
}
