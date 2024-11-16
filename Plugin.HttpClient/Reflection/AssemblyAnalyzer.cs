using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta;
using AlphaOmega.Debug.CorDirectory.Meta.Reader;
using AlphaOmega.Debug.CorDirectory.Meta.Tables;
using Plugin.HttpClient.Reflection.Data;

namespace Plugin.HttpClient.Reflection
{
	internal class AssemblyAnalyzer
	{
		public IEnumerable<ApiServiceInfo> FindEndpoints(String assemblyPath)
		{
			if(String.IsNullOrEmpty(assemblyPath))
				throw new ArgumentNullException(nameof(assemblyPath));

			using(PEFile reader = new PEFile(assemblyPath, StreamLoader.FromFile(assemblyPath)))
			{
				var metadata = reader.ComDescriptor.MetaData;
				if(metadata.IsEmpty)
					yield break;

				var tables = metadata.StreamTables;
				foreach(TypeDefRow typeDef in tables.TypeDef)
				{
					TypeReader type = new TypeReader(typeDef);
					AttributeReader route = type.GetAttributes().FirstOrDefault(a => Constant.Reflection.WebApi.RoutePrefixAttributes.Contains(a.FullName));
					if(route != null)
					{
						var routeArg = route.Attribute.FixedArgs.First(a => a.Type.Type == Cor.ELEMENT_TYPE.STRING);

						ApiServiceInfo item = new ApiServiceInfo(type, (String)routeArg.Value);
						yield return item;
					}
				}
			}
		}
	}
}