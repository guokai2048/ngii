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
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ����ϵͳ����Ա
    /// �� �ڣ�2019-06-19 22:29
    /// �� �������ü�¼
    /// </summary>
    public class CreditWriteController : MvcControllerBase
    {
        private CreditWriteBLL creditwritebll = new CreditWriteBLL();

        #region ��ͼ����
        /// <summary>
        /// ���� ��¼
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteIndex()
        {
            return View();
        }
        /// <summary>
        ///���� ��¼ ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteForm()
        {
            return View();
        }
        /// <summary>
        ///  ���� ����
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteReport()
        {
            return View();
        }
        /// <summary>
        ///  ���� ���� ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditWriteCount()
        {
            return View();
        }


        #endregion

        #region ��ȡ����

        /// <summary>
        /// ��ȡ�б� ���ü�¼
        /// </summary>
        /// <param name="splitPage">��ҳ����</param>
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
        /// ��ȡ�б� ���ñ���
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
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = creditwritebll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// ��ȡʵ�� 
        /// </summary>
        public ActionResult GetFormJsonV(string userID, string iYear)
        {
            int xYear = iYear.ToInt();
            var data = SxSqlHelperBLL.GetIRepository().FindEntity<CreditWriteEntity>(t => t.userID == userID && t.iYear == xYear);
            return ToJsonResult(data);
        }

        /// <summary>
        /// �ظ����� 
        /// </summary>
        /// <param name="cinvClassName">����ֶ�ֵ</param>
        /// <param name="keyValue">����</param>
        /// <returns>True False</returns>
        public ActionResult ExistItemCode(string cinvClassName, string keyValue)
        {
            bool IsOk = new DataCommBLL().CheckExistData("cinvClassName", cinvClassName, "CR_CreditWrite", "id", keyValue);
            return Content((!IsOk).ToString());
        }
        #endregion

        #region �ύ����
        /// <summary>
        /// ɾ������
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string keyValue)
        {
            creditwritebll.RemoveForm(keyValue);
            return Success("ɾ���ɹ���");
        }

        /// <summary>
        /// ģ�� �����Ƽ���
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
                return Success("�������");

            }
            else
            {
                return Error("��ǰ�û�û�����ü�¼");
            }
        }


        /// <summary>
        /// ��������������޸ģ�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <param name="entity">ʵ�����</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, CreditWriteEntity entity)
        {
            int iyear = entity.iYear.ToInt();
            if (iyear <= 0)
            {
                return Error("���¼�벻��ȷ");
            }
            string sql = "select id from CR_CreditWrite where userID='" + entity.userID + "' and iYear='" + iyear + "' ";
            if (keyValue.ToStringV() != Str.Empty)
            {
                sql += " and id!='" + keyValue + "'";
            }
            DataTable tt = SxSqlHelperBLL.FindTable(sql);
            if (TB.CheckTB(tt))
            {
                return Error("�û�[" + entity.userName + "]���[" + iyear.ToString() + "]��Ϣ�Ѿ����ڣ������ظ�¼��");
            }
            creditwritebll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
