using System;

namespace Plugin.HttpClient.Events
{
	internal class ProjectClosedEventArgs : EventArgs
	{
		/// <summary>The path to project that is closing</summary>
		public String ProjectFileName { get; }

		public ProjectClosedEventArgs(String projectFileName)
			=> this.ProjectFileName = projectFileName;
	}
}