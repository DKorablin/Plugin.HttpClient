using System;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class TestProgressChangedArgs : EventArgs
	{
		public HttpProjectItem Item { get; }

		public RequestTest Result { get; }

		public TestProgressChangedArgs(RequestTest result)
			=> this.Result = result;

		public TestProgressChangedArgs(HttpProjectItem item)
			=> this.Item = item;
	}
}
