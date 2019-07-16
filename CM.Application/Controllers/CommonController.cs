using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data;
using CM.Util.Offices;
using System.Collections;
using CM.Application.Busines.Comm;
using Newtonsoft.Json.Linq;

namespace CM.Application.Web.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CommonController : MvcControllerBase
    {
        /// <summary>
        /// 通用上传文件页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Upfile()
        {
            return View();
        }
        /// <summary>
        /// 通用上传文件
        /// </summary>
        /// <returns>返回上传的路径</returns>
        public ActionResult uploadFile()
        {
            System.Web.HttpPostedFileBase file = Request.Files["UploadFile"];
            string filepath = Request.Form["filepath"];

            if (file == null || file.ContentLength <= 0)
                return Error("文件不存在，请重试!!");
            string fileName = file.FileName.Replace(Path.GetExtension(file.FileName), "") + "-" + System.Guid.NewGuid().ToString();
            string filePath = HttpContext.Server.MapPath("/Uploads/" + filepath);
            if (!System.IO.Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            filePath = System.IO.Path.Combine(HttpContext.Server.MapPath("/Uploads/" + filepath), fileName + Path.GetExtension(file.FileName));
            string refilepath = System.IO.Path.Combine("/Uploads/" + filepath + "/" + fileName + Path.GetExtension(file.FileName));
            //保存文件
            file.SaveAs(filePath);
            return ToJsonResult(refilepath);
        }
    }
}