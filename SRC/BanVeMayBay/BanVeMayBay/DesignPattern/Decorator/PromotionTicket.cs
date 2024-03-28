using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanVeMayBay.DesignPattern.Decorator
{
    public class PromotionTicket : Iticket
    {
        private readonly Iticket _ticket;

        public PromotionTicket(Iticket ticket)
        {
            _ticket = ticket;
        }

        public string Description => _ticket.Description + "Promotion , được các ưu đãi của vé business , được ưu tiên loại 1 , được thêm rượu vang ";
    }
}