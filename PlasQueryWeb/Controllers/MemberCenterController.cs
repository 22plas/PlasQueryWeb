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
        // GET: /会员中心/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
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