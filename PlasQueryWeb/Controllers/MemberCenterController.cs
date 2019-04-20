using PlasCommon;
using PlasModel;
using PlasQueryWeb.App_Start;
using PlasQueryWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlasQueryWeb.Controllers
{
    public class MemberCenterController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //
        // GET: /会员中心首页/
        public ActionResult Index()
        {
            return View();
        }
        //登录
        public ActionResult Login()
        {
            return View();
        }
        //注册
        public ActionResult Register()
        {
            return View();
        }
        //用户信息
        public ActionResult UserInfo() {
            return View();
        }
        //公司资料
        public ActionResult CompanyInfo()
        {
            return View();
        }
        //新增公司资料
        public ActionResult CompanyInfoCreate()
        {
            return View();
        }
        //收货地址
        public ActionResult DeliveryAddress() {
            return View();
        }
        //收货地址
        public ActionResult DeliveryAddressCreate()
        {
            return View();
        }
        //物料收藏
        public ActionResult MaterialCollection() {
            return View();
        }
        //物料收藏
        public ActionResult MaterialLook()
        {
            return View();
        }
        //我要报价
        public ActionResult SellerOffer() {
            return View();
        }
        //新增卖家报价
        public ActionResult SellerOfferCreate()
        {
            return View();
        }

        //买家询价
        public ActionResult BuyerOffer()
        {
            return View();
        }
        //新增买家询价
        public ActionResult BuyerOfferCreate()
        {
            return View();
        }

        //收到的报价
        public ActionResult GetSellerOffer()
        {
            return View();
        }
        //收到的询价
        public ActionResult GetBuyerOffer()
        {
            return View();
        }

        //行情浏览记录
        public ActionResult QuotationRecord() {
            return View();
        }
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult SetCode(string phone)
        {
            string returncode = Common.GenerateCheckCodeNum(6);
            bool returnresult = Common.SmsSend(phone, returncode);
            if (returnresult)
            {
                var returnstr = new
                {
                    code = returncode
                };
                return Json(Common.ToJsonResult("Success", "成功", returnstr), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(Common.ToJsonResult("Fail", "失败"), JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 保存注册数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowCrossSiteJson]
        [HttpGet]
        public ActionResult SaveRegister(cp_user model)
        {
            if (db.cp_user.Any(s => s.Phone == model.Phone))
            {
                return Json(Common.ToJsonResult("Error", "用户已存在"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                model.LeaderUserName = string.Empty;
                db.cp_user.Add(model);
                db.SaveChanges();
            }
            return Json(Common.ToJsonResult("Fail", "失败"), JsonRequestBehavior.AllowGet);
        }
    }
}