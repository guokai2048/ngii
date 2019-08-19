using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CM.Application.Web.App_Start
{
    /// <summary>
    /// MES专用工具
    /// </summary>
    public class SHTool
    {
        /// <summary>
        /// 获取存货档案前三段数据
        /// </summary> 
        public static string SysCinvCode(string cinvCode)
        {
            //原始图纸
            //21e8-004-526-e9签名s  
            //21e8-004-8802签名D
            //21e8-005-616签名N
            //21e8-005-616

            //第13位，如果是数字，+1截取，如果非数字，直接截取
            //第13位，如果是数字，+1截取，如果非数字，直接截取
            if (cinvCode.Length <= 12)
            {
                return cinvCode;
            }
            else
            {
                string tmp1 = cinvCode.Substring(12, 1);

                try
                {
                    int.Parse(tmp1);
                    return cinvCode.Substring(0, 13);
                }
                catch
                {
                    return cinvCode.Substring(0, 12);
                }

            }
        }

    }
}