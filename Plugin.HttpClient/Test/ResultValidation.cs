using System;
using System.Net;

namespace Plugin.HttpClient.Test
{
	internal class ResultValidation : ResultBase
	{
		public HttpStatusCode? ExpectedCode { get; }
		public HttpStatusCode? ActualCode { get; }

		public String ActualResponsePart { get; }
		public String ExpectedResponsePart { get; }

		public ResultValidation(ResultBase result, HttpStatusCode expectedCode, HttpStatusCode actualCode)
			: base(result)
		{
			this.IsSuccess = false;
			this.ExpectedCode = expectedCode;
			this.ActualCode = actualCode;
		}

		public ResultValidation(ResultBase result, String expectedResponsePart, String actualResponsePart)
			: base(result)
		{
			this.IsSuccess = false;
			this.ActualResponsePart = actualResponsePart;
			this.ExpectedResponsePart = expectedResponsePart;
		}
	}
}