using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class RequestBuilder
	{
		private static Dictionary<PropertyInfo, PropertyInfo> _propertyCacheMapping;
		private static Dictionary<String, PropertyInfo> _httpItemPropertyCache;

		public HttpItem Item { get; }
		private TemplateEngine _templates;

		internal static Dictionary<String, PropertyInfo> HttpItemPropertyCache
		{
			get => RequestBuilder._httpItemPropertyCache
				?? (RequestBuilder._httpItemPropertyCache = typeof(HttpItem).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite).ToDictionary(k => k.Name, v => v));
		}

		private static Dictionary<PropertyInfo, PropertyInfo> HttpItemToWebRequestMapping
		{
			get => RequestBuilder._propertyCacheMapping
				?? (RequestBuilder._propertyCacheMapping = RequestBuilder.GetEqualProperties<HttpItem, HttpWebRequest>());
		}

		public RequestBuilder(HttpProjectItem item)
			: this(item, item.Items.Project.Templates.GetTemplateValuesWithSource().ToArray())
		{
		}

		public RequestBuilder(HttpItem item, TemplateItem[] templates)
		{
			this.Item = item;
			this._templates = new RequestTemplateBuilder(templates);
		}

		public String CreatePowerShellScript()
		{//TODO: Make it more readable
			var request = this.CreateRequest(null, out String data);
			List<String> headers = new List<String>();
			foreach(var key in request.Headers.AllKeys)
			{
				var value = request.Headers[key];
				headers.Add($"\"{FormatPSString(key)}\"=\"{FormatPSString(value)}\"");
			}

			String body = String.Empty;
			if(data != null)
				body = "-Body \"" + FormatPSString(data) + "\"";

			return String.Format(Constant.PowerShellScriptTemplate,
				request.RequestUri.ToString(),
				request.Method,
				String.Join("; ", headers.ToArray()),
				body);

			String FormatPSString(String value)
				=> value.Replace("\r", "`r")
					.Replace("\n", "`n")
					.Replace("\t", "`t")
					.Replace("\"", "`\"")
					.Replace("\'", "`'");
		}

		public String CreateCurlScript()
		{
			var request = this.CreateRequest(null, out String data);
			List<String > headers = new List<String>();
			foreach(var key in request.Headers.AllKeys)
			{
				var value = request.Headers[key];
				if(key.ToLowerInvariant().Equals("cookie"))
					headers.Add($"--cookie \"{FormatCurlString(value)}\"");
				else
					headers.Add($"--header \"{FormatCurlString(key)}: {FormatCurlString(value)}\"");
			}

			String body = String.Empty;
			if(data != null)
				body = "--data ^\"" + FormatCurlString(data) + "\"";
			else if(request.ContentLength == 0)
				body = "--data {}";//Adding empty body if ContentLength != -1

			return "curl -v "
				+ "--request " + request.Method
				+ " "
				+ String.Join(" ", headers.ToArray())
				+ " "
				+ body
				+ " "
				+ request.RequestUri.ToString();

			String FormatCurlString(String value)
				=> value.Replace(Environment.NewLine, "^" + Environment.NewLine)
					.Replace("\t", "\\t")
					.Replace("\"", "\\\"");
		}

		public HttpItem CloneRequest()
		{
			HttpItem result = new HttpItem();
			foreach(var property in HttpItemPropertyCache)
			{
				Object value = property.Value.GetValue(this.Item, null);
				value = this._templates.ApplyTemplateV2R(property.Value.Name, property.Value.PropertyType, value, true);
				if(value != null)
					property.Value.SetValue(result, value, null);
			}
			return result;
		}

		public HttpWebRequest CreateRequest(CookieContainer cookies)
		{
			HttpWebRequest request = this.CreateRequest(cookies, out String data);
			if(data != null)
				using(StreamWriter writer = new StreamWriter(request.GetRequestStream()))
					writer.Write(data);

			return request;
		}

		private HttpWebRequest CreateRequest(CookieContainer cookies, out String data)
		{
			String address = this.ApplyTemplate<String>(nameof(this.Item.Address));
			if(String.IsNullOrEmpty(address))
				throw new ArgumentNullException(nameof(this.Item.Address));

			Uri requestUri = new Uri(address);
			// PATH for .NET Framework 4 or less: URL reverting is done for such situation: /collection/te%5Cst (Because by default it will be converted to: /collection/te/st)
			// Source: https://stackoverflow.com/questions/781205/getting-a-url-with-an-url-encoded-slash
			if(Environment.Version.Major <= 4)
				ForceCanonicalPathAndQuery(requestUri);

			HttpWebRequest result = (HttpWebRequest)WebRequest.Create(requestUri);

			Dictionary<PropertyInfo, PropertyInfo> properties = RequestBuilder.HttpItemToWebRequestMapping;
			foreach(KeyValuePair<PropertyInfo, PropertyInfo> property in properties)
			{
				Object value = property.Key.GetValue(this.Item, null);
				//Apply template including null value
				value = this._templates.ApplyTemplateV2(property.Key.Name, value, true);
				if(value != null)
					property.Value.SetValue(result, value, null);
			}

			// Adding cookies
			if(cookies != null)
				result.CookieContainer = cookies;

			// Adding CustomHeaders
			String[] customHeaders = this.ApplyTemplate<String[]>(nameof(this.Item.CustomHeaders));
			if(customHeaders?.Length > 0)
				foreach(String header in customHeaders)
				{
					String formattedValue = this._templates.ApplyTemplateV2<String>(nameof(this.Item.CustomHeaders), header, true);
					if(!String.IsNullOrEmpty(formattedValue))
					{
						const String cookieKey = "Cookie:";
						Int32 keyIndex = formattedValue.IndexOf(cookieKey, StringComparison.OrdinalIgnoreCase);
						if(keyIndex > -1)//Cookies will not be automatically placed to the headers
						{
							String[] cookies1 = formattedValue.Substring(cookieKey.Length).Split(';');
							result.CookieContainer.ParseCookieHeader(requestUri, cookies1);
						} else
							result.Headers.Add(formattedValue);
					}
				}

			// Rewriting Host header
			String host = this.ApplyTemplate<String>(nameof(this.Item.Host));
			if(host != null)
				result.Headers.GetType().InvokeMember("ChangeInternal",
					BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.InvokeMethod, null,
					result.Headers, new Object[] { "Host", host });

			// Rewriting proxy
			result.Proxy = this.CreateProxy();

			// Adding access credentials to remote server
			String userName = this.ApplyTemplate<String>(nameof(this.Item.UserName));
			String password = this.ApplyTemplate<String>(nameof(this.Item.Password));
			AuthorizationType authorizationType = this.ApplyTemplate<AuthorizationType>(nameof(this.Item.AuthorizationType));
			if(userName != null && password != null)
				switch(authorizationType)
				{
				case AuthorizationType.NTLM:
					result.Credentials = new NetworkCredential(userName, password);
					break;
				case AuthorizationType.Basic:
					String credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password));
					result.Headers.Add(HttpRequestHeader.Authorization, "Basic " + credentials);
					break;
				}

			// Adding client certificates
			String[] clientCertificates = this.ApplyTemplate<String[]>(nameof(this.Item.ClientCertificates));
			if(clientCertificates != null)
				foreach(String path in clientCertificates)
					result.ClientCertificates.Add(new X509Certificate(File.ReadAllBytes(path)));

			// Generating payload
			data = this.ApplyTemplate<String>(nameof(this.Item.Data));

			// Setting content-length manually
			Int64? contentLength = this.ApplyTemplate<Int64?>(nameof(this.Item.ContentLength));
			if(contentLength != null)
				result.ContentLength = contentLength.Value;

			return result;

			void ForceCanonicalPathAndQuery(Uri uri)
			{
				String paq = uri.PathAndQuery; // need to access PathAndQuery
				FieldInfo flagsFieldInfo = typeof(Uri).GetField("m_Flags", BindingFlags.Instance | BindingFlags.NonPublic);
				UInt64 flags = (UInt64)flagsFieldInfo.GetValue(uri);
				flags &= ~((UInt64)0x30); // Flags.PathNotCanonical|Flags.QueryNotCanonical
				flagsFieldInfo.SetValue(uri, flags);
			}
		}

		/// <summary>Инициализация прокси</summary>
		/// <returns>Интерфейс прокси</returns>
		private IWebProxy CreateProxy()
		{
			WebProxy result = null;

			// Прописывание прокси
			String proxyUserName = this.ApplyTemplate<String>(nameof(this.Item.ProxyUserName));
			String proxyPassword = this.ApplyTemplate<String>(nameof(this.Item.ProxyPassword));
			String proxyAddress = this.ApplyTemplate<String>(nameof(this.Item.ProxyAddress));
			if(proxyUserName != null || proxyPassword != null || proxyAddress != null)
			{
				result = new WebProxy();
				if(proxyAddress != null)
					result.Address = new Uri(proxyAddress);
				if(proxyUserName != null || proxyPassword != null)
					result.Credentials = new NetworkCredential(proxyUserName, proxyPassword, this.ApplyTemplate<String>(nameof(this.Item.ProxyDomain)));

				result.UseDefaultCredentials = this.ApplyTemplate<Boolean>(nameof(this.Item.UseDefaultProxyCredentials));
				String[] proxyBypassList = this.ApplyTemplate<String[]>(nameof(this.Item.ProxyBypassList));
				if(proxyBypassList?.Length > 0)
					result.BypassList = proxyBypassList;
			}
			return result;
		}

		private String CreateFileUploadPayload(String filePath)
		{//TODO: We need to decide how is better to send files to the server
			using(MemoryStream stream = new MemoryStream())
			{
				String boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

				String contentType = Utils.GetContentTypeFromExtension(Path.GetExtension(filePath)) ?? this.ApplyTemplate<String>(nameof(this.Item.ContentType));
				String fileName = Path.GetFileNameWithoutExtension(filePath);

				String header = $"Content-Disposition: form-data; name=\"{fileName}\"; filename=\"{filePath}\"\r\nContent-Type: {contentType}\r\n\r\n";
				Byte[] headerBytes = Encoding.UTF8.GetBytes(header);
				stream.Write(headerBytes, 0, headerBytes.Length);

				FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
				Byte[] buffer = new Byte[4096];
				Int32 bytesRead = 0;
				while((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
					stream.Write(buffer, 0, bytesRead);
				fileStream.Close();

				Byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
				stream.Write(trailer, 0, trailer.Length);

				using(StreamReader reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}

		public void ReplaceTemplateValuesFromResponseString(String responseString)
		{
			String template = this.Item.Response;
			if(responseString != null && template != null)
			{
				String[] keys = this._templates.GetTemplateValuesWithSource()
					.Select(t => t.Key)
					.Union(new String[] { Constant.Project.DiscardValueName })
					.ToArray();

				TemplateValuePosition[] foundItems = TemplateEngine.GetValuesFromPayloadV2(responseString, template, keys).ToArray();

				foreach(TemplateValuePosition foundItem in foundItems)
					foreach(TemplateItem templateItem in this._templates.SelectedTemplateValues)
						if(foundItem.Key == templateItem.Key)
						{
							templateItem.Value = foundItem.Value;
							break;
						}
			}
		}

		private T ApplyTemplate<T>(String propertyName)
		{
			Object value = HttpItemPropertyCache[propertyName].GetValue(this.Item, null);
			return this._templates.ApplyTemplateV2(propertyName, (T)value, true);
		}

		private static Dictionary<PropertyInfo, PropertyInfo> GetEqualProperties<T1, T2>()
		{
			PropertyInfo[] T1Properties1 = typeof(T1).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] T2Properties1 = typeof(T2).GetProperties(BindingFlags.Instance | BindingFlags.Public);

			Dictionary<PropertyInfo, PropertyInfo> result = new Dictionary<PropertyInfo, PropertyInfo>();

			foreach(PropertyInfo property in T1Properties1)
				if(property.IsDefined(typeof(CategoryAttribute)) && property.CanRead && property.GetIndexParameters().Length == 0)//Хак для проверки наличия атрибута отображения в UI
					foreach(PropertyInfo requestProperty in T2Properties1)
						if(requestProperty.Name == property.Name && requestProperty.CanWrite && requestProperty.PropertyType == property.PropertyType && requestProperty.GetIndexParameters().Length == 0)
						{
							result.Add(property, requestProperty);
							break;
						}

			return result;
		}
	}
}