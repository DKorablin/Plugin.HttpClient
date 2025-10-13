using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AlphaOmega.Windows.Forms
{
	internal partial class SearchGrid : UserControl
	{
		private static Color FoundBackColor = Color.GreenYellow;

		private static Color FoundSelectedBackColor = Color.ForestGreen;

		private DataGridView _dataGrid;

		private ListView _listView;

		private TreeView _treeView;

		private Boolean _lockSize;

		/// <summary>ListView to perform search against</summary>
		public ListView ListView
		{
			get => this._listView;
			set
			{
				if(value == null && this._listView != null)
					this.DetachEvents();

				this._listView = value;

				if(value != null)
					this.AttachEvents();
			}
		}

		/// <summary>DataGridView to perform search against</summary>
		public DataGridView DataGrid
		{
			get => this._dataGrid;
			set
			{
				if(value == null && this._dataGrid != null)
					this.DetachEvents();

				this._dataGrid = value;

				if(value != null)
					this.AttachEvents();
			}
		}

		public TreeView TreeView
		{
			get => this._treeView;
			set
			{
				if(value == null && this._treeView != null)
					this.DetachEvents();

				this._treeView = value;

				if(value != null)
					this.AttachEvents();
			}
		}

		private Control GridControl
		{
			get
			{
				if(this.DataGrid != null)
					return this.DataGrid;
				else if(this.ListView != null)
					return this.ListView;
				else if(this.TreeView != null)
					return this.TreeView;
				else
					return null;
			}
		}
		//public new DockStyle Dock { get; set; }

		public new Boolean Visible
		{
			get => base.Visible;
			set
			{
				//if(base.Visible != value)//DockStyle=Fill overlays the open search control
				{
					base.Visible = value;
					if(DesignMode) return;

					if(this.GridControl != null)
					{
						Point location = this.GridControl.Location;
						Size size = this.GridControl.Size;
						Size thisSize = this.Size;
						this.Size = new Size(size.Width, thisSize.Height);
						this._lockSize = true;
						if(this.GridControl.Dock != DockStyle.Fill)
							if(value)
								this.GridControl.Size = new Size(size.Width, size.Height - thisSize.Height);
							else
								this.GridControl.Size = new Size(size.Width, size.Height + thisSize.Height);

						switch(this.Dock)
						{
						case DockStyle.Top://Attach search panel to top of grid
							if(value)
							{
								if(this.GridControl.Dock != DockStyle.Fill)
									this.GridControl.Location = new Point(location.X, location.Y + thisSize.Height);
								this.Location = new Point(location.X, location.Y);
							} else if(this.GridControl.Dock != DockStyle.Fill)
								this.GridControl.Location = new Point(location.X, location.Y - thisSize.Height);
							break;
						case DockStyle.Bottom://Attach search panel to bottom of grid
							if(value)
								this.Location = new Point(location.X, size.Height - thisSize.Height);
							break;
						default:
							throw new NotImplementedException();
						}
						this._lockSize = false;
					}
				}
			}
		}

		/// <summary>Text to search</summary>
		[DefaultValue("")]
		public String TextToFind
		{
			get => txtFind.Text;
			set => this.FindText(value);
		}

		/// <summary>Enable case sensitive search</summary>
		[DefaultValue(false)]
		public Boolean EnableFindCase
		{
			get => cbCase.Visible;
			set => cbCase.Visible = value;
		}

		/// <summary>Enable highlighting of found rows</summary>
		[DefaultValue(false)]
		public Boolean EnableFindHighlight
		{
			get => cbHilight.Visible;
			set => cbHilight.Visible = value;
		}

		/// <summary>Enable previous/next navigation among found results</summary>
		[DefaultValue(false)]
		public Boolean EnableFindPrevNext
		{
			get => bnNext.Visible && bnPrevious.Visible;
			set => bnNext.Visible = bnPrevious.Visible = value;
		}

		/// <summary>Enable dynamic highlight while typing</summary>
		public Boolean EnableSearchHighlight { get; set; }

		public event EventHandler<EventArgs> OnSearch;
		public event EventHandler<EventArgs> OnSearchClosed;

		public SearchGrid()
		{
			InitializeComponent();
			this.Visible = false;
		}

		public void FindText(String text)
			=> this.FindText(text,cbHilight.Checked);

		public void FindText(String text, Boolean isHighlight)
		{
			txtFind.Text = text;
			if(!String.IsNullOrEmpty(text))
				this.Visible = true;
			cbHilight.Checked = isHighlight;
		}

		void GridControl_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.F | Keys.Control:
				//if(!this.Visible)//DockStyle=Fill overlays the open search control
				this.Visible = true;
				txtFind.Focus();
				txtFind.SelectAll();
				e.Handled = true;
				break;
			}
		}

		private void GridControl_Resize(Object sender, EventArgs e)
		{
			if(this.Visible && !this._lockSize)
			{
				Point location = this.GridControl.Location;
				Size size = this.GridControl.Size;
				Size thisSize = this.Size;
				this.Size = new Size(size.Width, thisSize.Height);
				switch(this.Dock)
				{
			case DockStyle.Top://Attach search panel to top of grid
					break;
			case DockStyle.Bottom://Attach search panel to bottom of grid
					this.Location = new Point(location.X, location.Y + size.Height);
					break;
				default: throw new NotImplementedException();
				}
			}
		}

		/// <summary>Data binding completed</summary>
		private void DataGrid_DataBindingComplete(Object sender, DataGridViewBindingCompleteEventArgs e)
		{
			if(e.ListChangedType == ListChangedType.Reset)
				this.txtFind_TextChanged(txtFind, e);
		}

		/// <summary>Handle key events while search box has focus</summary>
		private void SearchGrid_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Enter:
				if(this.OnSearch != null)
					this.OnSearch.Invoke(this, EventArgs.Empty);
				else//If Enter key is not overridden, then using default search behavior on focus on Text field
					this.FindNextCell(txtFind.Text, cbCase.Checked);
				e.Handled = true;
				break;
			case Keys.Escape:
				this.bnClose_Click(sender, e);
				e.Handled = true;
				break;
			}
		}

		/// <summary>Move highlight to next found cell</summary>
		private void bnNext_Click(Object sender, EventArgs e)
			=> this.FindNextCell(txtFind.Text, cbCase.Checked);

		private void bnPrevious_Click(Object sender, EventArgs e)
			=> this.FindPreviousCell(txtFind.Text);

		/// <summary>Highlight found cells</summary>
		private void cbHighlight_CheckedChanged(Object sender, EventArgs e)
		{
			if(cbHilight.Checked)
				this.ColorizeFounded(txtFind.Text);
			else
				this.ClearColorize();
		}

		/// <summary>Search using case sensitivity</summary>
		private void cbCase_CheckedChanged(Object sender, EventArgs e)
			=> this.txtFind_TextChanged(sender, e);

		/// <summary>Search when text changes</summary>
		private void txtFind_TextChanged(Object sender, EventArgs e)
		{
			if(this.Visible && txtFind.ForeColor == Color.Empty)
			{
				Boolean isFound = false;
				if(cbHilight.Checked)
					isFound = this.ColorizeFounded(txtFind.Text);
				else
					isFound = this.FindFirstCell(txtFind.Text);
				if(this.EnableSearchHighlight)
					this.HighlightSearch(isFound);
			}
		}

		private void txtFind_Enter(Object sender, EventArgs e)
		{
			if(txtFind.ForeColor == SystemColors.GrayText)
			{
				this.TextToFind = String.Empty;
				txtFind.ForeColor = Color.Empty;
			}
		}

		private void txtFind_Leave(Object sender, EventArgs e)
		{
			String text = this.TextToFind;
			if(String.IsNullOrEmpty(text))
			{
				txtFind.ForeColor = SystemColors.GrayText;
				this.TextToFind = "Find";
			} else
			{
				txtFind.ForeColor = Color.Empty;
			}
		}

		/// <summary>Detach events from underlying grid control</summary>
		private void DetachEvents()
		{
			if(this.DataGrid != null)
			{
				this.DataGrid.DataBindingComplete -= new DataGridViewBindingCompleteEventHandler(DataGrid_DataBindingComplete);
			} else if(this.ListView != null)
			{

			} else
				throw new NotSupportedException();

			this.GridControl.KeyDown -= new KeyEventHandler(GridControl_KeyDown);
			this.GridControl.Resize -= new EventHandler(GridControl_Resize);
		}

		/// <summary>Attach events to underlying grid control</summary>
		private void AttachEvents()
		{
			if(DesignMode)
				return;

			if(this.DataGrid != null)
				this.DataGrid.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DataGrid_DataBindingComplete);
			else if(this.ListView != null)
			{

			} else if(this.TreeView != null)
			{
			} else
				throw new NotSupportedException();

			this.GridControl.KeyDown+=new KeyEventHandler(GridControl_KeyDown);
			this.GridControl.Resize += new EventHandler(GridControl_Resize);
		}

		/// <summary>Highlight search textbox depending on match result</summary>
		/// <param name="isFounded">Values found</param>
		private void HighlightSearch(Boolean isFounded)
		{
			isFounded = isFounded || String.IsNullOrEmpty(txtFind.Text);
			txtFind.BackColor = isFounded ? Color.Empty : Color.Red;
			txtFind.ForeColor = isFounded ? Color.Empty : Color.White;
		}

		/// <summary>Find first matching cell</summary>
		/// <remarks>Column might not be textual; cannot determine user columns in that case</remarks>
		/// <param name="text">Text to search</param>
		/// <param name="matchCase">Cell text must match case</param>
		/// <returns>True if a cell was found</returns>
		private Boolean FindFirstCell(String text)
		{
			if(!String.IsNullOrEmpty(text))
				if(this.DataGrid != null)
				{
					foreach(DataGridViewRow row in this.DataGrid.Rows)
						foreach(DataGridViewCell cell in row.Cells)
							if(this.FindInCell(cell.Value, text))
							{
								this.DataGrid.CurrentCell = cell;
								return true;
							}
				} else if(this.ListView != null)
				{
					while(this.ListView.SelectedIndices.Count > 0)
						this.ListView.SelectedIndices.Remove(0);

					foreach(ListViewItem row in this.ListView.Items)
						foreach(ListViewItem.ListViewSubItem cell in row.SubItems)
							if(this.FindInCell(cell.Text, text))
							{
								row.Selected = true;
								row.EnsureVisible();
								return true;
							}
				} else if(this.TreeView != null)
				{
					this.TreeView.SelectedNode = null;
					foreach(TreeNode node in this.TreeView.Nodes)
						if(this.FindInCell(node.Text, text))
						{
							this.TreeView.SelectedNode = node;
							node.EnsureVisible();
							if(node.Parent != null && !node.Parent.IsExpanded)
								node.Parent.Expand();
							return true;
						} else if(this.FindFirstCellRec(text, node))
							return true;
				}
			return false;
		}

		private Boolean FindFirstCellRec(String text, TreeNode root)
		{
			foreach(TreeNode node in root.Nodes)
				if(this.FindInCell(node.Text, text))
				{
					this.TreeView.SelectedNode = node;
					node.EnsureVisible();
					if(node.Parent != null && !node.Parent.IsExpanded)
						node.Parent.Expand();
					return true;
				} else if(this.FindFirstCellRec(text, node))
					return true;
			return false;
		}

		private Boolean FindLastCellRec(String text, TreeNode root)
		{
			Int32 length = root.Nodes.Count;
			for(Int32 loop = length - 1;loop >= 0;loop--)
			{
				TreeNode node = root.Nodes[loop];
				if(this.FindInCell(node.Text, text))
				{
					this.TreeView.SelectedNode = node;
					node.EnsureVisible();
					if(node.Parent != null && !node.Parent.IsExpanded)
						node.Parent.Expand();
					return true;
				} else if(this.FindLastCellRec(text, node))
					return true;
			}
			return false;
		}

		/// <summary>Find next cell containing search text</summary>
		/// <param name="text">Text to search</param>
		/// <param name="matchCase">Match case</param>
		/// <returns>True if cell was found</returns>
		private Boolean FindNextCell(String text, Boolean matchCase)
		{
			if(!String.IsNullOrEmpty(text))
			{
				if(this.DataGrid != null)
				{
					Int32 columnIndex = this.DataGrid.CurrentCell.ColumnIndex + 1, rowIndex = this.DataGrid.CurrentCell.RowIndex;
					for(Int32 loop = rowIndex; loop < this.DataGrid.Rows.Count; loop++)
					{
						var row = this.DataGrid.Rows[loop];
						for(Int32 innerLoop = columnIndex; innerLoop < row.Cells.Count; innerLoop++)
							if(this.FindInCell(row.Cells[innerLoop].Value, text))
							{
								this.DataGrid.CurrentCell = row.Cells[innerLoop];
								return true;
							}
						columnIndex = 0;
					}
				} else if(this.ListView != null)
				{
					Int32 rowIndex;
					if(this.ListView.SelectedIndices.Count > 0)
					{
						rowIndex = this.ListView.SelectedIndices[this.ListView.SelectedIndices.Count - 1] + 1;
						this.ListView.SelectedIndices.Clear();//Remove all selected elements (We need to show one new found one, not the selected ones)
					} else
						rowIndex = 0;

					for(Int32 loop = rowIndex; loop < this.ListView.Items.Count; loop++)
					{
						ListViewItem row = this.ListView.Items[loop];
						foreach(ListViewItem.ListViewSubItem cell in row.SubItems)
							if(this.FindInCell(cell.Text, text))
							{
								row.Selected = true;
								row.EnsureVisible();
								return true;
							}
					}
				} else if(this.TreeView != null)
				{
					if(this.TreeView.SelectedNode == null)
						return this.FindFirstCell(text);
					else if(this.FindFirstCellRec(text, this.TreeView.SelectedNode))
						return true;
					else
					{
						TreeNode root = this.TreeView.SelectedNode;
						do
						{
							while(root != null && root.NextNode == null)
								root = root.Parent;
							if(root == null || root.NextNode == null)
								break;
							root = root.NextNode;

							if(this.FindInCell(root.Text, text))
							{
								this.TreeView.SelectedNode = root;
								root.EnsureVisible();
								if(root.Parent != null && !root.Parent.IsExpanded)
									root.Parent.Expand();
								return true;
							} else if(this.FindFirstCellRec(text, root))
								return true;//Search for matching nodes in neighboring nodes.
						} while(root != null);
					}
				}
			}
			return false;
		}

		/// <summary>Find previous cell containing search text</summary>
		/// <param name="text">Text to search</param>
		/// <param name="matchCase">Match case</param>
		/// <returns>True if cell was found</returns>
		private Boolean FindPreviousCell(String text)
		{
			if(!String.IsNullOrEmpty(text))
			{
				if(this.DataGrid != null)
				{
					Int32 columnIndex = this.DataGrid.CurrentCell.ColumnIndex - 1, rowIndex = this.DataGrid.CurrentCell.RowIndex;
					for(Int32 loop = rowIndex;loop >= 0;loop--)
					{
						DataGridViewRow row = this.DataGrid.Rows[loop];
						for(Int32 innerLoop = columnIndex;innerLoop >= 0;innerLoop--)
							if(this.FindInCell(row.Cells[innerLoop].Value, text))
							{
								this.DataGrid.CurrentCell = row.Cells[innerLoop];
								return true;
							}
						columnIndex = this.DataGrid.ColumnCount - 1;
					}
				} else if(this.ListView != null)
				{
					/*Int32 rowIndex = this.ListView.SelectedIndices.Count > 0
						? this.ListView.SelectedIndices[0]
						: this.ListView.Items.Count - 1;*/
					Int32 rowIndex;
					if(this.ListView.SelectedIndices.Count > 0)
					{
						rowIndex = this.ListView.SelectedIndices[this.ListView.SelectedIndices.Count - 1] - 1;
						this.ListView.SelectedIndices.Clear();//Remove all selected elements (We need to show one new found one, not the selected ones)
					} else
						rowIndex = this.ListView.Items.Count - 1;

					for(Int32 loop = rowIndex; loop >= 0; loop--)
					{
						ListViewItem row = this.ListView.Items[loop];
						for(Int32 innerLoop = row.SubItems.Count - 1; innerLoop >= 0; innerLoop--)
							if(this.FindInCell(row.SubItems[innerLoop].Text, text))
							{
								row.Selected = true;
								row.EnsureVisible();
								return true;
							}
					}
				} else if(this.TreeView != null)
				{
					if(this.TreeView.SelectedNode == null)
					{
						Int32 count = this.TreeView.Nodes.Count;
						for(Int32 loop = count - 1;loop >= 0;loop--)
						{
							TreeNode node = this.TreeView.Nodes[loop];
							if(this.FindInCell(node.Text, text))
							{
								this.TreeView.SelectedNode = node;
								node.EnsureVisible();
								if(node.Parent != null && !node.Parent.IsExpanded)
									node.Parent.Expand();
								return true;
							} else if(this.FindLastCellRec(text, node))
								return true;
						}
					} else if(this.FindLastCellRec(text, this.TreeView.SelectedNode))
						return true;
					else
					{
						TreeNode root = this.TreeView.SelectedNode;
						do
						{
							while(root != null && root.PrevNode == null)
								root = root.Parent;
							if(root == null || root.PrevNode == null)
								break;
							root = root.PrevNode;

							if(this.FindInCell(root.Text, text))
							{
								this.TreeView.SelectedNode = root;
								root.EnsureVisible();
								if(root.Parent != null && !root.Parent.IsExpanded)
									root.Parent.Expand();
								return true;
							} else if(this.FindLastCellRec(text, root))
								return true;//Search for matching nodes in neighboring nodes.
						} while(root != null);
					}
				}
			}
			return false;
		}

		/// <summary>Highlight all matching cells</summary>
		/// <remarks>Column might be non-text; then user columns cannot be determined</remarks>
		/// <param name="text">Text to search in cells</param>
		/// <param name="matchCase">Match case</param>
		/// <returns>True if at least one match found</returns>
		private Boolean ColorizeFounded(String text)
		{
			Boolean result = false, isSearchEmpty = String.IsNullOrEmpty(text);
			if(this.DataGrid != null)
			{
				foreach(DataGridViewRow row in this.DataGrid.Rows)
					foreach(DataGridViewCell cell in row.Cells)
						if(!isSearchEmpty
							&& this.FindInCell(cell.Value, text))
						{
							result = true;
							cell.Style.BackColor = SearchGrid.FoundBackColor;
							cell.Style.SelectionBackColor = SearchGrid.FoundSelectedBackColor;
						} else
						{
							cell.Style.BackColor = Color.Empty;
							cell.Style.SelectionBackColor = Color.Empty;
						}
			} else if(this.ListView != null)
			{
				for(Int32 loop = 0; loop < this.ListView.Items.Count; loop++)
				{//We have to use index because listView can be in the VirtualMode
					ListViewItem row = this.ListView.Items[loop];
					foreach(ListViewItem.ListViewSubItem cell in row.SubItems)
						if(!isSearchEmpty
							&& this.FindInCell(cell.Text, text))
						{
							result = true;
							cell.BackColor = SearchGrid.FoundBackColor;
							//SelectionBackColor not supported
						} else
							cell.BackColor = Color.Empty;
				}
			} else if(this.TreeView != null)
			{
				foreach(TreeNode node in this.TreeView.Nodes)
				{
					if(!isSearchEmpty
						&& this.FindInCell(node.Text, text))
					{
						result = true;
						node.BackColor = SearchGrid.FoundBackColor;
					} else
						node.BackColor = Color.Empty;
					Boolean subResult = this.ColorizeFoundedRec(text, node);
					if(subResult)
						result = true;
				}
			}
			return result;
		}

		private Boolean ColorizeFoundedRec(String text, TreeNode root)
		{
			Boolean result = false;
			Boolean isSearchEmpty = String.IsNullOrEmpty(text);
			foreach(TreeNode node in root.Nodes)
			{
				if(!isSearchEmpty
					&& this.FindInCell(node.Text, text))
				{
					result = true;
					node.BackColor = SearchGrid.FoundBackColor;
				} else
					node.BackColor = Color.Empty;
				Boolean subResult = this.ColorizeFoundedRec(text, node);
				if(subResult)
					result = true;
			}
			return result;
		}

		/// <summary>Search text inside a cell value</summary>
		/// <param name="cell">Cell value</param>
		/// <param name="text">Text to search</param>
		/// <param name="matchCase">Match case</param>
		/// <returns>True if found</returns>
		private Boolean FindInCell(Object cellText, String text)
			=> cellText != null
				&& cellText.ToString().IndexOf(text, cbCase.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase) > -1;

		/// <summary>Clear highlight from grid/list/tree</summary>
		private void ClearColorize()
		{
			if(this.DataGrid != null)
			{
				foreach(DataGridViewRow row in this.DataGrid.Rows)
					foreach(DataGridViewCell cell in row.Cells)
						if(cell.Style.BackColor==SearchGrid.FoundBackColor)
							cell.Style.SelectionBackColor = cell.Style.BackColor = Color.Empty;
			} else if(this.ListView != null)
			{
				for(Int32 loop = 0; loop < this.ListView.Items.Count; loop++)
				{// We have to use index iteration because ListView can be in the VirtualMode
					ListViewItem row = this.ListView.Items[loop];
					foreach(ListViewItem.ListViewSubItem cell in row.SubItems)
						if(cell.BackColor == SearchGrid.FoundBackColor)
							cell.BackColor = Color.Empty;
				}
			} else if(this.TreeView != null)
			{
				foreach(TreeNode node in this.TreeView.Nodes)
				{
					if(node.BackColor == SearchGrid.FoundBackColor)
						node.BackColor = Color.Empty;
					ClearColorizeRec(node);
				}
			}

			void ClearColorizeRec(TreeNode root)
			{
				foreach(TreeNode node in root.Nodes)
				{
					if(node.BackColor == SearchGrid.FoundBackColor)
						node.BackColor = Color.Empty;
					ClearColorizeRec(node);
				}
			}
		}

		private void bnClose_MouseHover(Object sender, EventArgs e)
			=> bnClose.ImageIndex = 4;

		private void bnClose_MouseLeave(Object sender, EventArgs e)
			=> bnClose.ImageIndex = 3;

		private void bnClose_Click(Object sender, EventArgs e)
		{
			this.ClearColorize();
			this.DataGrid?.Focus();
			this.ListView?.Focus();
			this.TreeView?.Focus();

			this.Visible = false;
			this.OnSearchClosed?.Invoke(this, e);
		}
	}
}