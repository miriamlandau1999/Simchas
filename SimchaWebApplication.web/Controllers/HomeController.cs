using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaWebApplication.data;
using SimchaWebApplication.web.Models;

namespace SimchaWebApplication.web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IndexViewModel ivm = new IndexViewModel();
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            ivm.Simchas = db.GetSimchas();
            ivm.TotalContributors = db.GetContributorCount();
            return View(ivm);
        }

        public ActionResult Contributors()
        {
            ContributorsViewModel cvm = new ContributorsViewModel();
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            cvm.Contributors = db.GetContributors();
            cvm.Balance = db.GetTotalBalance();
            return View(cvm);
        }
        [HttpPost]
        public ActionResult NewContributor(Contributor contributor, Deposit deposit)
        {
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            deposit.ContributorId = db.AddContributor(contributor);
            db.AddDeposit(deposit);
            return Redirect("/home/Contributors");
        }
        public ActionResult NewDeposit(Deposit deposit)
        {
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.AddDeposit(deposit);
            return Redirect("/home/Contributors");
        }
        public ActionResult ShowHistory(int ContributorId)
        {
            ShowHistoryViewModel shvm = new ShowHistoryViewModel();
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            shvm.Contributor = db.GetContributor(ContributorId);
            shvm.Transactions = db.GetTransactions(ContributorId);
            return View(shvm);
        }
        [HttpPost]
        public ActionResult NewSimcha(Simcha Simcha)
        {
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.AddSimcha(Simcha);
            return Redirect("/home/index");
        }
        public ActionResult Contributions(int SimchaId)
        {
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            ContributionsViewModel cvm = new ContributionsViewModel();
            cvm.Simcha = db.GetSimcha(SimchaId);
            cvm.Contributors = new List<Contributor>();
            cvm.Contributors = (List<Contributor>) db.GetContributorsWithContribution(SimchaId);
            return View(cvm);

        }
        [HttpPost]
        public ActionResult UpdateContributions(List<ContributionInclude> Contributions, int SimchaId)
        {
            SimchaDb db = new SimchaDb(Properties.Settings.Default.ConStr);
            db.DeleteContributions(SimchaId);
            List<ContributionInclude> result = Contributions.FindAll(c => c.Include == true);
            result.ForEach(r => r.SimchaId = SimchaId);
            result.ForEach(r => db.AddContribution(r));
            return Redirect($"/home/Contributions?SimchaId={SimchaId}");
        }

    } 
}