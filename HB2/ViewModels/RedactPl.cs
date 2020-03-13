using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HB2.Models;

namespace HB2.ViewModels
{
    public class RedactPl
    {
        public int _Id { get; set; }
        public IQueryable<Operation> Opers { get; set; }
    }
}
