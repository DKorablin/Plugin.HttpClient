using System;
using System.Net;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class ResultResponse : ResultBase
	{
		public HttpStatusCode StatusCode { get; }

		public ResultResponse(HttpItem item, HttpStatusCode statusCode)
			: base(item, (HttpWebRequest)null, (HttpWebResponse)null)
		{//Added for testing purposes only
			System.Threading.Thread.Sleep(1000);
			this.StatusCode = statusCode;
			this.IsSuccess = true;
#if !DEBUG
			throw new InvalidOperationException("This .ctor should not be used on production environment.");
#endif
		}

		public ResultResponse(HttpItem item, HttpWebRequest request, HttpWebResponse response)
			: base(item, request, response)
		{
			_ = response ?? throw new ArgumentNullException(nameof(response));

			this.StatusCode = response.StatusCode;
			this.IsSuccess = true;
		}
	}
}