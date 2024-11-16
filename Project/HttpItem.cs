using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Plugin.HttpClient.UI;

namespace Plugin.HttpClient.Project
{
	[Serializable]
	[DefaultProperty(nameof(Address))]
	public class HttpItem : INotifyPropertyChanged
	{
		#region Fields
		private String _address;
		private String _method = "GET";
		private String _accept;
		private String _connection;
		private String _contentType;
		private String _expect;
		private String _referer;
		private String _transferEncoding;
		private String _userAgent;
		private String _data;

		private String[] _customHeaders;

		[OptionalField] private AuthorizationType _authorizationType = AuthorizationType.None;
		private String _userName;
		private String _password;

		private Boolean _allowAutoRedirect = false;

		private Int32 _timeout = 100000;
		private Int32 _readWriteTimeout = 300000;

		private Int64? _contentLength;

		private Boolean _sendChunked = false;

		private String[] _clientCertificates;

		private String _proxyAddress;

		private Boolean _useDefaultProxyCredentials = false;

		private String[] _proxyBypassList;
		private String _proxyUserName;
		private String _proxyPassword;
		private String _proxyDomain;

		private String _host;

		private HttpStatusCode _resultStatus = HttpStatusCode.OK;
		private String _response;

		[OptionalField] private String _description;
		#endregion Fields

		#region Properties
		#region Request
		[Category("Request")]
		[Description("Gets the Uniform Resource Identifier (URI) of the Internet resource that actually responds to the request")]
		public String Address
		{
			get => this._address;
			set => this.SetField(ref this._address, String.IsNullOrEmpty(value) ? null : value, nameof(Address));
		}

		[Category("Request")]
		[Description("Gets or sets a request HTTP method")]
		[DefaultValue("GET")]
		[Editor(typeof(HttpMethodEditor), typeof(UITypeEditor))]
		public String Method
		{
			get => this._method ?? "GET";
			set => this.SetField(ref this._method, String.IsNullOrEmpty(value) || value.Equals("GET", StringComparison.InvariantCultureIgnoreCase) ? null : value, nameof(Method));
		}

		[Category("Request")]
		[DisplayName("Allow auto redirect")]
		[Description("whether the request should follow redirection responses")]
		[DefaultValue(false)]
		public Boolean AllowAutoRedirect
		{
			get => this._allowAutoRedirect;
			set => this.SetField(ref this._allowAutoRedirect, value, nameof(AllowAutoRedirect));
		}

		[Category("Request")]
		[Description("Gets or sets payload request data")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Data
		{
			get => this._data;
			set => this.SetField(ref this._data, String.IsNullOrEmpty(value) ? null : value, nameof(Data));
		}

		[Category("Request")]
		[Description("The number of milliseconds to wait before the request times out. The default value is 100,000 milliseconds (100 seconds).")]
		[DefaultValue(100000)]
		public Int32 Timeout
		{
			get => this._timeout;
			set
			{
				if(value < 0 && value != System.Threading.Timeout.Infinite)
					throw new ArgumentOutOfRangeException("The value specified is less than zero and is not Infinite.");
				this.SetField(ref this._timeout, value, nameof(Timeout));
			}
		}

		[Category("Request")]
		[Description("Time-out in milliseconds when writing to or reading from a stream. The default value is 300,000 milliseconds (5 minutes).")]
		[DefaultValue(300000)]
		public  Int32 ReadWriteTimeout
		{
			get => this._readWriteTimeout;
			set
			{
				if(value <= 0 && value != System.Threading.Timeout.Infinite)
					throw new ArgumentOutOfRangeException("The value specified for a set operation is less than or equal to zero and is not equal to Infinite");
				this.SetField(ref this._readWriteTimeout, value, nameof(ReadWriteTimeout));
			}
		}

		[Category("Request")]
		[Description("Whether to send data in segments to the Internet resource")]
		[DefaultValue(false)]
		public Boolean SendChunked
		{
			get => this._sendChunked;
			set => this.SetField(ref this._sendChunked, value, nameof(SendChunked));
		}
		#endregion Request

		#region Credentials
		[Category("Credentials")]
		[DisplayName("Authorization type")]
		[DefaultValue(AuthorizationType.None)]
		[Description("What type of authorization to use for transfer credentials")]
		public AuthorizationType AuthorizationType
		{
			get => this._authorizationType;
			set => this.SetField(ref this._authorizationType, Enum.IsDefined(typeof(AuthorizationType), value) ? value : AuthorizationType.None, nameof(AuthorizationType));
		}

		[Category("Credentials")]
		[DisplayName("User Name")]
		[Description("Network user name to access remote resource")]
		public String UserName
		{
			get => this._userName;
			set => this.SetField(ref this._userName, String.IsNullOrEmpty(value) ? null : value, nameof(UserName));
		}

		[Category("Credentials")]
		//[PasswordPropertyText(true)]
		[Description("Network passwor to access remote resource")]
		public String Password
		{
			get => this._password;
			set => this.SetField(ref this._password, String.IsNullOrEmpty(value) ? null : value, nameof(Password));
		}

		[Category("Credentials")]
		[DisplayName("Client Certificates")]
		[Description("Path to X509 client security certificates that are associated with this request")]
		public String[] ClientCertificates
		{
			get => this._clientCertificates;
			set
			{
				if(value == null || value.Length == 0)
					this.SetField(ref this._clientCertificates, null, nameof(ClientCertificates));
				else
				{
					foreach(String path in value)
						if(!File.Exists(path))
							throw new FileNotFoundException("Certificate file not found", path);

					this.SetField(ref this._clientCertificates, value, nameof(ClientCertificates));
				}
			}
		}
		#endregion Credentials

		#region Proxy
		[Category("Proxy")]
		[DisplayName("Proxy Address")]
		[Description("A Uri instance that contains the address of the proxy server")]
		public String ProxyAddress
		{
			get => this._proxyAddress;
			set => this.SetField(ref this._proxyAddress, String.IsNullOrEmpty(value) ? null : value, nameof(ProxyAddress));
		}

		[Category("Proxy")]
		[DisplayName("Domain")]
		[Description("The name of the domain associated with the credentials")]
		public String ProxyDomain
		{
			get => this._proxyDomain;
			set => this.SetField(ref this._proxyDomain, String.IsNullOrEmpty(value) ? null : value, nameof(ProxyDomain));
		}

		[Category("Proxy")]
		[DisplayName("Login")]
		[Description("Proxy user name to access remote resource")]
		public String ProxyUserName
		{
			get => this._proxyUserName;
			set => this.SetField(ref this._proxyUserName, String.IsNullOrEmpty(value) ? null : value, nameof(ProxyUserName));
		}

		[Category("Proxy")]
		[DisplayName("Password")]
		[Description("Proxy password to access remote resource")]
		public String ProxyPassword
		{
			get => this._proxyPassword;
			set => this.SetField(ref this._proxyPassword, String.IsNullOrEmpty(value) ? null : value, nameof(ProxyPassword));
		}

		[Category("Proxy")]
		[DefaultValue(false)]
		[DisplayName("Default Credentials")]
		[Description("Set this property to true when requests made by this HttpWebRequest object should, if requested by the server, be authenticated using the credentials of the currently logged on user. For client applications, this is the desired behavior in most scenarios. For middle-tier applications, such as ASP.NET applications, instead of using this property, you would typically set the Credentials property to the credentials of the client on whose behalf the request is made.")]
		public Boolean UseDefaultProxyCredentials
		{
			get => this._useDefaultProxyCredentials;
			set => this.SetField(ref this._useDefaultProxyCredentials, value, nameof(UseDefaultProxyCredentials));
		}

		[Category("Proxy")]
		[DisplayName("BypassList")]
		[Description("An array that contains a list of regular expressions that describe URIs that do not use the proxy server when accessed")]
		public String[] ProxyBypassList
		{
			get => this._proxyBypassList;
			set => this.SetField(ref this._proxyBypassList, value == null || value.Length == 0 ? null : value, nameof(ProxyBypassList));
		}
		#endregion Proxy

		#region Headers
		[Category("Headers")]
		[Description("Gets or sets the value of the Accept HTTP header")]
		public String Accept
		{
			get => this._accept;
			set => this.SetField(ref this._accept, String.IsNullOrEmpty(value) ? null : value, nameof(Accept));
		}

		[Category("Headers")]
		[Description("Gets or sets the value of the Connection HTTP header")]
		[Editor(typeof(HttpConnectionEditor),typeof(UITypeEditor))]
		public String Connection
		{
			get => this._connection;
			set => this.SetField(ref this._connection, String.IsNullOrEmpty(value) ? null : value, nameof(Connection));
		}

		[Category("Headers")]
		[DisplayName("Content-Type")]
		[Description("Gets or sets the value of the Content-type HTTP header")]
		[Editor(typeof(HttpContentTypeEditor),typeof(UITypeEditor))]
		public String ContentType
		{
			get => this._contentType;
			set => this.SetField(ref this._contentType, String.IsNullOrEmpty(value) ? null : value, nameof(ContentType));
		}

		[Category("Headers")]
		[DisplayName("Content-Length")]
		[Description("Gets or sets the Content-length HTTP header")]
		//[DefaultValue(-1L)]
		public Int64? ContentLength
		{//We should check this property manually because HttpWebRequest uses -1 to identify field as null value but if we try to set -1 as value we will receive exception
			get => this._contentLength;
			set => this.SetField(ref this._contentLength, value, nameof(ContentLength));
		}

		[Category("Headers")]
		[Description("Gets or sets the value of the Expect HTTP header")]
		[Editor(typeof(HttpExpectHeaderEditor),typeof(UITypeEditor))]
		public String Expect
		{
			get => this._expect;
			set => this.SetField(ref this._expect, String.IsNullOrEmpty(value) ? null : value, nameof(Expect));
		}

		[Category("Headers")]
		[Description("Gets or sets the value of the Referer HTTP header")]
		public String Referer
		{
			get => this._referer;
			set => this.SetField(ref this._referer, String.IsNullOrEmpty(value) ? null : value, nameof(Referer));
		}

		[Category("Headers")]
		[DisplayName("Custom-Headers")]
		[Description("Custom HTTP headers")]
		public String[] CustomHeaders
		{
			get => this._customHeaders;
			set => this.SetField(ref this._customHeaders, value == null || value.Length == 0 ? null : value, nameof(CustomHeaders));
		}

		[Category("Headers")]
		[DisplayName("Transfer-Encoding")]
		[Description("Gets or sets the value of the Transfer-encoding HTTP header")]
		[Editor(typeof(HttpTransferEncodingEditor),typeof(UITypeEditor))]
		public String TransferEncoding
		{
			get => this._transferEncoding;
			set => this.SetField(ref this._transferEncoding, String.IsNullOrEmpty(value) ? null : value, nameof(TransferEncoding));
		}

		[Category("Headers")]
		[DisplayName("User-Agent")]
		[Description("Gets or sets the value of the User-agent HTTP header")]
		[Editor(typeof(HttpUserAgentEditor), typeof(UITypeEditor))]
		public String UserAgent
		{//TODO: The value for this property is stored in WebHeaderCollection. If WebHeaderCollection is set, the property value is lost
			get => this._userAgent;
			set => this.SetField(ref this._userAgent, String.IsNullOrEmpty(value) ? null : value, nameof(UserAgent));
		}

		[Category("Headers")]
		[DisplayName("Host")]
		[Description("The Host request header specifies the host and port number of the server to which the request is being sent.\r\nIf no port is included, the default port for the service requested is implied (e.g., 443 for an HTTPS URL, and 80 for an HTTP URL)")]
		public String Host
		{
			get => this._host;
			set => this.SetField(ref this._host, String.IsNullOrEmpty(value) ? null : value, nameof(Host));
		}
		#endregion Headers

		#region Response
		[Category("Response")]
		[DisplayName("Status")]
		[Description("Expected response status code")]
		[DefaultValue(HttpStatusCode.OK)]
		//[Editor(typeof(ColumnEditor<HttpStatusCode>), typeof(UITypeEditor))]
		public HttpStatusCode ResponseStatus
		{
			get => this._resultStatus;
			set => this.SetField(ref this._resultStatus, value, nameof(ResponseStatus));
		}

		[Category("Response")]
		[Description("Expected response from server")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Response
		{
			get => this._response;
			set => this.SetField(ref this._response, value, nameof(Response));
		}
		#endregion Response

		[Category("UI")]
		[Description("Custom user description field")]
		[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
		public String Description
		{
			get => this._description;
			set => this.SetField(ref this._description, String.IsNullOrEmpty(value) ? null : value, nameof(Description));
		}
		#endregion Properties

		#region INotifyPropertyChanged
		[field: NonSerialized]
		public event PropertyChangedEventHandler PropertyChanged;
		protected Boolean SetField<T>(ref T field, T value, String propertyName)
		{
			if(EqualityComparer<T>.Default.Equals(field, value))
				return false;

			field = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			return true;
		}
		#endregion INotifyPropertyChanged
	}
}