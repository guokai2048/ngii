using System.Web.Mvc;

namespace CM.Application.Web.Areas.CreditManage
{
    public class CreditManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CreditManage";
            }
        }
         
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
             this.AreaName + "_Default",
             this.AreaName + "/{controller}/{action}/{id}",
             new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
             new string[] { "CM.Application.Web.Areas." + this.AreaName + ".Controllers" }
           );
        }
    }
}