using System;
using System.Collections.Generic;
using System.Linq;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta.Reader;

namespace Plugin.HttpClient.Reflection.Data
{
	[Serializable]
	public class ApiServiceInfo
	{
		public String RouteUri { get ; }

		public String TypeName { get ; }

		public ApiMethodInfo[] Methods { get; }

		public ApiServiceInfo(TypeReader type, String routeUri)
		{
			_ = type ?? throw new ArgumentNullException(nameof(type));

			this.TypeName = type.FullName;
			this.RouteUri = routeUri;

			List<ApiMethodInfo> methods = new List<ApiMethodInfo>();
			methods.AddRange(this.GetMethods(type));
			this.Methods = methods.Count == 0 ? null : methods.ToArray();
			//this._type = type;
		}

		private IEnumerable<ApiMethodInfo> GetMethods(TypeReader type)
		{
			foreach(MethodReader method in type.GetMembers())
			{
				AttributeReader httpMethodAttribute = method.GetCustomAttributes().FirstOrDefault(a => Constant.Reflection.WebApi.HttpMethods.ContainsKey(a.FullName));
				if(httpMethodAttribute != null)
				{
					String httpMethod = Constant.Reflection.WebApi.HttpMethods[httpMethodAttribute.FullName];
					String methodRouteUrl = (String)httpMethodAttribute.Attribute.FixedArgs.FirstOrDefault(a => a.Type.Type == Cor.ELEMENT_TYPE.STRING)?.Value;
					if(methodRouteUrl == null)
					{
						httpMethodAttribute = method.GetCustomAttributes().First(a => a.FullName == Constant.Reflection.WebApi.MethodRouteAttribute);
						methodRouteUrl = (String)httpMethodAttribute.Attribute.FixedArgs.FirstOrDefault(a => a.Type.Type == Cor.ELEMENT_TYPE.STRING)?.Value
							?? String.Empty;
					}
					yield return new ApiMethodInfo(this, method, methodRouteUrl, httpMethod);
				}
			}
		}
	}
}