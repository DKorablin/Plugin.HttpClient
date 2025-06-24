using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.Permissions;
using System.Text;
using System.Xml.Serialization;
using Plugin.HttpClient.Reflection;
using static Plugin.HttpClient.Constant;

namespace Plugin.HttpClient.Project
{
	/// <summary>Http tests project instance</summary>
	[Serializable]
	public class HttpProject : ISerializable
	{//For serialization to work properly this and all related classes has to be public (Try to save project to XML or JSON format)
		#region Serialization
		private sealed class UniversalDeserializationBinder : SerializationBinder
		{
			public override Type BindToType(String assemblyName, String typeName)//Applying object mapping, excluding versioning
				=> Type.GetType(typeName);
		}

		private static BinaryFormatter BinSerializer
			=> new BinaryFormatter() { AssemblyFormat = FormatterAssemblyStyle.Simple, TypeFormat = FormatterTypeStyle.TypesWhenNeeded, Binder = new UniversalDeserializationBinder(), };

		private static XmlSerializer XmlSerializer
			=> new XmlSerializer(typeof(HttpProject), new Type[] { typeof(HttpItem) });

		private static DataContractSerializer XmlSerializer2
			=> new DataContractSerializer(typeof(HttpProject), new Type[] { typeof(KeyValuePair<String, TemplateItem[]>[]), typeof(HttpProjectItemCollection) });//HACK: .ctor difference between .NET 3.5 and .NET 5+

		private static DataContractJsonSerializer JsonSerializer
			=> new DataContractJsonSerializer(typeof(HttpProject), new Type[] { typeof(KeyValuePair<String, TemplateItem[]>[]), typeof(HttpProjectItemCollection) }, Int32.MaxValue, true, null, true);
		#endregion Serialization

		[NonSerialized]
		private HttpProjectItemCollection _items;

		[NonSerialized]
		private TemplateEngine _templates;

		[NonSerialized]
		private String _selectedTemplate;

		[NonSerialized]
		private Dictionary<String, TemplateItem[]> _templatesCollection;

		#region Properties
		/// <summary>Remote server request elements</summary>
		[XmlArray]
		public HttpProjectItemCollection Items
			=> this._items ?? (this._items = new HttpProjectItemCollection(this));

		internal TemplateEngine Templates
			=> this._templates ?? (this._templates = new TemplateEngine(this));

		/// <summary>Selected template name for <see cref="TemplatesCollection"/> dictionary</summary>
		public String SelectedTemplate
		{
			get => this._selectedTemplate;
			set
			{
				this._selectedTemplate = value ?? Constant.Project.DefaultTemplateName;
				this.Templates.SelectedTemplateValues = null;
			}
		}

		/// <summary>Templates collection, which are used to replace items while testing</summary>
		[XmlAnyElement]
		//[XmlArray]
		public Dictionary<String, TemplateItem[]> TemplatesCollection
		{
			get => this._templatesCollection;
			set
			{
				if(value == null || value.Count == 0)
					this._templatesCollection.Clear();
				else
					this._templatesCollection = value;
			}
		}
		#endregion Propeties

		public HttpProject()
		{
			this._selectedTemplate = Constant.Project.DefaultTemplateName;
			//this._templates = new List<TemplateItem>();
			this._templatesCollection = new Dictionary<String, TemplateItem[]>()
			{
				{ Constant.Project.DefaultTemplateName, new TemplateItem[]{ } },
			};

		}

		/// <summary>Serialization constructor</summary>
		/// <param name="info">Serialization Information</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		private HttpProject(SerializationInfo info, StreamingContext context)
			: this()
		{
			foreach(SerializationEntry entry in info)
			{
				switch(entry.Name)
				{
				case "Items":
				case "I":
					if(entry.Value is HttpProjectItemCollection collection)
					{
						this._items = collection;
						this._items.Project = this;
					} else if(entry.Value is Object[] items)
					{
						foreach(HttpProjectItem item in items)
							this.Items.Add(item);
					}
					break;
				case "T":
					if(entry.Value is TemplateItem[] templateItems)
					{
						this._templatesCollection = new Dictionary<String, TemplateItem[]>()
						{
							{ Constant.Project.DefaultTemplateName, templateItems },
						};
					}
					break;
				case "T1":
					this._templatesCollection = entry.Value is Dictionary<String, TemplateItem[]> dictionary
						? dictionary
						: throw new NotSupportedException();
					break;
				case "T2":
					if(entry.Value is Array array)
					{
						this._templatesCollection = new Dictionary<String, TemplateItem[]>();
						foreach(KeyValuePair<String, TemplateItem[]> keyValue in array)
							this._templatesCollection.Add(keyValue.Key, keyValue.Value);
					}
					break;
				case "ST":
					this._selectedTemplate = entry.Value as String ?? Constant.Project.DefaultTemplateName;
					break;
				default:
					throw new NotSupportedException($"Entry {entry.Name} not supported");
				}
			}
		}

		/// <summary>Write to a serialized stream the elements of a collection under a specific identifier</summary>
		/// <param name="info">Serialization Information</param>
		/// <param name="context">Stream</param>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(this._items != null && this._items.Count > 0)
				info.AddValue("I", this._items);
			if(this._selectedTemplate != Constant.Project.DefaultTemplateName)
				info.AddValue("ST", this._selectedTemplate);
			if(this._templatesCollection != null && this._templatesCollection.Count > 0)
			{
				//info.AddValue("T1", this._templatesCollection);
				info.AddValue("T2", this._templatesCollection.ToArray());
			}
		}

		/// <summary>Search project items by address and HTTP method</summary>
		/// <param name="relativeUrl">Absolute address used to find items in the project</param>
		/// <param name="method">HTTP method used</param>
		/// <returns>List of project items satisfying a given criteria</returns>
		/// <exception cref="ArgumentNullException">address and method can't be null or empty string</exception>
		private IEnumerable<HttpProjectItem> Find(String relativeUrl, String method)
		{
			if(String.IsNullOrEmpty(relativeUrl))
				throw new ArgumentNullException(nameof(relativeUrl));
			if(String.IsNullOrEmpty(method))
				throw new ArgumentNullException(nameof(method));

			return this.Items.Find(p => p.Address.EndsWith(relativeUrl) && p.Method == method);
		}

		#region Save/Load
		public void Save(String filePath)
		{
			String ext = Path.GetExtension(filePath).ToLowerInvariant().TrimStart('.');
			using(FileStream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				switch(ext)
				{
				case Constant.Project.Extensions.Binary:
				default://Default save format
					this.SaveAsBin(stream);
					break;
				case Constant.Project.Extensions.Json:
					this.SaveAsJson(stream);
					break;
				case Constant.Project.Extensions.Xml:
					this.SaveAsXml(stream);
					break;
				}
			}
		}

		/// <summary>Loads a file from file system</summary>
		/// <param name="filePath">Path to the file to load</param>
		/// <returns>Project with loaded contents</returns>
		/// <exception cref="ArgumentNullException">filePath is required</exception>
		/// <exception cref="FileNotFoundException">filePath not found</exception>
		public static HttpProject Load(String filePath)
		{
			if(String.IsNullOrEmpty(filePath))
				throw new ArgumentNullException(nameof(filePath));
			if(!File.Exists(filePath))
				throw new FileNotFoundException("Project file not found", filePath);

			String ext = Path.GetExtension(filePath).ToLowerInvariant().TrimStart('.');
			using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				switch(ext)
				{
				case Constant.Project.Extensions.Binary:
				default://Default save format
					return HttpProject.LoadAsBin(stream);
				case Constant.Project.Extensions.Json:
					return HttpProject.LoadAsJson(stream);
				case Constant.Project.Extensions.Xml:
					return HttpProject.LoadAsXml(stream);
				}
		}

		/// <summary>Import project from external file to current project</summary>
		/// <param name="projectFilePath">Path to project to import</param>
		/// <param name="ownerItem">The parent item that is used as the owner for adding items (If none specified it will be added to the root)</param>
		/// <returns>True if something added</returns>
		public Boolean ImportProject(String projectFilePath, HttpProjectItem ownerItem = null)
		{
			if(String.IsNullOrEmpty(projectFilePath))
				throw new ArgumentNullException(nameof(projectFilePath));
			if(!File.Exists(projectFilePath))
				throw new FileNotFoundException($"{projectFilePath} does not exist", projectFilePath);

			Boolean isItemsAdded = false;
			HttpProjectItemCollection firstItem = ownerItem?.Items
				?? this.Items;

			HttpProject importingProject = HttpProject.Load(projectFilePath);
			foreach(HttpProjectItem item in importingProject.Items)
			{
				if(!firstItem.Any(p => p.Equals(item)))
				{
					isItemsAdded = true;
					firstItem.Add(item);
				}
			}

			Boolean isTemplatesAdded = false;
			foreach(var kv in importingProject.TemplatesCollection)
			{
				if(this.TemplatesCollection.ContainsKey(kv.Key))
				{
					TemplateItem[] existingItems = this.TemplatesCollection[kv.Key];
					foreach(TemplateItem item in kv.Value)
					{
						if(!Array.Exists(existingItems, p => p.Key == item.Key))
						{
							isTemplatesAdded = true;
							Array.Resize(ref existingItems, existingItems.Length + 1);
							existingItems[existingItems.Length - 1] = item;
						}
					}

					if(isTemplatesAdded)
						this.TemplatesCollection[kv.Key] = existingItems;
				} else
				{
					this.TemplatesCollection.Add(kv.Key, kv.Value);
					isTemplatesAdded = true;
				}
			}

			return isItemsAdded || isTemplatesAdded;
		}

		/// <summary>Import WebAPI assembly file into current http project and skip endpoints that already exists in the current project.</summary>
		/// <param name="serverUrl">Host url</param>
		/// <param name="assemblyFilePath">Path to assembly to import</param>
		/// <param name="ownerItem">Parent item that used as an owner for added items</param>
		/// <exception cref="ArgumentNullException">Path to assembly required</exception>
		/// <exception cref="FileNotFoundException">Assembly not found in the specified path</exception>
		/// <returns>Items imported successfully or nothing found</returns>
		public Boolean ImportAssembly(String serverUrl, String assemblyFilePath, HttpProjectItem ownerItem = null)
		{
			if(String.IsNullOrEmpty(assemblyFilePath))
				throw new ArgumentNullException(nameof(assemblyFilePath));
			if(!File.Exists(assemblyFilePath))
				throw new FileNotFoundException($"{assemblyFilePath} does not exist", assemblyFilePath);

			Boolean result = false;
			AssemblyAnalyzer analyzer = new AssemblyAnalyzer();
			foreach(var route in analyzer.FindEndpoints(assemblyFilePath))
			{
				HttpProjectItem firstItem = null;
				foreach(var endpoint in route.Methods)
				{
					String relativeAddress = String.Join("/", new String[] { route.RouteUri, endpoint.RouteUri });
					String absoluteAddress = String.Join("/", new String[] { serverUrl, relativeAddress });
					if(this.Find(relativeAddress, endpoint.HttpMethod).Any())
						continue;

					HttpProjectItem item = new HttpProjectItem()
					{
						Address = absoluteAddress,
						Method = endpoint.HttpMethod,
					};

					if(firstItem == null)
						firstItem = item;
					else
						firstItem.Items.Add(item);
				}

				if(firstItem != null)//Item already added or nothing found
				{
					if(ownerItem != null)
						ownerItem.Items.Add(firstItem);
					else
						this.Items.Add(firstItem);

					result = true;
				}
			}

			return result;
		}

		public void SaveAsBin(Stream stream)
		{
			_ = stream ?? throw new ArgumentNullException(nameof(stream));

			BinaryFormatter formatter = HttpProject.BinSerializer;
			formatter.Serialize(stream, this);
		}

		public void SaveAsJson(Stream stream)
		{
			_ = stream ?? throw new ArgumentNullException(nameof(stream));

			DataContractJsonSerializer serializer = HttpProject.JsonSerializer;
			serializer.WriteObject(stream, this);
		}

		public void SaveAsXml(Stream stream)
		{
			_ = stream ?? throw new ArgumentNullException(nameof(stream));

			//HttpProject.XmlSerializer.Serialize(stream, this);
			DataContractSerializer serializer = HttpProject.XmlSerializer2;
			serializer.WriteObject(stream, this);
		}

		public static HttpProject LoadAsJson(Stream stream)
		{
			_ = stream ?? throw new ArgumentNullException(nameof(stream));

			DataContractJsonSerializer serializer = HttpProject.JsonSerializer;
			HttpProject result;
			try
			{
				result = (HttpProject)serializer.ReadObject(stream);
			} catch(SerializationException exc)
			{
				if(stream.CanSeek)
					stream.Position = 0;
				using(StreamReader reader = new StreamReader(stream))
					exc.Data.Add(nameof(Stream), reader.ReadToEnd());
				throw;
			}
			return result;
		}

		public static HttpProject LoadAsBin(Stream stream)
		{
			_ = stream ?? throw new ArgumentNullException(nameof(stream));

			BinaryFormatter formatter = HttpProject.BinSerializer;
			HttpProject result = (HttpProject)formatter.Deserialize(stream);
			return result;
		}

		public static HttpProject LoadAsXml(Stream stream)
		{
			_ = stream ?? throw new ArgumentNullException(nameof(stream));

			//HttpProject result = (HttpProject)HttpProject.XmlSerializer.Deserialize(stream);
			//return result;
			DataContractSerializer serializer = HttpProject.XmlSerializer2;
			HttpProject result;
			try
			{
				result = (HttpProject)serializer.ReadObject(stream);
			} catch(SerializationException exc)
			{
				if(stream.CanSeek)
					stream.Position = 0;
				using(StreamReader reader = new StreamReader(stream))
					exc.Data.Add(nameof(Stream), reader.ReadToEnd());
				throw;
			}
			return result;
		}
		#endregion Save/Load
	}
}