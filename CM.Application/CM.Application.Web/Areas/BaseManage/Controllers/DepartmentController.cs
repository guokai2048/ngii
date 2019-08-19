using LeaRun.Application.Busines.BaseManage;
using LeaRun.Application.Cache;
using LeaRun.Application.Code;
using LeaRun.Application.Entity.BaseManage;
using LeaRun.Util;
using LeaRun.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CM.Application.Web.Areas.BaseManage.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017-2018 zhangjh.com
    /// 创建人：zhangjh
    /// 日 期：2018.11.02 14:27
    /// 描 述：分类管理
    /// </summary>
    public class DepartmentController : MvcControllerBase
    {
        private OrganizeCache organizeCache = new OrganizeCache();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        private DepartmentCache departmentCache = new DepartmentCache();

        #region 视图功能
        /// <summary>
        /// 分类管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 分类表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        #endregion

        #region 获取数据



        /// <summary>
        /// 获取车间名称 列表 
        /// </summary> 
        [HttpGet]
        public ActionResult GetFactoryList(string organizeId)
        {
            List<DepartmentEntity> datas = departmentCache.GetList(organizeId).ToList();

            List<DepartmentEntity> lists = datas.Where(t => t.Nature == "车间").ToList<DepartmentEntity>();

            return Content(lists.ToJson());
        }


        /// <summary>
        /// 分类列表 
        /// </summary>
        /// <param name="organizeId">公司Id</param>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetTreeJson(string organizeId, string keyword)
        {
            var data = departmentCache.GetList(organizeId).ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.FullName.Contains(keyword), "DepartmentId");
            }
            var treeList = new List<TreeEntity>();
            foreach (DepartmentEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = data.Count(t => t._parentId == item.DepartmentId) == 0 ? false : true;
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item._parentId;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回机构+分类树形Json</returns>
        public ActionResult GetOrganizeTreeJson(string keyword)
        {
            var organizedata = organizeCache.GetList();
            var departmentdata = departmentBLL.GetList();
            if (!string.IsNullOrEmpty(keyword))
            {
                departmentdata = departmentdata.ToList<DepartmentEntity>().TreeWhere(t => t.DepartmentId == keyword || t._parentId == keyword, "DepartmentId");
            }

            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t._parentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                    //if (hasChildren == false)
                    //{
                    //    continue;
                    //}
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item._parentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {


                #region 分类
                TreeEntity tree = new TreeEntity();
                bool hasChildren = departmentdata.Count(t => t._parentId == item.DepartmentId) == 0 ? false : true;
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                if (item._parentId == "0")
                {
                    tree.parentId = item.OrganizeId;
                }
                else
                {
                    tree.parentId = item._parentId;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Department";
                treeList.Add(tree);
                #endregion
            }
            //if (!string.IsNullOrEmpty(keyword))
            //{
            //    treeList = treeList.TreeWhere(t => t.text.Contains(keyword), "id", "parentId");
            //}
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 分类列表 
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形列表Json</returns>
        public ActionResult GetTreeListJson(string condition, string keyword)
        {
            var organizedata = organizeCache.GetList();
            var departmentdata = departmentBLL.GetList().ToList();
            if (!string.IsNullOrEmpty(condition) && !string.IsNullOrEmpty(keyword))
            {
                #region 多条件查询
                switch (condition)
                {
                    case "FullName":    //分类名称
                        departmentdata = departmentdata.TreeWhere(t => t.FullName.Contains(keyword), "DepartmentId");
                        break;
                    case "EnCode":      //分类编号
                        departmentdata = departmentdata.TreeWhere(t => t.EnCode.Contains(keyword), "DepartmentId");
                        break;
                    case "ShortName":   //分类简称
                        departmentdata = departmentdata.TreeWhere(t => t.ShortName.Contains(keyword), "DepartmentId");
                        break;
                    case "Manager":     //负责人
                        departmentdata = departmentdata.TreeWhere(t => t.Manager.Contains(keyword), "DepartmentId");
                        break;
                    case "OuterPhone":  //电话号
                        departmentdata = departmentdata.TreeWhere(t => t.OuterPhone.Contains(keyword), "DepartmentId");
                        break;
                    case "InnerPhone":  //分机号
                        departmentdata = departmentdata.TreeWhere(t => t.Manager.Contains(keyword), "DepartmentId");
                        break;
                    default:
                        break;
                }
                #endregion
            }
            //var treeList = new List<TreeGridEntity>();
            //foreach (OrganizeEntity item in organizedata)
            //{
            //    TreeGridEntity tree = new TreeGridEntity();
            //    bool hasChildren = organizedata.Count(t => t._parentId == item.OrganizeId) == 0 ? false : true;
            //    if (hasChildren == false)
            //    {
            //        hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
            //        if (hasChildren == false)
            //        {
            //            continue;
            //        }
            //    }
            //    tree.id = item.OrganizeId;
            //    tree.hasChildren = hasChildren;
            //    tree._parentId = item._parentId;
            //    tree.expanded = true;
            //    item.EnCode = ""; item.ShortName = ""; item.Nature = ""; item.Manager = ""; item.OuterPhone = ""; item.InnerPhone = ""; item.Description = "";
            //    string itemJson = item.ToJson();
            //    itemJson = itemJson.Insert(1, "\"DepartmentId\":\"" + item.OrganizeId + "\",");
            //    itemJson = itemJson.Insert(1, "\"Sort\":\"Organize\",");
            //    tree.entityJson = itemJson;
            //    treeList.Add(tree);
            //}
            //foreach (DepartmentEntity item in departmentdata)
            //{
            //    TreeGridEntity tree = new TreeGridEntity();
            //    bool hasChildren = organizedata.Count(t => t._parentId == item.DepartmentId) == 0 ? false : true;
            //    tree.id = item.DepartmentId;
            //    if (item._parentId == "0")
            //    {
            //        tree._parentId = item.OrganizeId;
            //    }
            //    else
            //    {
            //        tree._parentId = item._parentId;
            //    }
            //    tree.expanded = true;
            //    tree.hasChildren = hasChildren;
            //    string itemJson = item.ToJson();
            //    itemJson = itemJson.Insert(1, "\"Sort\":\"Department\",");
            //    tree.entityJson = itemJson;
            //    treeList.Add(tree);
            //}
            //return Content(treeList.TreeJson());

            //分类
            foreach (DepartmentEntity item in departmentdata)
            {
                if (item._parentId != null)
                {
                    item._parentId = item._parentId.Equals("0") ? item.OrganizeId : item._parentId;
                }
            };
            //组织
            foreach (OrganizeEntity item in organizedata)
            {
                DepartmentEntity departmentEntity = new DepartmentEntity();
                departmentEntity.DepartmentId = item.OrganizeId;
                departmentEntity.EnCode = item.EnCode;
                departmentEntity.ShortName = item.ShortName;
                departmentEntity.FullName = item.FullName;
                departmentEntity._parentId = item._parentId;
                if (departmentEntity._parentId != null)
                {
                    departmentEntity._parentId = departmentEntity._parentId.Equals("0") ? null : departmentEntity._parentId;
                }

                departmentdata.Add(departmentEntity);
            }

            var jsonData = new
            {
                rows = departmentdata
            };
            return ToJsonResult(jsonData);

        }
        /// <summary>
        /// 分类实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = departmentBLL.GetEntity(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 分类编号不能重复
        /// </summary>
        /// <param name="EnCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistEnCode(string EnCode, string keyValue)
        {
            bool IsOk = departmentBLL.ExistEnCode(EnCode, keyValue);
            return Content(IsOk.ToString());
        }
        /// <summary>
        /// 分类名称不能重复
        /// </summary>
        /// <param name="FullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistFullName(string FullName, string keyValue)
        {
            bool IsOk = departmentBLL.ExistFullName(FullName, keyValue);
            return Content(IsOk.ToString());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RemoveForm(string keyValue)
        {
            departmentBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存分类表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">分类实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, DepartmentEntity departmentEntity)
        {
            departmentBLL.SaveForm(keyValue, departmentEntity);
            return Success("操作成功。");
        }
        #endregion
    }
}
