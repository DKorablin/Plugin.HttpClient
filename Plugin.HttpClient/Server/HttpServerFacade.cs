using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.Test;
using System.Drawing;

namespace Plugin.HttpClient.Server
{
	/// <summary>Http server facade</summary>
	internal class HttpServerFacade : IDisposable
	{
		private readonly PluginWindows _plugin;
		private readonly HttpListenerWrapper _wrapper;

		/// <summary>Service is listening now</summary>
		public Boolean IsListening => this._wrapper.IsListening;

		/// <summary>Http server facade ctor</summary>
		/// <param name="plugin">Plugin</param>
		public HttpServerFacade(PluginWindows plugin)
		{
			this._plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
			//this._plugin.Settings.PropertyChanged += Settings_PropertyChanged;//TODO: Enable/Disable server is not working on facade level

			this._wrapper = new HttpListenerWrapper(plugin);

			if(this._plugin.Settings.ServerEnabled)
				this.Start();
		}

		/// <summary>Launch server</summary>
		private void Start()
		{
			if(this.IsListening)
				return;

			String serverUrl = this._plugin.Settings.GetServerUrl();
			if(!serverUrl.EndsWith("/"))
				serverUrl += "/";
			try
			{
				this._wrapper.ProcessRequest += this.wrapper_OnProcessRequest;
				this._wrapper.Start(serverUrl);
				this._plugin.Trace.TraceEvent(TraceEventType.Start, 1, "Server started at Url:\r\n\t{0}", serverUrl);
			} catch(HttpListenerException exc)
			{
				switch(exc.ErrorCode)
				{
				case 5://Access is denied
					this.CheckAdministratorAccess(serverUrl, exc);
					throw;
				default:
					throw;
				}
			}
		}

		/// <summary>Stop server</summary>
		private void Stop()
		{
			if(this.IsListening)
			{
				this._wrapper.Stop();
				this._wrapper.ProcessRequest -= this.wrapper_OnProcessRequest;
				this._plugin.Trace.TraceEvent(TraceEventType.Stop, 1, "Server stopped");
			}
		}

		/// <summary>Stop server and clean all resources</summary>
		public void Dispose()
		{
			this.Stop();
			this._wrapper.Dispose();
		}

		private void wrapper_OnProcessRequest(HttpListenerContext context)
		{
			using(HttpListenerResponse response = context.Response)
			{
				Boolean isFound = this.SendResponse(context, response);
				if(!isFound && response.StatusCode == (Int32)HttpStatusCode.NotFound)
				{
					switch(context.Request.Url.PathAndQuery)
					{
					case "/favicon.ico":
						Byte[] icon = GetProcessIcon();
						if(icon != null)
						{
							response.StatusCode = (Int32)HttpStatusCode.OK;
							WriteToResponseStream(response, icon);
						}
						break;
					}
				}
			}

			Byte[] GetProcessIcon()
			{
				Icon ico = PluginWindows.GetApplicationIcon();
				if(ico == null)
					return null;

				using(MemoryStream stream = new MemoryStream())
				{
					ico.Save(stream);
					return stream.ToArray();
				}
			}
		}

		/// <summary>Search for responses in all opened and stored projects and send response to client</summary>
		/// <param name="request">Client request</param>
		private Boolean SendResponse(HttpListenerContext context, HttpListenerResponse response)
		{
			Boolean result = false;
			HttpListenerRequest request = context.Request;
			ResultBase testResult;
			try
			{
				HttpProjectItem item = this.FindProjectItem(request.Url, request);

				if(item == null)
					response.StatusCode = (Int32)HttpStatusCode.NotFound;
				else if(item.UserName != null && (context.User == null || context.User.Identity.Name != item.UserName))
					response.StatusCode = (Int32)HttpStatusCode.Unauthorized;
				else
				{
					result = true;
					response.StatusCode = (Int32)item.ResponseStatus;

					if(item.Response != null)
					{
						if(response.ContentEncoding == null)
							response.ContentEncoding = request.ContentEncoding;

						WriteToResponseStream(response, item.ResponseReal);
					}
				}

				testResult = new ResultBase(item, request, response);
			} catch(Exception exc)
			{
				if(Utils.IsFatal(exc))
					throw;
				else
				{
					response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
					WriteToResponseStream(response, exc.Message);
					testResult = new ResultException(null, request, exc);
				}
			}
			this._plugin.History.Add(History.HistoryActionType.Server, testResult);
			return result;
		}

		private static void WriteToResponseStream(HttpListenerResponse response, String message)
		{
			if(response.ContentEncoding == null)
				response.ContentEncoding = Encoding.UTF8;

			Byte[] data = response.ContentEncoding.GetBytes(message);
			WriteToResponseStream(response, data);
		}

		private static void WriteToResponseStream(HttpListenerResponse response, Byte[] data)
		{
			if(response.ContentEncoding == null)
				response.ContentEncoding = Encoding.UTF8;

			response.ContentLength64 = data.Length;
			using(Stream output = response.OutputStream)
				output.Write(data, 0, data.Length);
		}

		/// <summary>Get the result from loaded controllers.</summary>
		/// <param name="uri">Invoked URL</param>
		/// <param name="request">Incoming HTTP(S) request</param>
		/// <returns>Result in byte array or null</returns>
		private HttpProjectItem FindProjectItem(Uri uri, HttpListenerRequest request)
		{
			//We don't need this condition because we have relative search config control
			//String searchUrl = uri.ToString().Replace(this._plugin.Settings.GetServerUrl(), PluginSettings.Constants.TemplateServerUrl);
			String searchUrl = uri.ToString();

			HttpProjectItem template = new HttpProjectItem(request)
			{
				Address = searchUrl,
			};

			HttpProjectItem[] found = this._plugin.SearchForProjectItems(template).Take(2).ToArray();
			if(found.Length == 0)
				this._plugin.Trace.TraceEvent(TraceEventType.Information, 17, "Url: {0} - Not found in open projects", searchUrl);
			else if(found.Length > 1)
				this._plugin.Trace.TraceEvent(TraceEventType.Warning, 17, "Url: {0} - Multiple choices found. Choosing first", searchUrl);

			return found.Length == 0 ? null : found[0];
		}

		/// <summary>Check for administrator rights for current user and modify exception for fixing details</summary>
		private void CheckAdministratorAccess(String serverUrl, Exception exc)
		{
			Boolean isAdministrator;
			using(WindowsIdentity identity = WindowsIdentity.GetCurrent())
				isAdministrator = new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);

			if(!isAdministrator)
			{
				exc.Data.Add("netsh", $"netsh http add urlacl url={serverUrl} user={Environment.UserDomainName}\\{Environment.UserName}");
				this._plugin.Trace.TraceEvent(TraceEventType.Warning, 6, "You have to reserve host with netsh (see exception details for example) command or run application in [Administrator] mode.");
			}
		}
	}
}