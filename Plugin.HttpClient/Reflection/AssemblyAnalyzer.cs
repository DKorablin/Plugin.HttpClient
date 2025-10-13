using System;
using System.Collections.Generic;
using System.Linq;
using AlphaOmega.Debug;
using AlphaOmega.Debug.CorDirectory.Meta.Reader;
using AlphaOmega.Debug.CorDirectory.Meta.Tables;
using Plugin.HttpClient.Reflection.Data;

namespace Plugin.HttpClient.Reflection
{
	/// <summary>
	/// Provides lightweight reflection over a compiled .NET assembly (PE file) to discover Web API service types.
	/// Types are considered services when they are decorated with one of the Route Prefix attributes
	/// defined in <see cref="Constant.Reflection.WebApi.RoutePrefixAttributes"/>. For each matching type a
	/// corresponding <see cref="ApiServiceInfo"/> instance is produced that contains method endpoint data.
	/// Uses AlphaOmega.Debug low-level metadata readers instead of loading the assembly into the AppDomain.
	/// </summary>
	internal class AssemblyAnalyzer
	{
		/// <summary>
		/// Scans the specified assembly file for API service endpoint definitions.
		/// </summary>
		/// <param name="assemblyPath">Full path to the target managed assembly (.dll) to analyze.</param>
		/// <returns>
		/// Sequence of <see cref="ApiServiceInfo"/> describing each discovered service (route prefix + methods).
		/// Returns an empty sequence if the assembly contains no metadata or no matching types.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="assemblyPath"/> is null or empty.</exception>
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