using System;
using System.Diagnostics;

namespace Plugin.HttpClient.Project
{
	/// <summary>Template key-value item description</summary>
	[Serializable]
	[DebuggerDisplay(nameof(Key) + "={" + nameof(Key) + "} Value={" + nameof(Value) + "}")]
	public class TemplateItem
	{
		/// <summary>Template key name</summary>
		public String Key { get; set; }

		/// <summary>Template key value</summary>
		public String Value { get; set; }

		/// <summary>This key is a system property reference</summary>
		/// <remarks>It starts from "System." prefix</remarks>
		/// <returns>The key is a system property</returns>
		public Boolean IsSystem()
			=> this.Key?.StartsWith(Constant.Project.TemplateDefaultValuePrefix) ?? false;

		public Boolean IsSystemProperty(String propertyName)
			=> this.Key == Constant.Project.TemplateDefaultValuePrefix + propertyName;

		/// <summary>Gets key name without system prefix</summary>
		/// <returns>Key name or system key name without system prefix</returns>
		public String GetKey()
			=> this.IsSystem()
				? this.Key.Substring(Constant.Project.TemplateDefaultValuePrefix.Length)
				: this.Key;

		/// <summary>Create instance of <see cref="TemplateItem"/> without any values</summary>
		/// <remarks>Used for serialization and UI</remarks>
		public TemplateItem() { }

		/// <summary>Create instance of <see cref="TemplateItem"/> with specified Key and Value properties</summary>
		/// <param name="key">The template key name</param>
		/// <param name="value">The template value</param>
		public TemplateItem(String key, String value)
		{
			this.Key = key;
			this.Value = value;
		}
	}
}