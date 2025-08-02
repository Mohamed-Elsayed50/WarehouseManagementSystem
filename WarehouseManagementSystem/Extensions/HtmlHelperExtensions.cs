using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string IsOpen(this IHtmlHelper htmlHelper, params string[] controllerNames)
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"]?.ToString();
            return controllerNames.Contains(currentController) ? "open active" : "";
        }

        public static string IsActive(this IHtmlHelper htmlHelper, string controllerName)
        {
            var currentController = htmlHelper.ViewContext.RouteData.Values["controller"]?.ToString();
            return currentController == controllerName ? "active" : "";
        }
    }
}
