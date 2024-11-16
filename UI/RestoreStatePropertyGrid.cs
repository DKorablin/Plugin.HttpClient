using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.Test;

namespace Plugin.HttpClient.UI
{
	internal class RestoreStatePropertyGrid : PropertyGrid
	{
		private class GridItemCopy
		{
			public String Label { get; set; }
			public GridItemType GridItemType { get; set; }
			public Boolean Expanded { get; set; }
			public GridItemCopy(GridItem item)
			{
				this.Label = item.Label;
				this.GridItemType = item.GridItemType;
				this.Expanded = item.Expanded;
			}

			public Boolean Equals(GridItem item)
				=> this.Label == item.Label && this.GridItemType == item.GridItemType;
		}

		public new Object SelectedObject
		{
			get => base.SelectedObject;
			set
			{
				Object selected = base.SelectedObject;
				if(selected == null || value == null || selected.GetType() != value.GetType())
				{
					base.SelectedObject = value;
					return;
				}

				this.SuspendLayout();
				try
				{
					ScrollBar sb = this.GetScrollBar();
					Int32 position = sb.Value;
					GridItemCopy[] expandableItems = this.CollectExpandableItems().ToArray();
					base.SelectedObject = value;
					this.RestoreExpandedState(expandableItems);
					sb.Value = position;
				} finally
				{
					this.ResumeLayout();
				}
			}
		}

		public HttpProjectItem SelectedItem
		{
			get
			{
				var wrapper = (DynamicAttributesTypeDescriptor<HttpProjectItem>)this.SelectedObject;
				return wrapper.Instance;
			}
			set
			{
				var wrapper = new DynamicAttributesTypeDescriptor<HttpProjectItem>(value)
				{
					DynamicProperties = value.Items.Project.Templates.GetTemplateValuesSorted(true)
						.Where(t => RequestBuilder.HttpItemPropertyCache.ContainsKey(t.GetKey()))
						.Select(t => new { Template = t, RequestBuilder.HttpItemPropertyCache[t.GetKey()].PropertyType, })
						.ToDictionary(
							k => k.Template.GetKey(),
							v => new Attribute[] { new DefaultValueAttribute(Utils.Convert(v.Template.Value, v.PropertyType)), }),
				};
				this.SelectedObject = wrapper;
			}
		}

		private GridItem GetRootItem()
		{
			GridItem root = this.SelectedGridItem;
			while(root.Parent != null)
				root = root.Parent;

			return root;
		}

		private ScrollBar GetScrollBar()
		{//TODO: Not working in .NET 7
			foreach(Control ctl in this.Controls)
				if(ctl.Text == "PropertyGridView")
				{
					Type type = ctl.GetType();// .NET 7 fix
					FieldInfo field = type.GetField("_scrollBar", BindingFlags.Instance | BindingFlags.NonPublic);

					return field == null
						? (ScrollBar)ctl
							.GetType()
							.InvokeMember("scrollBar", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField, null, ctl, null)
						: (ScrollBar)field.GetValue(ctl);
				}

			return null;
		}

		private IEnumerable<GridItemCopy> CollectExpandableItems()
		{
			GridItem root = this.GetRootItem();

			if(root != null)
				foreach(GridItem g in this.FlattenGridItems(root))
					if(g.Expandable)
						yield return new GridItemCopy(g);
		}

		private void RestoreExpandedState(GridItemCopy[] expandableItems)
		{
			GridItem root = this.GetRootItem();

			if(root != null)
				foreach(GridItem g in this.FlattenGridItems(root))
					if(g.Expandable)
						foreach(GridItemCopy item in expandableItems)
							if(item.Equals(g) && g.Expanded != item.Expanded)
							{
								g.Expanded = item.Expanded;
								break;
							}
		}

		private IEnumerable<GridItem> FlattenGridItems( GridItem root)
		{
			foreach(GridItem item in root.GridItems)
			{
				yield return item;

				if(item.Expandable)
					foreach(GridItem subItem in this.FlattenGridItems(item))
						yield return subItem;
			}
		}
	}
}