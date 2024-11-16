using System;

namespace Plugin.HttpClient.Project
{
	/// <summary>Template key-value description with template name</summary>
	/// <remarks>For example if <see cref="TemplateItemSource"/> from <see cref="Constant.Project.DefaultTemplateName"/></remarks>
	internal class TemplateItemSource : TemplateItem
	{
		/// <summary>Template name for <see cref="HttpProject.TemplatesCollection"/> dictionary</summary>
		public String TemplateName { get; set; }

		public TemplateItemSource() { }//This is done for DataGridView to add a new item

		public TemplateItemSource(String templateName, TemplateItem item)
			: base(item.Key, item.Value)
			=> this.TemplateName = templateName;

		/// <summary>Gets template item for discarded values</summary>
		public static TemplateItemSource DiscardItemTemplate
		{
			get => new TemplateItemSource(Constant.Project.DefaultTemplateName, new TemplateItem(Constant.Project.DiscardValueName, Constant.Project.DiscardValueName));
		}
	}
}