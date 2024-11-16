using System;
using System.Collections.Generic;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Events
{
	/// <summary>Arguments to search for http item inside opened windows</summary>
	internal class SearchProjectItemEventArgs : EventArgs
	{
		/// <summary>What we're searching</summary>
		public HttpProjectItem Search { get; }

		/// <summary>What we have found</summary>
		public List<HttpProjectItem> Found { get; set; }

		public SearchProjectItemEventArgs(HttpProjectItem search)
		{
			this.Search = search;
			this.Found = new List<HttpProjectItem>();
		}
	}
}