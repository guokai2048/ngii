using LeaRun.Application.Entity.PublicInfoManage;
using LeaRun.Application.Busines.PublicInfoManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using LeaRun.Application.Busines.Comm;
using System.Linq;
using LeaRun.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using LeaRun.Application.Code;

namespace CM.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：超级管理员
    /// 日 期：2017-08-03 08:32
    /// 描 述：待办任务表
    /// </summary>
    public class MissionController : MvcControllerBase
    {
        private MissionBLL missionbll = new MissionBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MissionIndex()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MissionForm()
        {
            return View();
        }
        /// <summary>
        /// 我的待办
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyMissionIndex()
        {
            return View();
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = missionbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }

        /// <summary>
        /// 我的待办列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="splitPage">查询参数</param>
        /// <param name="usage">使用范围(前台使用)</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult getMyMission(Pagination pagination, SplitPage splitPage, string usage)
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
            //桌面使用
            if (!string.IsNullOrEmpty(usage) && usage.Equals("top"))
            {
                lisWhere.Add(new Conditions() { Key = "receiver", Value = OperatorProvider.Provider.Current().UserId });
                lisWhere.Add(new Conditions() { Key = "isDelete", Value = "0" });
                lisWhere.Add(new Conditions() { Key = "completeStatus", Value = "0" });

                splitPage.order = "asc";
                splitPage.sort = "CreateDate";
                splitPage.Conditions = lisWhere.ToArray();
                var data = missionbll.GetMyList(pagination, splitPage, usage);
                return Content(data.ToJson());
            }
            else
            {
                //列表使用
                lisWhere.Add(new Conditions() { Key = "receiver", Value = OperatorProvider.Provider.Current().UserId });
                lisWhere.Add(new Conditions() { Key = "isDelete", Value = "0" });
                splitPage.Conditions = lisWhere.ToArray();
                var data = missionbll.GetMyList(pagination, splitPage, usage);
                var jsonData = new
                {
                    rows = data,
                    total = pagination.records,
                };
                return ToJsonResult(jsonData);
            }

        }
        /// <summary>
        /// 我的待办数量
        /// </summary>
        /// <returns>返回分页列表Json</returns>
        public ActionResult getMyMissionNum(SplitPage splitPage)
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
            lisWhere.Add(new Conditions() { Key = "completeStatus", Value = "0" });

            splitPage.Conditions = lisWhere.ToArray();
            var data = missionbll.GetMyList(splitPage);
            var jsonData = new
            {
                missionNum = data.Count()
            };
            return ToJsonResult(jsonData);

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
            missionbll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, MissionEntity entity)
        {
            missionbll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
