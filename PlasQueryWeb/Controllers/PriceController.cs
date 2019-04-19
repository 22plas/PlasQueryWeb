using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class PriceController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET: 价格
        public ActionResult Index()
        {
            var ys_character = new DataTable();
            var company = new DataTable();
            //种类
            ys_character = bll.GetSearchParam(1);
            //生产厂家
            company = bll.GetSearchParam(4);

            ViewBag.ProductType = ys_character;
            ViewBag.company = company;
            return View();
        }
    }
}