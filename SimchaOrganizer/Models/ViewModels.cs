using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchaOrganizer.Data;

namespace SimchaOrganizer.Models
{
    public class HomePageVM
    {
        public IEnumerable<Simcha> Simchas { get; set; }
        public int ContributorCount { get; set; }
        public string Message { get; set; }
        public HomePageVM()
        {
            IEnumerable<Simcha> Simchas = new List<Simcha>();
        }
    }
    public class ContributorViewModel
    {
        public IEnumerable<Contributor> list { get; set; }
        public decimal Total { get; set; }
        public ContributorViewModel()
        {
            IEnumerable<Contributor> list = new List<Contributor>();
        }

    }
    public class ContributorListVM
    {
        public IEnumerable<Contributor> CList { get; set; }
        public ContributorListVM()
        {
            IEnumerable<Contributor> CList = new List<Contributor>();
        }
    }
    public class PersonHistoryVM
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }

        public List<History> ActionList { get; set; }
        public PersonHistoryVM()
        {
            var ActionList = new List<History>();
        }
    }
    public class ContributorManagerVM
    {
        public List<Contributor> PplForSimcha { get; set; }
        public IEnumerable<Contributor> AllPeople { get; set; }
        public string Simcha { get; set; }
        public int ID { get; set; }
        
        public ContributorManagerVM()
        {
            IEnumerable<Contributor> AllPeople = new List<Contributor>();
            List<Contributor> PplForSimcha = new List<Contributor>();
        }
        
    }
}