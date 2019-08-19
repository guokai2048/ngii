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
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ����ϵͳ����Ա
    /// �� �ڣ�2019-05-08 15:05
    /// �� �����û�Ա����
    /// </summary>
    public class PersonController : MvcControllerBase
    {
        private PersonBLL personbll = new PersonBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonIndex()
        {
            return View();
        }

        /// <summary>
        /// �б�ҳ�� ѡ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonIndexSelect()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PersonForm()
        {
            return View();
        }
        #endregion

        #region ��ȡ����

        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="splitPage">��ҳ����</param>
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
        /// ��ȡ�б�
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
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = personbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// �ظ����� 
        /// </summary>
        /// <param name="EnCode">����ֶ�ֵ</param>
        /// <param name="keyValue">����</param>
        /// <returns>True False</returns>
        public ActionResult ExistEnCode(string EnCode, string keyValue)
        {
            bool IsOk = new DataCommBLL().CheckExistData("EnCode", EnCode, "Base_Person", "UserId", keyValue);
            return Content((!IsOk).ToString());
        }
        #endregion

        #region �ύ����

        /// <summary>
        /// �����˻�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult DisabledEnCode(string keyValue)
        {
            PersonEntity per = personbll.GetEntity(keyValue);
            if (per == null || per.UserId.ToStringV() == Str.Empty)
            {
                return Error("Ա����Ϣ������");
            }
            else
            {
                personbll.UpdateState(per, 0);
                return Success("Ա����Ϣ ���óɹ���");

            }
        }
        /// <summary>
        /// �����˻�
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult EnabledEnCode(string keyValue)
        {
            PersonEntity per = personbll.GetEntity(keyValue);
            if (per == null || per.UserId.ToStringV() == Str.Empty)
            {
                return Error("Ա����Ϣ������");
            }
            else
            {
                personbll.UpdateState(per, 1);
                return Success("Ա����Ϣ ���óɹ���");
            }
        }

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
            personbll.RemoveForm(keyValue);
            return Success("ɾ���ɹ���");
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
        public ActionResult SaveForm(string keyValue, PersonEntity entity)
        {
            personbll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
