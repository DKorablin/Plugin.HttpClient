using System;
using System.Collections;
using System.ComponentModel;

namespace Plugin.HttpClient.UI
{
	class DictionaryPropertyDescriptor : PropertyDescriptor
	{
		private readonly IDictionary _dictionary;
		private readonly Object _key;

		internal DictionaryPropertyDescriptor(IDictionary d, Object key)
			: base(key.ToString(), null)
		{
			this._dictionary = d;
			this._key = key;
		}

		public override Type PropertyType { get => this._dictionary[_key].GetType(); }

		public override void SetValue(Object component, Object value)
			=> this._dictionary[_key] = value;

		public override Object GetValue(Object component)
			=> this._dictionary[_key];

		public override Boolean IsReadOnly { get => false; }

		public override Type ComponentType { get => null; }

		public override Boolean CanResetValue(Object component)
			=> false;

		public override void ResetValue(Object component)
		{
		}

		public override Boolean ShouldSerializeValue(Object component)
			=> false;
	}
}