using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.Test
{
	internal class RequestTest
	{
		public HttpProjectItem Item { get; }

		public ResultBase Result { get; private set; }

		public TemplateItem[] Variables { get; private set; }

		private CookieContainer Cookies { get; }

		public RequestTest() { }

		public RequestTest(HttpProjectItem item, CookieContainer cookies)
			: this(item, cookies, item.Items.Project.Templates.GetTemplateValuesWithSource().ToArray())
		{
		}

		public RequestTest(HttpProjectItem item, CookieContainer cookies, TemplateItem[] variables)
		{
			this.Item = item ?? throw new ArgumentNullException(nameof(item));
			this.Cookies = cookies;
			this.Variables = variables;
		}

		public void InvokeTest()
		{
			try
			{
				RequestBuilder builder = new RequestBuilder(this.Item, this.Variables);
				this.Result = this.InvokeTest(builder);
				//this.Result = this.InvokeTestAsync(request);

				// this wil replace values in the this.Variables array because they're passed as a reference type
				builder.ReplaceTemplateValuesFromResponseString(this.Result.ResponseBody);
			} catch(Exception exc)
			{
				if(Utils.IsFatal(exc))
					throw;

				//WebException: This can occur if we creating request stream & disconnected from the network
				//ArgumentException: Failed to parse string values to HttpWebRequest object
				this.Result = new ResultException(this.Item, (HttpWebRequest)null, exc);
			}
		}

		public ResultBase InvokeTest(RequestBuilder builder)
		{
			_ = builder ?? throw new ArgumentNullException(nameof(builder));

			HttpWebRequest request = builder.CreateRequest(this.Cookies);
			HttpWebResponse response = null;
			ResultBase result;
			Stopwatch sw = new Stopwatch();
			sw.Start();

			try
			{
				response = (HttpWebResponse)request.GetResponse();
				result = this.Validate2(builder, request, response);
			} catch(WebException exc)
			{
				response = (HttpWebResponse)exc.Response;

				result = response ==null
					? new ResultFailure(builder.CloneRequest(), request, exc)
					: this.Validate2(builder, request, response);
			} catch(Exception exc)
			{
				result = new ResultException(builder.CloneRequest(), request, exc);
			} finally
			{
				sw.Stop();
				response?.Close();
			}

			result.Elapsed = sw.Elapsed;
			return result;
		}

		private readonly ManualResetEvent allDone = new ManualResetEvent(false);
		const Int32 DefaultTimeout = 2 * 60 * 1000; // 2 minutes timeout

		internal class RequestState
		{
			// This class stores the State of the request.
			public const Int32 BUFFER_SIZE = 1024;
			public HttpWebRequest request;
			public HttpWebResponse response;

			public RequestState()
			{
			}
		}

		public ResultBase InvokeTestAsync(RequestBuilder builder)
		{
			_ = builder ?? throw new ArgumentNullException(nameof(builder));

			HttpWebRequest request = builder.CreateRequest(this.Cookies);
			HttpWebResponse response = null;
			ResultBase result;
			Stopwatch sw = new Stopwatch();
			sw.Start();

			RequestState state = new RequestState()
			{
				request = request,
			};

			try
			{
				IAsyncResult resultAsync = request.BeginGetResponse(new AsyncCallback(ResponseCallback), state);
				ThreadPool.RegisterWaitForSingleObject(resultAsync.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), request, DefaultTimeout, true);

				allDone.WaitOne();

				result = this.Validate2(builder, state.request, state.response);
			} catch(WebException exc)
			{
				response = (HttpWebResponse)exc.Response;

				result = response == null
					? new ResultFailure(builder.CloneRequest(), request, exc)
					: this.Validate2(builder, request, response);
			} catch(Exception exc)
			{
				result = new ResultException(builder.CloneRequest(), request, exc);
			} finally
			{
				sw.Stop();
				response?.Close();
			}

			result.Elapsed = sw.Elapsed;
			return result;

			void TimeoutCallback(Object requestState, Boolean timedOut)
			{
				if(timedOut && requestState is HttpWebRequest r)
					r?.Abort();
			}

			void ResponseCallback(IAsyncResult asynchronousResult)
			{
				// State of request is asynchronous.
				RequestState requestState = (RequestState)asynchronousResult.AsyncState;
				HttpWebRequest requestStateObject = requestState.request;
				requestState.response = (HttpWebResponse)requestStateObject.EndGetResponse(asynchronousResult);

				allDone.Set();
			}
		}

		private ResultBase Validate2(RequestBuilder builder, HttpWebRequest request, HttpWebResponse response)
		{
			ResultBase result = new ResultResponse(builder.CloneRequest(), request, response);
			ValidationFailureStatus validation = this.Validate(builder.Item, response.StatusCode, result.ResponseBody);
			if(validation != null)
			{
				if(validation.ActualCode != validation.ExpectedCode)
					result = new ResultValidation(result, validation.ExpectedCode.Value, validation.ActualCode.Value);
				else if(validation.ActualResponsePart != validation.ExpectedResponsePart)
					result = new ResultValidation(result, validation.ExpectedResponsePart, validation.ActualResponsePart);
				else
					throw new NotImplementedException("This validation type is not implemented");
			}

			return result;
		}

		private ValidationFailureStatus Validate(HttpItem item, HttpStatusCode actual, String responseActual)
		{
			ValidationFailureStatus result = new ValidationEngine(this.Variables).ValidatePayloadWithTemplate(responseActual, item.Response);
			if(result == null)
			{
				if(item.ResponseStatus != actual)
					result = new ValidationFailureStatus(item.ResponseStatus, actual);
			} else
			{
				if(item.ResponseStatus != actual)
				{
					result.ExpectedCode = item.ResponseStatus;
					result.ActualCode = actual;
				}
			}

			return result;
		}
	}
}