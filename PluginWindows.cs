using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Plugin.HttpClient.Events;
using Plugin.HttpClient.History;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.Server;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.HttpClient
{
	public class PluginWindows : IPlugin, IPluginSettings<PluginSettings>
	{
		#region Fields
		private TraceSource _trace;
		private PluginSettings _settings;
		private HttpServerFacade _server;
		private Dictionary<String, DockState> _documentTypes;
		private Boolean _certValidationRegistered = false;
		#endregion Fields

		internal event EventHandler<SearchForProjectEventArgs> OnSearchForProject;
		internal event EventHandler<SearchProjectItemEventArgs> OnSearchForProjectItems;
		internal event EventHandler<ProjectClosedEventArgs> OnProjectClosed;
		internal event EventHandler<ToggleProjectDirtyEventArgs> OnToggleProjectDirty;

		internal TraceSource Trace => this._trace ?? (this._trace = PluginWindows.CreateTraceSource<PluginWindows>());

		internal IHostWindows HostWindows { get; }

		/// <summary>История вызова HTTP запросов</summary>
		internal HistoryController History { get; } = new HistoryController();

		/// <summary>Настройки для взаимодействия из хоста</summary>
		Object IPluginSettings.Settings => this.Settings;

		/// <summary>Настройки для взаимодействия из плагина</summary>
		public PluginSettings Settings
		{
			get
			{
				if(this._settings == null)
				{
					this._settings = new PluginSettings(this);
					this.HostWindows.Plugins.Settings(this).LoadAssemblyParameters(this._settings);
					this._settings.PropertyChanged += this.Settings_PropertyChanged;
				}
				return this._settings;
			}
		}

		private IMenuItem MenuTest { get; set; }
		private IMenuItem MenuHttpTest { get; set; }

		private Dictionary<String, DockState> DocumentTypes
		{
			get
			{
				if(this._documentTypes == null)
					this._documentTypes = new Dictionary<String, DockState>()
					{
						{ typeof(PanelHttpClient).ToString(), DockState.DockRight },
						{ typeof(PanelHttpHistory).ToString(), DockState.DockRightAutoHide },
					};
				return this._documentTypes;
			}
		}

		public PluginWindows(IHostWindows hostWindows)
			=> this.HostWindows = hostWindows ?? throw new ArgumentNullException(nameof(hostWindows));

		public IWindow GetPluginControl(String typeName, Object args)
			=> this.CreateWindow(typeName, false, args);

		Boolean IPlugin.OnConnection(ConnectMode mode)
		{
			IMenuItem menuTools = this.HostWindows.MainMenu.FindMenuItem("Tools");
			if(menuTools == null)
			{
				this.Trace.TraceEvent(TraceEventType.Error, 10, "Menu item 'Tools' not found");
				return false;
			}

			this.MenuTest = menuTools.FindMenuItem("Test");
			if(this.MenuTest == null)
			{
				this.MenuTest = menuTools.Create("Test");
				this.MenuTest.Name = "Tools.Test";
				menuTools.Items.Add(this.MenuTest);
			}

			IMenuItem networkTests = this.MenuTest.FindMenuItem("Network");
			if(networkTests == null)
			{
				networkTests = this.MenuTest.Create("Network");
				networkTests.Name = "Tools.Test.Network";
				this.MenuTest.Items.Add(networkTests);
			}

			this.MenuHttpTest = networkTests.Create("HTTP Test Client");
			this.MenuHttpTest.Name = "Tools.Test.Network.HttpTestClient";
			this.MenuHttpTest.Click += (sender, e) => { this.CreateWindow(typeof(PanelHttpClient).ToString(), true); };

			networkTests.Items.AddRange(new IMenuItem[] { this.MenuHttpTest, });

			this._server = new HttpServerFacade(this);
			return true;
		}

		Boolean IPlugin.OnDisconnection(DisconnectMode mode)
		{
			if(this.MenuHttpTest != null)
				this.HostWindows.MainMenu.Items.Remove(this.MenuHttpTest);
			if(this.MenuTest != null && this.MenuTest.Items.Count == 0)
				this.HostWindows.MainMenu.Items.Remove(this.MenuTest);

			this.RegisterForCertificateValidation(false);
			this._server?.Dispose();
			return true;
		}

		internal void RegisterForCertificateValidation(Boolean isRegister)
		{
			if(isRegister)
			{
				ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;
				this._certValidationRegistered = true;
			} else if(this._certValidationRegistered)
			{
				ServicePointManager.ServerCertificateValidationCallback -= ServerCertificateValidationCallback;
				this._certValidationRegistered = false;
			}
		}

		/// <summary>Triggers OnProjectClosed event</summary>
		/// <param name="projectFileName"></param>
		internal void ProjectClosed(String projectFileName)
			=> this.OnProjectClosed?.Invoke(this, new ProjectClosedEventArgs(projectFileName));

		/// <summary>Triggers OnToggleprojectDirty event to notify that project is changed elswhere</summary>
		/// <param name="projectFilePath">The project file path that was marked as changed</param>
		/// <param name="isDirty">Project was changed and can be saved</param>
		internal void ToggleProjectDirty(String projectFilePath, Boolean isDirty)
			=> this.OnToggleProjectDirty?.Invoke(this, new ToggleProjectDirtyEventArgs(projectFilePath, isDirty));

		/// <summary>Search for project to attach to when editing tempate items</summary>
		/// <param name="projectFileName">The project file name to search for</param>
		/// <returns>Found project instance or null if project not found</returns>
		internal HttpProject SearchForProject(String projectFileName)
		{
			EventHandler<SearchForProjectEventArgs> evt = this.OnSearchForProject;
			if(evt != null)
			{
				SearchForProjectEventArgs args = new SearchForProjectEventArgs(projectFileName);
				evt(this, args);
				return args.Project;
			}
			return null;
		}

		/// <summary>Search for project item if we receive request from web server</summary>
		/// <param name="tempate">Search request template</param>
		/// <returns>Array of found items</returns>
		internal IEnumerable<Project.HttpProjectItem> SearchForProjectItems(Project.HttpProjectItem tempate)
		{
			//Added to avoid single project enumeration
			HashSet<Project.HttpProjectItem> hash = new HashSet<Project.HttpProjectItem>();

			EventHandler<SearchProjectItemEventArgs> evt = this.OnSearchForProjectItems;
			if(evt != null)
			{
				SearchProjectItemEventArgs args = new SearchProjectItemEventArgs(tempate);
				evt(this, args);
				foreach(Project.HttpProjectItem item in args.Found)
					if(hash.Add(item))
						yield return item;
			}

			Project.HttpProject project = this.Settings.LoadProject();
			foreach(Project.HttpProjectItem item in project.Items.Find(tempate, this.Settings.ServerSearchRelativeUrl))
				if(hash.Add(item))
					yield return item;
		}

		internal IWindow CreateWindow(String typeName, Boolean searchForOpened, Object args = null)
		{
			try
			{
				return this.DocumentTypes.TryGetValue(typeName, out DockState state)
					? this.HostWindows.Windows.CreateWindow(this, typeName, searchForOpened, state, args)
					: null;
			}catch(ObjectDisposedException)
			{//This can happen if window closes because lack of data to open. For example PanelTemplate window will be closed if no project is loaded
				return null;
			}
		}

		internal Icon GetApplicationIcon()
		{
			return Icon.ExtractAssociatedIcon(Process.GetCurrentProcess().MainModule.FileName);
			//FIX: .NET 7 opened forms icon returns not icon but png image that should be converted (Also it could be not application icon but one of the forms icon)
			/*return Application.OpenForms.Count == 0
				? null
				: Application.OpenForms[0].Icon;*/
		}

		private Boolean ServerCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			this.Trace.TraceInformation("CertificateValidation: PolicyErrors: {0} Certificate: {1}", sslPolicyErrors, certificate);
			return true;
		}

		private static TraceSource CreateTraceSource<T>(String name = null) where T : IPlugin
		{
			TraceSource result = new TraceSource(typeof(T).Assembly.GetName().Name + name);
			result.Switch.Level = SourceLevels.All;
			result.Listeners.Remove("Default");
			result.Listeners.AddRange(System.Diagnostics.Trace.Listeners);
			return result;
		}

		private void Settings_PropertyChanged(Object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
			case nameof(PluginSettings.ServerEnabled):
				if(this.Settings.ServerEnabled)
				{
					this._server?.Dispose();
					this._server = new HttpServerFacade(this);
				} else
				{
					this._server?.Dispose();
					this._server = null;
				}
				break;
			case nameof(PluginSettings.ServerUrl):
				this._server?.Dispose();
				this._server = new HttpServerFacade(this);
				break;
			}
		}
	}
}