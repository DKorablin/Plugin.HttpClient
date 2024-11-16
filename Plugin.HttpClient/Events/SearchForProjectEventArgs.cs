using System;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Events
{
	internal class SearchForProjectEventArgs : EventArgs
	{
		/// <summary>Path to project item to search for</summary>
		public String ProjectFileName { get; }

		/// <summary>Found project or null if project is not found</summary>
		public HttpProject Project { get; set; }

		public SearchForProjectEventArgs(String projectFileName)
			=> this.ProjectFileName = projectFileName;
	}
}