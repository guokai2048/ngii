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
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ����ϵͳ����Ա
    /// �� �ڣ�2019-06-21 22:32
    /// �� ������������
    /// </summary>
    public class CreditMoneyController : MvcControllerBase
    {
        private CreditMoneyBLL creditmoneybll = new CreditMoneyBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyIndex()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyForm()
        {
            return View();
        }

        /// <summary>
        /// ��ҳ�� ����
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyIndexAss()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ�� ����  ����
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyCount()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ�� ����  ����
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult CreditMoneyLook()
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
            SxSqlHelperBLL.getPageDataEasyUi(sqlw, "CR_CreditMoney", splitPage, ref total, ref dt);
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
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = creditmoneybll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// ��ȡʵ��  ���� 
        /// </summary>
        [HttpGet]
        public ActionResult GetFormJsonFile(string keyValue)
        {
            AttachmentBLL att = new AttachmentBLL(); //����
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
        /// �ظ����� 
        /// </summary>
        /// <param name="cinvClassName">����ֶ�ֵ</param>
        /// <param name="keyValue">����</param>
        /// <returns>True False</returns>
        public ActionResult ExistItemCode(string cinvClassName, string keyValue)
        {
            bool IsOk = new DataCommBLL().CheckExistData("cinvClassName", cinvClassName, "CR_CreditMoney", "id", keyValue);
            return Content((!IsOk).ToString());
        }
        #endregion

        #region �ύ����

        /// <summary>
        /// ģ�� �����Ƽ���
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
                return Error("��������Ϣ[" + id.ToStringV() + "]�����ܲ���");
            }
            System.Threading.Thread.Sleep(1 * 1000);

            bool isOK = creditmoneybll.PingJia(list, ref memo);
            if (isOK)
            {
                return Success("������ɡ�");
            }
            else

            {
                return Error(memo);
            }
        }

        /// <summary>
        /// ����¼��
        /// </summary> 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult CreditPingSave(string id, int? isOK, string isOKStr)
        {
            CreditMoneyEntity cur = creditmoneybll.GetEntity(id);
            if (cur == null)
            {
                return Error("��������Ϣ[" + id.ToStringV() + "] ���ܲ���");
            }
            cur.isOK = isOK.ToInt();
            cur.isOKStr = isOKStr.ToStringV();
            cur.pingDate = Time.Now22;
            cur.pingUserID = OperatorProvider.Provider.Current().UserId;
            cur.pingUserName = OperatorProvider.Provider.Current().UserName;


            SxSqlHelperBLL.GetIRepository().Update<CreditMoneyEntity>(cur);

            return Success("������ɡ�");


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
            creditmoneybll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, CreditMoneyEntity entity)
        {
            creditmoneybll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
