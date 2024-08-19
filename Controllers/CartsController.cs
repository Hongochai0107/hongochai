using HoNgocHai_2122110473_ASP.NET.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoNgocHai_2122110473_ASP.NET.Controllers
{
    public class CartController : Controller
    {
        HshopEntities db = new HshopEntities();
        // GET: Cart
        public ActionResult Index()
        {
            var cart = (List<CartModels>)Session["cart"];
            if (cart != null && cart.Any())
            {
                ViewBag.TotalPrice = cart.Sum(item => item.HangHoa.DonGia * item.Quantity);
                ViewBag.Discount = cart.Sum(item => item.HangHoa.GiamGia.HasValue ? (item.HangHoa.DonGia * item.HangHoa.GiamGia.Value) * item.Quantity : 0);
                ViewBag.GrandTotal = ViewBag.TotalPrice - ViewBag.Discount;
            }
            else
            {
                ViewBag.TotalPrice = 0;
                ViewBag.Discount = 0;
                ViewBag.GrandTotal = 0;
            }
            return View((List<CartModels>)Session["cart"]);
        }

        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            List<CartModels> cart = Session["cart"] as List<CartModels>;

            if (cart == null)
            {
                cart = new List<CartModels>();
                cart.Add(new CartModels { HangHoa = db.HangHoas.Find(id), Quantity = quantity });
                Session["cart"] = cart;
            }
            else
            {
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity += quantity;
                }
                else
                {
                    cart.Add(new CartModels { HangHoa = db.HangHoas.Find(id), Quantity = quantity });
                }
            }

            Session["count"] = cart.Count;

            // Tính tổng số lượng sản phẩm trong giỏ
            int cartCount = cart.Count;

            // Tính tổng giá trị của giỏ hàng nếu cần
            decimal totalPrice = cart.Sum(item => item.Quantity * (decimal)item.HangHoa.DonGia);
            // Trả về dữ liệu JSON để AJAX xử lý
            return Json(new { success = true, CartCount = cartCount, TotalPrice = totalPrice }, JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public JsonResult UpdateQuantity(int id, int quantity)
        {
            var cart = (List<CartModels>)Session["cart"];
            if (cart != null && cart.Any())
            {
                int index = cart.FindIndex(c => c.HangHoa.MaHH == id);
                if (index != -1)
                {
                    cart[index].Quantity = quantity;
                    Session["cart"] = cart;

                    var totalItemPrice = cart[index].Quantity * cart[index].HangHoa.DonGia;
                    var totalPrice = cart.Sum(item => item.HangHoa.DonGia * item.Quantity);
                    var discount = cart.Sum(item => item.HangHoa.GiamGia.HasValue ? (item.HangHoa.DonGia - item.HangHoa.GiamGia.Value) * item.Quantity : 0);
                    var grandTotal = totalPrice - discount;

                    return Json(new
                    {
                        Message = "Quantity updated successfully",
                        TotalPrice = totalPrice.ToString(),
                        Discount = discount.ToString(),
                        GrandTotal = grandTotal.ToString(),
                        TotalItemPrice = totalItemPrice.ToString()
                    });
                }
            }

            return Json(new { Message = "Error: Product not found in cart" });
        }

        private int isExist(int id)
        {
            List<CartModels> cart = (List<CartModels>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].HangHoa.MaHH.Equals(id))
                    return i;
            return -1;
        }
        // Remove a product from the cart by id



        [HttpPost]
        public ActionResult Remove(int id)
        {
            // Kiểm tra nếu giỏ hàng không có sản phẩm
            if (Session["cart"] == null)
            {
                return Json(new { success = false, message = "Giỏ hàng không tồn tại." }, JsonRequestBehavior.AllowGet);
            }

            // Lấy giỏ hàng từ session
            List<CartModels> cart = (List<CartModels>)Session["cart"];

            // Tìm và xóa sản phẩm theo id
            var itemToRemove = cart.FirstOrDefault(x => x.HangHoa.MaHH == id);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                Session["cart"] = cart;
                Session["count"] = cart.Count; // Cập nhật số lượng giỏ hàng

                // Trả về kết quả
                return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng.", cartCount = cart.Count }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = false, message = "Sản phẩm không tìm thấy trong giỏ hàng." }, JsonRequestBehavior.AllowGet);
        }

        private void ClearCart()
        {
            Session["cart"] = null;
            Session["count"] = 0;
        }

        public ActionResult Payment()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                // Retrieve cart information from session
                var lstCart = (List<CartModels>)Session["cart"];
                if (lstCart == null || !lstCart.Any())
                {
                    // Handle empty cart scenario
                    return RedirectToAction("Index", "Cart");
                }

                // Create a new order
                order objOrder = new order();
                objOrder.Name = "DonHang" + DateTime.Now.ToString("yyyyMMddHHmmss");
                objOrder.UserId = int.Parse(Session["UserId"].ToString());
                objOrder.CreatedOnUtc = 1;
                objOrder.Address = Session["Address"] != null ? Session["Address"].ToString() : "Default Address"; // Add address field
                objOrder.Status = 1;

                db.orders.Add(objOrder);
                db.SaveChanges(); // Save order to generate order ID

                // Retrieve generated order ID
                int intOrderId = objOrder.Id;

                // Calculate total amount for the order
                List<orderdetail> lstOrderDetail = new List<orderdetail>();
                decimal totalAmount = 0;
                foreach (var item in lstCart)
                {
                    orderdetail obj = new orderdetail();
                    obj.qty = item.Quantity;
                    obj.order_id = intOrderId;
                    obj.product_id = (int)item.HangHoa.MaHH; // Assuming product_id is int
                    obj.price = (decimal)item.HangHoa.DonGia; // Explicitly convert price to decimal
                    obj.discount = 0; // Assuming no discount for now
                    obj.amount = item.Quantity * obj.price; // Calculate amount and convert to decimal
                    obj.created_at = DateTime.Now;
                    obj.updated_at = DateTime.Now;

                    totalAmount += obj.amount; // Sum up the total amount

                    lstOrderDetail.Add(obj);
                }

                db.orderdetails.AddRange(lstOrderDetail);
                db.SaveChanges();

                // Update the order with the total amount
                objOrder.Amount = (double?)totalAmount;
                db.Entry(objOrder).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();



                // Clear the cart
                ClearCart();

                return RedirectToAction("PaymentSuccess", "Cart");
            }
        }

        public ActionResult PaymentSuccess()
        {
            return View();
        }
    }
}