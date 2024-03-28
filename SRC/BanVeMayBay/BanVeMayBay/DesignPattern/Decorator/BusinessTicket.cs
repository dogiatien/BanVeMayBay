using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanVeMayBay.DesignPattern.Decorator
{
    public class BusinessTicket : Iticket
    {
        private readonly Iticket _ticket;

        public BusinessTicket(Iticket ticket)
        {
            _ticket = ticket;
        }

        public string Description => _ticket.Description + " business ,được ưu tiên loại 2,được thêm snack, được thêm hành lý ";
    }
}