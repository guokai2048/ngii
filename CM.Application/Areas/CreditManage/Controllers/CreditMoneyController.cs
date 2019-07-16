using CM.Application.Entity.CreditManage;
using CM.Application.Busines.CreditManage;
using CM.Util;
using CM.Util.WebControl;
using CM.Application.Busines.Comm;
using System.Linq;
using CM.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using CM.Application.Busines.PublicInfoManage;
using CM.Application.Code;

namespace CM.Application.Web.Areas.CreditManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：系统管理员
    /// 日 期：2019-06-21 22:32
    /// 描 述：贷款申请
    /// </summary>
    public class CreditMoneyController : MvcControllerBase
    {
        private CreditMoneyBLL creditmoneybll = new CreditMoneyBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyIndex()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyForm()
        {
            return View();
        }

        /// <summary>
        /// 表单页面 评价
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyIndexAss()
        {
            return View();
        }
        /// <summary>
        /// 表单页面 评价  计算
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyCount()
        {
            return View();
        }
        /// <summary>
        /// 表单页面 评价  计算
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyLook()
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
            SxSqlHelperBLL.getPageDataEasyUi(sqlw, "CR_CreditMoney", splitPage, ref total, ref dt);
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
        public ActionResult GetDataGridJsonV(int? typeX)
        {
            SplitPage splitPage = new SplitPage()
            {
                Conditions = new Conditions[]
                {
                    new Conditions () { Key="isnull(isOK,-1)",Value=typeX.ToStringV() }
                }
            };
            splitPage.order = "desc";
            splitPage.sort = "createDate";
            string sqlw = SxSqlHelperBLL.getSqlw(splitPage);
            int total = 0;
            System.Data.DataTable dt = new System.Data.DataTable();
            SxSqlHelperBLL.getPageDataEasyUi(sqlw, "CR_CreditMoney", splitPage, ref total, ref dt);
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
            var data = creditmoneybll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 获取实体  附件 
        /// </summary>
        [HttpGet]
        public ActionResult GetFormJsonFile(string keyValue)
        {
            AttachmentBLL att = new AttachmentBLL(); //附件
            var data = creditmoneybll.GetEntity(keyValue);
            var attach = att.GetFormList(data.filesPath);
            var jsonData = new
            {
                entitys = data,
                attachs = attach
            };
            return ToJsonResult(jsonData);
        }

        /// <summary>
        /// 重复项检测 
        /// </summary>
        /// <param name="cinvClassName">检测字段值</param>
        /// <param name="keyValue">主键</param>
        /// <returns>True False</returns>
        public ActionResult ExistItemCode(string cinvClassName, string keyValue)
        {
            bool IsOk = new DataCommBLL().CheckExistData("cinvClassName", cinvClassName, "CR_CreditMoney", "id", keyValue);
            return Content((!IsOk).ToString());
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 模拟 信用云计算
        /// </summary> 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult CreditSumCount(string id)
        {

            string memo = Str.Empty;
            List<CreditMoneyEntity> list = new List<CreditMoneyEntity>();

            CreditMoneyEntity cur = creditmoneybll.GetEntity(id);
            if (cur != null)
            {
                list.Add(cur);
            }
            else
            {
                return Error("不存在信息[" + id.ToStringV() + "]，不能操作");
            }
            System.Threading.Thread.Sleep(1 * 1000);

            bool isOK = creditmoneybll.PingJia(list, ref memo);
            if (isOK)
            {
                return Success("评价完成。");
            }
            else

            {
                return Error(memo);
            }
        }

        /// <summary>
        /// 评价录入
        /// </summary> 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult CreditPingSave(string id, int? isOK, string isOKStr)
        {
            CreditMoneyEntity cur = creditmoneybll.GetEntity(id);
            if (cur == null)
            {
                return Error("不存在信息[" + id.ToStringV() + "] 不能操作");
            }
            cur.isOK = isOK.ToInt();
            cur.isOKStr = isOKStr.ToStringV();
            cur.pingDate = Time.Now22;
            cur.pingUserID = OperatorProvider.Provider.Current().UserId;
            cur.pingUserName = OperatorProvider.Provider.Current().UserName;


            SxSqlHelperBLL.GetIRepository().Update<CreditMoneyEntity>(cur);

            return Success("评价完成。");


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
            creditmoneybll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, CreditMoneyEntity entity)
        {
            creditmoneybll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
