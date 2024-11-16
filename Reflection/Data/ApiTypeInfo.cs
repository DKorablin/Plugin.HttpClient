using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using AlphaOmega.Debug.CorDirectory.Meta;
using AlphaOmega.Debug.CorDirectory.Meta.Reader;

namespace Plugin.HttpClient.Reflection.Data
{
	public class ApiTypeInfo
	{
		public ApiTypeInfo Base { get ; }

		/// <summary>Тип параметра</summary>
		public ApiTypeInfo Parent { get ; }

		/// <summary>Status code returned with this out parameter</summary>
		public HttpStatusCode? HttpCode { get; }

		/// <summary>Видимые вовне параметры типа</summary>
		public ApiTypeInfo[] Parameters { get ; }

		public String Name { get; }

		public String TypeName { get; }

		public Boolean IsArray { get ; }

		public Boolean IsNullable { get ; }

		public ApiTypeInfo(MemberArgument argument, ApiTypeInfo parent)
		{
			_ = argument ?? throw new ArgumentNullException(nameof(argument));

			ElementType type = argument.Type;
			this.IsArray = argument.Type.IsArray;
			this.Name = argument.Name;
			this.TypeName = argument.Type.TypeName;

			switch(argument.Type.Type)
			{
			case AlphaOmega.Debug.Cor.ELEMENT_TYPE.CLASS:
				if(argument.Type.TypeDefOrRef.TableType == AlphaOmega.Debug.Cor.MetaTableType.TypeRef)
					using(TypeRefReader typeReader = new TypeRefReader(argument.Type.TypeDefOrRef))
						foreach(var prop in typeReader.TypeDef.GetProperties())
						{

						}
				break;
			}

			//Для генериков типа IEnumerable<T> и Nullable<T>
			//TODO: Тут может быть рекурсия генерик параметров
			if(type.GenericArguments != null && type.TypeName.StartsWith(typeof(Nullable<>).Name))
			{
				this.IsNullable = true;
				type = type.GenericArguments[0];
			}

			/*//Для генериков типа IEnumerable<T> и Nullable<T>
			if(type.GenericArguments != null)
				if(type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					this.IsArray = true;
					type = type.GetGenericArguments()[0];
				}

			if(type.IsGenericType)//TODO: Тут может быть рекурсия генерик параметров
				if(type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					this.IsNullable = true;
					type = type.GetGenericArguments()[0];
				}

			String displayTypeName = (type.IsEnum
				? Enum.GetUnderlyingType(type).FullName
				: type.FullName).Replace('+', '.');
			this.DisplayTypeName = displayTypeName.Substring(displayTypeName.LastIndexOf('.') + 1);
			this.DisplayName = displayName ?? name;

			this.Name = name;//У return нет названия. TODO: Но название нужно для данных в XML формате

			if(parent == null || parent.TypeName != this.TypeName)
			{//Спасаемся от переполнения стека
				List<ApiTypeInfo> parameters = new List<ApiTypeInfo>();
				parameters.AddRange(this.GetParameters(type));
				this.Parameters = parameters.Count == 0 ? null : parameters.ToArray();
			}

			this.Parent = parent;
			this.Base = this.GetBaseType(type);*/
		}
	}
}