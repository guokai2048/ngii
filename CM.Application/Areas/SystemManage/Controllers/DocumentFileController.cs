using CM.Application.Entity.SystemManage;
using CM.Application.Busines.SystemManage;
using CM.Util;
using CM.Util.WebControl;
using CM.Application.Busines.Comm;
using System.Linq;
using CM.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CM.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ������������Ա
    /// �� �ڣ�2017-07-31 15:15
    /// �� ����֤����
    /// </summary>
    public class DocumentFileController : MvcControllerBase
    {
        private DocumentFileBLL documentfilebll = new DocumentFileBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DocumentFileIndex()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DocumentFileForm()
        {
            return View();
        }
        /// <summary>
        /// ����ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DocumentFileDetail()
        {
            return View();
        }
        #endregion

        #region ��ȡ����

        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="pagination">��ҳ����</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>���ط�ҳ�б�Json</returns>
        public ActionResult GetDataGridJson(Pagination pagination, string queryJson)
        {
            pagination.sidx = pagination.sort ;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page<=0? 1:pagination.page);
            pagination.rows = (pagination.rows<=0? 0:pagination.rows);
            var watch = CommonHelper.TimerStart();
            var data = documentfilebll.GetPageList(pagination, queryJson);
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
        /// ��ȡ�б�
        /// </summary>
        /// <param name="pagination">��ҳ����</param>
        /// <param name="queryJson">��ѯ����</param>
        /// <param name="itemId">����ID</param>
        /// <returns>���ط�ҳ�б�Json</returns>
        public ActionResult GetDataGridJsonV1(Pagination pagination, string queryJson, string itemId)
        {
            pagination.sidx = pagination.sort ;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page<=0? 1:pagination.page);
            pagination.rows = (pagination.rows<=0? 0:pagination.rows);

            var queryParam = queryJson.ToJObject();
            SplitPage splitPage = new Util.WebControl.SplitPage();
            List<Conditions> lisWhere = new List<Conditions>();
            //��ѯ����
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
            var data = documentfilebll.GetPageList(pagination, splitPage);
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
            return ToJsonResult(jsonData);
        }

        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="splitPage">��ѯ����</param>
        /// <returns></returns>
        public ActionResult GetDataGridJsonV2(Pagination pagination, SplitPage splitPage)
        {
            pagination.sidx = pagination.sort ;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page<=0? 1:pagination.page);
            pagination.rows = (pagination.rows<=0? 0:pagination.rows);
            var data = documentfilebll.GetPageList(pagination, splitPage); 
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
            return ToJsonResult(jsonData);
        }
        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="queryJson">��ѯ����</param>
        /// <returns>�����б�Json</returns>
        [HttpGet]
        public ActionResult GetListJson(string queryJson)
        {
            var data = documentfilebll.GetList(queryJson);
            return ToJsonResult(data);
        }
        /// <summary>
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = documentfilebll.GetEntity(keyValue);
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
            bool IsOk = new DataCommBLL().CheckExistData("cinvClassName", cinvClassName, "MES_DocumentFile", "id", keyValue);
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
            documentfilebll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, DocumentFileEntity entity)
        {
            documentfilebll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
