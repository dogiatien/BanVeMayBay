using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.Chain_Of_Responsibility
{
    public abstract class AuthenticationHandler
    {
        protected AuthenticationHandler nextHandler;

        public void SetNextHandler(AuthenticationHandler handler)
        {
            nextHandler = handler;
        }

        public abstract bool HandleRequest(FormCollection fc, BANVEMAYBAYEntities2 db);
    }
}