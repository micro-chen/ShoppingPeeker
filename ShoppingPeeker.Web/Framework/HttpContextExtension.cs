using Microsoft.AspNetCore.Http;

namespace System.Web
{
	/// <summary>
	/// 扩展 从http 上下文获取cookie信息
	/// </summary>
	public static class HttpContextExtension
	{

		public static void SetCookie<T>(this HttpContext context, string domain, string name, T value,bool isHttpOnly=false) where T : class
		{
			SetCookie<T>(context, domain, name, value, 0,isHttpOnly);
		}
		public static void SetCookie<T>(this HttpContext context, string domain, string name, T value, int day, bool isHttpOnly = false) where T : class
		{
			var ckOption = new CookieOptions();// context.Request.Cookies[name];
			if (day > 0) ckOption.Expires = DateTime.Today.AddDays(day);
			ckOption.Domain = domain;
            ckOption.HttpOnly = isHttpOnly;

			string valueJson = HttpUtility.UrlEncode(value.ToJson());
			context.Response.Cookies.Append(name, valueJson, ckOption);
		}
		public static T GetCookie<T>(this HttpContext context, string name) where T : class
		{
			var cookie = context.Request.Cookies[name];
			if (!cookie.IsNull()) return HttpUtility.UrlDecode(cookie).FromJson<T>();
			return null;
		}
		public static void RemoveCookie(this HttpContext context, string domain, string name)
		{
			var cookie = context.Request.Cookies[name];
			if (!cookie.IsNullOrEmpty())
			{
				context.Response.Cookies.Delete(name);
			}
		}


		

	}



}
