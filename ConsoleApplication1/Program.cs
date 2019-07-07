using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ConsoleApplication1
{
	internal class Program
	{
		public static async Task Main(string[] args)
		{
			var result = TranslateGoogle("Life is great and one is spoiled when it goes on and on and on", "en", "uk");

			Console.WriteLine(result);
		}

		public static string TranslateGoogle(string text, string fromCulture, string toCulture)
		{
			fromCulture = fromCulture.ToLower();
			toCulture = toCulture.ToLower();

			// normalize the culture in case something like en-us was passed 
			// retrieve only en since Google doesn't support sub-locales
			string[] tokens = fromCulture.Split('-');
			if (tokens.Length > 1)
				fromCulture = tokens[0];

			// normalize ToCulture
			tokens = toCulture.Split('-');
			if (tokens.Length > 1)
				toCulture = tokens[0];

			string url = string.Format(
				@"http://translate.google.com/translate_a/t?client=j&text={0}&hl=en&sl={1}&tl={2}",
				HttpUtility.UrlEncode(text), fromCulture, toCulture);

			// Retrieve Translation with HTTP GET call
			string html = null;
			try
			{
				WebClient web = new WebClient();

				// MUST add a known browser user agent or else response encoding doen't return UTF-8 (WTF Google?)
				//web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
				web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");

				// Make sure we have response encoding to UTF-8
				web.Encoding = Encoding.UTF8;
				html = web.DownloadString(url);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.GetBaseException().Message);
				return null;
			}

			return html;
			// Extract out trans":"...[Extracted]...","from the JSON string
			string result = Regex.Match(html, "trans\":(\".*?\"),\"", RegexOptions.IgnoreCase).Groups[1].Value;

			if (string.IsNullOrEmpty(result))
			{
				Console.WriteLine("Invalid search Result");
				return null;
			}

			//return WebUtils.DecodeJsString(result);

			// Result is a JavaScript string so we need to deserialize it properly
			JavaScriptSerializer ser = new JavaScriptSerializer();
			return ser.Deserialize(result, typeof(string)) as string;
		}
	}
	
	
}