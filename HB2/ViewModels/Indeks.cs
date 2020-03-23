using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HB2.Models;

namespace HB2.ViewModels
{
    public class Indeks
    {
        public IQueryable<Operation> operation;
        public int Sum { get; set; }
    }
}
