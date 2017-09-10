using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaWebApplication.data;

namespace SimchaWebApplication.web.Models
{
    public class ContributionsViewModel
    {
        public Simcha Simcha { get; set; }
        public List<Contributor> Contributors { get; set; }
        public IEnumerable<Contribution> Contributions { get; set; }
    }
}