using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaWebApplication.data
{
     public class Deposit
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }
}
