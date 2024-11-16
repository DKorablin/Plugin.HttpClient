using System;

namespace Plugin.HttpClient.Events
{
	internal class ToggleProjectDirtyEventArgs : EventArgs
	{
		/// <summary>The project name that is marked as dirty</summary>
		public String ProjectFilePath { get; }

		/// <summary>Mark project as dirty or as clean</summary>
		public Boolean IsDirty { get; }

		public ToggleProjectDirtyEventArgs(String projectFilePath, Boolean isDirty)
		{
			this.ProjectFilePath = projectFilePath;
			this.IsDirty = isDirty;
		}
	}
}