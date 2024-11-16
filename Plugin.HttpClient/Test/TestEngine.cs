using System;
using System.Diagnostics;
using System.Net;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.UI;

namespace Plugin.HttpClient.Test
{
	internal class TestEngine
	{
		private readonly TestStartArgs _args;
		public event EventHandler<TestProgressChangedArgs> OnTestChanged;

		public TestEngine(TestStartArgs args)
		{ 
			this._args = args;
		}

		public void Run(Func<Boolean> shouldCancel)
		{
			CookieContainer cookies = new CookieContainer();

			foreach(HttpProjectItem item in this._args.Items)
			{
				RequestTest test = new RequestTest(item, cookies, this._args.Variables);
				if(!this.InvokeTest(test, shouldCancel))
					break;

				if(this._args.Plugin.Settings.TestTimeout > 0)
					System.Threading.Thread.Sleep(this._args.Plugin.Settings.TestTimeout * 1000);
			}
		}

		private Boolean InvokeTest(RequestTest test, Func<Boolean> isChancelled)
		{
			test.Item.Image = NodeStateEnum.Running;
			this.OnTestChanged?.Invoke(this, new TestProgressChangedArgs(test.Item));
			NodeStateEnum status = NodeStateEnum.Failure;

			test.InvokeTest();
			if(test.Result.IsSuccess)
				status = NodeStateEnum.Success;
			else if(test.Result is ResultResponse resultF && resultF.StatusCode == HttpStatusCode.Forbidden)
				status = NodeStateEnum.FailureForbidden;
			else if(test.Result is ResultException exc)
				this._args.Plugin.Trace.TraceData(TraceEventType.Error, 10, exc.Exception);
			else if(test.Result is ResultValidation resultV)
			{//TODO: Add proper validation handling
				if(resultV.ActualCode != resultV.ExpectedCode)
					this._args.Plugin.Trace.TraceData(TraceEventType.Warning, 15, $"Expected HTTP code: {resultV.ExpectedCode} Actual HTTP code: {resultV.ActualCode}");
				else if(resultV.ActualResponsePart != resultV.ExpectedResponsePart)
					this._args.Plugin.Trace.TraceData(TraceEventType.Warning, 15, $"Response difference at:{Environment.NewLine}Expected:{Environment.NewLine}\t{resultV.ExpectedResponsePart}{Environment.NewLine}Actual:{Environment.NewLine}\t{resultV.ActualResponsePart}");
				else
					throw new NotImplementedException("Validation result is not implemented. Type: " + resultV.GetType());
			}//Here could be other type of results. For example ResultFailure

			//Успешное завершение тестирования
			test.Item.Image = status;
			test.Item.HttpResponse = test.Result.GetResponseWithHeaders();
			this.OnTestChanged?.Invoke(this, new TestProgressChangedArgs(test));

			Boolean isNotCancelled = !isChancelled();
			Boolean isNotFailure = status != NodeStateEnum.Failure;
			Boolean isNotStopOnError = !this._args.Plugin.Settings.StopOnError;
			return isNotCancelled
				&& (isNotFailure || !isNotStopOnError);
		}
	}
}