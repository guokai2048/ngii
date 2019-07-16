using CM.Application.Entity.PublicInfoManage;
using CM.Application.Busines.PublicInfoManage;
using CM.Util;
using CM.Util.WebControl;
using CM.Application.Busines.Comm;
using System.Linq;
using CM.Util.Extension;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using System.IO; 
namespace CM.Application.Web.Areas.PublicInfoManage.Controllers
{
    /// <summary>
    /// 版 本 V1.0
    /// Copyright (c) 2017 信息管理
    /// 创 建：超级管理员
    /// 日 期：2017-08-17 08:29
    /// 描 述：附件表
    /// </summary>
    public class AttachmentController : MvcControllerBase
    {
        private AttachmentBLL attachmentbll = new AttachmentBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AttachmentIndex()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AttachmentForm()
        {
            return View();
        }
        /// <summary>
        /// 158原始图纸批量上传
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUpload158()
        {
            return View("PIC\\PicUpload158");
        }
        /// <summary>
        /// 图纸批量上传
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUpload()
        {
            return View("PIC\\PicUpload");

        }
        /// <summary>
        /// 山航图纸批量上传  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUploadSH()
        {
            return View("PIC\\PicUploadSH");

        }
        /// <summary>
        /// 程序 批量上传
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUploadSoft()
        {
            return View("PIC\\PicUploadSoft");

        }


        /// <summary>
        /// 上传页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// 上传页面
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadCinvCode()
        {
            return View();
        }


        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="splitPage">查询对象</param>
        /// <returns></returns>
        public ActionResult GetDataGridJsonV2(Pagination pagination, SplitPage splitPage)
        {
            pagination.sidx = pagination.sort;
            pagination.sord = pagination.order;
            pagination.page = (pagination.page <= 0 ? 1 : pagination.page);
            pagination.rows = (pagination.rows <= 0 ? 0 : pagination.rows);
            var data = attachmentbll.GetPageList(pagination, splitPage);
            var jsonData = new
            {
                rows = data,
                total = pagination.records,
            };
            return ToJsonResult(jsonData);
        }
        /// <summary>
        /// 获取文件信息
        /// </summary> 
        public ActionResult GetFileData(string cinvCode, string picStr, string picType)
        {
            //含原始文件
            SplitPage spPic = new SplitPage();
            List<Conditions> listCon = new List<Conditions>();
            listCon.Add(new Conditions() { Key = "billType", Value = cinvCode });
            listCon.Add(new Conditions() { Key = "category", Value = picStr });
            if (!string.IsNullOrEmpty(picType))
            {
                listCon.Add(new Conditions() { Key = "picType", Value = picType });//1 原始，2 内部，3 工序
            }
            spPic.Conditions = listCon.ToArray();
            var attach = attachmentbll.GetList(spPic);
            return ToJsonResult(attach);
        }
        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = attachmentbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 由folder获取附件信息
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public ActionResult GetListByFolder(string folder)
        {
            string sql= "select * from Sys_Attachment where folder ='"+folder+"'";
            System.Data.DataTable dt = SxSqlHelperBLL.FindTable(sql);
            return ToJsonResult(dt);
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

            var data = attachmentbll.GetEntity(keyValue);
            if (data != null)
            {
                string filePath = Server.MapPath("~" + data.url);//路径
                if (System.IO.File.Exists(filePath))
                {
                    //删除物理文件
                    //Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filePath, 
                    //    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    //    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                    System.IO.File.Delete(filePath);
                }

                attachmentbll.RemoveForm(keyValue);
                return Success("删除成功。");
            }
            else
            {
                return Error("源数据获取有误,请重试");
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
        public ActionResult SaveForm(string keyValue, AttachmentEntity entity)
        {
            attachmentbll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion

        #region 上传数据
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="folder">文件夹</param>
        /// <param name="billType">单据类型</param>
        /// <param name="category">类别</param>
        /// <returns></returns>
        public ActionResult UploadFile(string folder, string billType, string category)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            //string FileEextension = Path.GetExtension(files[0].FileName);
            //string UserId = OperatorProvider.Provider.Current().UserId;
            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //创建文件夹，保存文件

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            //保存到数据库
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.Contains("."))
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }
            entity.folder = folder;
            entity.name = ffName;
            entity.realname = files[0].FileName;
            entity.billType = billType;
            entity.category = category;
            entity.url = virtualPath;
            entity = attachmentbll.checkAndSave(entity);

            return ToJsonResult(entity);
        }

        // 'formData': { 'folder': folder, 'cinvCode': cinvCode, 'picStr': picStr, 'gxCode': gxCode }, //传参

        /// <summary>
        /// 上传文件 新加功能(工序程序与图纸上传使用)
        /// </summary> 
        public ActionResult UploadFileV(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType, string fileID, string accID)
        {

            if (!string.IsNullOrEmpty(fileID))
            {
                //获取实体类
                AttachmentEntity tt = attachmentbll.GetEntity(fileID);
                if (tt != null)
                {
                    string filePath = Server.MapPath("~" + tt.url);//路径
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    //删除数据库记录
                    attachmentbll.RemoveForm(fileID);
                }
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }

            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //创建文件夹，保存文件

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            //保存到数据库
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.Contains("."))
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }
            entity.folder = folder;
            entity.name = ffName;
            entity.realname = files[0].FileName;
            entity.billType = cinvCode;
            entity.category = picStr;
            entity.useInfo = gxName;
            entity.useInfoCode = gxCode;
            entity.picType = picType;
            entity.url = virtualPath;
            entity = attachmentbll.checkAndSave(entity);


            if (!string.IsNullOrEmpty(accID))
            {
                
            }

            return ToJsonResult(entity);
        }

        /// <summary>
        /// 上传文件 新加功能[ing]
        /// </summary> 
        public ActionResult UploadFileVV(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType)
        {
            //return null;
            if (picType == 100)　//山航内部图纸批量上传
            {
                return UploadFile_SHPIC(folder, cinvCode, picStr, gxCode, gxName, 2);
            }
            if (picType == 4) //山航程序批量上传
            {
                return UploadFile_SHSoft(folder, cinvCode, picStr, gxCode, gxName, 4);
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //创建文件夹，保存文件

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            FileInfo fff = new FileInfo(fullFileName);
            decimal fffSize = (fff.Length / 1024.0 / 1024.0).ToDecimal(2);

            //保存到数据库
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.Contains("."))
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }


            entity.folder = folder;
            entity.name = ffName;//实名
            entity.realname = files[0].FileName;
            entity.billType = cinvCode;
            entity.category = picStr;
            entity.useInfo = gxName;
            entity.useInfoCode = gxCode;
            entity.picType = picType;
            entity.size = fffSize.ToStringV();
            entity.url = virtualPath;

            if (picType == 101)//山航原始图纸批量上传
            {
                
            }
            if (picType == 1)//在订单界面手动上传
            {
                entity.picType = 1;
            }

            entity = attachmentbll.checkAndSave(entity);


            return ToJsonResult(entity);
        }

        /// <summary>
        /// 上传文件 山航内部图纸专用[ing]
        /// </summary> 
        private ActionResult UploadFile_SHPIC(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            //8测试图纸2=A1.rar
            string sysFileName = files[0].FileName;

            if (sysFileName.LastIndexOf(".") > 0)
            {
                sysFileName = sysFileName.Substring(0, sysFileName.LastIndexOf("."));
            }
            if (!sysFileName.Contains("="))
            {
                return HttpNotFound();
            }
            List<string> li = Str.FormStringV(sysFileName, new char[] { '=' });
            cinvCode = li[0];
            picStr = li[1];

            folder = "1内部图纸\\" + cinvCode + "\\图纸";

            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //创建文件夹，保存文件

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            FileInfo fff = new FileInfo(fullFileName);
            decimal fffSize = (fff.Length / 1024.0 / 1024.0).ToDecimal(2);

            //保存到数据库
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.LastIndexOf(".") > 0)
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }


            entity.folder = folder;
            entity.name = ffName;//实名
            entity.realname = files[0].FileName;
            entity.billType = cinvCode;
            entity.category = picStr;
            entity.useInfo = gxName;
            entity.useInfoCode = gxCode;
            entity.picType = picType;
            entity.size = fffSize.ToStringV();
            entity.url = virtualPath;


            entity = attachmentbll.checkAndSave(entity);


            return ToJsonResult(entity);
        }

        /// <summary>
        /// 上传程序 山航内部图纸专用[ing]
        /// </summary> 
        private ActionResult UploadFile_SHSoft(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            //8测试图纸2=A1.rar
            string sysFileName = files[0].FileName;

            if (sysFileName.LastIndexOf(".") > 0)
            {
                sysFileName = sysFileName.Substring(0, sysFileName.LastIndexOf("."));
            }
            if (!sysFileName.Contains("="))
            {
                return HttpNotFound();
            }
            List<string> li = Str.FormStringV(sysFileName, new char[] { '=' });
            cinvCode = li[0];
            picStr = li[1];
            gxCode = li[2];

            folder = "1内部图纸\\" + cinvCode + "\\程序";

            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //创建文件夹，保存文件

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            FileInfo fff = new FileInfo(fullFileName);
            decimal fffSize = (fff.Length / 1024.0 / 1024.0).ToDecimal(2);

            //保存到数据库
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.LastIndexOf(".") > 0)
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }


            entity.folder = folder;
            entity.name = ffName;//实名
            entity.realname = files[0].FileName;
            entity.billType = cinvCode;
            entity.category = picStr;
            entity.useInfo = gxName;
            entity.useInfoCode = gxCode;
            entity.picType = picType;
            entity.size = fffSize.ToStringV();
            entity.url = virtualPath;


            entity = attachmentbll.checkAndSave(entity);


            return ToJsonResult(entity);
        }




        /// <summary>
        /// 上传文件 新加功能:根据文件名称自动匹配(NO)
        /// </summary> 
        public ActionResult UploadFileV2()
        {
            string folder = string.Empty;
            string cinvCode = string.Empty;
            string picStr = string.Empty;
            string gxCode = string.Empty;


            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string web_FileName = files[0].FileName;
            string fileRName = files[0].FileName.Substring(0, web_FileName.LastIndexOf("."));
            //取第一个“-”和最后一个"-"中间的内容


            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //创建文件夹，保存文件

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            //保存到数据库
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.Contains("."))
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }
            entity.folder = folder;
            entity.name = ffName;
            entity.realname = files[0].FileName;
            entity.billType = cinvCode;
            entity.category = picStr;
            entity.useInfo = gxCode;
            entity.url = virtualPath;
            entity = attachmentbll.checkAndSave(entity);

            return ToJsonResult(entity);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        public void DownloadFile(string id)
        {
            HttpContext curContext = System.Web.HttpContext.Current;

            //获取实体类
            AttachmentEntity entity = attachmentbll.GetEntity(id.ToStringV());
            string filePath = string.Empty;
            if (entity == null)
            {

            }
            else
            {
                filePath = Server.MapPath("~" + entity.url);//路径 
            }

            Response.ClearHeaders();
            Response.Clear();
            Response.Expires = 0;
            Response.Buffer = true;
            Response.AddHeader("Accept-Language", "zh-tw");


            string sysFile = Server.MapPath("~/Resource/DocumentFile/文件不存在.txt");//如果不存在的话，加载默认文件
            string name = System.IO.Path.GetFileName(sysFile);
            System.IO.FileStream files = new FileStream(sysFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (System.IO.File.Exists(filePath))
            {
                files = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                name = System.IO.Path.GetFileName(filePath);
            }

            byte[] byteFile = null;
            if (files.Length == 0)
            {
                byteFile = new byte[1];
            }
            else
            {
                byteFile = new byte[files.Length];
            }
            files.Read(byteFile, 0, (int)byteFile.Length);
            files.Close();

            //浏览器判断
            string curBrowser = curContext.Request.Browser.Id;//ie10plus  //firefox3plus
            if (curBrowser.ToLower().Contains("firefox"))
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
            }
            else
            {
                Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(name, System.Text.Encoding.UTF8));
            }

            Response.ContentType = "application/octet-stream;charset=gbk";
            Response.BinaryWrite(byteFile);
            Response.End();

        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <returns></returns>
        public ActionResult removeFile(string id)
        {
            //获取实体类
            AttachmentEntity entity = attachmentbll.GetEntity(id);
            if (entity != null)
            {
                string filePath = Server.MapPath("~" + entity.url);//路径
                if (System.IO.File.Exists(filePath))
                {
                    //删除物理文件
                    //Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filePath, 
                    //    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    //    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                    System.IO.File.Delete(filePath);
                }
                //删除数据库记录
                attachmentbll.RemoveForm(id);
            }
            return Success("删除成功。");

        }
    }
    #endregion
}

