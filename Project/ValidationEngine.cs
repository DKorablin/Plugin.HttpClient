using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.HttpClient.Test;

namespace Plugin.HttpClient.Project
{
	internal class ValidationEngine
	{
		private readonly String[] _templateItemKeys;

		public ValidationEngine(TemplateEngine templates)
			: this((IEnumerable<TemplateItem>)templates.GetTemplateValuesWithSource())
		{
		}

		public ValidationEngine(IEnumerable<TemplateItem> items)
			=> this._templateItemKeys = items
				.Select(t => t.Key)
				.Union(new String[] { Constant.Project.DiscardValueName })
				.ToArray();

		/// <summary>Validate received payload with expected payload</summary>
		/// <param name="responseActual">Actual response received from the server</param>
		/// <param name="responseExpected">Expected response that should be received from the server</param>
		/// <returns>actual response is equals to expected payload</returns>
		public ValidationFailureStatus ValidatePayloadWithTemplate(String responseActual, String responseExpected)
		{
			if(responseActual == null || responseExpected == null)
				return null;

			String[] keys = this._templateItemKeys;

			TemplateValuePosition[] foundItems = TemplateEngine.GetValuesFromPayloadV2(responseActual, responseExpected, keys).ToArray();
			String responseActualCheck = responseActual;
			for(Int32 loop = foundItems.Length - 1; loop >= 0; loop--)
			{
				TemplateValuePosition item = foundItems[loop];
				responseActualCheck = responseActualCheck.Remove(item.Index, item.Value.Length).Insert(item.Index, item.Key);
			}

			if(responseActualCheck != responseExpected)
			{
				Int32 index;
				for(index = 0; index < responseActualCheck.Length && index < responseExpected.Length; index++)
					if(responseActualCheck.Substring(0, index) != responseExpected.Substring(0, index))
						break;

				//TODO: Send the difference to trace
				String responseActualCheckPart = index >= responseActualCheck.Length
					? responseActualCheck
					: responseActualCheck.Substring(0, index);
				String responseExpectedCheckPart = index >= responseExpected.Length
					? responseExpected
					: responseExpected.Substring(0, index);
				return new ValidationFailureStatus(responseActualCheckPart, responseExpectedCheckPart);
			} else
				return null;
		}
	}
}