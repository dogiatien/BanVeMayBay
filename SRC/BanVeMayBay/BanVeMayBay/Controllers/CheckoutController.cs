using BanVeMayBay.Common;
using BanVeMayBay.DesignPattern.Decorator;
using BanVeMayBay.Models;
using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BanVeMayBay.Controllers
{
    public class CheckoutController : Controller
    {
        BANVEMAYBAYEntities2 db = new BANVEMAYBAYEntities2();
        public ActionResult Invalid()
        {
            return View("Invalid");
        }
        [HttpPost]
        public ActionResult login(FormCollection fc)
        {
            String Username = fc["username"];
            string Pass = Mystring.ToMD5(fc["password"]);
            var user_account = db.users.Where(m => m.access == 1 && m.status == 1 && (m.username == Username));
            var pass = user_account.FirstOrDefault()?.password;
            if (user_account.Count() == 0)
            {
                ViewBag.error = "Username Incorrect";
            }
            else
            {
                var pass_account = user_account.Where(m => m.access == 1 && m.status == 1 && m.password == Pass).FirstOrDefault();
                if (pass_account == null)
                {
                    ViewBag.error = "Incorrect password";
                }
                else
                {
                    var user = user_account.First();
                    Session.Add(CommonConstants.CUSTOMER_SESSION, user);
                    Session["userName11"] = user.fullname;
                    Session["id"] = user.ID;
                    if (!Response.IsRequestBeingRedirected)
                        Message.set_flash("Logged in successfully ", "success");
                    return Redirect("~/Home/Index");
                }
            }

            ViewBag.sess = Session["Admin_id"];
            return View("Login");

        }
        // GET: Checkout
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            var iddd = 0;
            user sessionUser = (user)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if (sessionUser != null)
            {
                int id1 = int.Parse(fc["datve"]);
                string ticketType1 = db.tickets.Where(t => t.id == id1).Select(t => t.ticketType).FirstOrDefault();
                iddd = sessionUser.ID;
                var list = new List<ticket>();

                // Tạo vé dựa trên loại vé
                Iticket ticketss = new RegularTicket();

                switch (ticketType1)
                {
                    case "Bussiness":
                        ticketss = new BusinessTicket(ticketss);
                        break;
                    case "Promotion":
                        ticketss = new PromotionTicket(ticketss);
                        break;
                    case "Economic":
                        break;
                    default:
                        break;
                }
                ViewBag.Note = ticketss.Description;
                if (fc["datve"] != null)
                {
                    int id = int.Parse(fc["datve"]);
                    var list1 = db.tickets.Find(id);
                    ViewBag.songuoi = int.Parse(fc["songuoi"]);

                    list.Add(list1);
                    ViewBag.ve1 = id;
                }
                else
                {
                    return Redirect("~/Home/index");
                }
              
                // neu co ve khu hoi
                if (!string.IsNullOrEmpty(fc["datveKH"]))
                {
                    int id2 = int.Parse(fc["datveKH"]);
                    var list2 = db.tickets.Find(id2);
                    ViewBag.ve2 = id2;
                    list.Add(list2);
                }
                

                return View("", list.ToList());
            }
            else
            {
               
                return View("Invalid");
            }
            //dừng..
            
        }

        [HttpPost]
        public ActionResult checkOut(order order,FormCollection fc)
        {
            var iddd = 0;
            user sessionUser = (user)Session[Common.CommonConstants.CUSTOMER_SESSION];
            if(sessionUser !=null)
            {
                iddd = sessionUser.ID;
            }
            float total =  float.Parse(fc["total"]);
            order.created_ate = DateTime.Now;
            order.status = 1;
            order.total = total;
            order.CusId = iddd;
            db.orders.Add(order);
            db.SaveChanges();
            int lastOrderID = order.ID;
            ordersdetail orderDetail = new ordersdetail();
            int id1 = int.Parse(fc["veOnvay"]);
            orderDetail.ticketId = id1;
            orderDetail.quantity = order.guestTotal;
            orderDetail.orderid = lastOrderID;
            db.ordersdetails.Add(orderDetail);
            // tru so luong nghe
            var ticket = db.tickets.Find(id1);
            ticket.Sold = ticket.Sold + order.guestTotal;
            db.Entry(ticket).State = EntityState.Modified;
            db.SaveChanges();
            //neu ton tai ve 2 chieu
            if (!string.IsNullOrEmpty(fc["veReturn"]))
            {
                int id2 = int.Parse(fc["veReturn"]);
                ordersdetail orderDetail2 = new ordersdetail();
                orderDetail2.ticketId = id2;
                orderDetail2.orderid = lastOrderID;
                orderDetail2.quantity = order.guestTotal;
                db.ordersdetails.Add(orderDetail2);
                // tru so luong nghe
                var ticket2 = db.tickets.Find(id2);
                ticket2.Sold = ticket2.Sold + order.guestTotal;
                db.Entry(ticket2).State = EntityState.Modified;
                db.SaveChanges();
            }
          
            
           
            return View("checkOutComfin", order);
        }
        // lay thong tin cac ve da book
        public ActionResult _BookingConnfig(int orderId)
        {
            var list = db.ordersdetails.Where(m => m.orderid == orderId).ToList();
            var list1 = new List<ticket>();
            foreach (var item in list)
            {
                ticket ticket = db.tickets.Find(item.ticketId);
                list1.Add(ticket);
            }

            return View("_BookingConnfig", list1.ToList());
        }

        public ActionResult PaymentWithPaypal(order order, string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/checkout/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("checkOutComfin", order);
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            //Adding Item Details like name, currency, price etc  
          
            
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = "50"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = "50", // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

    }
}