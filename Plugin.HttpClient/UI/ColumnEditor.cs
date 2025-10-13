using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Plugin.HttpClient.UI
{
	internal class ColumnEditor<T> : UITypeEditor
	{
		private ColumnEditorControl _control;

		#region Methods
		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			if(this._control == null)
				this._control = new ColumnEditorControl();

			this._control.SetStatus((T[])value);

			((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService))).DropDownControl(this._control);

			return this._control.Result;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown;

		#endregion Methods
		private sealed class ColumnEditorControl : UserControl
		{
			private readonly CheckedListBox cblColumns = new CheckedListBox();

			public T[] Result
			{
				get
				{
					List<T> result = new List<T>();

					for(Int32 loop = 0; loop < cblColumns.Items.Count; loop++)
						if(cblColumns.GetItemChecked(loop))
							result.Add((T)cblColumns.Items[loop]);
					return result.ToArray();
				}
			}

			public ColumnEditorControl()
			{
				base.SuspendLayout();
				base.BackColor = SystemColors.Control;
				this.cblColumns.FormattingEnabled = true;
				this.cblColumns.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
				this.cblColumns.BorderStyle = BorderStyle.None;
				base.Size = new Size(this.cblColumns.Width, this.cblColumns.Height);

				cblColumns.Items.Clear();
				foreach(Object value in Enum.GetValues(typeof(T)))
					cblColumns.Items.Add(value);

				base.Controls.AddRange(new Control[] { this.cblColumns });
				this.cblColumns.Focus();
				base.ResumeLayout();
			}

			public void SetStatus(T[] flags)
			{
				for(Int32 loop = 0; loop < cblColumns.Items.Count; loop++)
				{
					T item = (T)cblColumns.Items[loop];
					if(flags == null)
						cblColumns.SetItemChecked(loop, false);
					else
						foreach(T flag in flags)
							if(EqualityComparer<T>.Default.Equals(flag, item))
							{
								cblColumns.SetItemChecked(loop, true);
								break;
							}
				}
			}
		}
	}
}