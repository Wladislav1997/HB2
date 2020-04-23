using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HB2.Models;

namespace HB2.ViewModels
{
    public class ArchPl
    {
        public IQueryable<Plan> pl { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public int Clock { get; set; }
        public int Min { get; set; }
        public int _Days { get; set; }
        public int _Clock { get; set; }
        public int _Min { get; set; }
        public int mindoch { get; set; }
        public int maxdoch { get; set; }
        public int minras { get; set; }
        public int maxras { get; set; }
        public int minit { get; set; }
        public int maxit { get; set; }
        public int minpr { get; set; }
        public int maxpr { get; set; }
        public DateTime mind { get; set; }
        public DateTime maxd { get; set; }
        public DateTime mindp { get; set; }
        public DateTime maxdp { get; set; }
    }
}
