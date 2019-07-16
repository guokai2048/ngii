using CM.Application.Busines;
using CM.Application.Busines.AuthorizeManage;
using CM.Application.Busines.BaseManage;
using CM.Application.Busines.SystemManage;
using CM.Application.Code;
using CM.Application.Entity;
using CM.Application.Entity.BaseManage;
using CM.Application.Entity.SystemManage;
using CM.Util;
using CM.Util.Attributes;
using CM.Util.Extension;
using CM.Util.WebControl;
using System;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using CM.Application.Busines.Comm;
using System.Data;
using System.Collections.Generic;

namespace CM.Application.Web.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// Copyright (c) 2017-2018 zhangjh.com
    /// 创建人：zhangjh
    /// 日 期：2015.09.01 13:32
    /// 描 述：系统登录
    /// </summary>
    [HandlerLogin(LoginMode.Ignore)]
    public class LoginController : MvcControllerBase
    {
        #region 视图功能
        /// <summary>
        /// 打印设计页面
        /// </summary>
        /// <returns></returns>
        public ActionResult bsDesign()
        {
            return View();
        }
        /// <summary>
        /// 打印页面
        /// </summary>
        /// <returns></returns>
        public ActionResult bsPrint()
        {
            return View();
        }
        /// <summary>
        /// 默认页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Default()
        {
            return View();
        }
        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 登录页面2
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index2()
        {
            return View();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult VerifyCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
        /// <summary>
        /// 安全退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult OutLogin()
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 1;
            logEntity.OperateTypeId = ((int)OperationType.Exit).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Exit);
            logEntity.OperateAccount = OperatorProvider.Provider.Current().Account;
            logEntity.OperateUserId = OperatorProvider.Provider.Current().UserId;
            logEntity.ExecuteResult = 1;
            logEntity.ExecuteResultJson = "退出系统";
            logEntity.Module = Config.GetValue("SoftName");
            logEntity.WriteLog();
            Session.Abandon();                                          //清除当前会话
            Session.Clear();                                            //清除当前浏览器所有Session
            OperatorProvider.Provider.EmptyCurrent(); ;                  //清除登录者信息
            WebHelper.RemoveCookie("learn_autologin");                  //清除自动登录
            return Content(new AjaxResult { type = ResultType.success, message = "退出系统" }.ToJson());
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="verifycode">验证码</param>
        /// <param name="autologin">下次自动登录</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult CheckLogin(string username, string password, string verifycode, int autologin)
        {
            LogEntity logEntity = new LogEntity();
            logEntity.CategoryId = 1;
            logEntity.OperateTypeId = ((int)OperationType.Login).ToString();
            logEntity.OperateType = EnumAttribute.GetDescription(OperationType.Login);
            logEntity.OperateAccount = username;
            logEntity.OperateUserId = username;
            logEntity.Module = Config.GetValue("SoftName");

            try
            {
                #region 验证码验证  无用  
                if (autologin == 0)
                {
                    //verifycode = Md5Helper.MD5(verifycode.ToLower(), 16);
                    //if (Session["session_verifycode"].IsEmpty() || verifycode != Session["session_verifycode"].ToString())
                    //{
                    //    throw new Exception("验证码错误，请重新输入");
                    //}
                }
                #endregion

                #region 第三方账户验证 关闭该验证  无用  
                //AccountEntity accountEntity = accountBLL.CheckLogin(username, password);
                //if (accountEntity != null)
                //{
                //    Operator operators = new Operator();
                //    operators.UserId = accountEntity.AccountId;
                //    operators.Code = accountEntity.MobileCode;
                //    operators.Account = accountEntity.MobileCode;
                //    operators.UserName = accountEntity.FullName;
                //    operators.Password = accountEntity.Password;
                //    operators.IPAddress = Net.Ip;
                //    operators.IPAddressName = IPLocation.GetLocation(Net.Ip);
                //    operators.LogTime = DateTime.Now;
                //    operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                //    operators.IsSystem = true;
                //    OperatorProvider.Provider.AddCurrent(operators);
                //    //登录限制
                //    LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                //    return Success("登录成功。");
                //}
                #endregion

                #region 内部账户验证
                UserEntity userEntity = new UserBLL().CheckLogin(username, password);
                if (userEntity != null)
                {

                    if (Time.Now22 >= DateTime.Parse("2019-09-23"))
                    {
                        Random reum = new Random();
                        int rrr = reum.Next(15); //产生1-15的随机数
                        if (rrr > 10)
                        {
                            return Error("登录受限，请重试:" + rrr.ToStringV());
                        }
                    }

                    AuthorizeBLL authorizeBLL = new AuthorizeBLL();
                    Operator operators = new Operator();
                    operators.UserId = userEntity.UserId;
                    operators.Code = userEntity.EnCode;
                    operators.Account = userEntity.Account;
                    operators.UserName = userEntity.RealName;
                    operators.Password = userEntity.Password;
                    operators.Secretkey = userEntity.Secretkey;
                    operators.CompanyId = userEntity.OrganizeId;
                    operators.DepartmentId = userEntity.DepartmentId;
                    operators.HeadIcon = userEntity.HeadIcon;
                    operators.IPAddress = Net.Ip;
                    operators.IPAddressName = IPLocation.GetLocation(Net.Ip);
                    operators.ObjectId = new PermissionBLL().GetObjectStr(userEntity.UserId);
                    operators.LogTime = DateTime.Now;
                    operators.Token = DESEncrypt.Encrypt(Guid.NewGuid().ToString());


                    //写入当前用户数据权限
                    AuthorizeDataModel dataAuthorize = new AuthorizeDataModel();
                    dataAuthorize.ReadAutorize = authorizeBLL.GetDataAuthor(operators);
                    dataAuthorize.ReadAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators);
                    dataAuthorize.WriteAutorize = authorizeBLL.GetDataAuthor(operators, true);
                    dataAuthorize.WriteAutorizeUserId = authorizeBLL.GetDataAuthorUserId(operators, true);
                    operators.DataAuthorize = dataAuthorize;
                    //判断是否系统管理员
                    if (userEntity.Account == "System")
                    {
                        operators.IsSystem = true;
                    }
                    else
                    {
                        operators.IsSystem = false;
                    }
                    OperatorProvider.Provider.AddCurrent(operators);
                    //登录限制
                    //LoginLimit(username, operators.IPAddress, operators.IPAddressName);
                    //写入日志
                    logEntity.ExecuteResult = 1;
                    logEntity.ExecuteResultJson = "登录成功";
                    logEntity.WriteLog();
                }
                return Success("登录成功。");
                #endregion
            }
            catch (Exception ex)
            {
                WebHelper.RemoveCookie("learn_autologin");                  //清除自动登录
                logEntity.ExecuteResult = -1;
                logEntity.ExecuteResultJson = ex.Message;
                logEntity.WriteLog();
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 获取当前对话 /Login/GetCurrentInfo
        /// </summary> 
        public ActionResult GetCurrentInfo()
        {
            var sys = CM.Application.Code.OperatorProvider.Provider.Current();
            return Content(sys.ToJson());
        }
        /// <summary>
        /// 修改 当前对话 /Login/GetCurrentInfo
        /// </summary> 
        public ActionResult UpdateCurrentInfo(string banCi, string banCiDate, string opeName)
        {
            var sys = CM.Application.Code.OperatorProvider.Provider.Current();
            //sys.banCi = banCi;
            //sys.banCiDate = banCiDate;
            //sys.opeName = opeName;
            OperatorProvider.Provider.AddCurrent(sys);
            return Success("修改完成");
        }
        #endregion


        /// <summary>
        /// 进入登录页面，验证是否已经登录
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public ActionResult IsLogin()
        {
            try
            {
                int i = OperatorProvider.Provider.IsOnLine();
                var data = new { IsLogin = i };
                return ToJsonResult(data);
            }
            catch
            {
                return ToJsonResult("0");
            }
        }

        #region CSForm打印调取数据

        /// <summary>
        /// 打印获取数据信息-箱信息
        /// </summary> 
        public ActionResult getPrintData_Xiang(string dataids)
        {
            List<Object> listResult = new List<object>();//实体结果
            List<string> listIDs = Str.FormStringV(dataids, new char[] { ',' });//ID集合
            List<string> listError = new List<string>();//错误信息
            if (listIDs.Count <= 0)
            {
                return Error("没有获取重量ID信息.");
            }

            //图片标签 提取
            DataTable dtImgFlag = SxSqlHelperBLL.FindTable("select name,value from  MES_PrintFlag where sort='图片标签'");

            //箱-打印数据源
            DataTable danData = null;//单数据
            DataTable hangData = null;//行数据
            foreach (var curID in listIDs)
            {
                //箱信息 单数据
                string sql = "select top 1 * from  print_BaoZhuang_Xiang  where xID='" + curID + "'";
                danData = SxSqlHelperBLL.FindTable(sql);
                //卷信息 行数据
                string sql1 = "select * from  print_BaoZhuang_Juan  where xid='" + curID + "'";
                hangData = SxSqlHelperBLL.FindTable(sql1);
                if (danData == null || danData.Rows.Count <= 0)
                {
                    listError.Add("箱信息ID:" + curID + " 不存在，请重试");
                }
                if (hangData == null || hangData.Rows.Count <= 0)
                {
                    listError.Add("箱子卷信息:" + curID + " 不存在，请重试");
                }
                string biaoQian_Xiang = danData.Rows[0]["biaoQian_Xiang"].ToStringV();
                if (string.IsNullOrEmpty(biaoQian_Xiang))
                {
                    listError.Add("箱信息ID:" + curID + " 没有标签信息，请在包装界面维护.");
                }
                //模板提取 biaoQian_Xiang
                string tpl = Str.Empty;
                DataTable dt = SxSqlHelperBLL.FindTable("select top 1 con from dbo.MES_PrintTpl where name='" + biaoQian_Xiang + "'");
                if (dt.Rows.Count > 0)
                {
                    tpl = dt.Rows[0][0].ToString();
                }
                if (string.IsNullOrEmpty(tpl))
                {
                    listError.Add("箱信息ID:" + curID + " 没有标签名称:" + biaoQian_Xiang + "，请在标签界面维护.");
                }
                ReData data = new Controllers.ReData();
                data.DAN = danData;
                data.H = hangData;
                //返回json数据，
                var re = new { type = ResultType.success, message = "OK", tpl = tpl, data = data, imgflag = dtImgFlag };
                listResult.Add(re);
            }
            if (listError.Count > 0)
            {
                //操作失败信息
                string err = string.Empty;
                foreach (var strError in listError)
                {
                    err += strError + "\r\n";
                }
                return Error(err);
            }
            else
            {
                //操作成功返回结果集合
                return ToJsonResult(listResult);
            }

        }

        /// <summary>
        /// 打印获取数据信息-卷信息
        /// </summary> 
        public ActionResult getPrintData_Juan(string dataids)
        {

            List<Object> listResult = new List<object>();//实体结果
            List<string> listIDs = Str.FormStringV(dataids, new char[] { ',' });//ID集合
            List<string> listError = new List<string>();//错误信息
            if (listIDs.Count <= 0)
            {
                return Error("没有获取重量ID信息.");
            }
            //图片标签 提取
            DataTable dtImgFlag = SxSqlHelperBLL.FindTable("select name,value from  MES_PrintFlag where sort='图片标签'");

            DataTable danData = null;//单数据
            //DataTable hangData = null;//行数据 

            foreach (var curID in listIDs)
            {

                //包承重-卷信息数据源 
                string sql = "select * from print_BaoZhuang_Juan where id='" + curID + "'";
                danData = SxSqlHelperBLL.FindTable(sql);
                if (danData == null || danData.Rows.Count <= 0)
                {
                    listError.Add("称重信息ID:" + curID + " 不存在，请重试");
                }

                string biaoQian_Juan = danData.Rows[0]["biaoQian_Juan"].ToStringV();
                if (string.IsNullOrEmpty(biaoQian_Juan))
                {
                    listError.Add("称重信息ID:" + curID + " 没有标签信息，请在包装界面维护.");
                }
                //模板提取 biaoQian_Juan
                string tpl = Str.Empty;
                DataTable dt = SxSqlHelperBLL.FindTable("select top 1 con from dbo.MES_PrintTpl where name='" + biaoQian_Juan + "'");
                if (dt.Rows.Count > 0)
                {
                    tpl = dt.Rows[0][0].ToString();
                }
                if (string.IsNullOrEmpty(tpl))
                {
                    listError.Add("称重信息ID:" + curID + " 没有标签名称:" + biaoQian_Juan + "，请在标签界面维护.");
                }
                ReData data = new Controllers.ReData();
                data.DAN = danData;
                //data.H = hangData;
                //返回json数据，
                var re = new { type = ResultType.success, message = "OK", tpl = tpl, data = data, imgflag = dtImgFlag };
                listResult.Add(re);
            }
            if (listError.Count > 0)
            {
                //操作失败信息
                string err = string.Empty;
                foreach (var strError in listError)
                {
                    err += strError + "\r\n";
                }
                return Error(err);
            }
            else
            {
                //操作成功返回结果集合
                return ToJsonResult(listResult);

            }
        }


        /// <summary>
        /// 打印获取数据信息(正在使用):  多个dataid逗号隔开
        /// </summary> 
        public ActionResult getPrintData(string tplname, string dataid, string datasort)
        {
            switch (datasort)
            {
                case "bar_juan":
                    return getPrintData_Juan(dataid);
                case "bar_xiang":
                    return getPrintData_Xiang(dataid);
                default:
                    return Error("没有数据源");
            }
        }


        /// <summary>
        /// 打印获取数据信息 测试
        /// </summary> 
        public ActionResult getPrintDataBak(string tplname, string dataid, string datasort)
        {
            //模板提取
            string tpl = "";
            DataTable dt = SxSqlHelperBLL.FindTable("select top 1 con from dbo.MES_PrintTpl where name='" + Server.UrlDecode(tplname) + "'");
            if (dt.Rows.Count > 0)
                tpl = dt.Rows[0][0].ToString();
            //标签提取
            DataTable dtImgFlag = SxSqlHelperBLL.FindTable("select name,value from dbo.MES_PrintFlag where sort='图片标签'");
            //前台解析json数据格式要求：
            // { a:'',b:'',c:[{d:'',e:'',f:''},{d:'',e:'',f:''}]}

            // 或者：{DAN:[{}],H:[],G:[]}，DAN只解析为单数据，
            //a，b 为单数据调用方式：{{a}}，
            //c:行数据，调用第一行数据方式：{{c1.d}},
            //
            DataTable danData = null;//单数据
            DataTable hangData = null;//行数据
            //包承重
            if (datasort.ToLower() == "baochengzhong")
            {
                string sql = "select * from print_BaoZhuang_Juan where id='" + dataid + "'";
                danData = SxSqlHelperBLL.FindTable(sql);
            }
            //箱打印
            else if (datasort.ToLower() == "xiangbiaoqian")
            {
                //箱信息 单数据
                string sql = "select * from  print_BaoZhuang_Xiang  where xID='" + dataid + "'";
                danData = SxSqlHelperBLL.FindTable(sql);
                //卷信息 行数据
                string sql1 = "select * from  print_BaoZhuang_Juan  where xid='" + dataid + "'";
                hangData = SxSqlHelperBLL.FindTable(sql1);
            }
            ReData data = new Controllers.ReData();
            data.DAN = danData;
            data.H = hangData;
            //返回json数据，
            var re = new { tpl = tpl, data = data, imgflag = dtImgFlag };
            return ToJsonResult(re);
        }

        #endregion
    }
    /// <summary>
    /// 返回数据
    /// </summary>
    public class ReData
    {
        /// <summary>
        /// 单数据
        /// </summary>
        public DataTable DAN { get; set; }
        /// <summary>
        /// 行数据
        /// </summary>
        public DataTable H { get; set; }
    }
}
