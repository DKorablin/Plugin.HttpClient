using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BrightIdeasSoftware;
using Plugin.HttpClient.Events;
using Plugin.HttpClient.Project;
using Plugin.HttpClient.Properties;
using Plugin.HttpClient.Test;
using Plugin.HttpClient.UI;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.HttpClient
{
	internal partial class PanelHttpClient : UserControl, IPluginSettings<PanelHttpClientSettings>
	{
		private const String Caption = "Http Test Client";
		private PanelHttpClientSettings _settings;

		private IWindow Window => (IWindow)base.Parent;

		private PluginWindows Plugin => (PluginWindows)this.Window.Plugin;

		Object IPluginSettings.Settings => this.Settings;

		public PanelHttpClientSettings Settings => this._settings ?? (this._settings = new PanelHttpClientSettings());

		/// <summary>The form is available for interactive interaction with the client</summary>
		private Boolean IsFormEnabled { get; set; } = true;
		
		public PanelHttpClient()
		{
			this.InitializeComponent();
			splitMain.Panel2Collapsed = true;
		}

		protected override void OnLoad(EventArgs e)
		{
			this.Window.SetTabPicture(Resources.Application);
			lvRequests.Plugin = this.Plugin;
			txtResponse.SelectionTabs = Enumerable.Repeat(50, 30).ToArray();//Reduce default tab size
			this.LoadProject();

			this.Window.Closed += this.Window_Closed;
			this.Plugin.Settings.PropertyChanged += this.Settings_PropertyChanged;
			this.Plugin.OnSearchForProjectItems += this.Plugin_OnSearchForProjectItems;
			this.Plugin.OnSearchForProject += this.Plugin_OnSearchForProject;
			this.Plugin.OnToggleProjectDirty += this.Plugin_OnToggleProjectDirty;

			base.OnLoad(e);
		}

		private void Window_Closed(Object sender, EventArgs e)
		{
			this.Plugin.OnSearchForProjectItems -= this.Plugin_OnSearchForProjectItems;
			this.Plugin.Settings.PropertyChanged -= this.Settings_PropertyChanged;

			this.Plugin.Settings.ListViewState = lvRequests.SaveState();
			this.Plugin.ProjectClosed(this.Settings.ProjectFileName);
		}

		private void Plugin_OnToggleProjectDirty(Object sender, ToggleProjectDirtyEventArgs e)
		{
			if(e.ProjectFilePath == this.Settings.ProjectFileName)
				this.lvRequests.UpdateNodesText();
		}

		private void Plugin_OnSearchForProject(Object sender, SearchForProjectEventArgs e)
		{
			if(e.ProjectFileName == this.Settings.ProjectFileName)
				e.Project = this.lvRequests.Project;
		}

		private void Settings_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
			case nameof(PluginSettings.WordWrap):
				txtResponse.WordWrap = this.Plugin.Settings.WordWrap;
				break;
			}
		}

		private void Plugin_OnSearchForProjectItems(Object sender, SearchProjectItemEventArgs e)
		{
			if(this.Settings.ProjectFileName != null || lvRequests.IsDirty)
				e.Found.AddRange(lvRequests.Project.Items.Find(e.Search, this.Plugin.Settings.ServerSearchRelativeUrl));
		}

		private void SetWindowCaption()
			=> this.Window.Caption = this.Settings.ProjectFileName == null
				? PanelHttpClient.Caption
				: String.Join(" - ", new String[] { Path.GetFileName(this.Settings.ProjectFileName), PanelHttpClient.Caption, });

		/// <summary>Upload the project to the form</summary>
		/// <param name="filePath">File path or download from local storage</param>
		private void LoadProject()
		{
			if(!this.IsFormEnabled)
				return;

			String filePath = this.Settings.ProjectFileName;
			if(filePath != null && !File.Exists(filePath))
			{
				this.Plugin.Trace.TraceEvent(TraceEventType.Warning, 7, "File {0} not found", filePath);
				filePath = null;
			}

			lvRequests.LoadProject(filePath);
			Byte[] state = this.Plugin.Settings.ListViewState;
			if(state == null)
				lvRequests.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			else
				lvRequests.RestoreState(state);//TODO: It's too slow (10 seconds...)

			this.SetWindowCaption();
			tsmiProjectExport.Visible = filePath == null;
			tsmiProjectImport.Visible = filePath != null;

			ddlTemplateName.Items.Clear();
			var keys = lvRequests.Project.TemplatesCollection.Keys.ToArray();
			ddlTemplateName.Items.AddRange(keys);
			ddlTemplateName.SelectedIndex = Array.FindIndex(keys, k => k == lvRequests.Project.SelectedTemplate);
		}

		private void ToggleNodeSelected()
			=> this.ToggleNodeSelected(lvRequests.SelectedObject);

		private void ToggleNodeSelected(HttpProjectItem item)
		{
			if(item == null || item.Items.Project == null)
			{
				splitMain.Panel2Collapsed = true;
				pgProperties.SelectedObject = null;
				txtResponse.Text = null;
				tsbnRequestRemove.Enabled = false;
			} else
			{
				pgProperties.SelectedItem = item;
				if(item.LastResponse == null)
				{
					if(tabSettings.TabPages.Contains(tabPageResponse))
						tabSettings.TabPages.Remove(tabPageResponse);
				} else
				{
					this.bnResponseWithHeaders_Click(null, EventArgs.Empty);
					if(!tabSettings.TabPages.Contains(tabPageResponse))
						tabSettings.TabPages.Add(tabPageResponse);

					if(this.Plugin.Settings.ShowResponse)
						tabSettings.SelectedTab = tabPageResponse;
				}
				splitMain.Panel2Collapsed = false;
				tsbnRequestRemove.Enabled = true;
			}
		}

		/// <summary>Block or unblock a form from client actions</summary>
		/// <param name="lockControls">Block the form from client actions</param>
		private void ToggleControls(Boolean lockControls)
		{
			tsddlProject.Enabled = !lockControls;
			tsbnRequestAdd.Enabled = !lockControls;
			tsbnRequestRemove.Enabled = !lockControls && lvRequests.SelectedObject != null;
			ddlTemplateName.Enabled = !lockControls;
			gvTemplates.Enabled = !lockControls;
			bnAddTemplateName.Enabled = !lockControls;
			tsbnLaunch.Image = lockControls ? Resources.bnStop : Resources.bnCompile;

			this.IsFormEnabled = !lockControls;
			this.Cursor = lvRequests.Cursor = lockControls ? Cursors.WaitCursor : Cursors.Default;
		}

		private void PrepareTest(TestStartArgs args)
		{
			if(bwServerTest.IsBusy)
				bwServerTest.CancelAsync();
			else
			{
				this.ToggleControls(true);
				lvRequests.SaveProject();

				TestEngine engine = new TestEngine(args);

				bwServerTest.RunWorkerAsync(engine);
			}
		}

		private void lvRequests_CellEditFinished(Object sender, CellEditEventArgs e)
		{
			if(e.Cancel) return;

			HttpProjectItem item = lvRequests.SelectedObject;
			this.ToggleNodeSelected(item);
		}

		private void lvRequests_SelectionChanged(Object sender, EventArgs e)
		{
			HttpProjectItem item = lvRequests.SelectedObject;
			this.ToggleNodeSelected(item);
		}

		private void lvRequests_KeyDown(Object sender, KeyEventArgs e)
		{
			switch(e.KeyData)
			{
			case Keys.Delete:
				e.Handled = true;
				this.tsbnRequestRemove.PerformClick();
				break;
			case Keys.Return:
				if(lvRequests.SelectedObject != null)
				{
					e.Handled = true;
					this.cmsProject_ItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiProjectRun));
				}
				break;
			case Keys.S | Keys.Control:
				e.Handled = true;
				this.tsbnProjectSave.PerformClick();
				break;
			case Keys.C | Keys.Control:
				e.Handled = true;
				this.tsmiProjectCopy.PerformClick();
				break;
			case Keys.N | Keys.Control:
				e.Handled = true;
				this.tsbnRequestAdd.PerformClick();
				break;
			case Keys.N | Keys.Control | Keys.Alt:
				e.Handled = true;
				this.tsmiProjectNew.PerformClick();
				break;
			case Keys.O | Keys.Control:
				e.Handled = true;
				this.tsmiProjectOpen.PerformClick();
				break;
			case Keys.Apps:
				throw new NotImplementedException();
			}
		}

		private void lvRequests_MouseClick(Object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right && lvRequests.SelectedObject != null)
				this.OpenContextMenu(e.Location);
		}

		private void OpenContextMenu(Point location)
			=> cmsProject.Show(lvRequests, location);

		private void lvRequests_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			HttpProjectItem item = lvRequests.SelectedObject;
			if(item != null)
				this.cmsProject_ItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiProjectRun));
		}

		private void lvRequests_DirtyChanged(Object sender, ToggleProjectDirtyEventArgs e)
		{
			tsbnProjectSave.Enabled = e.IsDirty;
			tsmiProjectUndo.Visible = e.IsDirty;

			if(!lvRequests.IsDirty)
			{
				this.Settings.SetValues(e.ProjectFilePath);
				this.SetWindowCaption();
				tsmiProjectExport.Visible = lvRequests.FilePath == null;
				tsmiProjectImport.Visible = lvRequests.FilePath != null;
			}
		}

		private void tsddlProject_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			tsddlProject.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);

			if(!this.IsFormEnabled)
				return;

			if(e.ClickedItem == tsmiProjectNew)
			{
				using(SaveFileDialog dlg = new SaveFileDialog()
				{
					Filter = Constant.Project.Extensions.CreateFilter(Constant.Project.Extensions.FilterTypes.Projects | Constant.Project.Extensions.FilterTypes.Assemblies| Constant.Project.Extensions.FilterTypes.AllFiles),
					Title = "Choose file name for a new project...",
					DefaultExt = Constant.Project.Extensions.Xml,
					CheckFileExists = false,
					CheckPathExists = true,
					OverwritePrompt = false,
				})
				{
					if(dlg.ShowDialog() != DialogResult.OK)
						return;

					String fileName = dlg.FileName;
					Boolean isAssembly = Constant.Project.Extensions.IsAssembly(dlg.FileName);
					if(isAssembly)
						fileName = Path.GetFileNameWithoutExtension(fileName) + "." + Constant.Project.Extensions.Binary;
					if(File.Exists(fileName) && MessageBox.Show($"{fileName} already exists.{Environment.NewLine}Do you want to replace it?", String.Empty, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
						return;

					HttpProject project = new HttpProject();
					if(isAssembly)
						project.ImportAssembly(this.Plugin.Settings.GetServerUrl(), dlg.FileName);

					//Creating an empty project to mark that file exists. Because when a new window is opened we check that such file exists and was not removed
					project.Save(fileName);

					if(this.Plugin.CreateWindow(typeof(PanelHttpClient).ToString(), true,
							new PanelHttpClientSettings() { ProjectFileName = fileName }) == null)
						this.Plugin.Trace.TraceEvent(TraceEventType.Warning, 1, "Failed to open {0} window", typeof(PanelHttpClient));
				}
			} else if(e.ClickedItem == tsmiProjectOpen)
			{
				using(OpenFileDialog dlg = new OpenFileDialog()
				{
					Filter = Constant.Project.Extensions.CreateFilter(Constant.Project.Extensions.FilterTypes.Projects | Constant.Project.Extensions.FilterTypes.Assemblies | Constant.Project.Extensions.FilterTypes.AllFiles),
					Title = "Choose existing project or generate project based on WebAPI assembly...",
					DefaultExt = Constant.Project.Extensions.Xml,
					CheckFileExists = true,
					CheckPathExists = true,
				})
				{
					if(dlg.ShowDialog() != DialogResult.OK)
						return;

					Boolean isAssembly = Constant.Project.Extensions.IsAssembly(dlg.FileName);
					if(isAssembly)
					{
						HttpProject project = lvRequests.Project;
						if(project.ImportAssembly(this.Plugin.Settings.GetServerUrl(), dlg.FileName))
							lvRequests.UpdateProjectItems();
					} else
					{
						if(dlg.FileName == this.Settings.ProjectFileName)
							this.Settings.ProjectFileName = dlg.FileName;
						else if(this.Plugin.CreateWindow(typeof(PanelHttpClient).ToString(), true,
								new PanelHttpClientSettings() { ProjectFileName = dlg.FileName }) == null)
							this.Plugin.Trace.TraceEvent(TraceEventType.Warning, 1, "Failed to open {0} window", typeof(PanelHttpClient));
					}
				}
			}else if(e.ClickedItem == tsmiProjectAppend)
			{
				using(OpenFileDialog dlg = new OpenFileDialog()
				{
					Filter = Constant.Project.Extensions.CreateFilter(Constant.Project.Extensions.FilterTypes.Projects | Constant.Project.Extensions.FilterTypes.Assemblies | Constant.Project.Extensions.FilterTypes.AllFiles),
					Title = "Choose existing project or WebAPI assembly",
					DefaultExt = Constant.Project.Extensions.Xml,
					CheckFileExists = true,
					CheckPathExists = true,
				})
				{
					if(dlg.ShowDialog() != DialogResult.OK)
						return;

					Boolean isImported = false;
					if(Constant.Project.Extensions.IsAssembly(dlg.FileName))
					{
						HttpProject project = this.lvRequests.Project;
						if(project.ImportAssembly(this.Plugin.Settings.GetServerUrl(), dlg.FileName))
							isImported = true;
					} else if(Constant.Project.Extensions.IsProject(dlg.FileName))
					{
						HttpProject project = this.lvRequests.Project;
						if(project.ImportProject(dlg.FileName))
							isImported = true;
					}
					if(isImported)
						this.lvRequests.UpdateProjectItems();
				}
			}
			else if(e.ClickedItem == tsmiProjectExport)
				lvRequests.SaveProjectToFile(true);
			else if(e.ClickedItem == tsmiProjectImport)
				lvRequests.SaveProjectToStorage();
			else if(e.ClickedItem == tsmiProjectUndo)
				lvRequests.LoadProject(lvRequests.FilePath);
			else
				throw new NotImplementedException($"Unknown item: {e.ClickedItem}");
		}

		private void splitMain_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(splitMain.SplitterRectangle.Contains(e.Location))
				splitMain.Panel2Collapsed = true;
		}

		private void pgProperties_PropertyValueChanged(Object sender, PropertyValueChangedEventArgs e)
		{ 
			lvRequests.ToggleDirty(true);
			lvRequests.RefreshObject(pgProperties.SelectedItem);
		}

		private void tsbnLaunch_ButtonClick(Object sender, EventArgs e)
			=> this.tsbnLaunch_DropDownItemClicked(sender, new ToolStripItemClickedEventArgs(tsmiLaunch));

		private void tsbnLaunch_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == tsmiLaunch)
				this.PrepareTest(new TestStartArgs(this.Plugin, lvRequests.Project));
			else
				throw new NotImplementedException();
		}

		private void bwServerTest_DoWork(Object sender, DoWorkEventArgs e)
		{
			TestEngine engine = (TestEngine)e.Argument;
			engine.OnTestChanged += (s, args) => bwServerTest.ReportProgress(0, args);
			engine.Run(() => bwServerTest.CancellationPending);
		}

		private void bwServerTest_ProgressChanged(Object sender, ProgressChangedEventArgs e)
		{
			TestProgressChangedArgs test = (TestProgressChangedArgs)e.UserState;
			lvRequests.RefreshObject(test.Item);

			if(test.Result != null)
				this.Plugin.History.Add(History.HistoryActionType.Client, test.Result.Result);
		}

		private void bwServerTest_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			if(e.Error != null)
				this.Plugin.Trace.TraceData(TraceEventType.Error, 10, e.Error);

			this.ToggleControls(false);
			this.ToggleNodeSelected();
		}

		private void cmsRequest_Opening(Object sender, CancelEventArgs e)
			=> e.Cancel = lvRequests.SelectedObject == null;

		private void cmsProject_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			HttpProjectItem item = lvRequests.SelectedObject;
			if(e.ClickedItem == tsmiProjectCopy)
			{
				Clipboard.SetText(item.AddressReal);
				return;
			}

			cmsProject.Close();//We don't need to close contextMenu when default Copy menu is invoked (Because under that menu we have PowerShell copy menu)
			if(e.ClickedItem == tsmiProjectCopyPowerShell)
			{
				RequestBuilder builder = new RequestBuilder(item);
				String requestScriptV2 = builder.CreatePowerShellScript();
				Clipboard.SetText(requestScriptV2);
			} else if(e.ClickedItem == tsmiProjectCopyCurl)
			{
				RequestBuilder builder = new RequestBuilder(item);
				String requestScript = builder.CreateCurlScript();
				Clipboard.SetText(requestScript);
			}

			if(!this.IsFormEnabled)
				return;

			if(e.ClickedItem == tsmiProjectRun)
				this.PrepareTest(new TestStartArgs(this.Plugin, TestStartArgs.TestType.Single, item));
			else if(e.ClickedItem == tsmiProjectRunThread)
				this.PrepareTest(new TestStartArgs(this.Plugin, TestStartArgs.TestType.Thread, item));
			else if(e.ClickedItem == tsmiProjectProperties)
				this.ToggleNodeSelected();
			else if(e.ClickedItem == tsmiProjectToggle)
			{
				foreach(HttpProjectItem selected in lvRequests.SelectedObjects)
				{
					NodeStateEnum image = selected.Image == NodeStateEnum.Skip
						? NodeStateEnum.New
						: NodeStateEnum.Skip;
					selected.Image = image;

					foreach(HttpProjectItem children in selected.Items.EnumerateItems())
						children.Image = image;

					lvRequests.RefreshObject(selected);
				}
			}
		}

		private void tsbnRequestAdd_Click(Object sender, EventArgs e)
		{
			if(!this.IsFormEnabled)
				return;

			lvRequests.AddNewNode();
		}

		private void tsbnRequestRemove_Click(Object sender, EventArgs e)
		{
			if(!this.IsFormEnabled)
				return;

			lvRequests.RemoveSelectedHttpTest();
			this.ToggleNodeSelected();
		}

		private void tsbnProjectSave_Click(Object sender, EventArgs e)
		{
			if(!this.IsFormEnabled)
				return;

			lvRequests.SaveProject();
		}

		private void tsbnHistory_Click(Object sender, EventArgs e)
		{
			if(this.Plugin.CreateWindow(typeof(PanelHttpHistory).ToString(), true) == null)
				this.Plugin.Trace.TraceEvent(TraceEventType.Warning, 1, "Failed to open {0} window", typeof(PanelHttpHistory));
		}

		private void txtResponse_LinkClicked(Object sender, LinkClickedEventArgs e)
		{
			HttpProjectItem item = lvRequests.SelectedObject;
			HttpProjectItemCollection collection = item == null ? lvRequests.Project.Items : item.Items;
			_ = collection.Add(e.LinkText);
			lvRequests.RefreshSelectedObjects();
		}

		#region Templates
		private Int32? _searchHeight;
		private void tsTemplates_Resize(Object sender, EventArgs e)
		{
			if(this._searchHeight == null)
				this._searchHeight = ddlTemplateName.Height;
			ddlTemplateName.Size = new Size(tsTemplates.Width - bnAddTemplateName.Width - bnEditTemplates.Width - 5, ddlTemplateName.Height);
			ddlTemplateName.Height = this._searchHeight.Value;
		}

		private void ddlTemplateName_SelectedIndexChanged(Object sender, EventArgs e)
		{
			String name = (String)ddlTemplateName.SelectedItem ?? Constant.Project.DefaultTemplateName;
			HttpProject project = lvRequests.Project;
			if(project.TemplatesCollection.ContainsKey(name) && project.SelectedTemplate != name)
			{
				project.SelectedTemplate = name;
				gvTemplates.SetTemplateRows(project.SelectedTemplate, project.Templates.GetTemplateValuesWithSource());
				lvRequests.UpdateNodesText();
				this.ToggleNodeSelected();//We need to refresh property grid because templates can change
			}
		}

		private void ddlTemplateName_TextChanged(Object sender, EventArgs e)
			=> bnAddTemplateName.Image = lvRequests.Project.TemplatesCollection.ContainsKey(ddlTemplateName.Text.Trim())
				? Resources.iconDelete
				: Resources.FileSave;

		private void bnAddTemplateName_Click(Object sender, EventArgs e)
		{
			String name = ddlTemplateName.Text.Trim();
			HttpProject project = lvRequests.Project;
			if(project.TemplatesCollection.ContainsKey(name))
			{//Remove
				project.SelectedTemplate = Constant.Project.DefaultTemplateName;
				if(String.IsNullOrEmpty(name))
					project.TemplatesCollection[name] = new TemplateItem[] { };
				else
				{
					project.TemplatesCollection.Remove(name);
					ddlTemplateName.Items.Remove(name);
				}

				//Refreshing current templates selection
				gvTemplates.SetTemplateRows(project.SelectedTemplate, project.Templates.GetTemplateValuesWithSource());
			} else
			{//Add & copy all values from selected template (Also we can rename using this feature by adding new template and removing previous)
				TemplateItem[] copy = Array.ConvertAll(project.Templates.SelectedTemplateValues, t => new TemplateItem(t.Key, t.Value));
				project.SelectedTemplate = name;
				project.TemplatesCollection.Add(project.SelectedTemplate, copy);
				ddlTemplateName.Items.Add(name);

				//Copying all template items (including values) from previous template to a new template
				gvTemplates.SetTemplateRows(project.SelectedTemplate, project.Templates.GetTemplateValuesWithSource());
			}

			lvRequests.UpdateNodesText();
			this.ToggleNodeSelected();//We need to refresh property grid because templates can change
			this.ddlTemplateName_TextChanged(null, null);
		}

		private void bnEditTemplates_Click(Object sender, EventArgs e)
		{
			Boolean showProperties = bnEditTemplates.Checked;
			pgProperties.Visible = showProperties;
			gvTemplates.Visible = !showProperties;
			bnEditTemplates.Checked = !showProperties;
			if(showProperties)
				pgProperties.Focus();
			else
				gvTemplates.Focus();
			

			if(bnEditTemplates.Checked && gvTemplates.Templates == null)
			{
				gvTemplates.SetTemplateRows(lvRequests.Project.SelectedTemplate, lvRequests.Project.Templates.GetTemplateValuesWithSource());

				ddlTemplateName.Text = lvRequests.Project.SelectedTemplate;
				this.ddlTemplateName_TextChanged(null, null);
			}
		}

		private void bnResponseWithHeaders_Click(Object sender, EventArgs e)
		{
			Boolean withHeaders = !bnResponseWithHeaders.Checked;
			bnResponseWithHeaders.Checked = withHeaders;
			HttpProjectItem item = lvRequests.SelectedObject;

			txtResponse.Text = withHeaders
				? item.LastResponse.GetBodyWithHeaders()
				: item.LastResponse.TryFormatBody();
		}

		private void gvTemplates_OnToggleTemplatesDirty(Object sender, ToggleProjectDirtyEventArgs e)
		{
			lvRequests.Project.TemplatesCollection[lvRequests.Project.SelectedTemplate] = gvTemplates.Templates.ToArray();
			lvRequests.Project.Templates.SelectedTemplateValues = null;//Reset cache
			lvRequests.UpdateNodesText();
			lvRequests.ToggleDirty(e.IsDirty);
			this.ToggleNodeSelected();//We need to refresh property grid because templates can change
		}
		#endregion Templates

		private void cmsProperties_Opening(Object sender, CancelEventArgs e)
		{
			var lItem = pgProperties.SelectedGridItem;

			// SelectedObject cannot be used for child properties.
			//  Component is an internal property of the class System.Windows.Forms.PropertyGridInternal.GridEntry
			// ((System.Windows.Forms.PropertyGridInternal.GridEntry)(lItem)).Component
			// Accessed via reflection
			var lComponent = lItem.GetType().GetProperty("Component").GetValue(lItem, null);

			PropertiesReset.Enabled = lComponent == null
				? lItem.PropertyDescriptor.CanResetValue(pgProperties.SelectedObject)
				: lItem.PropertyDescriptor.CanResetValue(lComponent);
		}

		private void cmsProperties_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == PropertiesReset)
			{
				pgProperties.SelectedGridItem.PropertyDescriptor.SetValue(pgProperties.SelectedObject, null);
				pgProperties.Refresh();
				//pgProperties.ResetSelectedProperty(); <- This method will take default value from templates but not null value
				lvRequests.ToggleDirty(true);
			} else
				throw new NotImplementedException();
		}
	}
}