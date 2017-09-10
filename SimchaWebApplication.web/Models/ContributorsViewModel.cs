using SimchaWebApplication.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaWebApplication.web.Models
{
    public class ContributorsViewModel
    {
       public IEnumerable<Contributor> Contributors { get; set; }

        public decimal Balance { get; set; }

    }
}