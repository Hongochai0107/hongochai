using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HoNgocHai_2122110473_ASP.NET.Models;
using HoNgocHai_2122110473_ASP.NET.Helpers;
using System.Data.Entity.Validation;

namespace HoNgocHai_2122110473_ASP.NET.Controllers
{
    public class UsersController : Controller
    {
        private HshopEntities db = new HshopEntities();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaKH,MatKhau,HoTen,GioiTinh,NgaySinh,DiaChi,DienThoai,Email,Hinh,HieuLuc,VaiTro,RandomKey")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKH,MatKhau,HoTen,GioiTinh,NgaySinh,DiaChi,DienThoai,Email,Hinh,HieuLuc,VaiTro,RandomKey")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "MaKH,MatKhau,HoTen,GioiTinh,NgaySinh,DiaChi,DienThoai,Email,Hinh,VaiTro,Username")] User user)
        {
            if (ModelState.IsValid)
            {
                    user.MatKhau = PasswordHelper.HashPassword(user.MatKhau);

                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login", "Users");
            }

            return View(user);
        }


        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,MatKhau")] User model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.MatKhau))
                {
                    if (string.IsNullOrEmpty(model.Username))
                    {
                        ModelState.AddModelError("Username", "Username khong duoc de trong.");
                    }

                    if (string.IsNullOrEmpty(model.MatKhau))
                    {
                        ModelState.AddModelError("Password", "Password khong duoc de trong.");
                    }

                    return View(); // Return the view with validation errors
                }

                var user = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (user != null && PasswordHelper.VerifyPassword(model.MatKhau, user.MatKhau))
                {
                    // Successfully authenticated, set up session or authentication cookie
                    // Example:
                    Session["UserID"] = user.MaKH;
                    Session["UserName"] = user.HoTen;
                    Session["Address"] = user.DiaChi;
                    Session["Roles"] = user.VaiTro;
                    

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            return View(model);
        }
        [HttpPost]
        public ActionResult Logout()
        {
            // Xóa dữ liệu phiên của người dùng
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Users");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
