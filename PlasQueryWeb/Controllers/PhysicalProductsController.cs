﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class PhysicalProductsController : Controller
    {
        private PlasBll.ProductBll bll = new PlasBll.ProductBll();
        // GET:超级搜索
        public ActionResult Index()
        {
            int rowscount = 10;
            var ys_character = new DataTable();
            var texing = new DataTable();
            var zuran = new DataTable();
            var company = new DataTable();
            var jiagong = new DataTable();
            var used = new DataTable();
            var addliao = new DataTable();
            var tianjiaji = new DataTable();


            //种类
            ys_character = bll.GetSearchParam(1);
            //特性
            texing = bll.GetSearchParam(2);
            //阻燃等级
            zuran = bll.GetSearchParam(3);
            //生产厂家
            company = bll.GetSearchParam(4);
            //加工方法
            jiagong = bll.GetSearchParam(5);
            //产品用途
            used = bll.GetSearchParam(7);
            //填料/增强
            addliao = bll.GetSearchParam(8);
            //添加剂
            tianjiaji = bll.GetSearchParam(9);

            //属性值
            var attr = new DataTable();
            attr = bll.Sys_GetSuperSearchParam(-1).Tables[0];
            ViewBag.attr = attr;


            ViewBag.ProductType = ys_character;
            ViewBag.texing = texing;
            //zuran company jiagong used addliao tianjiaji
            ViewBag.zuran = zuran;
            ViewBag.company = company;
            ViewBag.jiagong = jiagong;
            ViewBag.used = used;
            ViewBag.addliao = addliao;
            ViewBag.tianjiaji = tianjiaji;
            return View();
        }

        //产品详情
        public ActionResult Detail(string prodid)
        {
            #region 产品详情
            var ds = bll.GetModelInfo(prodid);
            if (ds != null && ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ViewBag.ProModel = ds.Tables[0].Rows[0]["proModel"];
                }
                if (ds.Tables.Count > 1)
                {
                    //物性
                    ViewBag.PhysicalInfo = ds.Tables[1];
                    //从物性中匹配产品说明
                    for (var i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (ds.Tables[1].Rows[i]["lev"].ToString() == "1" && ds.Tables[1].Rows[i]["Attribute1"].ToString().Contains("产品说明"))
                        {
                            if (i + 1 <= ds.Tables[1].Rows.Count && ds.Tables[1].Rows[i + 1]["lev"].ToString() == "2")
                            {
                                ViewBag.description = ds.Tables[1].Rows[i + 1]["Attribute1"].ToString().Replace("\n", "").Replace("\r\n", "").Replace(" ", "");
                            }
                            break;
                        }
                    }

                }
                if (ds.Tables.Count > 2)
                {
                    //详情页标题：种类（Prd_SmallClass_l.Name）+型号（Product.ProModel）+产地（Product.PlaceOrigin）
                    ViewBag.Title = ds.Tables[2].Rows[0]["Title"].ToString();
                    //关键字：特性(product_l.characteristic)+用途(product_l.ProUse)
                    ViewBag.Keywords = ds.Tables[2].Rows[0]["keyword"].ToString();
                    //ViewBag.description2 =产品说明(只能用 exec readproduct '0004D924-5BD4-444F-A6D2-045D4EDB0DD3'命令中读出)
                }

            }
            #endregion

            #region 猜你喜欢和同厂家产品
            var LiveDs = bll.GetCompanyAndLiveProduct(10, prodid);
            #endregion

            ViewBag.ProdID = prodid;
            ViewBag.LiveProdcut = LiveDs;
            return View();
        }




        public ActionResult More(int rid, string rname, string more)
        {
            int datatCount = 1000000;
            var allData = new DataTable();
            if (rid > 0)
            {
                allData = bll.GetSearchParam(rid, datatCount);
            }
            ViewBag.allData = allData;
            ViewBag.rname = rname;
            ViewBag.rid = rid;
            ViewBag.more = more;
            return View();
        }

        //属性值
        public ActionResult Attribute()
        {
            var attr = new DataTable();
            attr = bll.Sys_GetSuperSearchParam(-1).Tables[0];
            ViewBag.attr = attr;
            return View();
        }

        /// <summary>
        /// 超级搜素
        /// </summary>
        /// <returns></returns>
        public JsonResult SuperMsgSearch(int pageindex, int pagesize, string guidstr)
        {
            string searchstr = string.Empty;//关键词
            if (!string.IsNullOrWhiteSpace(Request["searchstr"]))
            {
                searchstr = Request["searchstr"].ToString();
            }

            var ds = bll.Sys_SuperSearch(searchstr, 2052, pageindex, pagesize, guidstr);
            string jsonstr = PlasCommon.ToolHelper.DataTableToJson(ds.Tables[0]);
            int count = 0;
            if (ds.Tables.Contains("ds1") && ds.Tables[1] != null && ds.Tables[1].Rows.Count > 0)
            {
                int.TryParse(ds.Tables[1].Rows[0]["totalcount"].ToString(), out count);
            }
            return Json(new { data = jsonstr, totalCount = count }, JsonRequestBehavior.AllowGet);
        }

    }
}