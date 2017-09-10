using SimchaWebApplication.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimchaWebApplication.web.Models
{
    public class IndexViewModel
    {
       public IEnumerable<Simcha> Simchas { get; set; }

       public int TotalContributors { get; set; }
    }
}