using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.Chain_Of_Responsibility
{
    public class PasswordCheckHandler : AuthenticationHandler
    {
        public override bool HandleRequest(FormCollection fc, BANVEMAYBAYEntities2 db)
        {
            String password = Mystring.ToMD5(fc["password"]);
            var pass = db.users.FirstOrDefault(m => m.access == 1 && m.status == 1 && m.password == password);

            if (pass == null)
            {
                return false;
            }

            return nextHandler != null ? nextHandler.HandleRequest(fc, db) : true;
        }
    }

}