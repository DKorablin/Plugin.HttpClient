using System;
using System.Net;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class ResultException : ResultBase
	{
		public Exception Exception { get; }

		public ResultException(HttpItem item, HttpListenerRequest request, Exception exc)
			: base(item, request, null)
		{
			base.IsSuccess = false;
			this.Exception = exc;
		}

		public ResultException(HttpItem item, HttpWebRequest request, Exception exc)
			: base(item, request, null)
		{
			base.IsSuccess = false;
			this.Exception = exc;
		}
	}
}