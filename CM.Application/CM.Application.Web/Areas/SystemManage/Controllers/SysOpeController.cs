using LeaRun.Application.Entity.SystemManage;
using LeaRun.Application.Busines.SystemManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using LeaRun.Application.Busines.Comm;
using System.Linq;
using LeaRun.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;

namespace LeaRun.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// �� �� V1.0
    /// Copyright (c) 2017 ��ԴMES
    /// �� ������������Ա
    /// �� �ڣ�2019-03-12 14:07
    /// �� ����ϵͳ���ñ�
    /// </summary>
    public class SysOpeController : MvcControllerBase
    {
        private SysOpeBLL sysopebll = new SysOpeBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SysOpeIndex()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult SysOpeForm()
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
            var data = sysopebll.GetPageList(pagination, splitPage);
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
            var data = sysopebll.GetPageList(pagination, splitPage); 
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
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
            var data = sysopebll.GetEntity(keyValue);
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
            bool IsOk = new DataCommBLL().CheckExistData("cinvClassName", cinvClassName, "Sys_SysOpe", "keyWord", keyValue);
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
            sysopebll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, SysOpeEntity entity)
        {
            sysopebll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
