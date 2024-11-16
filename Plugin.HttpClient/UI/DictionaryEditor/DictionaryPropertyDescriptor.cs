﻿using System;
using System.Collections;
using System.ComponentModel;

namespace Plugin.HttpClient.UI
{
	class DictionaryPropertyDescriptor : PropertyDescriptor
	{
		private IDictionary _dictionary;
		private Object _key;

		internal DictionaryPropertyDescriptor(IDictionary d, Object key)
			: base(key.ToString(), null)
		{
			this._dictionary = d;
			this._key = key;
		}

		public override Type PropertyType { get { return _dictionary[_key].GetType(); } }

		public override void SetValue(Object component, Object value)
		{
			_dictionary[_key] = value;
		}

		public override Object GetValue(Object component)
		{
			return _dictionary[_key];
		}

		public override Boolean IsReadOnly { get { return false; } }

		public override Type ComponentType { get { return null; } }

		public override Boolean CanResetValue(Object component)
		{
			return false;
		}

		public override void ResetValue(Object component)
		{
		}

		public override Boolean ShouldSerializeValue(Object component)
		{
			return false;
		}
	}
}