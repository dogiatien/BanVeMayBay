using BanVeMayBay.Controllers;
using BanVeMayBay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.DesignPattern.Singleton
{
    public class SessionManager
    {
        private static SessionManager _instance;

        private static readonly object _lock = new object();

        // Để đảm bảo chỉ có một instance được tạo ra
        private SessionManager()
        {
             BANVEMAYBAYEntities2 db = new BANVEMAYBAYEntities2();
        }

        public static SessionManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SessionManager();
                    }
                    return _instance;
                }
            }
        }

        // Phương thức để kiểm tra xem người dùng đã đăng nhập hay chưa
        public bool IsUserLoggedIn(Controller controller)
        {
            return controller.Session[Common.CommonConstants.CUSTOMER_SESSION] != null;
        }

        // Phương thức để lưu thông tin phiên đăng nhập
        public void SetLoggedInUser(Controller controller, user loggedInUser)
        {
            controller.Session[Common.CommonConstants.CUSTOMER_SESSION] = loggedInUser;
        }

        // Phương thức để đăng xuất người dùng
        public void Logout(Controller controller)
        {
            controller.Session[Common.CommonConstants.CUSTOMER_SESSION] = null;
        }
        public user GetLoggedInUser(Controller controller)
        {
            return (user)controller.Session[Common.CommonConstants.CUSTOMER_SESSION];
        }
    }
}