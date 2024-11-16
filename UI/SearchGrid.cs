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

		/// <summary>Таблица по которой осуществлять поиск</summary>
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

		/// <summary>Таблица, по которой осуществлять поиск</summary>
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
				//if(base.Visible != value)//DockStyle=Fill перекрывает открытый контролл поиска
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
						case DockStyle.Top://Присобачить поиск к верхней части таблицы
							if(value)
							{
								if(this.GridControl.Dock != DockStyle.Fill)
									this.GridControl.Location = new Point(location.X, location.Y + thisSize.Height);
								this.Location = new Point(location.X, location.Y);
							} else if(this.GridControl.Dock != DockStyle.Fill)
								this.GridControl.Location = new Point(location.X, location.Y - thisSize.Height);
							break;
						case DockStyle.Bottom://Присобачить поиск к нижней части таблицы
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

		/// <summary>Текст для поиска</summary>
		[DefaultValue("")]
		public String TextToFind
		{
			get => txtFind.Text;
			set => this.FindText(value);
		}

		/// <summary>Разрешить поиск по регистру</summary>
		[DefaultValue(false)]
		public Boolean EnableFindCase
		{
			get => cbCase.Visible;
			set => cbCase.Visible = value;
		}

		/// <summary>Разрешить подсветку найденных строк</summary>
		[DefaultValue(false)]
		public Boolean EnableFindHilight
		{
			get => cbHilight.Visible;
			set => cbHilight.Visible = value;
		}

		/// <summary>Разрешить передвижение вверх и вниз по найденным результатам</summary>
		[DefaultValue(false)]
		public Boolean EnableFindPrevNext
		{
			get => bnNext.Visible && bnPrevious.Visible;
			set => bnNext.Visible = bnPrevious.Visible = value;
		}

		/// <summary>Разрешиь подстветку текста при вводе</summary>
		public Boolean EnableSearchHilight { get; set; }

		public event EventHandler<EventArgs> OnSearch;
		public event EventHandler<EventArgs> OnSearchClosed;

		public SearchGrid()
		{
			InitializeComponent();
			this.Visible = false;
		}

		public void FindText(String text)
			=> this.FindText(text,cbHilight.Checked);

		public void FindText(String text, Boolean isHilight)
		{
			txtFind.Text = text;
			if(!String.IsNullOrEmpty(text))
				this.Visible = true;
			cbHilight.Checked = isHilight;
		}

		void GridControl_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.F | Keys.Control:
				//if(!this.Visible)//DockStyle=Fill перекрывает открытый контролл поиска
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
				case DockStyle.Top://Присобачить поиск к верхней части таблицы
					break;
				case DockStyle.Bottom://Присобачить поиск к нижней части таблицы
					this.Location = new Point(location.X, location.Y + size.Height);
					break;
				default: throw new NotImplementedException();
				}
			}
		}

		/// <summary>Привязка данных закончена</summary>
		private void DataGrid_DataBindingComplete(Object sender, DataGridViewBindingCompleteEventArgs e)
		{
			if(e.ListChangedType == ListChangedType.Reset)
				this.txtFind_TextChanged(txtFind, e);
		}

		/// <summary>Обработка кнопок фокуса на элементе поиска</summary>
		private void SearchGrid_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Enter:
				if(this.OnSearch != null)
					this.OnSearch.Invoke(this, EventArgs.Empty);
				else//If Enter key is not overrided, then using default search behavior on focus on Text field
					this.FindNextCell(txtFind.Text, cbCase.Checked);
				e.Handled = true;
				break;
			case Keys.Escape:
				this.bnClose_Click(sender, e);
				e.Handled = true;
				break;
			}
		}

		/// <summary>Перевезти подсветку на следующую найденную ячейку</summary>
		private void bnNext_Click(Object sender, EventArgs e)
			=> this.FindNextCell(txtFind.Text, cbCase.Checked);

		private void bnPrevious_Click(Object sender, EventArgs e)
			=> this.FindPreviousCell(txtFind.Text);

		/// <summary>Подсветка найденных ячеек</summary>
		private void cbHilight_CheckedChanged(Object sender, EventArgs e)
		{
			if(cbHilight.Checked)
				this.ColorizeFounded(txtFind.Text);
			else
				this.ClearColorize();
		}

		/// <summary>Поиск используя регистр</summary>
		private void cbCase_CheckedChanged(Object sender, EventArgs e)
			=> this.txtFind_TextChanged(sender, e);

		/// <summary>Поиск по изменившемуся тексту</summary>
		private void txtFind_TextChanged(Object sender, EventArgs e)
		{
			if(this.Visible && txtFind.ForeColor == Color.Empty)
			{
				Boolean isFound = false;
				if(cbHilight.Checked)
					isFound = this.ColorizeFounded(txtFind.Text);
				else
					isFound = this.FindFirstCell(txtFind.Text);
				if(this.EnableSearchHilight)
					this.HilightSearch(isFound);
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

		/// <summary>Отвязать события от элемента управления</summary>
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

		/// <summary>Привязать события к элкменту управления</summary>
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

		/// <summary>Подсветка строки поиска текста</summary>
		/// <param name="isFounded">Значения найдены</param>
		private void HilightSearch(Boolean isFounded)
		{
			isFounded = isFounded || String.IsNullOrEmpty(txtFind.Text);
			txtFind.BackColor = isFounded ? Color.Empty : Color.Red;
			txtFind.ForeColor = isFounded ? Color.Empty : Color.White;
		}

		/// <summary>Поиск первой найденной ячейки</summary>
		/// <remarks>Возможно, колонка будет не текстовая, но тогда не возможно определить пользоватьельские колонки</remarks>
		/// <param name="text">Текст для поиска</param>
		/// <param name="matchCase">Текст в ячейке должен соответствовать регистру</param>
		/// <returns>Найденная первая ячейка или null</returns>
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

		/// <summary>Найти следующую ячейку содержащую найденный текст</summary>
		/// <param name="text">Текст для поиска</param>
		/// <param name="matchCase">Соответствовать регистру</param>
		/// <returns>Найденная ячейка или null</returns>
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
						this.ListView.SelectedIndices.Clear();//Убираю все выделенные элементы (Нам надо показывать один новый найденный, а не выбранные)
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
								return true;//Поиск соответсвующих узлов в соседних узлах.
						} while(root != null);
					}
				}
			}
			return false;
		}

		/// <summary>Найти текст в предыдущей ячейке от выделенной</summary>
		/// <param name="text">Текст для поика</param>
		/// <param name="matchCase">Соответствовать регистру</param>
		/// <returns>Найденная ячейка или null</returns>
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
						this.ListView.SelectedIndices.Clear();//Убираю все выделенные элементы (Нам надо показывать один новый найденный, а не выбранные)
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
								return true;//Поиск соответсвующих узлов в соседних узлах.
						} while(root != null);
					}
				}
			}
			return false;
		}

		/// <summary>Подсветить найденные ячейки</summary>
		/// <remarks>Возможно, колонка будет не текстовая, но тогда не возможно определить пользоватьельские колонки</remarks>
		/// <param name="text">Текст для поиска в ячейках</param>
		/// <param name="matchCase">Соответствовать регистру</param>
		/// <returns>Успех нахождения хоть одной записи</returns>
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

		/// <summary>Поиск в ячейке текста</summary>
		/// <param name="cell">Ячейка, в которой осуществлять поиск текста</param>
		/// <param name="text">Текст для поиска в ячейке</param>
		/// <param name="matchCase">Соответствовать регистру</param>
		/// <returns>Текст в ячейке найден</returns>
		private Boolean FindInCell(Object cellText, String text)
			=> cellText != null
				&& cellText.ToString().IndexOf(text, cbCase.Checked ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase) > -1;

		/// <summary>Отчистить подсветку таблицы</summary>
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
					this.ClearColorizeRec(node);
				}
			}
		}

		private void ClearColorizeRec(TreeNode root)
		{
			foreach(TreeNode node in root.Nodes)
			{
				if(node.BackColor == SearchGrid.FoundBackColor)
					node.BackColor = Color.Empty;
				this.ClearColorizeRec(node);
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