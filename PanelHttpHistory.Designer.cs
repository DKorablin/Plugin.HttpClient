namespace Plugin.HttpClient
{
	partial class PanelHttpHistory
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
			this.tsbnRetry = new System.Windows.Forms.ToolStripButton();
			this.tsbnRemove = new System.Windows.Forms.ToolStripButton();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.lvHistory = new System.Windows.Forms.ListView();
			this.colHistoryDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colHistoryElapsed = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colHistoryUrl = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.cmsHistory = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.tsmiHistorySelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.rtxtResponse = new System.Windows.Forms.RichTextBox();
			tsMain = new System.Windows.Forms.ToolStrip();
			tsMain.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.cmsHistory.SuspendLayout();
			this.SuspendLayout();
			// 
			// tsMain
			// 
			tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			tsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
			tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbnRetry,
            this.tsbnRemove});
			tsMain.Location = new System.Drawing.Point(0, 0);
			tsMain.Name = "tsMain";
			tsMain.Size = new System.Drawing.Size(200, 27);
			tsMain.TabIndex = 0;
			// 
			// tsbnRetry
			// 
			this.tsbnRetry.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRetry.Image = global::Plugin.HttpClient.Properties.Resources.bnCompile;
			this.tsbnRetry.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRetry.Name = "tsbnRetry";
			this.tsbnRetry.Size = new System.Drawing.Size(29, 24);
			this.tsbnRetry.Text = "Retry";
			this.tsbnRetry.Click += new System.EventHandler(this.tsbnRetry_Click);
			// 
			// tsbnRemove
			// 
			this.tsbnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.tsbnRemove.Image = global::Plugin.HttpClient.Properties.Resources.iconDelete;
			this.tsbnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsbnRemove.Name = "tsbnRemove";
			this.tsbnRemove.Size = new System.Drawing.Size(29, 24);
			this.tsbnRemove.Text = "Remove";
			this.tsbnRemove.ToolTipText = "Remove from history";
			this.tsbnRemove.Click += new System.EventHandler(this.tsbnRemove_Click);
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
			this.splitMain.Panel1.Controls.Add(this.lvHistory);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.rtxtResponse);
			this.splitMain.Size = new System.Drawing.Size(200, 158);
			this.splitMain.SplitterDistance = 78;
			this.splitMain.SplitterWidth = 5;
			this.splitMain.TabIndex = 1;
			// 
			// lvHistory
			// 
			this.lvHistory.AllowColumnReorder = true;
			this.lvHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHistoryDate,
            this.colHistoryElapsed,
            this.colHistoryUrl});
			this.lvHistory.ContextMenuStrip = this.cmsHistory;
			this.lvHistory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvHistory.FullRowSelect = true;
			this.lvHistory.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvHistory.HideSelection = false;
			this.lvHistory.Location = new System.Drawing.Point(0, 0);
			this.lvHistory.Margin = new System.Windows.Forms.Padding(4);
			this.lvHistory.Name = "lvHistory";
			this.lvHistory.Size = new System.Drawing.Size(200, 78);
			this.lvHistory.TabIndex = 0;
			this.lvHistory.UseCompatibleStateImageBehavior = false;
			this.lvHistory.View = System.Windows.Forms.View.Details;
			this.lvHistory.SelectedIndexChanged += new System.EventHandler(this.lvHistory_SelectedIndexChanged);
			this.lvHistory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvHistory_KeyDown);
			// 
			// colHistoryDate
			// 
			this.colHistoryDate.Text = "Date";
			// 
			// colHistoryElapsed
			// 
			this.colHistoryElapsed.Text = "Elapsed";
			// 
			// colHistoryUrl
			// 
			this.colHistoryUrl.Text = "Url";
			// 
			// cmsHistory
			// 
			this.cmsHistory.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.cmsHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiHistorySelectAll});
			this.cmsHistory.Name = "cmsHistory";
			this.cmsHistory.Size = new System.Drawing.Size(141, 28);
			this.cmsHistory.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsHistory_ItemClicked);
			// 
			// tsmiHistorySelectAll
			// 
			this.tsmiHistorySelectAll.Name = "tsmiHistorySelectAll";
			this.tsmiHistorySelectAll.Size = new System.Drawing.Size(140, 24);
			this.tsmiHistorySelectAll.Text = "Select &All";
			// 
			// rtxtResponse
			// 
			this.rtxtResponse.AcceptsTab = true;
			this.rtxtResponse.DetectUrls = false;
			this.rtxtResponse.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rtxtResponse.HideSelection = false;
			this.rtxtResponse.Location = new System.Drawing.Point(0, 0);
			this.rtxtResponse.Margin = new System.Windows.Forms.Padding(4);
			this.rtxtResponse.Name = "rtxtResponse";
			this.rtxtResponse.ReadOnly = true;
			this.rtxtResponse.Size = new System.Drawing.Size(200, 75);
			this.rtxtResponse.TabIndex = 0;
			this.rtxtResponse.Text = "";
			this.rtxtResponse.WordWrap = false;
			// 
			// PanelHttpHistory
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(tsMain);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "PanelHttpHistory";
			this.Size = new System.Drawing.Size(200, 185);
			tsMain.ResumeLayout(false);
			tsMain.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.cmsHistory.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitMain;
		private System.Windows.Forms.ListView lvHistory;
		private System.Windows.Forms.ColumnHeader colHistoryDate;
		private System.Windows.Forms.ColumnHeader colHistoryUrl;
		private System.Windows.Forms.RichTextBox rtxtResponse;
		private System.Windows.Forms.ToolStripButton tsbnRetry;
		private System.Windows.Forms.ToolStripButton tsbnRemove;
		private System.Windows.Forms.ContextMenuStrip cmsHistory;
		private System.Windows.Forms.ToolStripMenuItem tsmiHistorySelectAll;
		private System.Windows.Forms.ColumnHeader colHistoryElapsed;
	}
}
