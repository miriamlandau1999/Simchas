using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaWebApplication.data
{
    public class Contribution: Deposit
    {
        public string SimchaName { get; set; }
        public int SimchaId { get; set; }
    }
}
