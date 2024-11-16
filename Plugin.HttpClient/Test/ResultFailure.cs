using System;
using System.Net;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class ResultFailure : ResultBase
	{
		public WebExceptionStatus StatusCode { get; }

		public override String ResponseString { get; }

		public ResultFailure(HttpItem item, HttpWebRequest request, WebException exc)
			: base(item, request, null)
		{
			this.StatusCode = exc.Status;
			this.ResponseString = exc.InnerException == null
				? exc.Message
				: exc.InnerException.Message;

			base.IsSuccess = false;
		}
	}
}