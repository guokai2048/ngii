using LeaRun.Application.Entity.BaseManage;
using LeaRun.Application.Busines.BaseManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using LeaRun.Application.Busines.Comm;
using System.Linq;
using LeaRun.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CM.Application.Web.Areas.BaseManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：系统管理员
    /// 日 期：2019-05-08 15:05
    /// 描 述：用户员工表
    /// </summary>
    public class PersonController : MvcControllerBase
    {
        private PersonBLL personbll = new PersonBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonIndex()
        {
            return View();
        }

        /// <summary>
        /// 列表页面 选择
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonIndexSelect()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonForm()
        {
            return View();
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="splitPage">分页参数</param>
        /// <returns></returns>
        public ActionResult GetDataGridJson(SplitPage splitPage)
        {
            string sqlw = SxSqlHelperBLL.getSqlw(splitPage);
            int total = 0;
            System.Data.DataTable dt = new System.Data.DataTable();
            SxSqlHelperBLL.getPageDataEasyUi(sqlw, "Base_Person", splitPage, ref total, ref dt);
            if (!TB.CheckTB(dt))
            {
                dt = new System.Data.DataTable();
            }
            var jsonData = new { rows = dt, total = total };
            return ToJsonResult(jsonData);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        public ActionResult GetDataGridJsonV(string userID)
        {
            SplitPage s = new SplitPage()
            {
                Conditions = new Conditions[]
                {
                     new Conditions () {  Key="userID",Value=userID }
                 }

            };
            s.sort = "CreateDate";
            s.order = "desc";
            int total = 0;
            string sqlw = SxSqlHelperBLL.getSqlw(s);
            System.Data.DataTable dt = new System.Data.DataTable();
            SxSqlHelperBLL.getPageDataEasyUi(sqlw, "Base_Person", s, ref total, ref dt);
            if (!TB.CheckTB(dt))
            {
                dt = new System.Data.DataTable();
            }
            var jsonData = new { rows = dt, total = total };
            return ToJsonResult(jsonData);
        }

        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = personbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 重复项检测 
        /// </summary>
        /// <param name="EnCode">检测字段值</param>
        /// <param name="keyValue">主键</param>
        /// <returns>True False</returns>
        public ActionResult ExistEnCode(string EnCode, string keyValue)
        {
            bool IsOk = new DataCommBLL().CheckExistData("EnCode", EnCode, "Base_Person", "UserId", keyValue);
            return Content((!IsOk).ToString());
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 禁用账户
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult DisabledEnCode(string keyValue)
        {
            PersonEntity per = personbll.GetEntity(keyValue);
            if (per == null || per.UserId.ToStringV() == Str.Empty)
            {
                return Error("员工信息不存在");
            }
            else
            {
                personbll.UpdateState(per, 0);
                return Success("员工信息 禁用成功。");

            }
        }
        /// <summary>
        /// 启用账户
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult EnabledEnCode(string keyValue)
        {
            PersonEntity per = personbll.GetEntity(keyValue);
            if (per == null || per.UserId.ToStringV() == Str.Empty)
            {
                return Error("员工信息不存在");
            }
            else
            {
                personbll.UpdateState(per, 1);
                return Success("员工信息 启用成功。");
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string keyValue)
        {
            personbll.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, PersonEntity entity)
        {
            personbll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
