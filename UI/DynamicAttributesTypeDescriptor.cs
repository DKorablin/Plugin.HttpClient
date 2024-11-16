using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Plugin.HttpClient.UI
{
	internal class DynamicAttributesTypeDescriptor<T> : CustomTypeDescriptor
	{
		private readonly ICustomTypeDescriptor _originalDescriptor;

		public T Instance { get; }

		/// <summary>Most useful for changing EditorAttribute and TypeConvertorAttribute</summary>
		public IDictionary<String, Attribute[]> DynamicProperties { get; set; }

		public DynamicAttributesTypeDescriptor(T instance)
			: this(instance, TypeDescriptor.GetProvider(instance).GetTypeDescriptor(instance))
		{ }

		public DynamicAttributesTypeDescriptor(T instance, ICustomTypeDescriptor originalDescriptor) : base(originalDescriptor)
		{
			this.Instance = instance;
			this._originalDescriptor = originalDescriptor;
			this.DynamicProperties = new Dictionary<String, Attribute[]>();
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			var properties = base.GetProperties(attributes).Cast<PropertyDescriptor>();

			var bettered = properties.Select(pd =>
			{
				return this.DynamicProperties.TryGetValue(pd.Name, out Attribute[] values)
					? TypeDescriptor.CreateProperty(typeof(T), pd, values)
					: pd;
			});
			return new PropertyDescriptorCollection(bettered.ToArray());
		}

		public override PropertyDescriptorCollection GetProperties()
			=> GetProperties(null);
	}
}