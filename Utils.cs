using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient
{
	internal static class Utils
	{
		public static Boolean IsBitSet(UInt32 flags, Int32 bit)
			=> (flags & System.Convert.ToInt32(System.Math.Pow(2, bit))) != 0;

		public static UInt32[] BitToInt(params Boolean[] bits)
		{
			UInt32[] result = new UInt32[] { };
			Int32 counter = 0;
			for(Int32 loop = 0; loop < bits.Length; loop++)
			{
				if(result.Length <= loop)//Увеличиваю массив на один, если не помещается значение
					Array.Resize<UInt32>(ref result, result.Length + 1);

				for(Int32 innerLoop = 0; innerLoop < 32; innerLoop++)
				{
					result[loop] |= System.Convert.ToUInt32(bits[counter++]) << innerLoop;
					if(counter >= bits.Length)
						break;
				}
				if(counter >= bits.Length)
					break;
			}
			return result;
		}

		public static Boolean TryConvert<I>(I from, Type toType, out Object to)
		{
			Boolean result = true;
			try
			{
				to = Utils.Convert(from, toType);
			} catch(FormatException)
			{
				to = default;
				result = false;
			}
			return result;
		}

		public static Boolean TryConvert<I, O>(I from, out O to)
		{
			Boolean result = TryConvert<I>(from, typeof(O), out Object pass);
			to = result ? (O)pass : default;
			return result;
		}

		public static Object Convert<I>(I from, Type toType)
		{
			if(from == null)
				return null;

			Type fromType = from.GetType();
			if(fromType == toType)
				return from;

			TypeConverter converter = TypeDescriptor.GetConverter(toType);
			Boolean canConvert = converter.CanConvertFrom(fromType);
			if(canConvert)
				return converter.ConvertFrom(from);
			else if(toType == typeof(Object))
				return from;//HACK: This situation happenens when property value is null then it always be the Object type

			if(toType.IsArray)
			{
				Type toBaseType = toType.GetElementType();
				String[] strFrom = from.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				Array result = Array.CreateInstance(toBaseType, strFrom.Length);
				for(Int32 loop = 0; loop < result.Length; loop++)
					result.SetValue(Convert(strFrom[loop], toBaseType), loop);
				return result;
			} else
				throw new FormatException($"Can't convert from {fromType} to {toType}");
		}

		/// <summary>Проверка исключения на фатальное, после которого дальнейшее выполнение кода невозможно</summary>
		/// <param name="exception">Исключение для проверки</param>
		/// <returns>Исключение фатальное</returns>
		public static Boolean IsFatal(Exception exception)
		{
			while(exception != null)
			{
				if((exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) || exception is ThreadAbortException || exception is AccessViolationException || exception is SEHException)
					return true;
				if(!(exception is TypeInitializationException) && !(exception is TargetInvocationException))
					break;
				exception = exception.InnerException;
			}
			return false;
		}

		/// <summary>Gets the content-type from registry or null if unknown extension specified</summary>
		/// <param name="extension">Extension by whitch to receive content type</param>
		/// <returns>The content type from extension</returns>
		public static String GetContentTypeFromExtension(String extension)
		{//TODO: .NET 4.5 -> System.Web.MimeMapping.GetMimeMapping
			RegistryKey key = Registry.ClassesRoot.OpenSubKey(extension, false);
			Object value = key?.GetValue("Content Type", null);
			return value?.ToString();
		}

		/// <summary>Gets the file extension from content-type using registre</summary>
		/// <param name="contentType">Content-Type by which to receive file extension</param>
		/// <returns>The extension by content-type or null if specified content type is not found</returns>
		public static String GetExtensionFromContentType(String contentType)
		{
			RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"MIME\Database\Content Type\" + contentType, false);
			Object value = key?.GetValue("Extension", null);
			return value?.ToString();
		}

		/// <summary>Generic method invocation</summary>
		/// <param name="instance">Method source object</param>
		/// <param name="methodName">The mane of generic method to invoke</param>
		/// <param name="objectType">The type of generic object</param>
		/// <param name="parameters">Invocation method arguments.</param>
		/// <returns>Result ob method invocation</returns>
		public static Object InvokeGerericMethod(Object instance, String methodName, Type objectType, Object[] parameters)
		{
			MethodInfo method = instance.GetType().GetMethod(methodName);
			MethodInfo generic = method.MakeGenericMethod(objectType);
			return generic.Invoke(instance, parameters);
		}
	}
}