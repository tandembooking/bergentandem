using Microsoft.AspNetCore.Mvc.Filters;
using TandemBooking.Services;

namespace TandemBooking.Attributes
{
	public class LocalizationAttribute : ActionFilterAttribute
	{
		private string _DefaultLanguage = "NO";

		private ContentService _content;

		public LocalizationAttribute(ContentService content)
			: base()
		{
			_content = content;
		}

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			string language = ((string)filterContext.RouteData.Values["lang"]) ?? _DefaultLanguage;
			_content.setLanguage(language);
		}
	}
}
