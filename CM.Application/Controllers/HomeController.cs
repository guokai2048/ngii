using CM.Application.Busines;
using CM.Application.Busines.BaseManage;
using CM.Application.Busines.SystemManage;
using CM.Application.Code;
using CM.Application.Entity;
using CM.Application.Entity.SystemManage;
using CM.Util;
using CM.Util.Attributes;
using CM.Util.Log;
using CM.Util.Offices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CM.Application.Web.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017-2018 zhangjh.com
    /// 创建人：zhangjh
    /// 日 期：2015.09.01 13:32
    /// 描 述：系统首页
    /// </summary>
    [HandlerLogin(LoginMode.Enforce)]
    public class HomeController : Controller
    {
        UserBLL user = new UserBLL();
        DepartmentBLL department = new DepartmentBLL();

        #region 视图功能
        /// <summary>
        /// 后台框架页
        /// </summary>
        /// <returns></returns>
        public ActionResult AdminDefault()
        {
            return View();
        }
        public ActionResult AdminLTE()
        {
            return View();
        }
        public ActionResult AdminWindos()
        {
            return View();
        }
        public ActionResult AdminPretty()
        {
            return View();
        }
        public ActionResult AdminDefaultDesktop()
        {
            return View();
        }
        public ActionResult AdminLTEDesktop()
        {
            return View();
        }
        public ActionResult AdminWindosDesktop()
        {
            return View();
        }
        public ActionResult AdminPrettyDesktop()
        {
            return View();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 访问功能
        /// </summary>
        /// <param name="moduleId">功能Id</param>
        /// <param name="moduleName">功能模块</param>
        /// <param name="moduleUrl">访问路径</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult VisitModule(string moduleId, string moduleName, string moduleUrl)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 2;
            logEntity.OperateTypeId = ((int)OperationType.Visit).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Visit);
            logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
            logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
            logEntity.ModuleId = moduleId;
            logEntity.Module = moduleName;
            logEntity.ExecuteResult = 1;
            logEntity.ExecuteResultJson = "访问地址：" + moduleUrl;
            logEntity.WriteLog();
            return Content(moduleId);
        }
        /// <summary>
        /// 离开功能
        /// </summary>
        /// <param name="moduleId">功能模块Id</param>
        /// <returns></returns>
        public ActionResult LeaveModule(string moduleId)
        {
            return null;
        }
        #endregion
    }
}
