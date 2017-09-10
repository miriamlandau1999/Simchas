using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaWebApplication.data;

namespace SimchaWebApplication.web.Models
{
    public class ShowHistoryViewModel
    {
        public Contributor Contributor { get; set; }
        public IEnumerable<Contribution> Transactions { get; set; }
    }
}