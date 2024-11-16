using System;
using System.Collections;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.UI
{
	internal class ProjectTreeListItem
	{
		/// <summary>Тестовая нода проекта</summary>
		public HttpProjectItem Project { get; }

		/// <summary>Last server response</summary>
		public String HttpResponse { get; set; }

		/// <summary>Индекс изображения статуса теста</summary>
		public ProjectTreeNode.TreeImageList ImageIndex { get; set; }

		public ProjectTreeListItem(HttpProjectItem item)
			=> this.Project = item;
	}
}