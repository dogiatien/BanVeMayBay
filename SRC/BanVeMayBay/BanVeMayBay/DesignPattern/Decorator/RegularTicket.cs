using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanVeMayBay.DesignPattern.Decorator
{
    public class RegularTicket : Iticket
    {
        public string Description => "Vé ";
    }

}