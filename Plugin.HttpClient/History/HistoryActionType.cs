using System;

namespace Plugin.HttpClient.History
{
	/// <summary>Historical action type (Initiator of received request)</summary>
	internal enum HistoryActionType
	{
		/// <summary>Request initiated from client</summary>
		Client,
		/// <summary>Request from client processed</summary>
		Server,
	}
}