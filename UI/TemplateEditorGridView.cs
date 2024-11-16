using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Plugin.HttpClient.Events;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.Test;

namespace Plugin.HttpClient.UI
{
	internal class TemplateEditorGridView : DataGridView
	{
		private String _selectedTemplate;
		private readonly BindingSource bsTemplates;
		private readonly DataGridViewTextBoxColumn colKey;
		private readonly DataGridViewTextBoxColumn colValue;

		public event EventHandler<ToggleProjectDirtyEventArgs> OnToggleTemplatesDirty;

		[Browsable(false)]
		public IEnumerable<TemplateItem> Templates
		{
			get
			{
				if(this.DesignMode)
					return null;

				IEnumerable<TemplateItemSource> current = (IEnumerable<TemplateItemSource>)bsTemplates.DataSource;
				return current == null
					? null
					: current.Where(t => !String.IsNullOrEmpty(t.Key) && !String.IsNullOrEmpty(t.Value) && t.TemplateName == this._selectedTemplate)
					.Select(t => new TemplateItem(t.Key, t.Value));
			}
		}

		public void SetTemplateRows(String selectedTemplate, IEnumerable<TemplateItemSource> templates)
		{
			this.SuspendLayout();
			try
			{
				Int32 scrollOffset = this.VerticalScrollBar.Value;
				this._selectedTemplate = selectedTemplate;
				bsTemplates.DataSource = templates == null ? null : new List<TemplateItemSource>(templates.OrderBy(t => t.Key));
				this.VerticalScrollBar.Value = scrollOffset;
			} finally
			{
				this.ResumeLayout();
			}
		}

		private IEnumerable<TemplateItemSource> GetTemplateRows()
			=> (IEnumerable<TemplateItemSource>)bsTemplates.DataSource;

		public TemplateEditorGridView()
		{
			this.colKey = new DataGridViewTextBoxColumn()
			{
				//AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,//If grid is empty then first column will be too small
				Resizable = DataGridViewTriState.True,
				DataPropertyName = nameof(TemplateItem.Key),
				HeaderText = nameof(TemplateItem.Key),
				MinimumWidth = 6,
				Width = 125,
			};

			this.colValue = new DataGridViewTextBoxColumn()
			{
				AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
				DataPropertyName = nameof(TemplateItem.Value),
				HeaderText = nameof(TemplateItem.Value),
				MinimumWidth = 6,
			};
			this.colValue.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

			this.bsTemplates = new BindingSource()
			{
				AllowNew = true,
			};
		}

		protected override void OnCreateControl()
		{
			((ISupportInitialize)(this.bsTemplates)).BeginInit();

			this.AutoGenerateColumns = false;
			this.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
			this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
			this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
			this.ColumnHeadersVisible = false;
			this.AllowUserToResizeRows = false;
			this.DataSource = this.bsTemplates;
			this.RowHeadersVisible = false;
			this.RowHeadersWidth = 51;
			this.RowTemplate.Height = 24;
			if(!this.DesignMode)
			{
				this.Columns.AddRange(new DataGridViewColumn[] {
					this.colKey,
					this.colValue});
			}

			//this.bsTemplates.DataSource = typeof(Plugin.HttpClient.Project.TemplateItem);

			((ISupportInitialize)(this.bsTemplates)).EndInit();

			base.OnCreateControl();
		}

		private void ToggleTemplatesDirty()
			=> this.OnToggleTemplatesDirty?.Invoke(this, new ToggleProjectDirtyEventArgs(null, true));

		protected override void OnRowPrePaint(DataGridViewRowPrePaintEventArgs e)
		{
			DataGridViewRow row = base.Rows[e.RowIndex];

			TemplateItemSource item = (TemplateItemSource)row.DataBoundItem;
			if(item != null)
			{
				Color color;
				if(String.IsNullOrEmpty(item.Value))
					color = Color.Red;
				else if(item.Key == Constant.Project.DiscardValueName)
					color = Color.Brown;
				else if(item.TemplateName != this._selectedTemplate)
					color = Color.Gray;
				else if(item.IsSystem() && RequestBuilder.HttpItemPropertyCache.ContainsKey(item.GetKey()))
					color = Color.Green;
				else
					color = Color.Empty;
				row.DefaultCellStyle.ForeColor = color;
			}

			base.OnRowPrePaint(e);
		}

		protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
		{
			if(e.Control is TextBox ctlText)
				if(this.CurrentCell.ColumnIndex == 0)
				{
					ctlText.AutoCompleteMode = AutoCompleteMode.Suggest;
					ctlText.AutoCompleteSource = AutoCompleteSource.CustomSource;

					IEnumerable<String> autoCompleteData = RequestBuilder.HttpItemPropertyCache.Keys
						.Select(k => Constant.Project.TemplateDefaultValuePrefix + k)
						.Union(new String[] { Constant.Project.DiscardValueName });

					var source = new AutoCompleteStringCollection();
					source.AddRange(autoCompleteData.ToArray());
					ctlText.AutoCompleteCustomSource = source;
				} else
					ctlText.AutoCompleteCustomSource = null;

			base.OnEditingControlShowing(e);
		}

		private Boolean _isCellValidated = false;

		protected override void OnCellValidating(DataGridViewCellValidatingEventArgs e)
		{
			this._isCellValidated = false;
			var row = base.Rows[e.RowIndex];
			TemplateItemSource item = (TemplateItemSource)row.DataBoundItem;
			String newValue = (String)e.FormattedValue;

			if(row.IsNewRow && String.IsNullOrEmpty(newValue))
				return;

			switch(e.ColumnIndex)
			{
			case 0://Key
				if(item.Key == newValue)
					return;

				TemplateItem old = this.GetTemplateRows().FirstOrDefault(t => t.Key == newValue);
				if(old != null)
				{
					if(String.IsNullOrEmpty(old.Value))
					{
						Int32 index = Array.IndexOf(this.GetTemplateRows().ToArray(), old);
						this.Rows.RemoveAt(index);
					} else
					{
						e.Cancel = true;
						return;
					}
				}
				break;
			case 1://Value
				if(String.IsNullOrEmpty(newValue))
					return;
				if(item.Value == newValue)
					return;

				//Checking for property default value
				if(item.IsSystem())
					if(RequestBuilder.HttpItemPropertyCache.TryGetValue(item.GetKey(), out PropertyInfo prop))
						e.Cancel = !Utils.TryConvert<String>(newValue, prop.PropertyType, out _);
					else
						e.Cancel = true;//Including discarded value
				break;
			}

			if(!e.Cancel)
				this._isCellValidated = true;

			base.OnCellValidating(e);
		}

		protected override void OnCellValidated(DataGridViewCellEventArgs e)
		{
			if(!this._isCellValidated)
				return;

			TemplateItemSource item = (TemplateItemSource)this.Rows[e.RowIndex].DataBoundItem;
			if(String.IsNullOrEmpty(item.Key) || String.IsNullOrEmpty(item.Value))
				return;

			item.TemplateName = this._selectedTemplate;

			this.ToggleTemplatesDirty();

			base.OnCellValidated(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyCode)
			{
			case Keys.Delete:
			case Keys.Back:
				Boolean isRemoved = false;
				foreach(DataGridViewRow row in this.SelectedRows)
					if(!row.IsNewRow)
					{
						this.Rows.Remove(row);
						isRemoved = true;
					}

				if(isRemoved)
					this.ToggleTemplatesDirty();
				break;
			}
			base.OnKeyDown(e);
		}
	}
}