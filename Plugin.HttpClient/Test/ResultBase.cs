using System;
using System.IO;
using System.Net;
using System.Text;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	public class ResultBase
	{
		public Boolean IsSuccess { get; set; }

		public HttpItem Item { get; }

		public virtual String ResponseHeaders { get; }

		public virtual String ResponseContentType { get; }

		public virtual String ResponseBody { get; }

		/// <summary>Elapsed time only set when using as client</summary>
		public TimeSpan? Elapsed { get; set; }

		public ResultBase(HttpItem item, HttpWebRequest request, HttpWebResponse response)
		{//request can be null here (If exception related to request building logic)
			this.Item = item;
			this.ResponseHeaders = ResultBase.GetResponseHeaders(response);
			this.ResponseContentType = response?.ContentType;
			this.ResponseBody = ResultBase.GetResponseBody(response);
		}

		public ResultBase(HttpItem item, HttpListenerRequest request, HttpListenerResponse response)
		{//TODO: Here we need to rewrite everything to log requests
			_ = request ?? throw new ArgumentNullException(nameof(request));

			this.Item = new HttpProjectItem(request);
			this.ResponseHeaders = ResultBase.GetResponseHeaders(response);
			this.ResponseContentType = response?.ContentType;
			this.ResponseBody = item?.Response;
			this.IsSuccess = true;
		}

		public ResultBase(ResultBase result)
		{
			_ = result ?? throw new ArgumentNullException(nameof(result));

			this.IsSuccess = result.IsSuccess;
			this.Item = result.Item;
			this.ResponseHeaders = result.ResponseHeaders;
			this.ResponseBody = result.ResponseBody;
			this.Elapsed = result.Elapsed;
		}

		public String GetResponseWithHeaders()
			=> this.ResponseHeaders != null || this.ResponseBody != null
				? String.Join(Environment.NewLine, new String[] { this.ResponseHeaders, this.ResponseBody, })
				: null;

		private static String GetResponseHeaders(HttpWebResponse response)
		{
			if(response == null)
				return null;

			String httpHeader = "/" + String.Join(" ", new String[] { response.ProtocolVersion.ToString(), ((Int32)response.StatusCode).ToString(), response.StatusDescription }) + Environment.NewLine;
			String headers = response.Headers.ToString();

			return String.Join(Environment.NewLine, new String[] { httpHeader, headers, });
		}

		private static String GetResponseHeaders(HttpListenerResponse response)
		{
			if(response == null)
				return null;

			String httpHeader = "/" + String.Join(" ", new String[] { response.ProtocolVersion.ToString(), response.StatusCode.ToString(), response.StatusDescription }) + Environment.NewLine;
			String headers = response.Headers.ToString();

			return String.Join(Environment.NewLine, new String[] { httpHeader, headers, });
		}

		private static String GetResponseBody(HttpWebResponse response)
		{
			if(response == null)
				return null;

			Encoding encoding = Encoding.UTF8;
			try
			{
				encoding = Encoding.GetEncoding(response.CharacterSet);
			} catch(ArgumentException)
			{//Invalid encoding
			}

			using(StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
				return reader.ReadToEnd();
		}

		/*public static T GetResponseType<T>(this HttpWebResponse response)
		{
			return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(response.GetResponseStream());
		}*/
	}
}