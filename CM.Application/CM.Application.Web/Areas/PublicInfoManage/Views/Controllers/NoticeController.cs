using LeaRun.Application.Busines.PublicInfoManage;
using LeaRun.Application.Code;
using LeaRun.Application.Entity.PublicInfoManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using System.Web.Mvc;

namespace CM.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017-2018 zhangjh.com
    /// 创建人：zhangjh
    /// 日 期：2015.12.7 16:40
    /// 描 述：电子公告
    /// </summary>
    public class NoticeController : MvcControllerBase
    {
        private NoticeBLL noticeBLL = new NoticeBLL();

        #region 视图功能
        /// <summary>
        /// 公告管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        } 
        /// <summary>
        /// 公告管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult IndexLook()
        {
            return View();
        }
        /// <summary>
        /// 公告表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 公告详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Ignore)]
        public ActionResult Detail()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 公告列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            pagination.sidx = pagination.sort;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page <= 0 ? 1 : pagination.page);
            pagination.rows = (pagination.rows <= 0 ? 0 : pagination.rows);
            //var watch = CommonHelper.TimerStart();
            var data = noticeBLL.GetPageList(pagination, queryJson);
            var JsonData = new
            {
                rows = data,
                total = pagination.records
                //page = pagination.page,
                //records = pagination.records,
                //costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 公告实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = noticeBLL.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 首页公告列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        public ActionResult getMyNotice()
        {

            var data = noticeBLL.GetList();

            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RemoveForm(string keyValue)
        {
            noticeBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存公告表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="newsEntity">公告实体</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        public ActionResult SaveForm(string keyValue, NewsEntity newsEntity)
        {
            noticeBLL.SaveForm(keyValue, newsEntity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 发布 
        /// </summary>
        /// <param name="keyValue">主表ID</param>
        /// <param name="opeState">1 操作，其它取消</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult EnableForm(string keyValue, int opeState)
        {

            string mess = "请重试:未知提示";
            bool isOK = noticeBLL.EnableState(keyValue, opeState == 1 ? true : false, ref mess);
            if (isOK)
            {
                return Success("操作成功。");
            }
            else
            {
                return Error(mess);
            }
        }
        #endregion
    }
}
