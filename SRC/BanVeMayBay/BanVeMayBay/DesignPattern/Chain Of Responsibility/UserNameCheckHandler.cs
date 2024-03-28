using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.Chain_Of_Responsibility
{
    public class UserNameCheckHandler : AuthenticationHandler
    {
        
            public override bool HandleRequest(FormCollection fc, BANVEMAYBAYEntities2 db)
            {
                String username = fc["username"];
                var userAccount = db.users.FirstOrDefault(m => m.access == 1 && m.status == 1 && m.username == username);

                if (userAccount == null)
                {
                    return false;
                }

                return nextHandler != null ? nextHandler.HandleRequest(fc, db) : true;
            }
        }
    }

