using System;
using System.Collections.Generic;
using Plugin.HttpClient.Test;

namespace Plugin.HttpClient.History
{
	internal class HistoryController : IEnumerable<HttpHistoryItem>
	{
		private List<HttpHistoryItem> _items;

		public event EventHandler<HistoryChangedEventArgs> HistoryChanged;

		public void Add(HistoryActionType action, ResultBase test)
		{
			HttpHistoryItem item = new HttpHistoryItem(action, test.Item, test.GetResponseWithHeaders(), test.Elapsed);
			if(this._items == null)
				this._items = new List<HttpHistoryItem>();
			this._items.Add(item);

			this.OnHistoryChanged(HistoryChangedEventArgs.StateType.Added, item);
		}

		public void Remove(HttpHistoryItem item)
		{
			if(this._items != null)
			{
				this._items.Remove(item);
				if(this._items.Count == 0)
					this._items = null;
			}
			this.OnHistoryChanged(HistoryChangedEventArgs.StateType.Removed, item);
		}

		public void MoveToTop(HttpHistoryItem item, ResultBase test)
		{
			item.Update(test.GetResponseWithHeaders());

			Int32 index = this._items.IndexOf(item);
			if(index != 0)
			{
				this._items.RemoveAt(index);
				this._items.Insert(0, item);
				this.OnHistoryChanged(HistoryChangedEventArgs.StateType.Moved, item);
			}
		}

		public IEnumerator<HttpHistoryItem> GetEnumerator()
		{
			if(this._items == null)
				yield break;
			else
				foreach(HttpHistoryItem item in this._items)
					yield return item;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			=> this.GetEnumerator();

		private void OnHistoryChanged(HistoryChangedEventArgs.StateType state, HttpHistoryItem item)
			=> this.HistoryChanged?.Invoke(this, new HistoryChangedEventArgs(item, state));
	}
}