using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HB2.Models;

namespace HB2.ViewModels
{
    public class Index_1
    {
        public IQueryable<Operation> operat { get; set; }
        public int DochMon { get; set; }
        public int RasMon { get; set; }
        public int RazDochRas { get; set; }
    }
}
