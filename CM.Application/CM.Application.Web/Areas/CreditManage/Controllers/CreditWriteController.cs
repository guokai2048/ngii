using LeaRun.Application.Entity.CreditManage;
using LeaRun.Application.Busines.CreditManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using LeaRun.Application.Busines.Comm;
using System.Linq;
using LeaRun.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;

namespace CM.Application.Web.Areas.CreditManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：系统管理员
    /// 日 期：2019-06-19 22:29
    /// 描 述：信用记录
    /// </summary>
    public class CreditWriteController : MvcControllerBase
    {
        private CreditWriteBLL creditwritebll = new CreditWriteBLL();

        #region 视图功能
        /// <summary>
        /// 信用 记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteIndex()
        {
            return View();
        }
        /// <summary>
        ///信用 记录 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteForm()
        {
            return View();
        }
        /// <summary>
        ///  信用 报告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteReport()
        {
            return View();
        }
        /// <summary>
        ///  信用 报告 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteCount()
        {
            return View();
        }


        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表 信用记录
        /// </summary>
        /// <param name="splitPage">分页参数</param>
        /// <returns></returns>
        public ActionResult GetDataGridJson(SplitPage splitPage)
        {
            string sqlw = SxSqlHelperBLL.getSqlw(splitPage);
            int total = 0;
            System.Data.DataTable dt = new System.Data.DataTable();
            SxSqlHelperBLL.getPageDataEasyUi(sqlw, "CR_CreditWrite", splitPage, ref total, ref dt);
            if (!TB.CheckTB(dt))
            {
                dt = new System.Data.DataTable();
            }
            var jsonData = new { rows = dt, total = total };
            return ToJsonResult(jsonData);
        }

        /// <summary>
        /// 获取列表 信用报告
        /// </summary> 
        public ActionResult GetDataGridJsonAll(string userID)
        {
            decimal allFen = 0;
            DataTable dt = creditwritebll.GetCurrentFen(userID, ref allFen);

            if (!TB.CheckTB(dt))
            {
                dt = new System.Data.DataTable();
            }
            var jsonData = new { rows = dt, total = dt.Rows.Count, allFen = allFen };
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
            var data = creditwritebll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 获取实体 
        /// </summary>
        public ActionResult GetFormJsonV(string userID, string iYear)
        {
            int xYear = iYear.ToInt();
            var data = SxSqlHelperBLL.GetIRepository().FindEntity<CreditWriteEntity>(t => t.userID == userID && t.iYear == xYear);
            return ToJsonResult(data);
        }

        /// <summary>
        /// 重复项检测 
        /// </summary>
        /// <param name="cinvClassName">检测字段值</param>
        /// <param name="keyValue">主键</param>
        /// <returns>True False</returns>
        public ActionResult ExistItemCode(string cinvClassName, string keyValue)
        {
            bool IsOk = new DataCommBLL().CheckExistData("cinvClassName", cinvClassName, "CR_CreditWrite", "id", keyValue);
            return Content((!IsOk).ToString());
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
            creditwritebll.RemoveForm(keyValue);
            return Success("删除成功。");
        }

        /// <summary>
        /// 模拟 信用云计算
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult CreditSumCount(string userID)
        {
            //SplitPage sp = new SplitPage();

            //List<CreditWriteEntity> list = creditwritebll.GetList(sp).ToList<CreditWriteEntity>()
            //if(list==null||list.Count<=0)
            //{

            //}

            string sql = "select * from CR_CreditWrite where userID='" + userID + "'";
            DataTable tt = SxSqlHelperBLL.GetIRepository().FindTable(sql);
            if (TB.CheckTB(tt))
            {
                System.Threading.Thread.Sleep(1 * 1000);
                return Success("操作完成");

            }
            else
            {
                return Error("当前用户没有信用记录");
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
        public ActionResult SaveForm(string keyValue, CreditWriteEntity entity)
        {
            int iyear = entity.iYear.ToInt();
            if (iyear <= 0)
            {
                return Error("年度录入不正确");
            }
            string sql = "select id from CR_CreditWrite where userID='" + entity.userID + "' and iYear='" + iyear + "' ";
            if (keyValue.ToStringV() != Str.Empty)
            {
                sql += " and id!='" + keyValue + "'";
            }
            DataTable tt = SxSqlHelperBLL.FindTable(sql);
            if (TB.CheckTB(tt))
            {
                return Error("用户[" + entity.userName + "]年度[" + iyear.ToString() + "]信息已经存在，不能重复录入");
            }
            creditwritebll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
