using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Plugin.HttpClient.Test;

namespace Plugin.HttpClient.Project
{
	internal class TemplateEngine
	{
		private readonly HttpProject _project;
		private TemplateItem[] _selectedItemsCache;

		public TemplateEngine(HttpProject project)
			=> this._project = project;

		public virtual String SelectedTemplate => this._project.SelectedTemplate;

		public virtual TemplateItem[] SelectedTemplateValues
		{
			get => this._selectedItemsCache ?? (this._selectedItemsCache = this._project.TemplatesCollection[this._project.SelectedTemplate]);
			internal set => this._selectedItemsCache = value;
		}

		/// <summary>Gets list of all selected template values</summary>
		/// <returns>Sorted list of all selected template values</returns>
		public IEnumerable<TemplateItem> GetTemplateValuesSorted(Boolean isSystem)
			=> TemplateEngine.SortBy((IEnumerable<TemplateItem>)this.GetTemplateValuesWithSource().Where(p => p.IsSystem() == isSystem), CompareKeyByLengthDesc);

		/// <summary>Apply template keys from selected template to object property</summary>
		/// <param name="propertyName">The property name</param>
		/// <param name="baseValueType">The type of base property value</param>
		/// <param name="baseValue">The base property value</param>
		/// <param name="isApply">Apply template item and replace key by value OR replace template value by key</param>
		/// <returns></returns>
		public Object ApplyTemplateV2R(String propertyName, Type baseValueType, Object baseValue, Boolean isApply)
			=> Utils.InvokeGenericMethod(this, nameof(ApplyTemplateV2), baseValueType, new Object[] { propertyName, baseValue, isApply, });

		/// <summary>Apply template keys from selected template to object property</summary>
		/// <typeparam name="T">The type of current property</typeparam>
		/// <param name="propertyName">The property name</param>
		/// <param name="baseValue">Base property value</param>
		/// <param name="isApply">Apply template item and replace key by value OR replace template value by key</param>
		/// <returns></returns>
		public T ApplyTemplateV2<T>(String propertyName, T baseValue, Boolean isApply)
		{
			T result = isApply
				? this.GetTemplateSystemValue(propertyName, baseValue)
				: this.RevertFromTemplateSystemValue(propertyName, baseValue);

			if(result is String s)
				result = (T)(Object)ApplyTemplate(s);

			return result;

			String ApplyTemplate(String value)
			{
				StringBuilder formatter = new StringBuilder(value);
				foreach(TemplateItem item in this.GetTemplateValuesSorted(false))
				{
					if(isApply)//Apply template replacing keys by values
						formatter.Replace(item.Key, item.Value);
					else//Remove template replacing values by keys
						formatter.Replace(item.Value, item.Key);
				}
				return formatter.ToString();
			}
		}

		/// <summary>Gets the list of all template keys with values and with template name</summary>
		/// <remarks>We can't sort items here because we need different sort order when viewing templates and applying templates</remarks>
		/// <returns>Template items from selected template name and from <see cref="Constant.Project.DefaultTemplateName"/> if selected didn't contains any values</returns>
		public virtual IEnumerable<TemplateItemSource> GetTemplateValuesWithSource()
		{
			TemplateItem[] selected = this.SelectedTemplateValues;
			foreach(TemplateItem item in selected)
				yield return new TemplateItemSource(this.SelectedTemplate, item);

			if(this.SelectedTemplate != Constant.Project.DefaultTemplateName)
			{
				List<TemplateItem> current = new List<TemplateItem>(selected);
				foreach(TemplateItem item in this._project.TemplatesCollection[Constant.Project.DefaultTemplateName])
					if(!current.Any(t => t.Key == item.Key))//Adding template keys from default template name if they doesn't exists in current template
						yield return new TemplateItemSource(Constant.Project.DefaultTemplateName, item);
			}
		}

		/// <summary>Searching for template item in current selected template items or in the default template if not found at selected template</summary>
		/// <param name="fnSearch">Search callback function</param>
		/// <returns>Found template item or null</returns>
		private TemplateItem GetTemplateItem(Func<TemplateItem, Boolean> fnSearch)
		{
			TemplateItem result = this.SelectedTemplateValues.FirstOrDefault(t => fnSearch(t));

			return result != null || this.SelectedTemplate == Constant.Project.DefaultTemplateName
				? result
				: this._project.TemplatesCollection[Constant.Project.DefaultTemplateName].FirstOrDefault(t => fnSearch(t));
		}

		private T GetTemplateSystemValue<T>(String propertyName, T baseValue)
		{
			//if(!EqualityComparer<T>.Default.Equals(baseValue, default))
			//	return baseValue;//Item value and default object values are not the same

			/*if(!baseValue.Equals(attr.Value))
				return baseValue;//Attribute default value and property value are not the same*/

			// Searching value in the selected template and in defaultTemplate if not found in the selected template
			TemplateItem result = this.GetTemplateItem(k => k.IsSystemProperty(propertyName));
			if(result != null
				&& EqualityComparer<T>.Default.Equals(baseValue, default)
				&& Utils.TryConvert(result.Value, out T converted))
				return converted;

			// Searching for property in all properties (Maybe we should move it as the first check)
			if(!RequestBuilder.HttpItemPropertyCache.TryGetValue(propertyName, out PropertyInfo prop))
				return baseValue;

			// Trying to read value from attribute value
			DefaultValueAttribute attr = prop.GetCustomAttribute<DefaultValueAttribute>();
			return attr != null && EqualityComparer<T>.Default.Equals(baseValue, default)
				? (T)attr.Value//Replacing null or default value with value from the attribute
				: baseValue;
		}

		private T RevertFromTemplateSystemValue<T>(String propertyName, T value)
		{
			if(EqualityComparer<T>.Default.Equals(value, default))
				return default;//Value is equal to default we can pass it as is

			if(!RequestBuilder.HttpItemPropertyCache.TryGetValue(propertyName, out PropertyInfo prop))//We cant find property with such name and can't check for system template
				return value;

			DefaultValueAttribute attr = prop.GetCustomAttribute<DefaultValueAttribute>();
			if(attr != null && value.Equals(attr.Value))
				return default;//We found default value and it equals to current value. In that case default value is returned

			TemplateItem result = this.GetTemplateItem(k => k.IsSystemProperty(propertyName));
			if(result == null)
				return value;

			if(Utils.TryConvert(result.Value, out T test) && test.Equals(value))
				return default;

			return value;
		}

		internal static IEnumerable<TemplateValuePosition> GetValuesFromPayloadV2(String payload, String template, String[] keys)
		{
			//TODO: Need to handle situation when 2 template keys in a row. For example: {valid1}{invalid}\\{valid2}
			KeyValuePair<String, Int32>[] foundKeys = IndexOfAll(template, keys);
			if(foundKeys.Length == 0)
				yield break;

			Int32 payloadStart = 0;
			Int32 templateStart = 0;//Check position start index

			for(Int32 loop = 0; loop < foundKeys.Length; loop++)
			{
				Int32 templateIndex = foundKeys[loop].Value;//Template key start index
				String foundKey = foundKeys[loop].Key;

				String templatePart = template.Substring(templateStart, templateIndex - templateStart);
				if(payload.Length < payloadStart + templatePart.Length)
					break;//Payload size is less then template key

				String payloadPart = payload.Substring(payloadStart, templatePart.Length);
				if(templatePart != payloadPart)
					break;//Payload part is not equal to template part

				payloadStart += payloadPart.Length;
				templateStart += templatePart.Length + foundKey.Length;

				String nextFoundKey = String.Empty;
				if(loop + 1 < foundKeys.Length)
				{
					templateIndex = foundKeys[loop + 1].Value;
					nextFoundKey = foundKeys[loop + 1].Key;
				} else
				{
					if(template.Substring(templateIndex) == foundKey)//If template and payload ends with template key
					{
						String lastTemplateValue = payload.Substring(payloadStart);
						yield return new TemplateValuePosition(foundKey, lastTemplateValue, payloadStart);
						break;
					}

					templateIndex = template.Length;
				}

				String nextTemplatePart = template.Substring(templateStart, templateIndex - templateStart);
				Int32 nextPayloadIndex = payload.IndexOf(nextTemplatePart, payloadStart);
				if(nextPayloadIndex == -1)
					break;//Can't find the next payload index

				String templateValue = payload.Substring(payloadStart, nextPayloadIndex - payloadStart);
				yield return new TemplateValuePosition(foundKey, templateValue, payloadStart);

				if(String.IsNullOrEmpty(nextFoundKey))
					break;

				payloadStart = nextPayloadIndex;
				foundKey = nextFoundKey;
			}

			KeyValuePair<String, Int32>[] IndexOfAll(String str, String[] keysIn)
			{
				Int32 startIndex = 0;
				List<KeyValuePair<String, Int32>> result = new List<KeyValuePair<String, Int32>>();
				foreach(String key in keysIn)
				{
					Int32 index = str.IndexOf(key, startIndex);
					while(index > -1)
					{
						if(result.Any(kv => kv.Value + kv.Key.Length == index))
							throw new InvalidOperationException($"Template key {key} can't follow each other");
						result.Add(new KeyValuePair<String, Int32>(key, index));

						index = str.IndexOf(key, index + key.Length);
					}
				}
				return result.OrderBy(kv => kv.Value).ToArray();
			}
		}

		#region Sorting
		static IEnumerable<T> SortBy<T>(IEnumerable<T> input, Comparison<T> comparison) where T : TemplateItem
		{
			List<T> result = new List<T>(input);

			result.Sort(comparison);

			return result;
		}

		private static Int32 CompareKeyByLengthDesc(TemplateItem x, TemplateItem y)
			=> CompareByStringLengthDesc(x.Key, y.Key);

		private static Int32 CompareValueByLengthDesc(TemplateItem x, TemplateItem y)
			=> CompareByStringLengthDesc(x.Value, y.Value);

		private static Int32 CompareByStringLengthDesc(String x, String y)
		{
			Int32 result = CompareByStringLength(x, y);
			return result * -1;
		}

		private static Int32 CompareByStringLength(String x, String y)
		{
			if(x == null)
				return y == null
					? 0//If x is null and y is null, they're equal
					: -1;//If x is null and y is not null, y is greater
			else
			{//If x is not null...
				if(y == null)
					return 1;//...and y is null, x is greater.
				else
				{// ...and y is not null, compare the lengths of the two strings.
					Int32 result = x.Length.CompareTo(y.Length);

					return result != 0
						? result// If the strings are not of equal length, the longer string is greater.
						: x.CompareTo(y);// If the strings are of equal length, sort them with ordinary string comparison.
				}
			}
		}
		#endregion Sorting
	}
}