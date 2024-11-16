using System;
using System.Collections;
using System.ComponentModel;

namespace Plugin.HttpClient.UI
{
	class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
	{
		public IDictionary Dictionary { get; }

		public DictionaryPropertyGridAdapter(IDictionary d)
			=> this.Dictionary = d;

		public String GetComponentName()
			=> TypeDescriptor.GetComponentName(this, true);

		public EventDescriptor GetDefaultEvent()
			=> TypeDescriptor.GetDefaultEvent(this, true);

		public String GetClassName()
			=> TypeDescriptor.GetClassName(this, true);

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
			=> TypeDescriptor.GetEvents(this, attributes, true);

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
			=> TypeDescriptor.GetEvents(this, true);

		public TypeConverter GetConverter()
			=> TypeDescriptor.GetConverter(this, true);

		public Object GetPropertyOwner(PropertyDescriptor pd)
			=> this.Dictionary;

		public AttributeCollection GetAttributes()
			=> TypeDescriptor.GetAttributes(this, true);

		public Object GetEditor(Type editorBaseType)
			=> TypeDescriptor.GetEditor(this, editorBaseType, true);

		public PropertyDescriptor GetDefaultProperty()
			=> null;

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
			=> ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			ArrayList properties = new ArrayList();
			foreach(DictionaryEntry e in this.Dictionary)
				properties.Add(new DictionaryPropertyDescriptor(this.Dictionary, e.Key));

			PropertyDescriptor[] props = (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

			return new PropertyDescriptorCollection(props);
		}
	}
}