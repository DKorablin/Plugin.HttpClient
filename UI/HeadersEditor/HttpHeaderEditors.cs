﻿using System;
using System.Collections.Generic;

namespace Plugin.HttpClient.UI
{
	public class HttpTransferEncodingEditor : HttpEditorBase
	{
		public override IEnumerable<KeyValuePair<String, String>> GetValues()
		{
			return Constant.Http.TransferEncoding;
		}
	}

	public class HttpMethodEditor : HttpEditorBase
	{
		public override IEnumerable<KeyValuePair<String, String>> GetValues()
		{
			return Constant.Http.Methods;
		}
	}

	public class HttpExpectHeaderEditor : HttpEditorBase
	{
		public override IEnumerable<KeyValuePair<String, String>> GetValues()
		{
			return Constant.Http.Expect;
		}
	}

	public class HttpUserAgentEditor : HttpEditorBase
	{
		public override IEnumerable<KeyValuePair<String, String>> GetValues()
		{
			return Constant.Http.UserAgents;
		}
	}

	public class HttpConnectionEditor : HttpEditorBase
	{
		public override IEnumerable<KeyValuePair<String, String>> GetValues()
		{
			return Constant.Http.Connection;
		}
	}

	public class HttpContentTypeEditor : HttpEditorBase
	{
		public override IEnumerable<KeyValuePair<String, String>> GetValues()
		{
			return Constant.Http.ContentTypes;
		}
	}
}