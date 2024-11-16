using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta;
using AlphaOmega.Debug.CorDirectory.Meta.Reader;

namespace Plugin.HttpClient.Reflection.Data
{
	[Serializable]
	public class ApiMethodInfo
	{
		public ApiServiceInfo Parent { get; }

		public String RouteUri { get; }

		public ApiTypeInfo[] In { get ; }

		public Dictionary<HttpStatusCode, ApiTypeInfo> Out { get ; }

		public String HttpMethod { get ; }

		public ApiMethodInfo(ApiServiceInfo parent, MethodReader method, String routeUri,String httpMethod)
		{
			_ = method ?? throw new ArgumentNullException(nameof(method));
			this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));

			this.RouteUri = routeUri;
			this.HttpMethod = httpMethod;

			List<ApiTypeInfo> inParams = new List<ApiTypeInfo>();
			inParams.AddRange(this.GetInArguments(method));
			List<ApiTypeInfo> outParams = new List<ApiTypeInfo>();
			outParams.AddRange(this.GetOutParameters(method));

			this.In = inParams.Count == 0 ? null : inParams.ToArray();
			//this.Out = outParams.Count == 0 ? null : outParams.ToArray();
		}

		private IEnumerable<ApiTypeInfo> GetInArguments(MethodReader method)
		{//TODO: Add arguments reading logic
			foreach(var argument in method.GetArguments())
			{
				yield return new ApiTypeInfo(argument, null);
			}
		}

		private IEnumerable<ApiTypeInfo> GetOutParameters(MethodReader method)
		{//TODO: Add aouout attributes reader logic
			yield break;
		}
	}
}
