using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HB2.Models;

namespace HB2.ViewModels
{
    public class ArchOpAcPl
    {
        public string Name { get; set; }
        public string NameAct { get; set; }
        public int minsum { get; set; }
        public int maxsum { get; set; }
        public DateTime StData { get; set; }
        public DateTime FinData { get; set; }
        public IQueryable<P> PS { get; set; }
        public IQueryable<P1> P1S { get; set; }
    }
}
