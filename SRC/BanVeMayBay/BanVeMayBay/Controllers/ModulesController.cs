using BanVeMayBay.DesignPattern.Singleton;
using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.Controllers
{
    public class ModulesController : Controller
    {
        // GET: Modules
        BANVEMAYBAYEntities2 db = new BANVEMAYBAYEntities2();
        public ActionResult _Header()
        {
            if (SessionManager.Instance.IsUserLoggedIn(this))
            {
                ViewBag.sessionFullname = SessionManager.Instance.GetLoggedInUser(this).fullname;
            }
            else
            {
                ViewBag.sessionFullname = null; // hoặc bất kỳ giá trị mặc định nào bạn muốn hiển thị khi không có phiên đăng nhập
            }
            return View("_Header");
        }
        public ActionResult _Mainmenu()
        {

            var list = db.menus.Where(m => m.status == 1 && m.parentid == 0).ToList();
            return View("_Mainmenu", list);
        }
        public ActionResult _Footer()
        {
            return View("_Footer");
        }

        public ActionResult Slider()
        {
            return View("Slider");
        }
        public ActionResult LogoSlide()
        {
            return View("LogoSlide");
        }
    }
}