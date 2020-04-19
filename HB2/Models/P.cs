using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HB2.Models
{
    public class P
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameAct { get; set; }
        public int Sum { get; set; }
        public string Coment { get; set; }
        public DateTime Data { get; set; }

        public Operation Operation { get; set; }
        public int OperationId { get; set; }
    }
}
