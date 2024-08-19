using HoNgocHai_2122110473_ASP.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HoNgocHai_2122110473_ASP.NET.Controllers
{
    public class HomeController : Controller
    {
        private HshopEntities db = new HshopEntities();
        public ActionResult Index()
        {
            HomeModel model = new HomeModel();
            model.Loais = db.Loais.ToList();

            model.HangHoas = db.HangHoas.ToList();

            model.NhaCungCap =db.NhaCungCaps.ToList();
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}