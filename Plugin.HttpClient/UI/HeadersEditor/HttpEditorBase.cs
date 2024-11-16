﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Plugin.HttpClient.UI
{
	public abstract class HttpEditorBase : UITypeEditor
	{
		private IWindowsFormsEditorService _editorService;

		public abstract IEnumerable<KeyValuePair<String, String>> GetValues();

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown; //base.GetEditStyle(context);

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			_editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			// use a list box
			ListBox lb = new ListBox
			{
				SelectionMode = SelectionMode.One,
				DisplayMember = "Key",
				ValueMember = "Value",
			};
			lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

			//context.Instance
			foreach(KeyValuePair<String, String> item in this.GetValues())
			{
				Int32 index = lb.Items.Add(item);
				if(item.Value.Equals(value))
					lb.SelectedIndex = index;
			}

			// show this model stuff
			_editorService.DropDownControl(lb);
			return lb.SelectedItem == null
				? value // no selection, return the passed-in value as is
				: ((KeyValuePair<String, String>)lb.SelectedItem).Value;
		}

		private void OnListBoxSelectedValueChanged(Object sender, EventArgs e)
			=> _editorService.CloseDropDown();// close the drop down as soon as something is clicked
	}
}