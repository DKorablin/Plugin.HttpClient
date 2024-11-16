using System;

namespace Plugin.HttpClient.History
{
	internal class HistoryChangedEventArgs : EventArgs
	{
		public enum StateType
		{
			Added,
			Moved,
			Removed,
		}

		public HttpHistoryItem Item { get; }

		public StateType State { get; }

		public HistoryChangedEventArgs(HttpHistoryItem item, StateType state)
		{
			this.Item = item?? throw new ArgumentNullException(nameof(item));
			this.State = state;
		}
	}
}