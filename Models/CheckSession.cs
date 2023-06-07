using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackOffice.Models
{
    public class CheckSession : ActionFilterAttribute
    {
        public static bool sess=false;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (!sess)            
            {
                filterContext.Result = new RedirectResult(string.Format("/Login"));

            }
        }
    }
}
