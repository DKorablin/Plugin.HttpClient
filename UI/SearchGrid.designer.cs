namespace AlphaOmega.Windows.Forms
{
	partial class SearchGrid
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchGrid));
			this.txtFind = new System.Windows.Forms.TextBox();
			this.bnNext = new System.Windows.Forms.Button();
			this.ilIcons = new System.Windows.Forms.ImageList(this.components);
			this.bnPrevious = new System.Windows.Forms.Button();
			this.cbCase = new System.Windows.Forms.CheckBox();
			this.cbHilight = new System.Windows.Forms.CheckBox();
			this.bnClose = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// txtFind
			// 
			this.txtFind.Location = new System.Drawing.Point(37, 5);
			this.txtFind.Name = "txtFind";
			this.txtFind.Size = new System.Drawing.Size(131, 20);
			this.txtFind.TabIndex = 1;
			this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
			this.txtFind.Enter += new System.EventHandler(this.txtFind_Enter);
			this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchGrid_KeyDown);
			this.txtFind.Leave += new System.EventHandler(this.txtFind_Leave);
			// 
			// bnNext
			// 
			this.bnNext.ImageKey = "findNext.ico";
			this.bnNext.ImageList = this.ilIcons;
			this.bnNext.Location = new System.Drawing.Point(174, 3);
			this.bnNext.Name = "bnNext";
			this.bnNext.Size = new System.Drawing.Size(30, 22);
			this.bnNext.TabIndex = 2;
			this.bnNext.UseVisualStyleBackColor = true;
			this.bnNext.Click += new System.EventHandler(this.bnNext_Click);
			this.bnNext.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchGrid_KeyDown);
			// 
			// ilIcons
			// 
			this.ilIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilIcons.ImageStream")));
			this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
			this.ilIcons.Images.SetKeyName(0, "findHilight.ico");
			this.ilIcons.Images.SetKeyName(1, "findNext.ico");
			this.ilIcons.Images.SetKeyName(2, "findPrevious.ico");
			this.ilIcons.Images.SetKeyName(3, "findClose.ico");
			this.ilIcons.Images.SetKeyName(4, "findClose_Hover.ico");
			// 
			// bnPrevious
			// 
			this.bnPrevious.ImageKey = "findPrevious.ico";
			this.bnPrevious.ImageList = this.ilIcons;
			this.bnPrevious.Location = new System.Drawing.Point(210, 3);
			this.bnPrevious.Name = "bnPrevious";
			this.bnPrevious.Size = new System.Drawing.Size(30, 22);
			this.bnPrevious.TabIndex = 3;
			this.bnPrevious.UseVisualStyleBackColor = true;
			this.bnPrevious.Click += new System.EventHandler(this.bnPrevious_Click);
			this.bnPrevious.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchGrid_KeyDown);
			// 
			// cbCase
			// 
			this.cbCase.AutoSize = true;
			this.cbCase.Location = new System.Drawing.Point(313, 7);
			this.cbCase.Name = "cbCase";
			this.cbCase.Size = new System.Drawing.Size(82, 17);
			this.cbCase.TabIndex = 5;
			this.cbCase.Text = "Mat&ch case";
			this.cbCase.UseVisualStyleBackColor = true;
			this.cbCase.CheckedChanged += new System.EventHandler(this.cbCase_CheckedChanged);
			this.cbCase.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchGrid_KeyDown);
			// 
			// cbHilight
			// 
			this.cbHilight.Appearance = System.Windows.Forms.Appearance.Button;
			this.cbHilight.AutoSize = true;
			this.cbHilight.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.cbHilight.ImageKey = "findHilight.ico";
			this.cbHilight.ImageList = this.ilIcons;
			this.cbHilight.Location = new System.Drawing.Point(246, 3);
			this.cbHilight.Name = "cbHilight";
			this.cbHilight.Size = new System.Drawing.Size(61, 23);
			this.cbHilight.TabIndex = 4;
			this.cbHilight.Text = "     &Hilight";
			this.cbHilight.UseVisualStyleBackColor = true;
			this.cbHilight.CheckedChanged += new System.EventHandler(this.cbHilight_CheckedChanged);
			this.cbHilight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchGrid_KeyDown);
			// 
			// bnClose
			// 
			this.bnClose.FlatAppearance.BorderSize = 0;
			this.bnClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.bnClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.bnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.bnClose.ImageIndex = 3;
			this.bnClose.ImageList = this.ilIcons;
			this.bnClose.Location = new System.Drawing.Point(3, 3);
			this.bnClose.Name = "bnClose";
			this.bnClose.Size = new System.Drawing.Size(30, 22);
			this.bnClose.TabIndex = 6;
			this.bnClose.TabStop = false;
			this.bnClose.UseVisualStyleBackColor = true;
			this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
			this.bnClose.MouseLeave += new System.EventHandler(this.bnClose_MouseLeave);
			this.bnClose.MouseHover += new System.EventHandler(this.bnClose_MouseHover);
			// 
			// SearchGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.bnClose);
			this.Controls.Add(this.cbHilight);
			this.Controls.Add(this.cbCase);
			this.Controls.Add(this.bnPrevious);
			this.Controls.Add(this.bnNext);
			this.Controls.Add(this.txtFind);
			this.Name = "SearchGrid";
			//this.Size = new System.Drawing.Size(396, 29);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchGrid_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.Button bnNext;
		private System.Windows.Forms.Button bnPrevious;
		private System.Windows.Forms.CheckBox cbCase;
		private System.Windows.Forms.ImageList ilIcons;
		private System.Windows.Forms.CheckBox cbHilight;
		private System.Windows.Forms.Button bnClose;
	}
}
