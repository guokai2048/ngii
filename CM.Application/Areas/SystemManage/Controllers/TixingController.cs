using CM.Application.Entity.SystemManage;
using CM.Application.Busines.SystemManage;
using CM.Util;
using CM.Util.WebControl;
using CM.Application.Busines.Comm;
using System.Linq;
using CM.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using CM.Application.Code;

namespace CM.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：超级管理员
    /// 日 期：2017-08-01 15:17
    /// 描 述：证件提醒
    /// </summary>
    public class TixingController : MvcControllerBase
    {
        private TixingBLL tixingbll = new TixingBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TixingIndex()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult TixingForm()
        {
            return View();
        }
        /// <summary>
        /// 我的提醒列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyTixingIndex()
        {
            return View();
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult GetDataGridJson(Pagination pagination, string queryJson)
        {
            pagination.sidx = pagination.sort ;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page<=0? 1:pagination.page);
            pagination.rows = (pagination.rows<=0? 0:pagination.rows);
            var watch = CommonHelper.TimerStart();
            var data = tixingbll.GetPageList(pagination, queryJson);
            var jsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return ToJsonResult(jsonData);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <param name="itemId">分类ID</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult GetDataGridJsonV1(Pagination pagination, string queryJson, string itemId)
        {
            pagination.sidx = pagination.sort ;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page<=0? 1:pagination.page);
            pagination.rows = (pagination.rows<=0? 0:pagination.rows);

            var queryParam = queryJson.ToJObject();
            SplitPage splitPage = new Util.WebControl.SplitPage();
            List<Conditions> lisWhere = new List<Conditions>();
            //查询条件
            if (!queryParam["condition"].IsEmpty() && !queryParam["keyword"].IsEmpty())
            {
                string condition = queryParam["condition"].ToString();
                string keyword = queryParam["keyword"].ToString(); 
                lisWhere.Add(new Util.WebControl.Conditions() { Key = condition, Value=keyword, IsLike=true });
            }
            if(!string.IsNullOrEmpty(itemId))
            {
                lisWhere.Add(new Util.WebControl.Conditions() { Key = "cinvClassCode", Value = itemId, IsLike = true });
            }
            splitPage.Conditions = lisWhere.ToArray();
            var data = tixingbll.GetPageList(pagination, splitPage);
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
            return ToJsonResult(jsonData);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="splitPage">查询对象</param>
        /// <returns></returns>
        public ActionResult GetDataGridJsonV2(Pagination pagination, SplitPage splitPage)
        {
            pagination.sidx = pagination.sort ;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page<=0? 1:pagination.page);
            pagination.rows = (pagination.rows<=0? 0:pagination.rows);
            var data = tixingbll.GetPageList(pagination, splitPage); 
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
            return ToJsonResult(jsonData);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(string queryJson)
        {
            var data = tixingbll.GetList(queryJson);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = tixingbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 我的提醒列表
        /// </summary>
        /// <param name="splitPage">查询参数</param>
        /// <param name="usage">使用范围(前台使用)</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult getMyTixing(SplitPage splitPage, string usage)
        {

            List<Conditions> lisWhere = new List<Conditions>();
            if (splitPage.Conditions == null)
            {
                lisWhere = new List<Conditions>();
            }
            else
            {
                lisWhere = splitPage.Conditions.ToList<Conditions>();
            }

            lisWhere.Add(new Conditions() { Key = "receiver", Value = OperatorProvider.Provider.Current().UserId });
            lisWhere.Add(new Conditions() { Key = "isDelete", Value = "0" });

            splitPage.order = "asc";
            splitPage.sort = "CreateDate";
            splitPage.Conditions = lisWhere.ToArray();
            var data = tixingbll.GetMyList(splitPage, usage);
            return Content(data.ToJson());

        }
        #endregion

        #region 提交数据
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
            tixingbll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, TixingEntity entity)
        {
            tixingbll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
