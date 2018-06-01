using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimchaOrganizer.Data;
using SimchaOrganizer.Models;
using System.Data.SqlClient;

namespace SimchaOrganizer.Controllers
{
    public class HomeController : Controller
    {
        DBManager manager = new DBManager(Properties.Settings.Default.ConStr);
        public ActionResult Index()
        {
            HomePageVM viewModel = new HomePageVM();
            if (TempData["PopUp"] != null)
            {
                viewModel.Message = (string)TempData["PopUp"];
            }
            viewModel.ContributorCount = manager.TotalContributors();
            viewModel.Simchas = manager.SimchaList();
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult AddSimcha(string simchaname, DateTime date)
        {
            manager.AddSimcha(simchaname, date);
            return Redirect("/");
        }
        public ActionResult ContributorList(int id)
        {
            var cl = new ContributorListVM();
            cl.CList = manager.ContributorList(id);
            return View(cl);
        }


        public ActionResult Contributors()
        {
            var cvm = new ContributorViewModel();
            cvm.list = manager.ContributorList();
            decimal total = 0;
            foreach(Contributor c in cvm.list)
            {
                total += c.TotalLeft;
            }
            cvm.Total = total;
            return View(cvm);
        }
        public ActionResult ContributorManager(int id)
        {
            ContributorManagerVM viewModel = new ContributorManagerVM
            {
                ID = id,
                Simcha = manager.GetSimchaForID(id),
                AllPeople = manager.ContributorList(),
                PplForSimcha = manager.ContributorList(id)
                
            };
           
            return View(viewModel);
        }
        [HttpPost]
        public ActionResult UpdateSimcha(List<UpdatedInfo> person)
        {
            TempData["PopUp"] = "New Contributions made";
            var NewAdd = new List<UpdatedInfo>();
            var UpdateOnly = new List<UpdatedInfo>();
            var Old = manager.ContributorList(person[0].SimchaID);
            foreach(UpdatedInfo p in person)
            {
                Contributor c = Old.FirstOrDefault(x => x.ID == p.ContributorID);
                if(c != null && p.DidContribute)
                {
                    
                    UpdateOnly.Add(p);
                }
                else if(c == null && p.DidContribute)
                {
                    NewAdd.Add(p);
                }
                
            }
            manager.AddContributor(NewAdd);
            manager.UpdateContribution(UpdateOnly);
            return Redirect("/");
        }
        public ActionResult CHistory(int id)
        {
            var history = new PersonHistoryVM();
            history.ActionList = manager.History(id);
            var list = manager.ContributorList();
            var c = list.FirstOrDefault(o => o.ID == id);
            history.Name = c.Name;
            history.Amount = c.TotalLeft;
            history.ActionList.OrderByDescending(d => d.Date);
            return View(history);
        }
        [HttpPost]
        public ActionResult AddDeposit(int id, string amount, DateTime date)
        {
            manager.AddDeposit(id, Convert.ToDecimal(amount), date);
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public ActionResult Edit(Contributor c)
        {
            manager.EditPerson(c);
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public ActionResult AddPerson(Contributor c, int amount, string date)
        {
            c.DateJoined = Convert.ToDateTime(date);
            manager.AddPerson(c);
            manager.AddInitialDeposit(amount, c.ID, c.DateJoined);
            
            return Redirect("/home/contributors");

        
        }
       
    }
}