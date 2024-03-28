using BanVeMayBay.Common;
using BanVeMayBay.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using BanVeMayBay.Chain_Of_Responsibility;
using BanVeMayBay.DesignPattern.Singleton;

namespace BanVeMayBay.Controllers
{
    public class CustomerController : Controller
    {
        private BANVEMAYBAYEntities2 db = new BANVEMAYBAYEntities2();
        private static CustomerController _instance;
        private static readonly object _lock = new object();
        // GET: Customer
        public ActionResult Login()
        {
            if (SessionManager.Instance.IsUserLoggedIn(this))
            {
                // Nếu đã đăng nhập, chuyển hướng đến trang tài khoản của người dùng
                return RedirectToAction("Myaccount");
            }
            return View("Login");
        }
        [HttpPost]
        public ActionResult Login(FormCollection fc)
        {
            // Tạo chuỗi xử lý
            AuthenticationHandler usernameHandler = new UserNameCheckHandler();
            AuthenticationHandler passwordHandler = new PasswordCheckHandler();

            usernameHandler.SetNextHandler(passwordHandler);
            if (SessionManager.Instance.IsUserLoggedIn(this))
            {
                // Nếu đã đăng nhập, chuyển hướng đến trang tài khoản của người dùng
                return RedirectToAction("Myaccount");
            }
            // Bắt đầu xử lý
            if (usernameHandler.HandleRequest(fc, db))
            {
                //// Đăng nhập thành công
                //var username = fc["username"];
                //var user = db.users.FirstOrDefault(u => u.username == username);
                //Session.Add(CommonConstants.CUSTOMER_SESSION, user);
                
                //Session["id"] = user.ID;
                //if (!Response.IsRequestBeingRedirected)
                //    Message.set_flash("Logged in successfully", "success");
                //return Redirect("~/tai-khoan");
                var username = fc["username"];
                var user = db.users.FirstOrDefault(u => u.username == username);

                // Sử dụng SessionManager để lưu thông tin phiên đăng nhập
                SessionManager.Instance.SetLoggedInUser(this, user);
                //Session["userName11"] = user.fullname;

                if (!Response.IsRequestBeingRedirected)
                    Message.set_flash("Logged in successfully", "success");

                return Redirect("~/tai-khoan");
            }

            ViewBag.sess = Session["Admin_id"];
            return View("Login");
        }

        public void logout()
        {
            
            //Session[Common.CommonConstants.CUSTOMER_SESSION] = "";
            SessionManager.Instance.Logout(this);
            //Session["userName11"] = "";
            Response.Redirect("~/dang-nhap");
            Message.set_flash("Sign out successful", "success");
        }
        public ActionResult register()
        {
            if (SessionManager.Instance.IsUserLoggedIn(this))
            {
                // Nếu đã đăng nhập, chuyển hướng đến trang tài khoản của người dùng
                return RedirectToAction("Myaccount");
            }
            return View("register");
        }
        [HttpPost]
        public ActionResult register(user muser, FormCollection fc)
        {
            string uname = fc["uname"];
            string fname = fc["fname"];
            string Pass = Mystring.ToMD5(fc["psw"]);
            string Pass2 = Mystring.ToMD5(fc["repsw"]);
            if (SessionManager.Instance.IsUserLoggedIn(this))
            {
                // Nếu đã đăng nhập, chuyển hướng đến trang tài khoản của người dùng
                return RedirectToAction("Myaccount");
            }
            if (Pass2 != Pass)
            {
                ViewBag.error = "password incorrect";
                return View("loginEndRegister");
            }
            string email = fc["email"];
            string address = fc["address"];
            string phone = fc["phone"];
            if (ModelState.IsValid)
            {
                var Luser = db.users.Where(m => m.status == 1 && m.username == uname && m.access == 1);
                if (Luser.Count() > 0)
                {
                    ViewBag.error = "Username available";
                    return View("loginEndRegister");
                }
                else
                {
                    muser.img = "defalt.png";
                    muser.password = Pass;
                    muser.username = uname;
                    muser.fullname = fname;
                    muser.email = email;
                    muser.address = address;
                    muser.phone = phone;
                    muser.gender = "nam";
                    muser.created_at = DateTime.Now;
                    muser.updated_at = DateTime.Now;
                    muser.created_by = 1;
                    muser.updated_by = 1;
                    muser.access = 1;
                    muser.status = 1;
                    db.users.Add(muser);
                    db.SaveChanges();
                    Message.set_flash("Successful account registration, Login here ", "success");
                    return Redirect("~/dang-nhap");
                }

            }
            Message.set_flash("Account registration failed", "danger");
            return View("register");
        }

        public ActionResult Myaccount()
        {
            user sessionUser = (user)Session[Common.CommonConstants.CUSTOMER_SESSION];
            return View("Myaccount", sessionUser);
        }
        [HttpPost]
        public ActionResult Myaccount(user user, FormCollection fc)
        {
            var pswO = fc["pswO"];
            var pswN = fc["pswN"];
            var pswR = fc["pswR"];
            if (pswO != null)
            {
                if (pswO.ToMD5() != user.password)
                {
                    ViewBag.success = "Old password is incorrect.";
                    return View("Myaccount", user);
                }
                if (pswN == null || pswR == null || pswN.Length < 6 || pswR.Length < 6)
                {
                    ViewBag.success = "The new password is not valid.";
                    return View("Myaccount", user);
                }
                if (pswN.ToMD5() != pswR.ToMD5())
                {
                    ViewBag.success = "Password incorrect.";
                    return View("Myaccount", user);
                }
                else
                {
                    user.password = pswN.ToMD5();
                }
            }

            Session[Common.CommonConstants.CUSTOMER_SESSION] = "";
            Session.Add(CommonConstants.CUSTOMER_SESSION, user);
            user.created_at = DateTime.Now;
            user.updated_at = DateTime.Now;
            user.created_by = 1;
            user.updated_by = 1;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.success = "Update successful.";
            return View("Myaccount", user);
        }
        public ActionResult ListOderCus()
        {

            user sessionUser = (user)Session[Common.CommonConstants.CUSTOMER_SESSION];
            var listOrder = db.orders.Where(m => m.CusId == sessionUser.ID).OrderByDescending(m => m.ID).ToList();
            return View("ListOderCus", listOrder);
        }
        public ActionResult orderDetailCus(int id)
        {
            var sigleOrder = db.orders.Find(id);
            return View("orderDetailCus", sigleOrder);
        }
        public ActionResult canelOrder(int OrderId)
        {


            order morder = db.orders.Find(OrderId);
            var orderDetail = db.ordersdetails.Where(m => m.orderid == morder.ID).ToList();
            foreach (var item in orderDetail)
            {
                var id = int.Parse(item.ticketId.ToString());
                ticket ticket = db.tickets.Find(id);
                DateTime ngaymuon = Convert.ToDateTime(
                    morder.created_ate);
                DateTime ngaytra = Convert.ToDateTime(ticket.departure_date);
                TimeSpan Time = ngaytra - ngaymuon;
                int TongSoNgay = Time.Days;
                if (TongSoNgay >= 14)
                {
                    ticket.Sold = ticket.Sold - item.quantity;
                    db.Entry(ticket).State = EntityState.Modified;
                    db.SaveChanges();
                    if (item == null)
                    {
                        Message.set_flash("Error Cancel Order", "danger");
                        return Redirect("~/tai-khoan");
                    }
                    db.ordersdetails.Remove(item);
                    db.SaveChanges();
                }
                else
                {
                    Message.set_flash("Tickets cannot be canceled 14 days before flight date", "dangger");
                    return Redirect("~/tai-khoan");
                }


            }

            db.orders.Remove(morder);
            db.SaveChanges();
            Message.set_flash("Canceled 1 order", "success");
            return Redirect("~/tai-khoan");
        }
    }
}