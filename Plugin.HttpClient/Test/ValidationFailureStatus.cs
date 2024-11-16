using System;
using System.Net;

namespace Plugin.HttpClient.Test
{
	internal class ValidationFailureStatus
	{
		public HttpStatusCode? ExpectedCode { get; set; }
		public HttpStatusCode? ActualCode { get; set; }

		public String ActualResponsePart { get; }
		public String ExpectedResponsePart { get; }

		public ValidationFailureStatus(HttpStatusCode expectedCode, HttpStatusCode actualCode)
		{
			this.ExpectedCode = expectedCode;
			this.ActualCode = actualCode;
		}

		public ValidationFailureStatus(String actualResponsePart, String expectedResponsePart)
		{
			this.ActualResponsePart = actualResponsePart;
			this.ExpectedResponsePart = expectedResponsePart;
		}
	}
}