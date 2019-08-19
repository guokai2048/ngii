using LeaRun.Application.Busines.SystemManage;
using LeaRun.Application.Code;
using LeaRun.Util;
using LeaRun.Util.Extension;
using LeaRun.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CM.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017-2018 zhangjh.com
    /// 创建人：zhangjh
    /// 日 期：2018.11.18 9:56
    /// 描 述：系统日志
    /// </summary>
    public class LogController : MvcControllerBase
    {
        #region 视图功能
        /// <summary>
        /// 日志管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RemoveLog()
        {
            return View();
        }
        /// <summary>
        ///  日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            pagination.sidx = pagination.sort;
            pagination.sord = pagination.order;
            var data = LogBLL.GetPageList(pagination, queryJson);
            var JsonData = new
            {
                rows = data,
                total = pagination.records,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }


        /// <summary>
        /// 日志列表
        /// </summary> 
        public ActionResult GetPageListV1(Pagination pagination, SplitPage splitPage, int? CategoryId)
        {
            pagination.sidx = pagination.sort;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page <= 0 ? 1 : pagination.page);
            pagination.rows = (pagination.rows <= 0 ? 0 : pagination.rows);
            List<Conditions> lisWhere = new List<Conditions>();
            if (splitPage.Conditions == null)
            {
                lisWhere = new List<Conditions>();
            }
            else
            {
                lisWhere = splitPage.Conditions.ToList<Conditions>();
            }
            if (CategoryId.ToInt() > 0)
            {
                lisWhere.Add(new Conditions() { Key = "CategoryId", Value = CategoryId.ToStringV() });
            }

            splitPage.Conditions = lisWhere.ToArray();

            var data = LogBLL.GetPageListV(pagination, splitPage);
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
            return ToJsonResult(jsonData);
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = LogBLL.GetEntity(keyValue);
            return ToJsonResult(data);
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="categoryId">日志分类Id</param>
        /// <param name="keepTime">保留时间段内</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveLog(int categoryId, string keepTime)
        {
            LogBLL.RemoveLog(categoryId, keepTime);
            return Success("清空成功。");
        }
        #endregion
    }
}
