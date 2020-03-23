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

        public List<P> ps { get; set; }
        public int Procent
        {
            get
            {
                if (ps.Count != 0)
                {
                    int _p;
                    int _s = 0;
                    foreach (P p in ps)
                    {
                        _s = +p.Sum;
                    }
                    _p = Sum / 100;

                    return _s / _p;
                }
                else
                {
                    return 0;
                }
            }
        }

    }
}
