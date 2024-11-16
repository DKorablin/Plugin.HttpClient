using System;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.History
{
	internal class HttpHistoryItem
	{
		public HistoryActionType Action { get; }

		public DateTime InvokeDate { get; private set; }

		public HttpItem Request { get; }

		public String Response { get; private set; }

		public TimeSpan? Elapsed { get; private set; }

		public HttpHistoryItem(HistoryActionType action, HttpItem request, String response, TimeSpan? elapsed)
		{
			this.Action = action;
			this.InvokeDate = DateTime.Now;
			this.Request = request;
			this.Response = response;
			this.Elapsed = elapsed;
		}

		public void Update(String response)
		{
			this.InvokeDate = DateTime.Now;
			this.Response = response;
		}
	}
}