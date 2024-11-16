using System;
using System.Reflection;

namespace Plugin.HttpClient
{
	internal static class TypeExtender
	{
		public static T GetCustomAttribute<T>(this MemberInfo item) where T : Attribute
		{
			Object[] items = Attribute.GetCustomAttributes(item, typeof(T), true);
			return items.Length == 0 ? default : (T)items[0];
		}

		public static Boolean IsDefined(this MemberInfo info, Type attributeType)
			=> Array.Exists(Attribute.GetCustomAttributes(info,true), delegate (Object item) { return item.GetType() == attributeType; });
	}
}