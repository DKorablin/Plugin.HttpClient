using System;
using System.Linq;
using System.Text;
using Plugin.HttpClient.Test;

namespace Plugin.HttpClient.Project
{
	/// <summary>Basic information with response payload</summary>
	internal class HttpItemResponse
	{
		/// <summary>The response body</summary>
		public String Body { get; }

		/// <summary>The response headers</summary>
		public String Headers { get; }

		public String ContentType { get; }

		public HttpItemResponse(ResultBase result)
		{
			this.Body = result.ResponseBody;
			this.Headers = result.ResponseHeaders;
			this.ContentType = result.ResponseContentType;
		}

		public String GetBodyWithHeaders()
			=> this.Headers != null || this.Body != null
				? String.Join(Environment.NewLine, new String[] { this.Headers, this.TryFormatBody(), })
				: null;

		public String TryFormatBody()
			=> this.Body != null && this.ContentType?.ToLowerInvariant().StartsWith("application/json") == true
				? FormatJson(this.Body)
				: this.Body;

		public static String FormatJson(String text, String indentString = "\t")
		{
			Int32 indent = 0;
			Boolean quoted = false;
			StringBuilder result = new StringBuilder();
			for(var loop = 0; loop < text.Length; loop++)
			{
				var ch = text[loop];
				switch(ch)
				{
				case '{':
				case '[':
					result.Append(ch);
					if(!quoted)
					{
						result.AppendLine();
						Array.ForEach(Enumerable.Range(0, ++indent).ToArray(), (_) => result.Append(indentString));
					}
					break;
				case '}':
				case ']':
					if(!quoted)
					{
						result.AppendLine();
						Array.ForEach(Enumerable.Range(0, --indent).ToArray(), (_) => result.Append(indentString));
					}
					result.Append(ch);
					break;
				case '"':
					result.Append(ch);
					Boolean escaped = false;
					var index = loop;
					while(index > 0 && text[--index] == '\\')
						escaped = !escaped;
					if(!escaped)
						quoted = !quoted;
					break;
				case ',':
					result.Append(ch);
					if(!quoted)
					{
						result.AppendLine();
						Array.ForEach(Enumerable.Range(0, indent).ToArray(), (_) => result.Append(indentString));
					}
					break;
				case ':'://TODO: Check for {"url":"url('http://google.com')"}
					result.Append(ch);
					if(!quoted)
						result.Append(" ");
					break;
				default:
					result.Append(ch);
					break;
				}
			}
			return result.ToString();
		}
	}
}