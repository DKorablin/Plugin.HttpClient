using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.UI;

namespace Plugin.HttpClient.Test
{
	internal class TestStartArgs : EventArgs
	{
		/// <summary>Тип теста</summary>
		public enum TestType
		{
			/// <summary>Тест всех узлов в проекте</summary>
			Full,
			/// <summary>Тест одного узла и всех дочерних узлов</summary>
			Thread,
			/// <summary>Тест одного узла</summary>
			Single,
		}

		public PluginWindows Plugin { get; }

		/// <summary>Logic to use to perform this test</summary>
		public TestType Type { get; private set; }

		/// <summary>The project item on what to perform test</summary>
		//public HttpProjectItem Item { get; set; }

		//public HttpProject Project { get; set; }

		public HttpProjectItem[] Items { get; set; }

		public TemplateItem[] Variables { get; }

		public TestStartArgs(PluginWindows plugin, HttpProject project)
			: this(plugin, TestType.Full, project, null)
		{
		}

		public TestStartArgs(PluginWindows plugin, TestType type, HttpProjectItem item)
			: this(plugin, type, null, item)
		{
		}

		private TestStartArgs(PluginWindows plugin, TestType type, HttpProject project, HttpProjectItem item)
		{
			TemplateEngine engine;
			IEnumerable<HttpProjectItem> items;
			switch(type)
			{
			case TestType.Full:
				_ = project ?? throw new ArgumentNullException(nameof(project));

				items = project.Items.EnumerateItems();
				engine = project.Templates;
				break;
			case TestType.Thread:
				_ = item ?? throw new ArgumentNullException(nameof(item));

				items = new HttpProjectItem[] { item }.Concat(item.Items.EnumerateItems());
				engine = item.Items.Project.Templates;
				break;
			case TestType.Single:
				_ = item ?? throw new ArgumentNullException(nameof(item));

				items = new HttpProjectItem[] { item };
				engine = item.Items.Project.Templates;
				break;
			default: throw new NotSupportedException();
			}

			this.Items = items.Where(i => i.Image != ProjectTreeNode.TreeImageList.Skip).ToArray();

			foreach(HttpProjectItem elem in this.Items)
				elem.Image = ProjectTreeNode.TreeImageList.New;

			this.Plugin = plugin;
			this.Type = type;

			TemplateItemSource discardItem = TemplateItemSource.DiscardItemTemplate;
			this.Variables = engine.GetTemplateValuesWithSource()
				.Union(new TemplateItemSource[] { discardItem, })
				.ToArray();
			//this.Project = project;
			//this.Item = item;
		}
	}
}