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
    /// �� �� V1.0
    /// Copyright (c) 2017 ��Ϣ����
    /// �� ������������Ա
    /// �� �ڣ�2017-08-17 08:29
    /// �� ����������
    /// </summary>
    public class AttachmentController : MvcControllerBase
    {
        private AttachmentBLL attachmentbll = new AttachmentBLL();

        #region ��ͼ����
        /// <summary>
        /// �б�ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AttachmentIndex()
        {
            return View();
        }
        /// <summary>
        /// ��ҳ��
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AttachmentForm()
        {
            return View();
        }
        /// <summary>
        /// 158ԭʼͼֽ�����ϴ�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUpload158()
        {
            return View("PIC\\PicUpload158");
        }
        /// <summary>
        /// ͼֽ�����ϴ�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUpload()
        {
            return View("PIC\\PicUpload");

        }
        /// <summary>
        /// ɽ��ͼֽ�����ϴ�  
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUploadSH()
        {
            return View("PIC\\PicUploadSH");

        }
        /// <summary>
        /// ���� �����ϴ�
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PicUploadSoft()
        {
            return View("PIC\\PicUploadSoft");

        }


        /// <summary>
        /// �ϴ�ҳ��
        /// </summary>
        /// <returns></returns>
        public ActionResult Upload()
        {
            return View();
        }

        /// <summary>
        /// �ϴ�ҳ��
        /// </summary>
        /// <returns></returns>
        public ActionResult UploadCinvCode()
        {
            return View();
        }


        #endregion

        #region ��ȡ����

        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="pagination">��ҳ</param>
        /// <param name="splitPage">��ѯ����</param>
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
        /// ��ȡ�ļ���Ϣ
        /// </summary> 
        public ActionResult GetFileData(string cinvCode, string picStr, string picType)
        {
            //��ԭʼ�ļ�
            SplitPage spPic = new SplitPage();
            List<Conditions> listCon = new List<Conditions>();
            listCon.Add(new Conditions() { Key = "billType", Value = cinvCode });
            listCon.Add(new Conditions() { Key = "category", Value = picStr });
            if (!string.IsNullOrEmpty(picType))
            {
                listCon.Add(new Conditions() { Key = "picType", Value = picType });//1 ԭʼ��2 �ڲ���3 ����
            }
            spPic.Conditions = listCon.ToArray();
            var attach = attachmentbll.GetList(spPic);
            return ToJsonResult(attach);
        }
        /// <summary>
        /// ��ȡʵ�� 
        /// </summary>
        /// <param name="keyValue">����ֵ</param>
        /// <returns>���ض���Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = attachmentbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// ��folder��ȡ������Ϣ
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

            var data = attachmentbll.GetEntity(keyValue);
            if (data != null)
            {
                string filePath = Server.MapPath("~" + data.url);//·��
                if (System.IO.File.Exists(filePath))
                {
                    //ɾ�������ļ�
                    //Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filePath, 
                    //    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    //    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                    System.IO.File.Delete(filePath);
                }

                attachmentbll.RemoveForm(keyValue);
                return Success("ɾ���ɹ���");
            }
            else
            {
                return Error("Դ���ݻ�ȡ����,������");
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
        public ActionResult SaveForm(string keyValue, AttachmentEntity entity)
        {
            attachmentbll.SaveForm(keyValue, entity);
            return Success("�����ɹ���");
        }
        #endregion

        #region �ϴ�����
        /// <summary>
        /// �ϴ��ļ�
        /// </summary>
        /// <param name="folder">�ļ���</param>
        /// <param name="billType">��������</param>
        /// <param name="category">���</param>
        /// <returns></returns>
        public ActionResult UploadFile(string folder, string billType, string category)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            //string FileEextension = Path.GetExtension(files[0].FileName);
            //string UserId = OperatorProvider.Provider.Current().UserId;
            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //�����ļ��У������ļ�

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            //���浽���ݿ�
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

        // 'formData': { 'folder': folder, 'cinvCode': cinvCode, 'picStr': picStr, 'gxCode': gxCode }, //����

        /// <summary>
        /// �ϴ��ļ� �¼ӹ���(���������ͼֽ�ϴ�ʹ��)
        /// </summary> 
        public ActionResult UploadFileV(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType, string fileID, string accID)
        {

            if (!string.IsNullOrEmpty(fileID))
            {
                //��ȡʵ����
                AttachmentEntity tt = attachmentbll.GetEntity(fileID);
                if (tt != null)
                {
                    string filePath = Server.MapPath("~" + tt.url);//·��
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    //ɾ�����ݿ��¼
                    attachmentbll.RemoveForm(fileID);
                }
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }

            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //�����ļ��У������ļ�

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            //���浽���ݿ�
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
        /// �ϴ��ļ� �¼ӹ���[ing]
        /// </summary> 
        public ActionResult UploadFileVV(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType)
        {
            //return null;
            if (picType == 100)��//ɽ���ڲ�ͼֽ�����ϴ�
            {
                return UploadFile_SHPIC(folder, cinvCode, picStr, gxCode, gxName, 2);
            }
            if (picType == 4) //ɽ�����������ϴ�
            {
                return UploadFile_SHSoft(folder, cinvCode, picStr, gxCode, gxName, 4);
            }
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //�����ļ��У������ļ�

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            FileInfo fff = new FileInfo(fullFileName);
            decimal fffSize = (fff.Length / 1024.0 / 1024.0).ToDecimal(2);

            //���浽���ݿ�
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.Contains("."))
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }


            entity.folder = folder;
            entity.name = ffName;//ʵ��
            entity.realname = files[0].FileName;
            entity.billType = cinvCode;
            entity.category = picStr;
            entity.useInfo = gxName;
            entity.useInfoCode = gxCode;
            entity.picType = picType;
            entity.size = fffSize.ToStringV();
            entity.url = virtualPath;

            if (picType == 101)//ɽ��ԭʼͼֽ�����ϴ�
            {
                
            }
            if (picType == 1)//�ڶ��������ֶ��ϴ�
            {
                entity.picType = 1;
            }

            entity = attachmentbll.checkAndSave(entity);


            return ToJsonResult(entity);
        }

        /// <summary>
        /// �ϴ��ļ� ɽ���ڲ�ͼֽר��[ing]
        /// </summary> 
        private ActionResult UploadFile_SHPIC(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            //8����ͼֽ2=A1.rar
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

            folder = "1�ڲ�ͼֽ\\" + cinvCode + "\\ͼֽ";

            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //�����ļ��У������ļ�

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            FileInfo fff = new FileInfo(fullFileName);
            decimal fffSize = (fff.Length / 1024.0 / 1024.0).ToDecimal(2);

            //���浽���ݿ�
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.LastIndexOf(".") > 0)
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }


            entity.folder = folder;
            entity.name = ffName;//ʵ��
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
        /// �ϴ����� ɽ���ڲ�ͼֽר��[ing]
        /// </summary> 
        private ActionResult UploadFile_SHSoft(string folder, string cinvCode, string picStr, string gxCode, string gxName, int? picType)
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            //8����ͼֽ2=A1.rar
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

            folder = "1�ڲ�ͼֽ\\" + cinvCode + "\\����";

            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //�����ļ��У������ļ�

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            FileInfo fff = new FileInfo(fullFileName);
            decimal fffSize = (fff.Length / 1024.0 / 1024.0).ToDecimal(2);

            //���浽���ݿ�
            AttachmentEntity entity = new AttachmentEntity();

            string ffName = files[0].FileName;
            if (ffName.LastIndexOf(".") > 0)
            {
                ffName = ffName.Substring(0, ffName.LastIndexOf("."));
            }


            entity.folder = folder;
            entity.name = ffName;//ʵ��
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
        /// �ϴ��ļ� �¼ӹ���:�����ļ������Զ�ƥ��(NO)
        /// </summary> 
        public ActionResult UploadFileV2()
        {
            string folder = string.Empty;
            string cinvCode = string.Empty;
            string picStr = string.Empty;
            string gxCode = string.Empty;


            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //û���ļ��ϴ���ֱ�ӷ���
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string web_FileName = files[0].FileName;
            string fileRName = files[0].FileName.Substring(0, web_FileName.LastIndexOf("."));
            //ȡ��һ����-�������һ��"-"�м������


            string virtualPath = string.Format("/Resource/DocumentFile/{0}/{1}", folder, files[0].FileName);
            string fullFileName = Server.MapPath("~" + virtualPath);

            string path = Path.GetDirectoryName(fullFileName); //�����ļ��У������ļ�

            Directory.CreateDirectory(path);
            if (System.IO.File.Exists(fullFileName))
            {
                System.IO.File.Delete(fullFileName);
            }
            files[0].SaveAs(fullFileName);

            //���浽���ݿ�
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
        /// �����ļ�
        /// </summary>
        /// <returns></returns>
        public void DownloadFile(string id)
        {
            HttpContext curContext = System.Web.HttpContext.Current;

            //��ȡʵ����
            AttachmentEntity entity = attachmentbll.GetEntity(id.ToStringV());
            string filePath = string.Empty;
            if (entity == null)
            {

            }
            else
            {
                filePath = Server.MapPath("~" + entity.url);//·�� 
            }

            Response.ClearHeaders();
            Response.Clear();
            Response.Expires = 0;
            Response.Buffer = true;
            Response.AddHeader("Accept-Language", "zh-tw");


            string sysFile = Server.MapPath("~/Resource/DocumentFile/�ļ�������.txt");//��������ڵĻ�������Ĭ���ļ�
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

            //������ж�
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
        /// ɾ���ļ�
        /// </summary>
        /// <returns></returns>
        public ActionResult removeFile(string id)
        {
            //��ȡʵ����
            AttachmentEntity entity = attachmentbll.GetEntity(id);
            if (entity != null)
            {
                string filePath = Server.MapPath("~" + entity.url);//·��
                if (System.IO.File.Exists(filePath))
                {
                    //ɾ�������ļ�
                    //Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(filePath, 
                    //    Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                    //    Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);

                    System.IO.File.Delete(filePath);
                }
                //ɾ�����ݿ��¼
                attachmentbll.RemoveForm(id);
            }
            return Success("ɾ���ɹ���");

        }
    }
    #endregion
}

