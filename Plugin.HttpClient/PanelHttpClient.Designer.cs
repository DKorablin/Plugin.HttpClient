using Plugin.HttpClient.Events;
using Plugin.HttpClient.UI;

namespace Plugin.HttpClient
{
	partial class PanelHttpClient
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStrip tsMain;
			System.Windows.Forms.ToolStripSeparator tss2;
			System.Windows.Forms.ToolStripSeparator tsddlProjectSeparator;
			System.Windows.Forms.ToolStripSeparator tss4;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelHttpClient));
			this.tsbnRequestAdd = new System.Windows.Forms.ToolStripButton();
			this.tsbnRequestRemove = new System.Windows.Forms.ToolStripButton();
			this.tsbnProjectSave = new System.Windows.Forms.ToolStripButton();
			this.tsddlProject = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiProjectNew = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectExport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectImport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbnLaunch = new System.Windows.Forms.ToolStripSplitButton();
			this.tsmiLaunch = new System.Windows.Forms.ToolStripMenuItem();
			this.tsbnHistory = new System.Windows.Forms.ToolStripButton();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.gridSearch = new AlphaOmega.Windows.Forms.SearchGrid();
			this.lvRequests = new Plugin.HttpClient.UI.HttpTestsListView();
			this.ilStatus = new System.Windows.Forms.ImageList(this.components);
			this.tabSettings = new System.Windows.Forms.TabControl();
			this.tabPageSettings = new System.Windows.Forms.TabPage();
			this.pgProperties = new Plugin.HttpClient.UI.RestoreStatePropertyGrid();
			this.cmsProperties = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.PropertiesReset = new System.Windows.Forms.ToolStripMenuItem();
			this.gvTemplates = new Plugin.HttpClient.UI.TemplateEditorGridView();
			this.tsTemplates = new System.Windows.Forms.ToolStrip();
			this.ddlTemplateName = new Plugin.HttpClient.UI.DefaultTextToolStripComboBox();
			this.bnAddTemplateName = new System.Windows.Forms.ToolStripButton();
			this.bnEditTemplates = new System.Windows.Forms.ToolStripButton();
			this.tabPageResponse = new System.Windows.Forms.TabPage();
			this.txtResponse = new System.Windows.Forms.RichTextBox();
			this.cmsProject = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiProjectRun = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectRunThread = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiProjectToggle = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.tsmiProjectCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectCopyPowerShell = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectCopyCurl = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectProperties = new System.Windows.Forms.ToolStripMenuItem();
			this.bwServerTest = new System.ComponentModel.BackgroundWorker();
			this.tsmiProjectUndo = new System.Windows.Forms.ToolStripMenuItem();
			tsMain = new System.Windows.Forms.ToolStrip();
			tss2 = new System.Windows.Forms.ToolStripSeparator();
			tsddlProjectSeparator = new System.Windows.Forms.ToolStripSeparator();
			tss4 = new System.Windows.Forms.ToolStripSeparator();
			tsMain.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvRequests)).BeginInit();
			this.tabSettings.SuspendLayout();
			this.tabPageSettings.SuspendLayout();
			this.cmsProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gvTemplates)).BeginInit();
			this.tsTemplates.SuspendLayout();
			this.tabPageResponse.SuspendLayout();
			this.cmsProject.SuspendLayout();
			this.SuspendLayout();
			// 
			// tsMain
			// 
			tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnRequestAdd,
            this.tsbnRequestRemove,
            tss2,
            this.tsbnProjectSave,
            this.tsddlProject,
            this.tsbnLaunch,
            tss4,
            this.tsbnHistory});
			tsMain.Location = new System.Drawing.Point(0, 0);
			tsMain.Name = "tsMain";
			tsMain.Size = new System.Drawing.Size(255, 27);
			tsMain.TabIndex = 0;
			tsMain.Text = "toolStrip1";
			// 
			// tsbnRequestAdd
			// 
			this.tsbnRequestAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRequestAdd.Image = global::Plugin.HttpClient.Properties.Resources.iconPublish;
			this.tsbnRequestAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRequestAdd.Name = "tsbnRequestAdd";
			this.tsbnRequestAdd.Size = new System.Drawing.Size(29, 24);
			this.tsbnRequestAdd.Text = "Add";
			this.tsbnRequestAdd.Click += new System.EventHandler(this.tsbnRequestAdd_Click);
			// 
			// tsbnRequestRemove
			// 
			this.tsbnRequestRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRequestRemove.Enabled = false;
			this.tsbnRequestRemove.Image = global::Plugin.HttpClient.Properties.Resources.iconDelete;
			this.tsbnRequestRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRequestRemove.Name = "tsbnRequestRemove";
			this.tsbnRequestRemove.Size = new System.Drawing.Size(29, 24);
			this.tsbnRequestRemove.Text = "Remove";
			this.tsbnRequestRemove.Click += new System.EventHandler(this.tsbnRequestRemove_Click);
			// 
			// tss2
			// 
			tss2.Name = "tss2";
			tss2.Size = new System.Drawing.Size(6, 27);
			// 
			// tsbnProjectSave
			// 
			this.tsbnProjectSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnProjectSave.Image = global::Plugin.HttpClient.Properties.Resources.FileSave;
			this.tsbnProjectSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnProjectSave.Name = "tsbnProjectSave";
			this.tsbnProjectSave.Size = new System.Drawing.Size(29, 24);
			this.tsbnProjectSave.Text = "Save...";
			this.tsbnProjectSave.Click += new System.EventHandler(this.tsbnProjectSave_Click);
			// 
			// tsddlProject
			// 
			this.tsddlProject.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsddlProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiProjectNew,
            this.tsmiProjectOpen,
            tsddlProjectSeparator,
            this.tsmiProjectExport,
            this.tsmiProjectImport,
            this.tsmiProjectUndo});
			this.tsddlProject.Image = global::Plugin.HttpClient.Properties.Resources.iconOpen;
			this.tsddlProject.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsddlProject.Name = "tsddlProject";
			this.tsddlProject.Size = new System.Drawing.Size(34, 24);
			this.tsddlProject.ToolTipText = "Project";
			this.tsddlProject.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsddlProject_DropDownItemClicked);
			// 
			// tsmiProjectNew
			// 
			this.tsmiProjectNew.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmiProjectNew.Name = "tsmiProjectNew";
			this.tsmiProjectNew.Size = new System.Drawing.Size(224, 26);
			this.tsmiProjectNew.Text = "&New...";
			// 
			// tsmiProjectOpen
			// 
			this.tsmiProjectOpen.Image = global::Plugin.HttpClient.Properties.Resources.iconOpen;
			this.tsmiProjectOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmiProjectOpen.Name = "tsmiProjectOpen";
			this.tsmiProjectOpen.Size = new System.Drawing.Size(224, 26);
			this.tsmiProjectOpen.Text = "&Open...";
			// 
			// tsddlProjectSeparator
			// 
			tsddlProjectSeparator.Name = "tsddlProjectSeparator";
			tsddlProjectSeparator.Size = new System.Drawing.Size(221, 6);
			// 
			// tsmiProjectExport
			// 
			this.tsmiProjectExport.Name = "tsmiProjectExport";
			this.tsmiProjectExport.Size = new System.Drawing.Size(224, 26);
			this.tsmiProjectExport.Text = "&Export...";
			// 
			// tsmiProjectImport
			// 
			this.tsmiProjectImport.Name = "tsmiProjectImport";
			this.tsmiProjectImport.Size = new System.Drawing.Size(224, 26);
			this.tsmiProjectImport.Text = "&Import";
			// 
			// tsmiProjectUndo
			// 
			this.tsmiProjectUndo.Name = "tsmiProjectUndo";
			this.tsmiProjectUndo.Size = new System.Drawing.Size(224, 26);
			this.tsmiProjectUndo.Text = "&Undo";
			// 
			// tsbnLaunch
			// 
			this.tsbnLaunch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.tsbnLaunch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnLaunch.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLaunch});
			this.tsbnLaunch.Image = global::Plugin.HttpClient.Properties.Resources.bnCompile;
			this.tsbnLaunch.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnLaunch.Name = "tsbnLaunch";
			this.tsbnLaunch.Size = new System.Drawing.Size(39, 24);
			this.tsbnLaunch.ToolTipText = "Launch test";
			this.tsbnLaunch.ButtonClick += new System.EventHandler(this.tsbnLaunch_ButtonClick);
			this.tsbnLaunch.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsbnLaunch_DropDownItemClicked);
			// 
			// tsmiLaunch
			// 
			this.tsmiLaunch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tsmiLaunch.Name = "tsmiLaunch";
			this.tsmiLaunch.Size = new System.Drawing.Size(142, 26);
			this.tsmiLaunch.Text = "&Launch";
			// 
			// tss4
			// 
			tss4.Name = "tss4";
			tss4.Size = new System.Drawing.Size(6, 27);
			// 
			// tsbnHistory
			// 
			this.tsbnHistory.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.tsbnHistory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnHistory.Image = global::Plugin.HttpClient.Properties.Resources.bnHistory;
			this.tsbnHistory.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnHistory.Name = "tsbnHistory";
			this.tsbnHistory.Size = new System.Drawing.Size(29, 24);
			this.tsbnHistory.ToolTipText = "Open history panel";
			this.tsbnHistory.Click += new System.EventHandler(this.tsbnHistory_Click);
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 27);
			this.splitMain.Margin = new System.Windows.Forms.Padding(4);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.gridSearch);
			this.splitMain.Panel1.Controls.Add(this.lvRequests);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.tabSettings);
			this.splitMain.Size = new System.Drawing.Size(255, 258);
			this.splitMain.SplitterDistance = 110;
			this.splitMain.SplitterWidth = 5;
			this.splitMain.TabIndex = 1;
			this.splitMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.splitMain_MouseDoubleClick);
			// 
			// gridSearch
			// 
			this.gridSearch.DataGrid = null;
			this.gridSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gridSearch.EnableFindCase = true;
			this.gridSearch.EnableFindHilight = true;
			this.gridSearch.EnableFindPrevNext = true;
			this.gridSearch.EnableSearchHilight = false;
			this.gridSearch.ListView = this.lvRequests;
			this.gridSearch.Location = new System.Drawing.Point(0, 81);
			this.gridSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.gridSearch.Name = "gridSearch";
			this.gridSearch.Size = new System.Drawing.Size(255, 29);
			this.gridSearch.TabIndex = 1;
			this.gridSearch.TreeView = null;
			this.gridSearch.Visible = false;
			// 
			// lvRequests
			// 
			this.lvRequests.AllowDrop = true;
			this.lvRequests.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.F2Only;
			this.lvRequests.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvRequests.FullRowSelect = true;
			this.lvRequests.HideSelection = false;
			this.lvRequests.Location = new System.Drawing.Point(0, 0);
			this.lvRequests.Margin = new System.Windows.Forms.Padding(4);
			this.lvRequests.Name = "lvRequests";
			this.lvRequests.Plugin = null;
			this.lvRequests.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
			this.lvRequests.ShowCommandMenuOnRightClick = true;
			this.lvRequests.ShowGroups = false;
			this.lvRequests.Size = new System.Drawing.Size(255, 110);
			this.lvRequests.SmallImageList = this.ilStatus;
			this.lvRequests.TabIndex = 0;
			this.lvRequests.UseCompatibleStateImageBehavior = false;
			this.lvRequests.UseFilterIndicator = true;
			this.lvRequests.UseFiltering = true;
			this.lvRequests.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvRequests.View = System.Windows.Forms.View.Details;
			this.lvRequests.DirtyChanged += new System.EventHandler<ToggleProjectDirtyEventArgs>(this.lvRequests_DirtyChanged);
			this.lvRequests.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvRequests_KeyDown);
			this.lvRequests.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvRequests_MouseClick);
			this.lvRequests.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvRequests_MouseDoubleClick);
			this.lvRequests.SelectionChanged += new System.EventHandler(lvRequests_SelectionChanged);
			this.lvRequests.CellEditFinished += lvRequests_CellEditFinished;
			// 
			// ilStatus
			// 
			this.ilStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStatus.ImageStream")));
			this.ilStatus.TransparentColor = System.Drawing.Color.Magenta;
			this.ilStatus.Images.SetKeyName(0, "circleGray.bmp");
			this.ilStatus.Images.SetKeyName(1, "circleYellow.bmp");
			this.ilStatus.Images.SetKeyName(2, "circleGreen.bmp");
			this.ilStatus.Images.SetKeyName(3, "circleRed.bmp");
			this.ilStatus.Images.SetKeyName(4, "circleRed - Forbidden.bmp");
			this.ilStatus.Images.SetKeyName(5, "iconOpen.bmp");
			this.ilStatus.Images.SetKeyName(6, "pauseGray.bmp");
			// 
			// tabSettings
			// 
			this.tabSettings.Controls.Add(this.tabPageSettings);
			this.tabSettings.Controls.Add(this.tabPageResponse);
			this.tabSettings.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabSettings.Location = new System.Drawing.Point(0, 0);
			this.tabSettings.Margin = new System.Windows.Forms.Padding(4);
			this.tabSettings.Name = "tabSettings";
			this.tabSettings.SelectedIndex = 0;
			this.tabSettings.Size = new System.Drawing.Size(255, 143);
			this.tabSettings.TabIndex = 1;
			// 
			// tabPageSettings
			// 
			this.tabPageSettings.Controls.Add(this.pgProperties);
			this.tabPageSettings.Controls.Add(this.gvTemplates);
			this.tabPageSettings.Controls.Add(this.tsTemplates);
			this.tabPageSettings.Location = new System.Drawing.Point(4, 25);
			this.tabPageSettings.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageSettings.Name = "tabPageSettings";
			this.tabPageSettings.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageSettings.Size = new System.Drawing.Size(247, 114);
			this.tabPageSettings.TabIndex = 0;
			this.tabPageSettings.Text = "Settings";
			this.tabPageSettings.UseVisualStyleBackColor = true;
			// 
			// pgProperties
			// 
			this.pgProperties.ContextMenuStrip = this.cmsProperties;
			this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgProperties.LineColor = System.Drawing.SystemColors.ControlDark;
			this.pgProperties.Location = new System.Drawing.Point(4, 32);
			this.pgProperties.Margin = new System.Windows.Forms.Padding(4);
			this.pgProperties.Name = "pgProperties";
			this.pgProperties.Size = new System.Drawing.Size(239, 78);
			this.pgProperties.TabIndex = 0;
			this.pgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProperties_PropertyValueChanged);
			// 
			// cmsProperties
			// 
			this.cmsProperties.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsProperties.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PropertiesReset});
			this.cmsProperties.Name = "cmsProperties";
			this.cmsProperties.Size = new System.Drawing.Size(115, 28);
			this.cmsProperties.Opening += new System.ComponentModel.CancelEventHandler(this.cmsProperties_Opening);
			this.cmsProperties.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsProperties_ItemClicked);
			// 
			// PropertiesReset
			// 
			this.PropertiesReset.Name = "PropertiesReset";
			this.PropertiesReset.Size = new System.Drawing.Size(114, 24);
			this.PropertiesReset.Text = "Reset";
			// 
			// gvTemplates
			// 
			this.gvTemplates.AllowUserToResizeRows = false;
			this.gvTemplates.AutoGenerateColumns = false;
			this.gvTemplates.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.gvTemplates.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			this.gvTemplates.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvTemplates.ColumnHeadersVisible = false;
			this.gvTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gvTemplates.Location = new System.Drawing.Point(4, 32);
			this.gvTemplates.Name = "gvTemplates";
			this.gvTemplates.RowHeadersVisible = false;
			this.gvTemplates.RowHeadersWidth = 51;
			this.gvTemplates.RowTemplate.Height = 24;
			this.gvTemplates.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.gvTemplates.Size = new System.Drawing.Size(239, 78);
			this.gvTemplates.TabIndex = 6;
			this.gvTemplates.Visible = false;
			this.gvTemplates.OnToggleTemplatesDirty += new System.EventHandler<Plugin.HttpClient.Events.ToggleProjectDirtyEventArgs>(this.gvTemplates_OnToggleTemplatesDirty);
			// 
			// tsTemplates
			// 
			this.tsTemplates.CanOverflow = false;
			this.tsTemplates.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsTemplates.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.tsTemplates.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddlTemplateName,
            this.bnAddTemplateName,
            this.bnEditTemplates});
			this.tsTemplates.Location = new System.Drawing.Point(4, 4);
			this.tsTemplates.Name = "tsTemplates";
			this.tsTemplates.Size = new System.Drawing.Size(239, 28);
			this.tsTemplates.TabIndex = 5;
			this.tsTemplates.Resize += new System.EventHandler(this.tsTemplates_Resize);
			// 
			// ddlTemplateName
			// 
			this.ddlTemplateName.DefaultText = "Default";
			this.ddlTemplateName.ForeColor = System.Drawing.Color.Gray;
			this.ddlTemplateName.MaxLength = 2147483647;
			this.ddlTemplateName.Name = "ddlTemplateName";
			this.ddlTemplateName.PlaceHolderText = "Default";
			this.ddlTemplateName.Size = new System.Drawing.Size(150, 28);
			this.ddlTemplateName.SelectedIndexChanged += new System.EventHandler(this.ddlTemplateName_SelectedIndexChanged);
			this.ddlTemplateName.TextChanged += new System.EventHandler(this.ddlTemplateName_TextChanged);
			// 
			// bnAddTemplateName
			// 
			this.bnAddTemplateName.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnAddTemplateName.Image = global::Plugin.HttpClient.Properties.Resources.FileSave;
			this.bnAddTemplateName.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnAddTemplateName.Name = "bnAddTemplateName";
			this.bnAddTemplateName.Size = new System.Drawing.Size(29, 25);
			this.bnAddTemplateName.Text = "Add new template list name";
			this.bnAddTemplateName.Click += new System.EventHandler(this.bnAddTemplateName_Click);
			// 
			// bnEditTemplates
			// 
			this.bnEditTemplates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnEditTemplates.Image = global::Plugin.HttpClient.Properties.Resources.bnTemplates;
			this.bnEditTemplates.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnEditTemplates.Name = "bnEditTemplates";
			this.bnEditTemplates.Size = new System.Drawing.Size(29, 25);
			this.bnEditTemplates.Text = "Edit templates";
			this.bnEditTemplates.Click += new System.EventHandler(this.bnEditTemplates_Click);
			// 
			// tabPageResponse
			// 
			this.tabPageResponse.Controls.Add(this.txtResponse);
			this.tabPageResponse.Location = new System.Drawing.Point(4, 25);
			this.tabPageResponse.Margin = new System.Windows.Forms.Padding(4);
			this.tabPageResponse.Name = "tabPageResponse";
			this.tabPageResponse.Padding = new System.Windows.Forms.Padding(4);
			this.tabPageResponse.Size = new System.Drawing.Size(247, 114);
			this.tabPageResponse.TabIndex = 1;
			this.tabPageResponse.Text = "Response";
			this.tabPageResponse.UseVisualStyleBackColor = true;
			// 
			// txtResponse
			// 
			this.txtResponse.AcceptsTab = true;
			this.txtResponse.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtResponse.HideSelection = false;
			this.txtResponse.Location = new System.Drawing.Point(4, 4);
			this.txtResponse.Margin = new System.Windows.Forms.Padding(4);
			this.txtResponse.Name = "txtResponse";
			this.txtResponse.Size = new System.Drawing.Size(239, 106);
			this.txtResponse.TabIndex = 0;
			this.txtResponse.Text = "";
			this.txtResponse.WordWrap = false;
			this.txtResponse.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtResponse_LinkClicked);
			// 
			// cmsProject
			// 
			this.cmsProject.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsProject.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiProjectRun,
            this.tsmiProjectRunThread,
            this.toolStripSeparator1,
            this.tsmiProjectToggle,
            this.toolStripSeparator2,
            this.tsmiProjectCopy,
			this.tsmiProjectProperties});
			this.cmsProject.Name = "cmsProject";
			this.cmsProject.Size = new System.Drawing.Size(154, 112);
			this.cmsProject.Opening += new System.ComponentModel.CancelEventHandler(this.cmsRequest_Opening);
			this.cmsProject.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsProject_ItemClicked);
			// 
			// tsmiProjectRun
			// 
			this.tsmiProjectRun.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
			this.tsmiProjectRun.Name = "tsmiProjectRun";
			this.tsmiProjectRun.Size = new System.Drawing.Size(153, 24);
			this.tsmiProjectRun.Text = "&Run";
			// 
			// tsmiProjectRunThread
			// 
			this.tsmiProjectRunThread.Name = "tsmiProjectRunThread";
			this.tsmiProjectRunThread.Size = new System.Drawing.Size(153, 24);
			this.tsmiProjectRunThread.Text = "Run &Thread";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(150, 6);
			// 
			// tsmiProjectToggle
			// 
			this.tsmiProjectToggle.Name = "tsmiProjectToggle";
			this.tsmiProjectToggle.Size = new System.Drawing.Size(153, 24);
			this.tsmiProjectToggle.Text = "&Toggle";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(150, 6);
			// 
			// tsmiProjectCopy
			// 
			this.tsmiProjectCopy.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.tsmiProjectCopyPowerShell,
			this.tsmiProjectCopyCurl});
			this.tsmiProjectCopy.Name = "tsmiProjectCopy";
			this.tsmiProjectCopy.Size = new System.Drawing.Size(153, 24);
			this.tsmiProjectCopy.Text = "&Copy";
			this.tsmiProjectCopy.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsProject_ItemClicked);
			// 
			// tsmiProjectCopyPowerShell
			// 
			this.tsmiProjectCopyPowerShell.Name = "tsmiProjectCopyPowerShell";
			this.tsmiProjectCopyPowerShell.Size = new System.Drawing.Size(204, 26);
			this.tsmiProjectCopyPowerShell.Text = "Power&Shell script";
			// 
			// tsmiProjectCopyCurl
			// 
			this.tsmiProjectCopyCurl.Name = "tsmiProjectCopyCurl";
			this.tsmiProjectCopyCurl.Size = new System.Drawing.Size(204, 26);
			this.tsmiProjectCopyCurl.Text = "C&url script";
			// 
			// tsmiProjectProperties
			// 
			this.tsmiProjectProperties.Name = "tsmiProjectProperties";
			this.tsmiProjectProperties.Size = new System.Drawing.Size(204, 26);
			this.tsmiProjectProperties.Text = "&Properties";
			// 
			// bwServerTest
			// 
			this.bwServerTest.WorkerReportsProgress = true;
			this.bwServerTest.WorkerSupportsCancellation = true;
			this.bwServerTest.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwServerTest_DoWork);
			this.bwServerTest.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwServerTest_ProgressChanged);
			this.bwServerTest.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwServerTest_RunWorkerCompleted);
			// 
			// PanelHttpClient
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(tsMain);
			this.DoubleBuffered = true;
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelHttpClient";
			this.Size = new System.Drawing.Size(255, 285);
			tsMain.ResumeLayout(false);
			tsMain.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvRequests)).EndInit();
			this.tabSettings.ResumeLayout(false);
			this.tabPageSettings.ResumeLayout(false);
			this.tabPageSettings.PerformLayout();
			this.cmsProperties.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gvTemplates)).EndInit();
			this.tsTemplates.ResumeLayout(false);
			this.tsTemplates.PerformLayout();
			this.tabPageResponse.ResumeLayout(false);
			this.cmsProject.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitMain;
		private Plugin.HttpClient.UI.RestoreStatePropertyGrid pgProperties;
		private HttpClient.UI.HttpTestsListView lvRequests;
		private System.Windows.Forms.ToolStripSplitButton tsbnLaunch;
		private System.Windows.Forms.ToolStripMenuItem tsmiLaunch;
		private System.Windows.Forms.ImageList ilStatus;
		private System.ComponentModel.BackgroundWorker bwServerTest;
		private System.Windows.Forms.TabControl tabSettings;
		private System.Windows.Forms.TabPage tabPageSettings;
		private System.Windows.Forms.TabPage tabPageResponse;
		private System.Windows.Forms.RichTextBox txtResponse;
		private System.Windows.Forms.ContextMenuStrip cmsProject;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectRun;
		private System.Windows.Forms.ToolStripDropDownButton tsddlProject;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectOpen;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectNew;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectExport;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectImport;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectUndo;
		private System.Windows.Forms.ToolStripButton tsbnProjectSave;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectCopy;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectCopyPowerShell;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectCopyCurl;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectProperties;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectRunThread;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton tsbnRequestAdd;
		private System.Windows.Forms.ToolStripButton tsbnRequestRemove;
		private System.Windows.Forms.ToolStripButton tsbnHistory;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectToggle;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

		private System.Windows.Forms.ToolStrip tsTemplates;
		private DefaultTextToolStripComboBox ddlTemplateName;
		private System.Windows.Forms.ToolStripButton bnAddTemplateName;
		private System.Windows.Forms.ToolStripButton bnEditTemplates;
		private TemplateEditorGridView gvTemplates;
		private AlphaOmega.Windows.Forms.SearchGrid gridSearch;
		private System.Windows.Forms.ContextMenuStrip cmsProperties;
		private System.Windows.Forms.ToolStripMenuItem PropertiesReset;
	}
}