using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Simcha
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int TotalAmount { get; set; }
    }
    public class Contributor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Cell { get; set; }
    }
    public class History
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public string Amount { get; set; }
    }
}
