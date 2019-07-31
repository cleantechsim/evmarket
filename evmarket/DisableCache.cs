
using Microsoft.AspNetCore.Mvc.Filters;

public class DisableCache : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store";
        filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
        filterContext.HttpContext.Response.Headers["Expires"] = "-1";

        base.OnResultExecuting(filterContext);
    }
}
