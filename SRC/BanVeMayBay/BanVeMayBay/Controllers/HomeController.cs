using BanVeMayBay.DesignPattern.Stategy;
using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.Controllers
{
    public class HomeController : Controller
    {
        //Stategy
        private IPriceFilterStrategy _priceFilterStrategy;
        private BusinessPriceFilterStrategy _businessPriceFilterStrategy;
        private FirstClassPriceFilterStrategy _firstClassPriceFilterStrategy;
        private EconomyPriceFilterStrategy _conomyPriceFilterStrategy;
        BANVEMAYBAYEntities2 db = new BANVEMAYBAYEntities2();
    
        // GET: Home
        public ActionResult Index()
        {
            DateTime date_now = DateTime.Now;
            string date_now1 = date_now.ToString("dd-MM-yyyy");
            DateTime date_now2 = DateTime.Parse(date_now1);
            ViewBag.dateNow = date_now;
            // lay cac chuyen bay trong ngay
            //var list = db..Take(20).ToList();
            var cities = db.cities.ToList();
            ViewBag.cities = cities;
            var tickets = db.tickets.Where(m => m.status == 1 && m.departure_date == date_now2).ToList();
            ViewBag.tickets = tickets;
            List<ticket> ticketss = db.tickets.Where(m => m.status == 1 ).ToList();
            List<ticket> filteredTickets;

            IPriceFilterStrategy priceFilterStrategy;

            //switch (ticketType)
            //{
            //    case "business":
            //        priceFilterStrategy = new BusinessPriceFilterStrategy();
            //        break;
            //    case "firstclass":
            //        priceFilterStrategy = new FirstClassPriceFilterStrategy();
              
            //        break;
            //    default:
            //        priceFilterStrategy = new EconomyPriceFilterStrategy();
        
            //        break;
            //}

            
            
            // Trả về View với ViewBag.FilteredTickets
            return View();




        }

        [HttpGet]
        public ActionResult FilterTickets(string ticketType)
        {
            DateTime currentDate = DateTime.Now.Date;
            List<ticket> tickets = db.tickets.Where(m => m.status == 1).ToList();
            List<ticket> filteredTickets;

            IPriceFilterStrategy priceFilterStrategy;

            switch (ticketType)
            {
                case "business":
                    priceFilterStrategy = new BusinessPriceFilterStrategy();
                    break;
                case "firstclass":
                    priceFilterStrategy = new FirstClassPriceFilterStrategy();
                    break;
                case "economy":
                    priceFilterStrategy = new EconomyPriceFilterStrategy();
                    break;
                default:
                    priceFilterStrategy = new EconomyPriceFilterStrategy();
                    break;
            }

            filteredTickets = priceFilterStrategy.Filter(tickets);
            ViewBag.ticketss = filteredTickets;
            return PartialView("FilterTickets", filteredTickets);
        }

        public ActionResult flightOfday()
        {
            DateTime date_now = DateTime.Now;
            string date_now1 = date_now.ToString("dd-MM-yyyy");
            DateTime date_now2 = DateTime.Parse(date_now1);
            ViewBag.dateNow = date_now;
            // lay cac chuyen bay trong ngay
            var list = db.tickets.Where(m => m.status == 1 && m.departure_date == date_now2).Take(20).ToList();
            return View("flightOfDay", list);
        }
        
    }
}