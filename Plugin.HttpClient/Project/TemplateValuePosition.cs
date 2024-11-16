using System;

namespace Plugin.HttpClient.Project
{
	internal class TemplateValuePosition : TemplateItem
	{
		/// <summary>Template start position</summary>
		public Int32 Index { get; set; }

		/// <summary>Create instance of <see cref="TemplateValuePosition"/> with all required fields</summary>
		/// <param name="key">Found tempate item key</param>
		/// <param name="value">Parsed value on the key position</param>
		/// <param name="index">String index where the template item found</param>
		public TemplateValuePosition(String key, String value, Int32 index)
			: base(key, value)
		{
			this.Index = index;
		}
	}
}