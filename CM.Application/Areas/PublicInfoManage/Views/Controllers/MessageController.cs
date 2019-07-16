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
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ������������Ա
    /// �� �ڣ�2017-08-07 08:26
    /// �� ������Ϣ��
    /// </summary>
    public class MessageController : MvcControllerBase
    {
        private MessageBLL messagebll = new MessageBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MessageIndex()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MessageForm()
        {
            return View();
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MessageDetail()
        {
            return View();
        }
        /// <summary>
        /// �ҵ���Ϣ�б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyMessageIndex()
        {
            return View();
        }
        #endregion

        #region ��ȡ����

        /// <summary>
        /// �ҵ���Ϣ�б�
        /// </summary>
        /// <param name="pagination">��ҳ����</param>
        /// <param name="splitPage">��ѯ����</param>
        /// <param name="usage">ʹ�÷�Χ(ǰ̨ʹ��)</param>
        /// <param name="curStatus">״̬</param>
        /// <returns>���ط�ҳ�б�Json</returns>
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
            //����ʹ��
            if (!string.IsNullOrEmpty(usage) && usage.Equals("top"))
            {
                lisWhere.Add(new Conditions() { Key = "receiver", Value = OperatorProvider.Provider.Current().UserId });
                lisWhere.Add(new Conditions() { Key = "isDelete", Value = "0" });
                lisWhere.Add(new Conditions() { Key = "curStatus", Value = "0" });//δ��

                splitPage.order = "desc";
                splitPage.sort = "CreateDate";
                splitPage.Conditions = lisWhere.ToArray();
                var data = messagebll.GetMyList(pagination, splitPage, usage);
                return Content(data.ToJson());
            }
            else
            {
                //�б�ʹ��
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
        /// �ҵ���Ϣ����
        /// </summary>
        /// <returns>���ط�ҳ�б�Json</returns>
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
            lisWhere.Add(new Conditions() { Key = "curStatus", Value = "0" });//δ��

            splitPage.Conditions = lisWhere.ToArray();
            var data = messagebll.GetMyList(splitPage);
            var jsonData = new
            {
                messageNum = data.Count()
            };
            return ToJsonResult(jsonData);

        }
        /// <summary>
        /// ��Ϣʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
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

        #region �ύ����

        /// <summary>
        /// �������� ����
        /// </summary>
        /// <param name="entitys">����</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RecycleForm(string entitys)
        {
            List<MessageEntity> mainEntitys = entitys.ToList<MessageEntity>();
            string mess = "������:δ֪��ʾ";
            bool isOK = messagebll.RecycleForm(mainEntitys, ref mess);

            if (isOK)
            {
                return Success("ɾ���ɹ���");
            }
            else
            {
                return Error(mess);
            }
        }

        /// <summary>
        /// �Ѷ����� ����
        /// </summary>
        /// <param name="entitys">����</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult ReadForm(string entitys)
        {
            List<MessageEntity> mainEntitys = entitys.ToList<MessageEntity>();
            string mess = "������:δ֪��ʾ";
            bool isOK = messagebll.ReadForm(mainEntitys, ref mess);

            if (isOK)
            {
                return Success("ɾ���ɹ���");
            }
            else
            {
                return Error(mess);
            }


        }


        /// <summary>
        /// ɾ������ ����
        /// </summary>
        /// <param name="entitys">����</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string entitys)
        {

            List<MessageEntity> mainEntitys = entitys.ToList<MessageEntity>();
            string mess = "������:δ֪��ʾ";
            bool isOK = messagebll.RemoveFormPL(mainEntitys, ref mess);

            if (isOK)
            {
                return Success("ɾ���ɹ���");
            }
            else
            {
                return Error(mess);
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
        public ActionResult SaveForm(string keyValue, MessageEntity entity)
        {
            messagebll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
