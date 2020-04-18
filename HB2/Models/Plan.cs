using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HB2.Models
{
    public class Plan
    {
        public int Id { get; set; }
        public int Procent { get; set; }
        public int DochMonth { get; set; }
        public int RasMonth { get; set; }
        public int RaznDochRas { get; set; }
        public string Name { get; set; }
        private DateTime data;
        public DateTime Data
        {
            set { data = value; }
            get { return data; }
        }
        private DateTime dataperiod;
        public DateTime DataPeriod
        {
            set { dataperiod = value; }
            get { return dataperiod; }
        }
        public TimeSpan Period
        {
            get { return dataperiod.Subtract(data); }
        }

        public List<Operation> Operations { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }
    }
}
