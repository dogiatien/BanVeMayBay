using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BanVeMayBay.DesignPattern.Stategy
{
    public interface IPriceFilterStrategy 
    {
        List<ticket> Filter(List<ticket> tickets);
    }
}