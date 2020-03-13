using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HB2.Models
{
    public class OpR
    {
        public int Id { get; set; }

        public int OpRSum { get; set; }
        public string OpRComent { get; set; }

        public int OperationId { get; set; }
        public Operation Operation { get; set; }
    }
}
