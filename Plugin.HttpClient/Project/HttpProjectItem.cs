using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using BrightIdeasSoftware;
using Plugin.HttpClient.UI;

namespace Plugin.HttpClient.Project
{
	[Serializable]
	[DefaultProperty(nameof(Address))]
	[DebuggerDisplay("[{" + nameof(Method) + "}] Address={" + nameof(Address) + "} AddressReal={" + nameof(AddressReal) + "}")]
	public class HttpProjectItem : HttpItem
	{
		private HttpProjectItemCollection _items = new HttpProjectItemCollection();

		[NonSerialized]
		private HttpItemResponse _lastResponse;

		[NonSerialized]
		private NodeStateEnum _image = NodeStateEnum.New;

		#region Properties
		/// <summary>The last test response information</summary>
		internal HttpItemResponse LastResponse
		{
			get => this._lastResponse;
			set => base.SetField(ref this._lastResponse, value, nameof(this.LastResponse));
		}

		internal NodeStateEnum Image
		{
			get => this._image;
			set => base.SetField(ref this._image, value, nameof(this.Image));
		}

		public new String Address
		{
			get => base.Address;
			set
			{
				if(!String.IsNullOrEmpty(value))
					base.Address = this.ApplyTemplate(nameof(this.Address), value, false);
			}
		}

		/// <summary>Formatted url using template</summary>
		[Browsable(false)]
		[OLVColumn("Address", DisplayIndex = 0, IsEditable = true, IsTileViewColumn = true, UseFiltering = false, UseInitialLetterForGroup = false, AutoCompleteEditor = false, WordWrap = false)]
		public String AddressReal
		{
			get => this.ApplyTemplate(nameof(this.AddressReal), base.Address, true);
		}

		[OLVColumn(MinimumWidth = 50, IsEditable = false, UseFiltering = true, FillsFreeSpace = false)]
		public new String Method
		{
			get => this.ApplyTemplate(nameof(this.Method), base.Method, true);
			set => base.Method = this.ApplyTemplate(nameof(this.Method), value, false);
		}

		[OLVColumn(IsEditable = true, UseFiltering = true, IsVisible = false)]
		public new Boolean AllowAutoRedirect
		{
			get => this.ApplyTemplate(nameof(this.AllowAutoRedirect), base.AllowAutoRedirect, true);
			set => base.AllowAutoRedirect = this.ApplyTemplate(nameof(this.AllowAutoRedirect), value, false);
		}

		public new String Data
		{
			get => base.Data;
			set => base.Data = this.ApplyTemplate(nameof(this.Data), value, false);
		}

		[Browsable(false)]
		public String DataReal
		{
			get => this.ApplyTemplate(nameof(this.DataReal), base.Data, true);
		}

		[OLVColumn(IsEditable =false, UseFiltering = true, IsVisible = false)]
		public new Int32 Timeout
		{
			get => this.ApplyTemplate(nameof(this.Timeout), base.Timeout, true);
			set => base.Timeout = this.ApplyTemplate(nameof(this.Timeout), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new Int32 ReadWriteTimeout
		{
			get => this.ApplyTemplate(nameof(this.ReadWriteTimeout), base.ReadWriteTimeout, true);
			set => base.ReadWriteTimeout = this.ApplyTemplate(nameof(this.ReadWriteTimeout), value, false);
		}

		[OLVColumn(IsEditable = true, UseFiltering = true, IsVisible = false)]
		public new Boolean SendChunked
		{
			get => this.ApplyTemplate(nameof(this.SendChunked), base.SendChunked, true);
			set => base.SendChunked = this.ApplyTemplate(nameof(this.SendChunked), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new AuthorizationType AuthorizationType
		{
			get => this.ApplyTemplate(nameof(this.AuthorizationType), base.AuthorizationType, true);
			set => base.AuthorizationType = this.ApplyTemplate(nameof(this.AuthorizationType), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String UserName
		{
			get => this.ApplyTemplate(nameof(this.UserName), base.UserName, true);
			set => base.UserName = this.ApplyTemplate(nameof(this.UserName), value, false);
		}

		public new String Password
		{
			get => this.ApplyTemplate(nameof(this.Password), base.Password, true);
			set => base.Password = this.ApplyTemplate(nameof(this.Password), value, false);
		}

		public new String[] ClientCertificates
		{
			get => this.ApplyTemplate(nameof(this.ClientCertificates), base.ClientCertificates, true);
			set => base.ClientCertificates = this.ApplyTemplate(nameof(this.ClientCertificates), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String ProxyAddress
		{
			get => this.ApplyTemplate(nameof(this.ProxyAddress), base.ProxyAddress, true);
			set => base.ProxyAddress = this.ApplyTemplate(nameof(this.ProxyAddress), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String ProxyDomain
		{
			get => this.ApplyTemplate(nameof(this.ProxyDomain), base.ProxyDomain, true);
			set => base.ProxyDomain = this.ApplyTemplate(nameof(this.ProxyDomain), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String ProxyUserName
		{
			get => this.ApplyTemplate(nameof(this.ProxyUserName), base.ProxyUserName, true);
			set => base.ProxyUserName = this.ApplyTemplate(nameof(this.ProxyUserName), value, false);
		}

		public new String ProxyPassword
		{
			get => this.ApplyTemplate(nameof(this.ProxyPassword), base.ProxyPassword, true);
			set => base.ProxyPassword = this.ApplyTemplate(nameof(this.ProxyPassword), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new Boolean UseDefaultProxyCredentials
		{
			get => this.ApplyTemplate(nameof(this.UseDefaultProxyCredentials), base.UseDefaultProxyCredentials, true);
			set => base.UseDefaultProxyCredentials = this.ApplyTemplate(nameof(this.UseDefaultProxyCredentials), value, false);
		}

		public new String[] ProxyBypassList
		{
			get => this.ApplyTemplate(nameof(this.ProxyBypassList), base.ProxyBypassList, true);
			set => base.ProxyBypassList = this.ApplyTemplate(nameof(this.ProxyBypassList), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String Accept
		{
			get => this.ApplyTemplate(nameof(this.Accept), base.Accept, true);
			set => base.Accept = this.ApplyTemplate(nameof(this.Accept), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String Connection
		{
			get => this.ApplyTemplate(nameof(this.Connection), base.Connection, true);
			set => base.Connection = this.ApplyTemplate(nameof(this.Connection), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String ContentType
		{
			get => this.ApplyTemplate(nameof(this.ContentType), base.ContentType, true);
			set => base.ContentType = this.ApplyTemplate(nameof(this.ContentType), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new Int64? ContentLength
		{
			get => this.ApplyTemplate(nameof(this.ContentLength), base.ContentLength, true);
			set => base.ContentLength = this.ApplyTemplate(nameof(this.ContentLength), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String Expect
		{
			get => this.ApplyTemplate(nameof(this.Expect), base.Expect, true);
			set => base.Expect = this.ApplyTemplate(nameof(this.Expect), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String Referer
		{
			get => this.ApplyTemplate(nameof(this.Referer), base.Referer, true);
			set => base.Referer = this.ApplyTemplate(nameof(this.Referer), value, false);
		}

		public new String[] CustomHeaders
		{
			get => this.ApplyTemplate(nameof(this.CustomHeaders), base.CustomHeaders, true);
			set => base.CustomHeaders = this.ApplyTemplate(nameof(this.CustomHeaders), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String TransferEncoding
		{
			get => this.ApplyTemplate(nameof(this.TransferEncoding), base.TransferEncoding, true);
			set => base.TransferEncoding = this.ApplyTemplate(nameof(this.TransferEncoding), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String UserAgent
		{
			get => this.ApplyTemplate(nameof(this.UserAgent), base.UserAgent, true);
			set => base.UserAgent = this.ApplyTemplate(nameof(this.UserAgent), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new String Host
		{
			get => this.ApplyTemplate(nameof(this.Host), base.Host, true);
			set => base.Host = this.ApplyTemplate(nameof(this.Host), value, false);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, IsVisible = false)]
		public new HttpStatusCode ResponseStatus
		{
			get => this.ApplyTemplate(nameof(this.ResponseStatus), base.ResponseStatus, true);
			set => base.ResponseStatus = this.ApplyTemplate(nameof(this.ResponseStatus), value, false);
		}

		public new String Response
		{
			get => base.Response;
			set => base.Response = this.ApplyTemplate(nameof(this.Response), value, false);
		}

		[Browsable(false)]
		public String ResponseReal
		{
			get => this.ApplyTemplate(nameof(this.ResponseReal), base.Response, true);
		}

		[OLVColumn(IsEditable = false, UseFiltering = true, FillsFreeSpace = false)]
		public new String Description
		{
			get => this.ApplyTemplate(nameof(this.Description), base.Description, true);
			set => base.Description = this.ApplyTemplate(nameof(this.Description), value, false);
		}

		/// <summary>Child requests of remote server for testing</summary>
		[Browsable(false)]
		public HttpProjectItemCollection Items
		{
			get => this._items;
			set => base.SetField(ref this._items, value, nameof(Items));
		}
		#endregion Properties

		#region Methods
		public HttpProjectItem()
		{ }

		internal HttpProjectItem(HttpListenerRequest request)
		{
			this.Address = request.Url.ToString();
			this.Method = request.HttpMethod;
			this.ContentType = request.ContentType;
			this.Accept = request.AcceptTypes == null ? null : String.Join(",", request.AcceptTypes);
			this.UserAgent = request.UserAgent;
			this.ContentLength = request.ContentLength64;
			if(request.UrlReferrer != null)
				this.Referer = request.UrlReferrer.ToString();

			if(request.HasEntityBody)
				using(StreamReader reader = new StreamReader(request.InputStream))
					this.Data = reader.ReadToEnd();

			List<String> headers = new List<String>(request.Headers.Count);
			for(Int32 loop = 0; loop < request.Headers.AllKeys.Length; loop++)
			{
				String key = request.Headers.AllKeys[loop];
				String value = request.Headers[loop];
				headers.Add(key + ": " + value);
			}
			this.CustomHeaders = headers.ToArray();
		}

		internal HttpProjectItem(HttpWebRequest request)
		{
			this.Address = request.RequestUri.ToString();
			this.Method = request.Method;
			this.ContentType = request.ContentType;
			this.Accept = request.Accept;
			this.UserAgent = request.UserAgent;
			this.ContentLength = request.ContentLength;
			this.Referer = request.Referer;

			List<String> headers = new List<String>(request.Headers.Count);
			for(Int32 loop = 0; loop < request.Headers.AllKeys.Length; loop++)
			{
				String key = request.Headers.AllKeys[loop];
				String value = request.Headers[loop];
				headers.Add(key + ": " + value);
			}
			this.CustomHeaders = headers.ToArray();
		}

		internal HttpProjectItem(HttpProjectItem request)
		{
			Dictionary<PropertyInfo, PropertyInfo> properties = HttpProjectItem.GetEqualProperties<HttpProjectItem, HttpProjectItem>();
			foreach(KeyValuePair<PropertyInfo, PropertyInfo> property in properties)
			{
				Object value = property.Key.GetValue(request, null);
				if(value != null)
					property.Value.SetValue(this, value, null);
			}
		}

		/// <summary>Apply or clear template values</summary>
		/// <param name="value">Value ot apply or clear template values</param>
		/// <param name="isApply">
		/// True - Replace found template key to template values
		/// False - Replace template values to template keys
		/// </param>
		/// <returns>Value after replacing template keys or template values</returns>
		private T ApplyTemplate<T>(String propertyName, T value, Boolean isApply)
			=> this.Items.Project != null//Project can be null of transferred from a different project (Drag&Drop)
				? this.Items.Project.Templates.ApplyTemplateV2(propertyName, value, isApply)
				: value;

		private static Dictionary<PropertyInfo, PropertyInfo> GetEqualProperties<T1, T2>()
		{
			PropertyInfo[] T1Properties1 = typeof(T1).GetProperties(BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo[] T2Properties1 = typeof(T2).GetProperties(BindingFlags.Instance | BindingFlags.Public);

			Dictionary<PropertyInfo, PropertyInfo> result = new Dictionary<PropertyInfo, PropertyInfo>();

			foreach(PropertyInfo property in T1Properties1)
				if(property.IsDefined(typeof(CategoryAttribute)) && property.CanRead && property.GetIndexParameters().Length == 0)//Hack to check if display attribute exists in UI
					foreach(PropertyInfo requestProperty in T2Properties1)
						if(requestProperty.Name == property.Name && requestProperty.CanWrite && requestProperty.PropertyType == property.PropertyType && requestProperty.GetIndexParameters().Length == 0)
						{
							result.Add(property, requestProperty);
							break;
						}

			return result;
		}

		/// <summary>Compare 2 instances</summary>
		/// <param name="item">The instance of the element to compare with the current one</param>
		/// <param name="relativeOnly">Compare only the relative part of the address, without the host</param>
		/// <returns>Instances are equal</returns>
		public Boolean Equals(HttpProjectItem item, Boolean relativeOnly = false)
		{
			if(this.Address != null)
				if(relativeOnly)
				{
					if(new Uri(this.AddressReal).PathAndQuery != new Uri(item.AddressReal).PathAndQuery)
						return false;//Corrected links are used here, otherwise Uri will not be parsed
				} else if(this.Address != item.Address)
					return false;
			if(this.Method != null && this.Method != item.Method)
				return false;
			if(this.ContentType != null && this.ContentType != item.ContentType)
				return false;
			if(this.Accept != null && this.Accept != item.Accept)
				return false;
			if(this.Data != null && this.Data != item.Data)
				return false;

			return true;
		}
		#endregion Methods
	}
}