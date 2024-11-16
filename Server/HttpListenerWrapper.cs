using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;

namespace Plugin.HttpClient.Server
{
	/// <summary>Wrapper over HttpListener with listeners array</summary>
	internal class HttpListenerWrapper : IDisposable
	{
		private const Int32 ListenersCount = 10;
		private HttpListener _listener;
		private Thread[] _workers;
		private readonly PluginWindows _plugin;
		private Thread _listenerThread;
		private readonly ManualResetEvent _stop, _ready;
		private readonly Queue<HttpListenerContext> _queue;

		/// <summary>Users process events</summary>
		public event Action<HttpListenerContext> ProcessRequest;

		/// <summary>Server launched</summary>
		public Boolean IsListening => this._listener != null && this._listener.IsListening;

		public IEnumerable<String> Endpoints
			=> this._listener == null
				? Enumerable.Empty<String>()
				: this._listener.Prefixes;

		public HttpListenerWrapper(PluginWindows plugin)
		{
			this._plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));
			this._queue = new Queue<HttpListenerContext>();
			this._stop = new ManualResetEvent(false);
			this._ready = new ManualResetEvent(false);
		}

		/// <summary>Запустить сервер</summary>
		/// <param name="hostUrl">Url сервера</param>
		/// <param name="listenersCount">Кол-во обработчиков запросов с клиентов</param>
		public void Start(String hostUrl)
		{
			if(this.ProcessRequest == null)
				throw new InvalidOperationException("Callback event not defined");

			HttpListener listener = new HttpListener();
			listener.Prefixes.Add(hostUrl);

			listener.IgnoreWriteExceptions = true;

			this._listener = listener;
			this._listener.Start();

			Thread[] workers = new Thread[ListenersCount];
			for(Int32 loop = 0; loop < workers.Length; loop++)
			{
				workers[loop] = new Thread(Worker);
				workers[loop].Start();
			}
			this._workers = workers;

			this._listenerThread = new Thread(HandleRequests);
			this._listenerThread.Start();
		}

		/// <summary>Остановить сервер</summary>
		public void Stop()
		{
			this._stop.Set();

			if(this._listenerThread != null && this._listenerThread.IsAlive)
				this._listenerThread.Join();
			this._listenerThread = null;

			if(this._workers != null)
				foreach(Thread worker in this._workers)
					worker.Join();

			if(this._listener != null && this._listener.IsListening)
				this._listener.Stop();
		}

		private void HandleRequests()
		{
			while(this._listener.IsListening)
			{
				IAsyncResult context = this._listener.BeginGetContext(this.ContextReady, null);

				if(WaitHandle.WaitAny(new WaitHandle[] { this._stop, context.AsyncWaitHandle }) == 0)
					return;
			}
		}

		private void ContextReady(IAsyncResult ar)
		{
			try
			{
				lock(this._queue)
				{
					this._queue.Enqueue(this._listener.EndGetContext(ar));
					this._ready.Set();
				}
			} catch(HttpListenerException exc)
			{
				switch(exc.ErrorCode)
				{
				case 995://The I/O operation has been aborted because of either a thread exit or an application request
					break;
				default:
					throw;
				}
			} catch(Exception exc)
			{
				if(Utils.IsFatal(exc))
					throw;
				else
					this._plugin.Trace.TraceData(TraceEventType.Error, 10, exc);
			}
		}

		private void Worker()
		{
			WaitHandle[] wait = new WaitHandle[] { this._ready, this._stop };
			while(WaitHandle.WaitAny(wait) == 0)
			{
				HttpListenerContext context;
				lock(this._queue)
				{
					if(this._queue.Count > 0)
						context = this._queue.Dequeue();
					else
					{
						this._ready.Reset();
						continue;
					}
				}

				try
				{
					this.ProcessRequest(context);
				} catch(Exception exc)
				{
					if(Utils.IsFatal(exc))
						throw;
					else
						this._plugin.Trace.TraceData(TraceEventType.Error, 10, exc);
				}
			}
		}

		public void Dispose()
			=> this.Stop();
	}
}