using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.UI;

namespace Plugin.HttpClient
{
	public class PluginSettings : INotifyPropertyChanged
	{
		internal static class Constants
		{
			public const String ProjectFileKey = "HttpProject." + Constant.Project.Extensions.Xml;
			public const String TemplateIpAddr = "%ipAddress%";
			public const String ServerUrl = "http://" + TemplateIpAddr + ":8280";
			public const String Project = "Project";
		}

		private readonly PluginWindows _plugin;

		private Int32 _testTimeout = 0;
		private Boolean _serverEnabled = false;
		private String _serverUrl = Constants.ServerUrl;
		private Boolean _serverSearchRelativeUrl = true;
		private SecurityProtocolType _securityProtocol = ServicePointManager.SecurityProtocol;
		private Boolean _stopOnError = false;
		private Boolean _showResponse = false;
		private Boolean _wordWrap = false;
		private Byte[] _listViewState = null;
		private Boolean _certificateValidation = false;
		private static IPAddress _HostAddress;

		[Category("Test")]
		[DisplayName("Stop on Error")]
		[Description("Stop testing when an error occurs in one of the requests")]
		[DefaultValue(false)]
		public Boolean StopOnError
		{
			get => this._stopOnError;
			set => this.SetField(ref this._stopOnError, value, nameof(this.StopOnError));
		}

		[Category("Test")]
		[DisplayName("Test Timeout (sec)")]
		[Description("Timeout in seconds between tests for thread tests. Valid values between 0 and 255")]
		[DefaultValue(0)]
		public Int32 TestTimeout
		{
			get => this._testTimeout;
			set => this.SetField(ref this._testTimeout, value < 0 || value > Byte.MaxValue ? 0 : value, nameof(this.TestTimeout));
		}

		[Category("UI")]
		[DisplayName("Show Response")]
		[Description("Show the response tab when selecting a node in the requests tree")]
		[DefaultValue(false)]
		public Boolean ShowResponse
		{
			get => this._showResponse;
			set => this.SetField(ref this._showResponse, value, nameof(this.ShowResponse));
		}

		[Category("UI")]
		[DefaultValue(false)]
		[DisplayName("Word Wrap")]
		[Description("White space word wrap in the response tab")]
		public Boolean WordWrap
		{
			get => this._wordWrap;
			set => this.SetField(ref this._wordWrap, value, nameof(this.WordWrap));
		}

		[Category("UI")]
		[Browsable(false)]
		public Byte[] ListViewState
		{//TODO: We can't process this event in other windows because it will fire when each window is closed. Maybe we need to store window state in project file for each project...
			get => this._listViewState;
			set => this.SetField(ref this._listViewState, value, nameof(this.ListViewState));
		}

		[Category("Transport")]
		[DisplayName("Security Protocol")]
		[Description("This property selects the version of the Secure Sockets Layer (SSL) or Transport Layer Security (TLS) protocol to use for new connections that use the Secure Hypertext Transfer Protocol (HTTPS) scheme only; existing connections are not changed.")]
		[Editor(typeof(ColumnEditorTyped<SecurityProtocolType>), typeof(UITypeEditor))]
		[DefaultValue(SecurityProtocolType.Tls)]
		public SecurityProtocolType SecurityProtocol
		{
			get => this._securityProtocol;
			set
			{
				if(value != 0)
				{
					this.SetField(ref this._securityProtocol, value, nameof(this.SecurityProtocol));
					ServicePointManager.SecurityProtocol = value;
				}
			}
		}

		[Category("Transport")]
		[DisplayName("Ignore Server Certificate")]
		[Description("Ignore certificate validation errors and log them to trace")]
		[DefaultValue(false)]
		public Boolean CertificateValidation
		{
			get => this._certificateValidation;
			set
			{
				this.SetField(ref this._certificateValidation, value, nameof(this.CertificateValidation));
				this._plugin.RegisterForCertificateValidation(value);
			}
		}

		[Category("Server")]
		[DisplayName("Enabled")]
		[DefaultValue(false)]
		[Description("Turn on the test server and start it at the address specified in Server.Url")]
		public Boolean ServerEnabled
		{
			get => this._serverEnabled;
			set => this.SetField(ref this._serverEnabled, value, nameof(this.ServerEnabled));
		}

		[Category("Server")]
		[DisplayName("Url")]
		[DefaultValue(Constants.ServerUrl)]
		[Description("Test host for project items. Use " + Constants.TemplateIpAddr + " template for Dns.GetHostName().\r\nUse \"Search relative Url's\" to use only path and query while searching template to ignore server protocol, IP address and port.\r\nFor SSL don't forget to specify certificate at:\r\nnetsh http add sslcert ipport=[ipAddress]:8280 certhash=[thumbprint] appid={fdddc14d-5911-47bf-92dd-815a383c3f27}")]
		public String ServerUrl
		{
			get => this._serverUrl;
			set => this.SetField(ref this._serverUrl,
					String.IsNullOrEmpty(value) ? Constants.ServerUrl : value,
					nameof(this.ServerUrl));
		}

		[Category("Server")]
		[DisplayName("Search relative Url's")]
		[DefaultValue(true)]
		[Description("Search only by path and query and ignore protocol, IP address and port when searching template for response")]
		public Boolean ServerSearchRelativeUrl
		{
			get => this._serverSearchRelativeUrl;
			set => this.SetField(ref this._serverSearchRelativeUrl, value, nameof(this.ServerSearchRelativeUrl));
		}

		/// <summary>Current computer host address</summary>
		private static IPAddress HostAddress
		{
			get
			{
				if(_HostAddress == null)
				{
					IPHostEntry ip = Dns.GetHostEntry(Dns.GetHostName());
					_HostAddress = Array.Find(ip.AddressList, (IPAddress addr) => { return addr.AddressFamily == AddressFamily.InterNetwork; });
				}
				return _HostAddress;
			}
		}

		internal PluginSettings(PluginWindows plugin)
			=> this._plugin = plugin;

		/// <summary>Gets host with custom formatting</summary>
		/// <returns>The host with custom formatting</returns>
		internal String GetServerUrl()
		{
			String result = this.ServerUrl;

			return result.Contains(Constants.TemplateIpAddr)
				? result.Replace(Constants.TemplateIpAddr, PluginSettings.HostAddress.ToString())
				: result;
		}

		/// <summary>Internal stored project</summary>
		internal HttpProject LoadProject()
		{//Don't cache project here. Otherwise unsaved changes will be prsist between local windows while application is opened
			using(Stream stream = this._plugin.HostWindows.Plugins.Settings(this._plugin).LoadAssemblyBlob(PluginSettings.Constants.ProjectFileKey))
				return stream == null
					? null
					: HttpProject.LoadAsXml(stream);
		}

		internal void SaveProject(HttpProject project)
		{
			if(project == null || (project.Items.Count == 0 && project.TemplatesCollection.Count == 0))
				this._plugin.HostWindows.Plugins.Settings(this._plugin).RemoveAssemblyBlob(PluginSettings.Constants.ProjectFileKey);
			else
				using(MemoryStream stream = new MemoryStream())
				{
					project.SaveAsXml(stream);
					this._plugin.HostWindows.Plugins.Settings(this._plugin).SaveAssemblyBlob(PluginSettings.Constants.ProjectFileKey, stream);
				}

			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Constants.Project));
		}

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private Boolean SetField<T>(ref T field, T value, String propertyName)
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