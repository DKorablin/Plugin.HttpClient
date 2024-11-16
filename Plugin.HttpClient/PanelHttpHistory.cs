using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Plugin.HttpClient.History;
using Plugin.HttpClient.Test;
using SAL.Windows;

namespace Plugin.HttpClient
{
	public partial class PanelHttpHistory : UserControl
	{
		private IWindow Window => (IWindow)base.Parent;

		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		private ListViewItem SelectedListItem
			=> lvHistory.SelectedItems.Count == 0 ? null : lvHistory.SelectedItems[0];

		public PanelHttpHistory()
		{
			this.InitializeComponent();
			splitMain.Panel2Collapsed = true;
		}

		protected override void OnCreateControl()
		{
			this.Window.Caption = "Http Test History";
			this.FillListItemsFromHistory();
			this.ToggleNodeSelected(null);

			this.Window.Closed += this.Window_Closed;
			this.Plugin.History.HistoryChanged += this.History_HistoryChanged;
			base.OnCreateControl();
		}

		private void Window_Closed(Object sender, EventArgs e)
			=> this.Plugin.History.HistoryChanged -= this.History_HistoryChanged;

		private void History_HistoryChanged(Object sender, HistoryChangedEventArgs e)
		{
			if(lvHistory.InvokeRequired)
			{
				lvHistory.Invoke((MethodInvoker)delegate { this.History_HistoryChanged(sender, e); });
				return;
			}

			switch(e.State)
			{
			case HistoryChangedEventArgs.StateType.Removed:
				ListViewItem listItem = this.FindListItemFromHistory(e.Item);
				lvHistory.Items.Remove(listItem);
				break;
			case HistoryChangedEventArgs.StateType.Added:
				ListViewItem listItem1 = this.CreateListItemFromHistory(e.Item);
				lvHistory.Items.Add(listItem1);
				break;
			case HistoryChangedEventArgs.StateType.Moved:
				ListViewItem listItem2 = this.FindListItemFromHistory(e.Item);

				Int32 indexInHistory = 0;
				foreach(HttpHistoryItem hItem in this.Plugin.History)
					if(Object.ReferenceEquals(listItem2.Tag, hItem))
						break;
					else
						indexInHistory++;

				lvHistory.Items.Remove(listItem2);
				lvHistory.Items.Insert(indexInHistory, listItem2);
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void lvHistory_SelectedIndexChanged(Object sender, EventArgs e)
		{
			ListViewItem listItem = this.SelectedListItem;
			this.ToggleNodeSelected(listItem);
		}

		private void tsbnRetry_Click(Object sender, EventArgs e)
		{
			HttpHistoryItem hItem = (HttpHistoryItem)this.SelectedListItem?.Tag;
			if(hItem == null)
				return;

			RequestBuilder builder = new RequestBuilder(hItem.Request, new Project.TemplateItem[] { });
			ResultBase result = new RequestTest().InvokeTest(builder);
			if(result is ResultException exception)
				this.Plugin.Trace.TraceData(TraceEventType.Error, 10, exception.Exception);

			this.Plugin.History.MoveToTop(hItem, result);
		}

		private void tsbnRemove_Click(Object sender, EventArgs e)
		{
			while(lvHistory.SelectedItems.Count > 0)
			{
				ListViewItem listItem = lvHistory.SelectedItems[0];
				HttpHistoryItem hItem = (HttpHistoryItem)listItem.Tag;
				this.Plugin.History.Remove(hItem);
			}
		}

		private void cmsHistory_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiHistorySelectAll)
				foreach(ListViewItem item in lvHistory.Items)
					item.Selected = true;
		}

		private void FillListItemsFromHistory()
		{
			List<ListViewItem> itemsToAdd = new List<ListViewItem>();
			foreach(HttpHistoryItem hItem in this.Plugin.History)
				itemsToAdd.Add(this.CreateListItemFromHistory(hItem));

			lvHistory.Items.AddRange(itemsToAdd.ToArray());
			lvHistory.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private ListViewItem FindListItemFromHistory(HttpHistoryItem hItem)
		{
			foreach(ListViewItem listItem in lvHistory.Items)
			{
				HttpHistoryItem listHistoryItem = (HttpHistoryItem)listItem.Tag;
				if(Object.ReferenceEquals(listHistoryItem, hItem))
					return listItem;
			}

			throw new ArgumentException("History item not found in the history", nameof(hItem));
		}

		private ListViewItem CreateListItemFromHistory(HttpHistoryItem hItem)
		{
			ListViewItem result = new ListViewItem() { Tag = hItem, };
			String[] empty = Enumerable.Repeat<String>(String.Empty, lvHistory.Columns.Count).ToArray();
			result.SubItems.AddRange(empty);
			result.SubItems[colHistoryDate.Index].Text = hItem.InvokeDate.ToString();
			result.SubItems[colHistoryUrl.Index].Text = hItem.Request.Address.ToString();
			result.SubItems[colHistoryElapsed.Index].Text = hItem.Elapsed?.ToString();
			return result;
		}

		private void ToggleNodeSelected(ListViewItem listItem)
		{
			HttpHistoryItem hItem = (HttpHistoryItem)listItem?.Tag;

			splitMain.Panel2Collapsed = hItem == null;
			tsbnRetry.Enabled = tsbnRemove.Enabled = hItem != null;

			rtxtResponse.Text = hItem?.Response;
		}

		private void lvHistory_KeyDown(Object sender, KeyEventArgs e)
		{
			e.Handled = true;
			switch(e.KeyData)
			{
			case Keys.Delete:
				this.tsbnRemove_Click(sender, e);
				break;
			case Keys.A | Keys.Control:
				foreach(ListViewItem item in lvHistory.Items)
					item.Selected = true;
				break;
			case Keys.C | Keys.Control:
				ListViewItem listItem = lvHistory.SelectedItems.Count == 0 ? null : lvHistory.SelectedItems[0];
				Clipboard.SetText(listItem.Text);
				break;
			default:
				e.Handled = false;
				break;
			}
		}
	}
}