using CM.Application.Entity.PublicInfoManage;
using CM.Application.Busines.PublicInfoManage;
using CM.Util;
using CM.Util.WebControl;
using CM.Application.Busines.Comm;
using System.Linq;
using CM.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using CM.Application.Code;

namespace CM.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：超级管理员
    /// 日 期：2017-08-07 08:26
    /// 描 述：消息表
    /// </summary>
    public class MessageController : MvcControllerBase
    {
        private MessageBLL messagebll = new MessageBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MessageIndex()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MessageForm()
        {
            return View();
        }
        /// <summary>
        /// 表单详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MessageDetail()
        {
            return View();
        }
        /// <summary>
        /// 我的消息列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyMessageIndex()
        {
            return View();
        }
        #endregion

        #region 获取数据

        /// <summary>
        /// 我的消息列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="splitPage">查询参数</param>
        /// <param name="usage">使用范围(前台使用)</param>
        /// <param name="curStatus">状态</param>
        /// <returns>返回分页列表Json</returns>
        public ActionResult getMyMessage(Pagination pagination, SplitPage splitPage, string usage, string curStatus)
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
                lisWhere.Add(new Conditions() { Key = "curStatus", Value = "0" });//未读

                splitPage.order = "desc";
                splitPage.sort = "CreateDate";
                splitPage.Conditions = lisWhere.ToArray();
                var data = messagebll.GetMyList(pagination, splitPage, usage);
                return Content(data.ToJson());
            }
            else
            {
                //列表使用
                lisWhere.Add(new Conditions() { Key = "receiver", Value = OperatorProvider.Provider.Current().UserId });
                lisWhere.Add(new Conditions() { Key = "isDelete", Value = "0" });
                lisWhere.Add(new Conditions() { Key = "curStatus", Value = curStatus });

                splitPage.order = "desc";
                splitPage.sort = "CreateDate";
                splitPage.Conditions = lisWhere.ToArray();

                var data = messagebll.GetMyList(pagination, splitPage, usage);
                var jsonData = new
                {
                    rows = data,
                    total = pagination.records,
                };
                return ToJsonResult(jsonData);
            }


        }
        /// <summary>
        /// 我的消息数量
        /// </summary>
        /// <returns>返回分页列表Json</returns>
        public ActionResult getMyMessageNum(SplitPage splitPage)
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
            lisWhere.Add(new Conditions() { Key = "curStatus", Value = "0" });//未读

            splitPage.Conditions = lisWhere.ToArray();
            var data = messagebll.GetMyList(splitPage);
            var jsonData = new
            {
                messageNum = data.Count()
            };
            return ToJsonResult(jsonData);

        }
        /// <summary>
        /// 消息实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            SplitPage splitPage = new CM.Util.WebControl.SplitPage();
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
            lisWhere.Add(new Conditions() { Key = "id", Value = keyValue });

            splitPage.Conditions = lisWhere.ToArray();
            var data = messagebll.GetMyList(splitPage).FirstOrDefault();

            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 回收数据 批量
        /// </summary>
        /// <param name="entitys">集合</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RecycleForm(string entitys)
        {
            List<MessageEntity> mainEntitys = entitys.ToList<MessageEntity>();
            string mess = "请重试:未知提示";
            bool isOK = messagebll.RecycleForm(mainEntitys, ref mess);

            if (isOK)
            {
                return Success("删除成功。");
            }
            else
            {
                return Error(mess);
            }
        }

        /// <summary>
        /// 已读数据 批量
        /// </summary>
        /// <param name="entitys">集合</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult ReadForm(string entitys)
        {
            List<MessageEntity> mainEntitys = entitys.ToList<MessageEntity>();
            string mess = "请重试:未知提示";
            bool isOK = messagebll.ReadForm(mainEntitys, ref mess);

            if (isOK)
            {
                return Success("删除成功。");
            }
            else
            {
                return Error(mess);
            }


        }


        /// <summary>
        /// 删除数据 批量
        /// </summary>
        /// <param name="entitys">集合</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string entitys)
        {

            List<MessageEntity> mainEntitys = entitys.ToList<MessageEntity>();
            string mess = "请重试:未知提示";
            bool isOK = messagebll.RemoveFormPL(mainEntitys, ref mess);

            if (isOK)
            {
                return Success("删除成功。");
            }
            else
            {
                return Error(mess);
            }

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
        public ActionResult SaveForm(string keyValue, MessageEntity entity)
        {
            messagebll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
