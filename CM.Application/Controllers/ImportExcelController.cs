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
    public class ImportExcelController : MvcControllerBase
    {
        /// <summary>
        /// 通用导入Excel，返回Excel数据
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string mbFileName)
        {
            ViewBag.mbFileName = mbFileName;
            return View();
        }        
        /// <summary>
        /// 导入excel
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportExcel()
        {
            System.Web.HttpPostedFileBase file = Request.Files["UploadFile"];
            if (file == null || file.ContentLength <= 0)
                return Error("文件不存在，请重试!!");
            string fileName = file.FileName.Replace(Path.GetExtension(file.FileName), "") + "-" + System.Guid.NewGuid().ToString();
            string filePath = HttpContext.Server.MapPath("/Uploads/临时文件/");
            if (!System.IO.Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }


            filePath = System.IO.Path.Combine(HttpContext.Server.MapPath("/Uploads/临时文件/"), fileName + Path.GetExtension(file.FileName));
            //保存文件
            file.SaveAs(filePath);
            try
            {
                //当前Excel内容
                DataTable tbExcel = ExcelHelper.ExcelImport(filePath, string.Empty);
                //标准模板
                DataTable mbExcel = ExcelHelper.ExcelImport(Server.MapPath("/Uploads/系统模版/" + Request.Form["mbFileName"] + ".xlsx"), "模板对应列");
                //上传信息和标准模板比较，如果一致，替换数据库列名，并返回数据
                Hashtable hs = new Hashtable();
                for (int i = 0; i < mbExcel.Columns.Count; i++)
                {
                    hs.Add(mbExcel.Columns[i].ColumnName, mbExcel.Rows[0][i]);
                }
                bool isOk = true;
                for (int i = 0; i < tbExcel.Columns.Count; i++)
                {
                    if (tbExcel.Columns[i].ColumnName != mbExcel.Columns[i].ColumnName)
                    {
                        isOk = false;
                        break;
                    }
                    else
                        tbExcel.Columns[i].ColumnName = hs[tbExcel.Columns[i].ColumnName].ToString();
                }
                if (isOk)
                {
                    DataTable newTb = tbExcel.Clone();
                    for (int i = 0; i < tbExcel.Rows.Count; i++)
                    {
                        if (!isEmptyRow(tbExcel.Rows[i], tbExcel.Columns.Count))
                            newTb.Rows.Add(tbExcel.Rows[i].ItemArray);
                    }
                    return ToJsonResult(newTb);
                }
                else
                    return Error("模板错误：请重新下载模板！");
            }
            catch (System.Exception ee)
            {
                return Error("系统性错误码：" + ee.Message);
            }
            finally
            {
                System.IO.File.Delete(filePath);
            }
        }
        /// <summary>
        /// 判断是否为空行 任何一列有数据，就不为空行，返回false
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="cellCount"></param>
        /// <returns>空行返回 true</returns>
        private bool isEmptyRow(DataRow dr, int cellCount)
        {
            bool IsNull = true;
            for (int i = 0; i < cellCount; i++)
            {
                if (!string.IsNullOrEmpty(dr[i].ToString().Trim()))
                {
                    IsNull = false;
                    break;
                }
            }
            return IsNull;
        }
        /// <summary>
        /// 获取原材料信息 合金、状态、配料类型
        /// </summary>
        /// <returns></returns>
        public ActionResult getYuanCaiLiaoInfo()
        {
            string sql1 = "select cinvCode from dbo.Sys_Materiel where cinvClassCode='YCL002'";
            string sql2 = @"select ItemValue from dbo.Base_DataItemDetail 
                            where ItemId in(select itemid from base_dataitem where itemcode = 'YCLState')";
            string sql3 = "select cinvCode from dbo.Sys_Materiel where cinvClassCode='PM100'";
            DataTable dt1 = SxSqlHelperBLL.FindTable(sql1);
            DataTable dt2 = SxSqlHelperBLL.FindTable(sql2);
            DataTable dt3 = SxSqlHelperBLL.FindTable(sql3);
            var json = new { hejin = dt1, state = dt2, plsort = dt3 };
            return ToJsonResult(json);
        }
        /// <summary>
        /// 获取所有供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult getSuppliser()
        {
            string sql = "select spuCode as supCode,spuName as supName from dbo.Sys_Supplier";
            DataTable dt = SxSqlHelperBLL.FindTable(sql);
            return ToJsonResult(dt);
        }
    }
}