using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AlphaOmega.Windows.Forms;
using Plugin.HttpClient.Project;

namespace Plugin.HttpClient.UI
{
	internal class HttpTestsTreeView : DraggableTreeView<ProjectTreeNode, ProjectTreeNode.TagExtender>
	{
		private enum StorageType
		{
			Unknown = 0,
			Binary = 1,
			Json = 2,
			Xml = 3,
		}

		private static class NativeMethods
		{
			[DllImport("User32.dll", EntryPoint = "SendMessage", SetLastError = true)]
			public static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wParam, String lParam);

			public const Int32 TVM_GETEDITCONTROL = 0x110F;
			public const Int32 WM_SETTEXT = 0xC;
		}

		private HttpProject _project;

		public HttpProject Project
		{
			get => this._project ?? (this._project = new HttpProject());
			private set => this._project = value;
		}

		public String FilePath { get; private set; }

		public Boolean IsDirty { get; private set; }

		public PluginWindows Plugin { get; set; }

		public new ProjectTreeNode SelectedNode
		{
			get => (ProjectTreeNode)base.SelectedNode;
			set => base.SelectedNode = value;
		}

		private const String ClipboardMagic = "Clipboard";
		internal const Int32 MaxTextLength = 259;//SEE: https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.treenode.text

		public event EventHandler<EventArgs> DirtyChanged;

		protected override Boolean IsFolderNode(ProjectTreeNode treeNode)
		{
			switch(treeNode.ImageIndex)
			{
			case ProjectTreeNode.TreeImageList.Failure:
			case ProjectTreeNode.TreeImageList.New:
			case ProjectTreeNode.TreeImageList.Running:
			case ProjectTreeNode.TreeImageList.Success:
			case ProjectTreeNode.TreeImageList.Skip:
				return true;//Элементы тестов могут быть дочерними
			case ProjectTreeNode.TreeImageList.Folder://TODO: Это заглушка, которая никогда не должна сработать
				return false;
			default:
				throw new NotImplementedException();
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.V | Keys.Control:
				if(Clipboard.ContainsText())
				{
					e.Handled = true;
					ProjectTreeNode node = new ProjectTreeNode(Clipboard.GetText())
					{
						HttpResponse = ClipboardMagic,
					};
					base.Nodes.Add(node);
					node.BeginEdit();
				}
				break;
			}

			base.OnKeyDown(e);
		}

		protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
		{
			ProjectTreeNode node = (ProjectTreeNode)e.Node;

			HttpProjectItem item = node.Project;
			if(item != null && item.AddressReal?.Length > HttpTestsTreeView.MaxTextLength)
			{//HACK: We can't edit such big string using TreeView UI (Use PropertyGrid instead for editing)
				e.CancelEdit = true;
				return;
			}

			if(item != null && item.Address != item.AddressReal)
				SetEditCtrlText(item.Address);
			else if(Object.ReferenceEquals(node.HttpResponse, ClipboardMagic))
			{
				node.HttpResponse = null;
				SetEditCtrlText(e.Node.Text);
			}

			base.OnBeforeLabelEdit(e);

			void SetEditCtrlText(String text)
			{
				IntPtr hwndEdit = NativeMethods.SendMessage(base.Handle, NativeMethods.TVM_GETEDITCONTROL, IntPtr.Zero, null);
				if(hwndEdit == IntPtr.Zero)
					throw new Win32Exception();

				StringBuilder hwndText = new StringBuilder(text);
				NativeMethods.SendMessage(hwndEdit, NativeMethods.WM_SETTEXT, IntPtr.Zero, hwndText.ToString());
			}
		}

		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			ProjectTreeNode node = (ProjectTreeNode)e.Node;

			if(String.IsNullOrEmpty(e.Label))
			{
				if(node.Project == null)
					node.Remove();
				else
					e.CancelEdit = true;
			} else
			{
				e.CancelEdit = true;

				if(node.Project == null)
				{
					HttpProjectItemCollection parentCollection = node.Parent == null ? this.Project.Items : node.Parent.Project.Items;
					node.Project = parentCollection.Add(e.Label);
					this.SelectedNode = node;
				} else
					node.Project.Address = e.Label;

				node.UpdateNodeText();
				this.ToggleDirty(true);
			}

			base.OnAfterLabelEdit(e);
		}

		protected override void OnDragDrop(DragEventArgs args)
		{
			base.OnDragDrop(args);

			if(base.IsDataPresent(args) && base.HasMoved)
			{
				ProjectTreeNode movingNode = base.GetDragDropNode(args);
				HttpProjectItem movingRow = movingNode == null
					? base.GetDragDropData(args).Project
					: movingNode.Project;

				if(movingNode == null)//Перенос из другого окна
				{
					this.Project.Items.Add(movingRow);
					ProjectTreeNode node = new ProjectTreeNode(movingRow);
					base.Nodes.Add(node);
				} else
				{//TODO: Тут надо высчитывать ноду от курсора, а не от родителя
					HttpProjectItem toRow = movingNode.Parent == null ? null : movingNode.Parent.Project;

					this.Project.Items.Move(toRow, movingRow, movingNode.Index);
				}
				this.ToggleDirty(true);
			}
		}

		public Boolean RemoveSelectedHttpTest()
		{
			ProjectTreeNode node = this.SelectedNode;
			Boolean result = MessageBox.Show("Are you shure you want to remove selected item?", node.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
			if(result)
			{
				HttpProjectItem item = node.Project;

				if(!this.Project.Items.Remove(item))
					throw new InvalidOperationException(String.Format("Ошибка удаления элемента {0} из массива", item));

				this.ToggleDirty(true);
				node.Remove();
			}

			return result;
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
					Filter = HttpProject.CreateFileExtensionsFilter(),
					DefaultExt = Constant.Project.Extensions.Binary,
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

		public void AddProjectNode(HttpProjectItem item)
		{
			ProjectTreeNode node = new ProjectTreeNode(item);
			base.Nodes.Add(node);
		}

		/// <summary></summary>
		/// <param name="filePath"></param>
		public void LoadProject(String filePath = null)
		{
			if(filePath == null)//Project loaded from internal storage
				this.Project = this.Plugin.Settings.LoadProject();
			else if(File.Exists(filePath))//Project loaded from file system
				this.Project = HttpProject.Load(filePath);
			else//Createing a new project
				this.Project = new HttpProject();

			base.SuspendLayout();
			try
			{
				base.Nodes.Clear();
				foreach(HttpProjectItem item in this.Project.Items)
					this.AddProjectNode(item);
			} finally
			{
				base.ResumeLayout();
			}

			this.FilePath = filePath;
			this.ToggleDirty(false);
		}

		public ProjectTreeNode FindNode(HttpProjectItem item)
		{
			foreach(ProjectTreeNode node in base.EnumerateNodes(null))
				if(node.Project == item)
					return node;
			return null;
		}

		public void UpdateNodesText()
		{
			foreach(ProjectTreeNode node in this.EnumerateNodes(null))
				node.UpdateNodeText();

			this.ToggleDirty(true);
		}

		public void ToggleDirty(Boolean isDirty)
		{
			this.IsDirty = isDirty;
			this.DirtyChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}