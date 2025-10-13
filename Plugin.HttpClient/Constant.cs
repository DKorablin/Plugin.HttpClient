using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Plugin.HttpClient
{
	internal static class Constant
	{
		public static class Reflection
		{//TODO: I need to separate namespace from member name
			public static class Wcf
			{
				public const String ServiceContract = "System.ServiceModel.ServiceContractAttribute";
				public const String WebInvoke = "System.ServiceModel.Web.WebInvokeAttribute";
			}
			public static class WebApi
			{
				public static readonly String[] RoutePrefixAttributes = new String[] { "System.Web.Http.RoutePrefixAttribute", "Microsoft.AspNetCore.Mvc.RouteAttribute", };
				public const String MethodRouteAttribute = "System.Web.Http.RouteAttribute";
				public const String ResponseType = "System.Web.Http.Description.ResponseTypeAttribute";

				public static readonly Dictionary<String, String> HttpMethods = new Dictionary<String, String>()
				{
					{"System.Web.Http.HttpDeleteAttribute","DELETE"},
					{"System.Web.Http.HttpGetAttribute","GET"},
					{"System.Web.Http.HttpHeadAttribute","HEAD"},
					{"System.Web.Http.HttpOptionsAttribute","OPTIONS"},
					{"System.Web.Http.HttpPostAttribute","POST"},
					{"System.Web.Http.HttpPutAttribute","PUT"},
					{"System.Web.Http.HttpTraceAttribute","TRACE"},
					{"System.Web.Http.HttpPatchAttribute","PATCH" },
					{"System.Web.Mvc.HttpDeleteAttribute","DELETE"},
					{"System.Web.Mvc.HttpGetAttribute","GET"},
					{"System.Web.Mvc.HttpHeadAttribute","HEAD"},
					{"System.Web.Mvc.HttpOptionsAttribute","OPTIONS"},
					{"System.Web.Mvc.HttpPostAttribute","POST"},
					{"System.Web.Mvc.HttpPutAttribute","PUT"},
					{"System.Web.Mvc.HttpTraceAttribute","TRACE"},
					{"System.Web.Mvc.HttpPatchAttribute","PATCH" },
					{"Microsoft.AspNetCore.Mvc.HttpDeleteAttribute","DELETE"},
					{"Microsoft.AspNetCore.Mvc.HttpGetAttribute","GET"},
					{"Microsoft.AspNetCore.Mvc.HttpHeadAttribute","HEAD"},
					{"Microsoft.AspNetCore.Mvc.HttpOptionsAttribute","OPTIONS"},
					{"Microsoft.AspNetCore.Mvc.HttpPostAttribute","POST"},
					{"Microsoft.AspNetCore.Mvc.HttpPutAttribute","PUT"},
					{"Microsoft.AspNetCore.Mvc.HttpTraceAttribute","TRACE"},
					{"Microsoft.AspNetCore.Mvc.HttpPatchAttribute","PATCH" }
				};
			}
			public static class WS
			{
				public const String WebService = "System.Web.Services.WebServiceAttribute";
				public const String WebMethod = "System.Web.Services.WebMethodAttribute";
			}
			public const String DataContract = "System.Runtime.Serialization.DataContractAttribute";
			public const String DataMember = "System.Runtime.Serialization.DataMemberAttribute";
		}

		public static class Http
		{
			public static readonly Dictionary<String, String> TransferEncoding = new Dictionary<String, String> { { "chunked", "chunked" }, { "compress", "compress" }, { "deflate", "deflate" }, { "gzip", "gzip" }, { "identity", "identity" }, };

			public static readonly Dictionary<String, String> Methods = new Dictionary<String, String> { { "GET", "GET" }, { "HEAD", "HEAD" }, { "POST", "POST" }, { "PUT", "PUT" }, { "DELETE", "DELETE" }, { "CONNECT", "CONNECT" }, { "OPTIONS", "OPTIONS" }, { "TRACE", "TRACE" }, { "PATCH", "PATCH" }, };

			public static readonly Dictionary<String, String> Expect = new Dictionary<String, String> { { "100-continue", "100-continue" } };

			public static readonly Dictionary<String, String> ContentTypes = new Dictionary<String, String>
			{
				{ "application/json", "application/json" },
				{ "application/x-www-form-urlencoded", "application/x-www-form-urlencoded" },
				{ "multipart/form-data", "multipart/form-data" },
				{ "text/html", "text/html" },
				{ "text/xml", "text/xml" },
			};

			public static readonly Dictionary<String, String> Connection = new Dictionary<String, String> { { "keep-alive", "keep-alive" }, { "close", "close" }, };

			public static readonly Dictionary<String, String> UserAgents = new Dictionary<String, String>() {
			//Android Mobile User Agents
			{"Samsung Galaxy S9","Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36"},
			{"Samsung Galaxy S8","Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/60.0.3112.107 Mobile Safari/537.36"},
			{"Samsung Galaxy S7","Mozilla/5.0 (Linux; Android 7.0; SM-G930VC Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/58.0.3029.83 Mobile Safari/537.36"},
			{"Samsung Galaxy S7 Edge","Mozilla/5.0 (Linux; Android 6.0.1; SM-G935S Build/MMB29K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/55.0.2883.91 Mobile Safari/537.36"},
			{"Samsung Galaxy S6","Mozilla/5.0 (Linux; Android 6.0.1; SM-G920V Build/MMB29K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.36"},
			{"Samsung Galaxy S6 Edge Plus","Mozilla/5.0 (Linux; Android 5.1.1; SM-G928X Build/LMY47X) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.83 Mobile Safari/537.36"},
			{"Nexus 6P","Mozilla/5.0 (Linux; Android 6.0.1; Nexus 6P Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.83 Mobile Safari/537.36"},
			{"Sony Xperia XZ","Mozilla/5.0 (Linux; Android 7.1.1; G8231 Build/41.2.A.0.219; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/59.0.3071.125 Mobile Safari/537.36"},
			{"Sony Xperia Z5","Mozilla/5.0 (Linux; Android 6.0.1; E6653 Build/32.2.A.0.253) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.36"},
			{"HTC One X10","Mozilla/5.0 (Linux; Android 6.0; HTC One X10 Build/MRA58K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/61.0.3163.98 Mobile Safari/537.36"},
			{"HTC One M9","Mozilla/5.0 (Linux; Android 6.0; HTC One M9 Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.98 Mobile Safari/537.3"},
			//iPhone User Agents
			{"Apple iPhone XR (Safari)","Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1"},
			{"Apple iPhone XS (Chrome)","Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/69.0.3497.105 Mobile/15E148 Safari/605.1"},
			{"Apple iPhone XS Max (Firefox)","Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) FxiOS/13.2b11866 Mobile/16A366 Safari/605.1.15"},
			{"Apple iPhone X","Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A372 Safari/604.1"},
			{"Apple iPhone 8","Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1"},
			{"Apple iPhone 8 Plus","Mozilla/5.0 (iPhone; CPU iPhone OS 11_0 like Mac OS X) AppleWebKit/604.1.38 (KHTML, like Gecko) Version/11.0 Mobile/15A5370a Safari/604.1"},
			{"Apple iPhone 7","Mozilla/5.0 (iPhone9,3; U; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1"},
			{"Apple iPhone 7 Plus","Mozilla/5.0 (iPhone9,4; U; CPU iPhone OS 10_0_1 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) Version/10.0 Mobile/14A403 Safari/602.1"},
			{"Apple iPhone 6","Mozilla/5.0 (Apple-iPhone7C2/1202.466; U; CPU like Mac OS X; en) AppleWebKit/420+ (KHTML, like Gecko) Version/3.0 Mobile/1A543 Safari/419.3"},
			//MS Windows Phone User Agents
			{"Microsoft Lumia 650","Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; RM-1152) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Mobile Safari/537.36 Edge/15.15254"},
			{"Microsoft Lumia 550","Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; RM-1127_16056) AppleWebKit/537.36(KHTML, like Gecko) Chrome/42.0.2311.135 Mobile Safari/537.36 Edge/12.10536"},
			{"Microsoft Lumia 950","Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Microsoft; Lumia 950) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.1058"},
			//Tablet User Agents
			{"Google Pixel C","Mozilla/5.0 (Linux; Android 7.0; Pixel C Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.98 Safari/537.36"},
			{"Sony Xperia Z4 Tablet","Mozilla/5.0 (Linux; Android 6.0.1; SGP771 Build/32.2.A.0.253; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.98 Safari/537.36"},
			{"Nvidia Shield Tablet K1","Mozilla/5.0 (Linux; Android 6.0.1; SHIELD Tablet K1 Build/MRA58K; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/55.0.2883.91 Safari/537.36"},
			{"Samsung Galaxy Tab S3","Mozilla/5.0 (Linux; Android 7.0; SM-T827R4 Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.116 Safari/537.36"},
			{"Samsung Galaxy Tab A","Mozilla/5.0 (Linux; Android 5.0.2; SAMSUNG SM-T550 Build/LRX22G) AppleWebKit/537.36 (KHTML, like Gecko) SamsungBrowser/3.3 Chrome/38.0.2125.102 Safari/537.36"},
			{"Amazon Kindle Fire HDX 7","Mozilla/5.0 (Linux; Android 4.4.3; KFTHWI Build/KTU84M) AppleWebKit/537.36 (KHTML, like Gecko) Silk/47.1.79 like Chrome/47.0.2526.80 Safari/537.36"},
			{"LG G Pad 7.0","Mozilla/5.0 (Linux; Android 5.0.2; LG-V410/V41020c Build/LRX22G) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/34.0.1847.118 Safari/537.36"},
			//Desktop User Agents
			{"Windows 10-based PC using Edge browser","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246"},
			{"Chrome OS-based laptop using Chrome browser (Chromebook)","Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36"},
			{"Mac OS X-based computer using a Safari browser","Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9"},
			{"Windows 7-based PC using a Chrome browser","Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36"},
			{"Linux-based PC using a Firefox browser","Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1"},
			//Set Top Boxes User Agents
			{"Chromecast","Mozilla/5.0 (CrKey armv7l 1.5.16041) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.0 Safari/537.36"},
			{"Roku Ultra","Roku4640X/DVP-7.70 (297.70E04154A)"},
			{"Minix NEO X5","Mozilla/5.0 (Linux; U; Android 4.2.2; he-il; NEO-X5-116A Build/JDQ39) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Safari/534.30"},
			{"Amazon 4K Fire TV","Mozilla/5.0 (Linux; Android 5.1; AFTS Build/LMY47O) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/41.99900.2250.0242 Safari/537.36"},
			{"Google Nexus Player","Dalvik/2.1.0 (Linux; U; Android 6.0.1; Nexus Player Build/MMB29T)"},
			{"Apple TV 5th Gen 4K","AppleTV6,2/11.1"},
			{"Apple TV 4th Gen","AppleTV5,3/9.1.1"},

			{"Nintendo Wii U","Mozilla/5.0 (Nintendo WiiU) AppleWebKit/536.30 (KHTML, like Gecko) NX/3.0.4.2.12 NintendoBrowser/4.3.1.11264.US"},
			{"Xbox One S","Mozilla/5.0 (Windows NT 10.0; Win64; x64; XBOX_ONE_ED) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/14.14393"},
			{"Xbox One","Mozilla/5.0 (Windows Phone 10.0; Android 4.2.1; Xbox; Xbox One) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2486.0 Mobile Safari/537.36 Edge/13.10586"},
			{"Playstation 4","Mozilla/5.0 (PlayStation 4 3.11) AppleWebKit/537.73 (KHTML, like Gecko)"},
			{"Playstation Vita","Mozilla/5.0 (PlayStation Vita 3.61) AppleWebKit/537.73 (KHTML, like Gecko) Silk/3.2"},
			{"Nintendo 3DS","Mozilla/5.0 (Nintendo 3DS; U; ; en) Version/1.7412.EU"},
			//Bots and Crawlers User Agents
			{"Google bot","Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)"},
			{"Bing bot","Mozilla/5.0 (compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm)"},
			{"Yahoo! bot","Mozilla/5.0 (compatible; Yahoo! Slurp; http://help.yahoo.com/help/us/ysearch/slurp)"},
			//E Readers User Agents
			{"Amazon Kindle 4","Mozilla/5.0 (X11; U; Linux armv7l like Android; en-us) AppleWebKit/531.2+ (KHTML, like Gecko) Version/5.0 Safari/533.2+ Kindle/3.0+"},
			{"Amazon Kindle 3","Mozilla/5.0 (Linux; U; en-US) AppleWebKit/528.5+ (KHTML, like Gecko, Safari/528.5+) Version/4.0 Kindle/3.0 (screen 600x800; rotate)"}
		};
		}

		public static class Project
		{
			public const String DefaultTemplateName = "";
			public const String TemplateDefaultValuePrefix = "System.";
			/// <summary>Template key name for discarded value at response</summary>
			public const String DiscardValueName = TemplateDefaultValuePrefix + "Discard";

			public static class Extensions
			{
				public const String Binary = "bhtst";
				public const String Xml = "xhtst";
				public const String Json = "jhtst";

				/// <summary>Filters for OpenFile dialog box</summary>
				[Flags]
				public enum FilterTypes
				{
					/// <summary>Include known projects extensions filter</summary>
					Projects,
					/// <summary>Include .dll filter for WebAPI assemblies (*.dll)</summary>
					Assemblies,
					/// <summary>Include filter for all files (*.*)</summary>
					AllFiles,
				}

				/// <summary>Check is this file is .NET assembly</summary>
				/// <param name="fileName">The file which extension to check</param>
				/// <returns>true when extension belong to assembly</returns>
				public static Boolean IsAssembly(String fileName)
					=> ".dll".Equals(Path.GetExtension(fileName).ToLowerInvariant());

				/// <summary>Check is this file is HTTP project</summary>
				/// <param name="fileName">The file name which extension to check</param>
				/// <returns>true if extension belongs to project</returns>
				public static Boolean IsProject(String fileName)
				{
					String ext = Path.GetExtension(fileName).ToLowerInvariant();
					switch(ext)
					{
					case "." + Binary:
					case "." + Xml:
					case "." + Json:
						return true;
					default:
						return false;
					}
				}

				public static String CreateFilter(FilterTypes extensionFilter = FilterTypes.Projects)
				{
					StringBuilder result = new StringBuilder();
					if((extensionFilter & FilterTypes.Projects) == FilterTypes.Projects)
					{
						String[] extensions = new String[] { Constant.Project.Extensions.Binary, Constant.Project.Extensions.Json, Constant.Project.Extensions.Xml, };
						String hint = String.Join(", *.", extensions);
						String filter = String.Join(";*.", extensions);
						result.Append($"Http test list (*.{hint})|*.{filter}");
					}

					if((extensionFilter & FilterTypes.Assemblies) == FilterTypes.Assemblies)
					{
						if(result.Length > 0)
							result.Append("|");
						result.Append("WebAPI Assembly (*.dll)|*.dll");
					}
					if((extensionFilter & FilterTypes.AllFiles) == FilterTypes.AllFiles)
					{
						if(result.Length > 0)
							result.Append("|");
						result.Append("All files (*.*)|*.*");
					}

					return result.ToString();
				}
			}
		}

		public const String PowerShellScriptTemplate = "Invoke-WebRequest -Uri \"{0}\" -Method {1} -Headers @{{{2}}} {3}";
		public const String CurlScriptPrefix = "curl";
	}
}