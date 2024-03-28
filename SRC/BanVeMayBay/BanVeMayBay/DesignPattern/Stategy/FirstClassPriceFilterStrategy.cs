using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanVeMayBay.DesignPattern.Stategy
{
    public class FirstClassPriceFilterStrategy : IPriceFilterStrategy
    {
        public List<ticket> Filter(List<ticket> tickets)
        {
            return tickets.Where(t => t.ticketType == "Promotion").ToList();
        }
    }
}