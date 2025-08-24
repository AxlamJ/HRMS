using HrManagement.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace HrManagement.Filters
{
    public class GlobalizationAttribute : ActionFilterAttribute
    {
        //public readonly IWebHostEnvironment _environment;
        //public GlobalizationAttribute(IWebHostEnvironment _env) {
        //    _env = _environment;
        //}
        /// <summary>
        /// For every web request this method executes before the action method.
        /// 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Session.GetString("UserName") != null)
                {
                    var baseDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
                    var configsFile = System.IO.Path.Combine(baseDir, "LayoutConfigs.json");
                    if (File.Exists(configsFile))
                    {
                        if (filterContext.Controller is Controller controller)
                        {
                            controller.ViewBag.LayoutConfigs = File.ReadAllText(configsFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException(ex);
            }
        }
    }
}
