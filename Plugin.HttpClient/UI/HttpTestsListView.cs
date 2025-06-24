using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Plugin.HttpClient.Events;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.UI
{
	internal class HttpTestsListView : TreeListView
	{
		private const String ClipboardMagic = "Clipboard";
		private enum StorageType
		{
			Unknown = 0,
			Binary = 1,
			Json = 2,
			Xml = 3,
		}

		private HttpProject _project;

		public HttpProject Project
		{
			get => this._project ?? (this._project = new HttpProject());
			private set => this._project = value;
		}

		public new HttpProjectItem SelectedObject
		{
			get => (HttpProjectItem)base.SelectedObject;
			set => base.SelectedObject = value;
		}

		public String FilePath { get; private set; }

		public Boolean IsDirty { get; private set; }

		[Browsable(false)]
		public PluginWindows Plugin { get; set; }

		public event EventHandler<ToggleProjectDirtyEventArgs> DirtyChanged;

		protected override void OnCreateControl()
		{
			if(this.IsDesignMode) return;

			base.CanExpandGetter = (item) =>
			{
				return ((HttpProjectItem)item).Items.Count > 0;
			};
			base.ChildrenGetter = (item) =>
			{
				return ((HttpProjectItem)item).Items;
			};

			OLVColumn[] columns = Generator.GenerateColumns(typeof(HttpProjectItem)).ToArray();
			OLVColumn colAddress = Array.Find(columns, c => c.AspectName == nameof(HttpProjectItem.AddressReal))
				?? throw new InvalidOperationException("Address column not found");
			colAddress.ImageGetter = (item) => (Int32)((HttpProjectItem)item).Image;

			base.AllColumns.AddRange(columns);
			base.Columns.AddRange(columns.Where(c => c.IsVisible == true).ToArray());

			this.TreeColumnRenderer.IsShowGlyphs = true;
			this.TreeColumnRenderer.UseTriangles = true;

			this.UseNotifyPropertyChanged = true;//INotifyPropertyChanged implementation

			// Performance optimization to reduce OnHover redraw
			this.UseHotItem = false;
			this.UseHyperlinks = false;
			this.UseHotControls = false;

			this.IsSimpleDragSource = true;
			this.DropSink = new RearrangingDropSink(true)
			{
				CanDropBetween = true,
				CanDropOnBackground = true,
				CanDropOnItem = true,
			};
			/*base.IsSimpleDropSink = true;
			SimpleDropSink dropSing = (SimpleDropSink)base.DropSink;
			dropSing.AcceptExternal = false;
			dropSing.CanDropBetween = true;
			dropSing.CanDropOnBackground = true;*/

			base.OnCreateControl();
		}

		/// <summary>Adds project item node</summary>
		public void AddNewNode()
		{
			if(base.IsCellEditing)
				return;//Editing available only once at a time

			HttpProjectItem selected = (HttpProjectItem)this.SelectedObject;
			HttpProjectItem newItem = new HttpProjectItem();
			if(selected == null)
			{
				this.Project.Items.Add(newItem);
				this.AddObject(newItem);
			} else
			{
				selected.Items.Add(newItem);
				this.RefreshObject(selected);
				this.Expand(selected);
			}

			this.SelectedObject = newItem;
			this.EnsureVisible(this.SelectedIndex);
			this.EditModel(newItem);
		}

		protected override void OnCellEditStarting(CellEditEventArgs e)
		{
			HttpProjectItem item = (HttpProjectItem)e.RowObject;
			if(item != null && item.Address != item.AddressReal)
				((TextBox)e.Control).Text = item.Address;
			else if(Object.ReferenceEquals(item.HttpResponse, ClipboardMagic))
			{
				item.HttpResponse = null;
				throw new NotImplementedException();
			}
			base.OnCellEditStarting(e);
		}

		protected override void OnCellEditorValidating(CellEditEventArgs e)
		{
			String address = (String)e.NewValue;
			if(String.IsNullOrEmpty(address))
				e.Cancel = true;
			base.OnCellEditorValidating(e);
		}

		protected override void OnCellEditFinishing(CellEditEventArgs e)
		{
			HttpProjectItem item = (HttpProjectItem)e.RowObject;
			String newValue = (String)e.NewValue;
			HttpProjectItem parentItem = (HttpProjectItem)this.GetParent(item);
			if(e.Cancel == true || newValue.Length == 0)
			{
				if(item.Address == null)
				{
					parentItem?.Items.Remove(item);
					this.RemoveObject(item);
				}
				return;
			} else
			{
				item.Address = newValue;
				this.ToggleDirty(true);
				this.UpdateObject(item);
			}
		}

		protected override void OnItemsAdding(ItemsAddingEventArgs e)
		{
			if(this.IsCellEditing && e.ObjectsToAdd.Count > 0)
				e.Canceled = true;//I don't know why but if we invoke this.UpdateObject for existing node while editing, and node is a child node - new node will be added to the end of the list

			base.OnItemsAdding(e);
		}

		protected override void OnCanDrop(OlvDropEventArgs args)
		{
			Boolean canDrop = args.DragEventArgs.Data.GetDataPresent(DataFormats.FileDrop)
				&& args.DropTargetLocation == DropTargetLocation.Background;//TODO: Implement ability to drop items inside or between specific nodes

			if(canDrop)
			{
				String[] files = (String[])args.DragEventArgs.Data.GetData(DataFormats.FileDrop);
				canDrop = Array.TrueForAll(files, f => Constant.Project.Extensions.IsAssembly(f) || Constant.Project.Extensions.IsProject(f));
			}

			args.Effect = canDrop ? DragDropEffects.Move : DragDropEffects.None;

			base.OnCanDrop(args);
		}

		protected override void OnDropped(OlvDropEventArgs args)
		{
			String[] files = (String[])args.DragEventArgs.Data.GetData(DataFormats.FileDrop);

			Boolean isImported = false;
			foreach(String filePath in files)
				if(Constant.Project.Extensions.IsAssembly(filePath))
				{
					HttpProject project = this.Project;
					if(project.ImportAssembly(this.Plugin.Settings.GetServerUrl(), filePath))
						isImported = true;
				}else if(Constant.Project.Extensions.IsProject(filePath))
				{
					HttpProject project = this.Project;
					if(project.ImportProject(filePath))
						isImported = true;
				}

			if(isImported)
				this.UpdateProjectItems();

			base.OnDropped(args);
		}

		protected override void OnModelCanDrop(ModelDropEventArgs args)
		{
			args.Handled = true;
			args.Effect = DragDropEffects.None;
			if(args.SourceModels.Contains(args.TargetModel))
				args.InfoMessage = "It is impossible to drop on oneself";
			else
				args.Effect = DragDropEffects.Move;

			base.OnModelCanDrop(args);
		}

		protected override void OnModelDropped(ModelDropEventArgs args)
		{
			HttpProjectItem target = (HttpProjectItem)args.TargetModel;

			foreach(HttpProjectItem item in args.SourceModels.Cast<HttpProjectItem>())
			{
				if(item.Items.Project != null)
				{
					HttpProjectItem parent = this.Project.Items.FindParentByReference(item);
					if(parent == null)
						this.Project.Items.Remove(item);
					else
						parent.Items.Remove(item);
				}

				HttpProjectItemCollection collection;
				switch(args.DropTargetLocation)
				{
				case DropTargetLocation.None:
				case DropTargetLocation.Background:
					collection = this.Project.Items;
					break;
				case DropTargetLocation.Item:
					collection = target.Items;
					break;
				case DropTargetLocation.BelowItem:
				case DropTargetLocation.AboveItem:
					HttpProjectItem parent = this.Project.Items.FindParentByReference(target);
					collection = parent == null
						? this.Project.Items
						: parent.Items;
					break;
				default:
					throw new NotImplementedException();
				}

				switch(args.DropTargetLocation)
				{
				case DropTargetLocation.None:
				case DropTargetLocation.Item:
				case DropTargetLocation.Background:
					collection.Add(item);
					break;
				case DropTargetLocation.BelowItem:
					collection.Insert(item, target, true);
					break;
				case DropTargetLocation.AboveItem:
					collection.Insert(item, target, false);
					break;
				default:
					throw new NotImplementedException();
				}
			}

			args.RefreshObjects();
			/*if(target != null)
			{
				HttpProjectItem targetParent = this.Project.FindParentByReference(target);
				if(targetParent != null)
					this.RefreshObject(targetParent);
			}*/

			this.ToggleDirty(true);
			base.OnModelDropped(args);
		}

		public void SaveProject()
		{
			if(this.IsDirty)
			{
				if(this.FilePath == null)
					this.SaveProjectToStorage();
				else
					this.SaveProjectToFile(false);
			}
		}

		public void SaveProjectToStorage()
			=> this.SaveProject(null);

		public void SaveProjectToFile(Boolean showSaveDialog)
		{
			String filePath = this.FilePath;

			if(showSaveDialog)
				using(SaveFileDialog dlg = new SaveFileDialog()
				{
					Filter = Constant.Project.Extensions.CreateFilter(),
					DefaultExt = Constant.Project.Extensions.Xml,
					AddExtension = true,
					OverwritePrompt = true,
					FileName = this.FilePath,
				})
					if(dlg.ShowDialog() == DialogResult.OK)
						filePath = dlg.FileName;
					else
						return;

			this.SaveProject(filePath);
		}

		private void SaveProject(String filePath)
		{
			if(filePath == null)
				this.Plugin.Settings.SaveProject(this.Project);
			else
				this.Project.Save(filePath);

			this.FilePath = filePath;
			this.ToggleDirty(false);
		}

		public void RemoveSelectedHttpTest()
		{
			HttpProjectItem[] items = this.SelectedObjects.Cast<HttpProjectItem>().ToArray();
			Boolean isMultiDelete = false;

			if(items.Length == 0)
				return;
			else if(items.Length > 1)
			{
				isMultiDelete = true;
				if(MessageBox.Show("Are you sure you want to remove selected nodes?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					return;
			}

			foreach(HttpProjectItem item in items)
			{
				if(item == null) continue;

				if(isMultiDelete || MessageBox.Show("Are you sure you want to remove selected item?", item.Address, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if(!this.Project.Items.Remove(item))
						throw new InvalidOperationException($"Error removing element {item} from array");

					this.ToggleDirty(true);
					this.RemoveObject(item);
				}
			}
		}

		public void UpdateNodesText()
			=> this.Refresh();

		public void ToggleDirty(Boolean isDirty)
		{
			this.IsDirty = isDirty;
			this.DirtyChanged?.Invoke(this, new ToggleProjectDirtyEventArgs(this.FilePath, isDirty));
		}

		public HttpProjectItem FindNode(Func<HttpProjectItem, Boolean> callback)
			=> FindNode(this.Objects.Cast<HttpProjectItem>(), callback);

		private static HttpProjectItem FindNode(IEnumerable<HttpProjectItem> items, Func<HttpProjectItem, Boolean> callback)
		{
			foreach(HttpProjectItem item in items)
			{
				if(callback(item))
					return item;
				else if(item.Items.Count > 0)
				{
					HttpProjectItem result = FindNode(item.Items, callback);
					if(result != null) return result;
				}
			}

			return null;
		}

		/// <summary>Load project items to the list view</summary>
		/// <param name="filePath">Path to the project with HTTP test items</param>
		public void LoadProject(String filePath = null)
		{
			this.FilePath = filePath;
			this.Project = filePath == null
				? this.Plugin.Settings.LoadProject()//Project loaded from internal storage
				: File.Exists(filePath)
					? HttpProject.Load(filePath)//Project loaded from file system
					: new HttpProject();//Creating a new project

			this.UpdateProjectItems();

			this.ToggleDirty(false);
		}

		public void UpdateProjectItems()
		{//TODO: I don't like this idea. Better to add new items
			base.ClearObjects();
			this.SetObjects(this.Project.Items);
		}
	}
}