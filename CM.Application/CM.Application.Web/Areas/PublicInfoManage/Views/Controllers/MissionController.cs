using LeaRun.Application.Entity.PublicInfoManage;
using LeaRun.Application.Busines.PublicInfoManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using LeaRun.Application.Busines.Comm;
using System.Linq;
using LeaRun.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using LeaRun.Application.Code;

namespace CM.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ������������Ա
    /// �� �ڣ�2017-08-03 08:32
    /// �� �������������
    /// </summary>
    public class MissionController : MvcControllerBase
    {
        private MissionBLL missionbll = new MissionBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MissionIndex()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MissionForm()
        {
            return View();
        }
        /// <summary>
        /// �ҵĴ���
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MyMissionIndex()
        {
            return View();
        }
        #endregion

        #region ��ȡ����

        /// <summary>
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = missionbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }

        /// <summary>
        /// �ҵĴ����б�
        /// </summary>
        /// <param name="pagination">��ҳ����</param>
        /// <param name="splitPage">��ѯ����</param>
        /// <param name="usage">ʹ�÷�Χ(ǰ̨ʹ��)</param>
        /// <returns>���ط�ҳ�б�Json</returns>
        public ActionResult getMyMission(Pagination pagination, SplitPage splitPage, string usage)
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
                lisWhere.Add(new Conditions() { Key = "completeStatus", Value = "0" });

                splitPage.order = "asc";
                splitPage.sort = "CreateDate";
                splitPage.Conditions = lisWhere.ToArray();
                var data = missionbll.GetMyList(pagination, splitPage, usage);
                return Content(data.ToJson());
            }
            else
            {
                //�б�ʹ��
                lisWhere.Add(new Conditions() { Key = "receiver", Value = OperatorProvider.Provider.Current().UserId });
                lisWhere.Add(new Conditions() { Key = "isDelete", Value = "0" });
                splitPage.Conditions = lisWhere.ToArray();
                var data = missionbll.GetMyList(pagination, splitPage, usage);
                var jsonData = new
                {
                    rows = data,
                    total = pagination.records,
                };
                return ToJsonResult(jsonData);
            }

        }
        /// <summary>
        /// �ҵĴ�������
        /// </summary>
        /// <returns>���ط�ҳ�б�Json</returns>
        public ActionResult getMyMissionNum(SplitPage splitPage)
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
            lisWhere.Add(new Conditions() { Key = "completeStatus", Value = "0" });

            splitPage.Conditions = lisWhere.ToArray();
            var data = missionbll.GetMyList(splitPage);
            var jsonData = new
            {
                missionNum = data.Count()
            };
            return ToJsonResult(jsonData);

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
            missionbll.RemoveForm(keyValue);
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
        public ActionResult SaveForm(string keyValue, MissionEntity entity)
        {
            missionbll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion
    }
}
