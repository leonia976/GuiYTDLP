using System.Drawing;

namespace GuiYTDLP;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    private System.Windows.Forms.GroupBox grpParameters;
    private System.Windows.Forms.TextBox txtUrls;
    private System.Windows.Forms.TextBox txtSavePath;
    private System.Windows.Forms.Button btnBrowse;
    private System.Windows.Forms.ComboBox cmbPreset;
    private System.Windows.Forms.Button btnAdd;
    private System.Windows.Forms.GroupBox grpDownloads;
    private System.Windows.Forms.ListView lvDownloads;
    private System.Windows.Forms.ColumnHeader colTitle;
    private System.Windows.Forms.ColumnHeader colPreset;
    private System.Windows.Forms.ColumnHeader colSize;
    private System.Windows.Forms.ColumnHeader colProgress;
    private System.Windows.Forms.ColumnHeader colStatus;
    private System.Windows.Forms.ColumnHeader colSpeed;
    private System.Windows.Forms.ColumnHeader colEta;
    private System.Windows.Forms.ContextMenuStrip contextMenu;
    private System.Windows.Forms.ToolStripMenuItem menuDelete;
    private System.Windows.Forms.ToolStripMenuItem menuCopyUrl;
    private System.Windows.Forms.ToolStripMenuItem menuOpenFolder;
    private System.Windows.Forms.Button btnClearAll;
    private System.Windows.Forms.Button btnStartDownload;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        grpParameters = new GroupBox();
        btnClearLink = new Button();
        BtnPaste = new Button();
        chkPlaylist = new CheckBox();
        btnAdd = new Button();
        cmbPreset = new ComboBox();
        btnBrowse = new Button();
        txtSavePath = new TextBox();
        txtUrls = new TextBox();
        grpDownloads = new GroupBox();
        lvDownloads = new ListView();
        colTitle = new ColumnHeader();
        colPreset = new ColumnHeader();
        colSize = new ColumnHeader();
        colProgress = new ColumnHeader();
        colStatus = new ColumnHeader();
        colSpeed = new ColumnHeader();
        colEta = new ColumnHeader();
        contextMenu = new ContextMenuStrip(components);
        menuDelete = new ToolStripMenuItem();
        menuCopyUrl = new ToolStripMenuItem();
        menuOpenFolder = new ToolStripMenuItem();
        btnClearAll = new Button();
        btnStartDownload = new Button();
        btnUpdateBin = new Button();
        toolTip1 = new ToolTip(components);
        btnOpenFolder = new Button();
        grpParameters.SuspendLayout();
        grpDownloads.SuspendLayout();
        contextMenu.SuspendLayout();
        SuspendLayout();
        // 
        // grpParameters
        // 
        grpParameters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grpParameters.Controls.Add(btnClearLink);
        grpParameters.Controls.Add(BtnPaste);
        grpParameters.Controls.Add(chkPlaylist);
        grpParameters.Controls.Add(btnAdd);
        grpParameters.Controls.Add(cmbPreset);
        grpParameters.Controls.Add(btnBrowse);
        grpParameters.Controls.Add(txtSavePath);
        grpParameters.Controls.Add(txtUrls);
        grpParameters.Font = new Font("Cascadia Code", 9F);
        grpParameters.ForeColor = Color.FromArgb(0, 102, 204);
        grpParameters.Location = new Point(12, 10);
        grpParameters.Name = "grpParameters";
        grpParameters.Size = new Size(950, 175);
        grpParameters.TabIndex = 0;
        grpParameters.TabStop = false;
        grpParameters.Text = "Parameters - Links";
        // 
        // btnClearLink
        // 
        btnClearLink.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnClearLink.BackColor = Color.White;
        btnClearLink.FlatStyle = FlatStyle.System;
        btnClearLink.Font = new Font("Cascadia Code", 9F);
        btnClearLink.Location = new Point(902, 55);
        btnClearLink.Name = "btnClearLink";
        btnClearLink.Size = new Size(30, 30);
        btnClearLink.TabIndex = 9;
        btnClearLink.Text = "✕";
        toolTip1.SetToolTip(btnClearLink, "Clear link(s) ");
        btnClearLink.UseVisualStyleBackColor = false;
        btnClearLink.Click += btnClearLink_Click;
        // 
        // BtnPaste
        // 
        BtnPaste.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        BtnPaste.BackColor = Color.White;
        BtnPaste.FlatStyle = FlatStyle.System;
        BtnPaste.Font = new Font("Cascadia Code", 9F);
        BtnPaste.Location = new Point(902, 23);
        BtnPaste.Name = "BtnPaste";
        BtnPaste.Size = new Size(30, 30);
        BtnPaste.TabIndex = 8;
        BtnPaste.Text = "📋︎";
        toolTip1.SetToolTip(BtnPaste, "Paste link(s) ");
        BtnPaste.UseVisualStyleBackColor = false;
        BtnPaste.Click += BtnPaste_Click;
        // 
        // chkPlaylist
        // 
        chkPlaylist.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        chkPlaylist.AutoSize = true;
        chkPlaylist.Location = new Point(591, 134);
        chkPlaylist.Name = "chkPlaylist";
        chkPlaylist.Size = new Size(89, 20);
        chkPlaylist.TabIndex = 7;
        chkPlaylist.Text = "Show List";
        chkPlaylist.UseVisualStyleBackColor = true;
        chkPlaylist.CheckedChanged += chkPlaylist_CheckedChanged;
        // 
        // btnAdd
        // 
        btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAdd.BackColor = Color.White;
        btnAdd.FlatStyle = FlatStyle.System;
        btnAdd.Font = new Font("Cascadia Code", 9F);
        btnAdd.Location = new Point(902, 88);
        btnAdd.Name = "btnAdd";
        btnAdd.Size = new Size(30, 30);
        btnAdd.TabIndex = 6;
        btnAdd.Text = "+";
        toolTip1.SetToolTip(btnAdd, "Add link(s) to list");
        btnAdd.UseVisualStyleBackColor = false;
        btnAdd.Click += BtnAdd_Click;
        // 
        // cmbPreset
        // 
        cmbPreset.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cmbPreset.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPreset.FlatStyle = FlatStyle.Flat;
        cmbPreset.Font = new Font("Cascadia Code", 9F);
        cmbPreset.FormattingEnabled = true;
        cmbPreset.Items.AddRange(new object[] { "best", "mp4 (bestvideo+bestaudio)", "1080p", "720p", "mp3 (audio only)" });
        cmbPreset.Location = new Point(684, 132);
        cmbPreset.Name = "cmbPreset";
        cmbPreset.Size = new Size(248, 24);
        cmbPreset.TabIndex = 5;
        // 
        // btnBrowse
        // 
        btnBrowse.BackColor = Color.White;
        btnBrowse.FlatStyle = FlatStyle.System;
        btnBrowse.Font = new Font("Cascadia Code", 9F);
        btnBrowse.Location = new Point(12, 131);
        btnBrowse.Name = "btnBrowse";
        btnBrowse.Size = new Size(74, 23);
        btnBrowse.TabIndex = 4;
        btnBrowse.Text = "Save to:";
        btnBrowse.UseVisualStyleBackColor = false;
        btnBrowse.Click += BtnBrowse_Click;
        // 
        // txtSavePath
        // 
        txtSavePath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtSavePath.BorderStyle = BorderStyle.FixedSingle;
        txtSavePath.Font = new Font("Cascadia Code", 9F);
        txtSavePath.Location = new Point(92, 133);
        txtSavePath.Name = "txtSavePath";
        txtSavePath.PlaceholderText = "output dir folder";
        txtSavePath.ReadOnly = true;
        txtSavePath.Size = new Size(493, 21);
        txtSavePath.TabIndex = 3;
        // 
        // txtUrls
        // 
        txtUrls.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtUrls.BorderStyle = BorderStyle.FixedSingle;
        txtUrls.Font = new Font("Lucida Console", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        txtUrls.Location = new Point(12, 23);
        txtUrls.Multiline = true;
        txtUrls.Name = "txtUrls";
        txtUrls.ScrollBars = ScrollBars.Vertical;
        txtUrls.Size = new Size(874, 95);
        txtUrls.TabIndex = 1;
        // 
        // grpDownloads
        // 
        grpDownloads.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        grpDownloads.Controls.Add(lvDownloads);
        grpDownloads.Font = new Font("Cascadia Code", 9F);
        grpDownloads.ForeColor = Color.FromArgb(0, 102, 204);
        grpDownloads.Location = new Point(12, 195);
        grpDownloads.Name = "grpDownloads";
        grpDownloads.Size = new Size(950, 385);
        grpDownloads.TabIndex = 1;
        grpDownloads.TabStop = false;
        grpDownloads.Text = "Downloads";
        // 
        // lvDownloads
        // 
        lvDownloads.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lvDownloads.BackColor = Color.White;
        lvDownloads.BorderStyle = BorderStyle.None;
        lvDownloads.Columns.AddRange(new ColumnHeader[] { colTitle, colPreset, colSize, colProgress, colStatus, colSpeed, colEta });
        lvDownloads.ContextMenuStrip = contextMenu;
        lvDownloads.Font = new Font("Cascadia Code", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lvDownloads.FullRowSelect = true;
        lvDownloads.Location = new Point(12, 25);
        lvDownloads.Name = "lvDownloads";
        lvDownloads.OwnerDraw = true;
        lvDownloads.Size = new Size(926, 348);
        lvDownloads.TabIndex = 0;
        lvDownloads.UseCompatibleStateImageBehavior = false;
        lvDownloads.View = View.Details;
        lvDownloads.DrawColumnHeader += LvDownloads_DrawColumnHeader;
        lvDownloads.DrawItem += LvDownloads_DrawItem;
        lvDownloads.DrawSubItem += LvDownloads_DrawSubItem;
        // 
        // colTitle
        // 
        colTitle.Text = "Title";
        colTitle.Width = 220;
        // 
        // colPreset
        // 
        colPreset.Text = "Preset";
        colPreset.TextAlign = HorizontalAlignment.Center;
        colPreset.Width = 95;
        // 
        // colSize
        // 
        colSize.Text = "Size";
        colSize.TextAlign = HorizontalAlignment.Center;
        colSize.Width = 80;
        // 
        // colProgress
        // 
        colProgress.Text = "Progress";
        colProgress.TextAlign = HorizontalAlignment.Center;
        colProgress.Width = 135;
        // 
        // colStatus
        // 
        colStatus.Text = "Status";
        colStatus.TextAlign = HorizontalAlignment.Center;
        colStatus.Width = 105;
        // 
        // colSpeed
        // 
        colSpeed.Text = "Speed";
        colSpeed.TextAlign = HorizontalAlignment.Center;
        colSpeed.Width = 95;
        // 
        // colEta
        // 
        colEta.Text = "ETA";
        colEta.TextAlign = HorizontalAlignment.Center;
        colEta.Width = 75;
        // 
        // contextMenu
        // 
        contextMenu.Items.AddRange(new ToolStripItem[] { menuDelete, menuCopyUrl, menuOpenFolder });
        contextMenu.Name = "contextMenu";
        contextMenu.ShowImageMargin = false;
        contextMenu.Size = new Size(130, 70);
        // 
        // menuDelete
        // 
        menuDelete.ImageAlign = ContentAlignment.MiddleLeft;
        menuDelete.Name = "menuDelete";
        menuDelete.Size = new Size(129, 22);
        menuDelete.Text = "🗑 Delete";
        menuDelete.Click += MenuDelete_Click;
        // 
        // menuCopyUrl
        // 
        menuCopyUrl.Name = "menuCopyUrl";
        menuCopyUrl.Size = new Size(129, 22);
        menuCopyUrl.Text = "⧉ Copy URL";
        menuCopyUrl.Click += MenuCopyUrl_Click;
        // 
        // menuOpenFolder
        // 
        menuOpenFolder.Name = "menuOpenFolder";
        menuOpenFolder.Size = new Size(129, 22);
        menuOpenFolder.Text = "📂 Open Folder";
        menuOpenFolder.Click += MenuOpenFolder_Click;
        // 
        // btnClearAll
        // 
        btnClearAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnClearAll.BackColor = Color.White;
        btnClearAll.FlatStyle = FlatStyle.System;
        btnClearAll.Font = new Font("Cascadia Code", 9F);
        btnClearAll.Location = new Point(847, 592);
        btnClearAll.Name = "btnClearAll";
        btnClearAll.Size = new Size(40, 40);
        btnClearAll.TabIndex = 2;
        btnClearAll.Text = "🗑";
        btnClearAll.UseVisualStyleBackColor = false;
        btnClearAll.Click += BtnClearAll_Click;
        // 
        // btnStartDownload
        // 
        btnStartDownload.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnStartDownload.BackColor = Color.White;
        btnStartDownload.FlatStyle = FlatStyle.System;
        btnStartDownload.Font = new Font("Cascadia Code", 9F);
        btnStartDownload.Location = new Point(909, 592);
        btnStartDownload.Name = "btnStartDownload";
        btnStartDownload.Size = new Size(40, 40);
        btnStartDownload.TabIndex = 3;
        btnStartDownload.Text = "⬇⬇";
        toolTip1.SetToolTip(btnStartDownload, "Go Download");
        btnStartDownload.UseVisualStyleBackColor = false;
        btnStartDownload.Click += BtnStartDownload_Click;
        // 
        // btnUpdateBin
        // 
        btnUpdateBin.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnUpdateBin.BackColor = Color.White;
        btnUpdateBin.FlatStyle = FlatStyle.System;
        btnUpdateBin.Font = new Font("Cascadia Code", 9F);
        btnUpdateBin.Location = new Point(12, 592);
        btnUpdateBin.Name = "btnUpdateBin";
        btnUpdateBin.Size = new Size(148, 40);
        btnUpdateBin.TabIndex = 4;
        btnUpdateBin.Text = "🔄 Download Binary";
        toolTip1.SetToolTip(btnUpdateBin, "Update binary");
        btnUpdateBin.UseVisualStyleBackColor = false;
        btnUpdateBin.Click += btnUpdateBin_Click;
        // 
        // btnOpenFolder
        // 
        btnOpenFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnOpenFolder.Location = new Point(166, 592);
        btnOpenFolder.Name = "btnOpenFolder";
        btnOpenFolder.Size = new Size(40, 40);
        btnOpenFolder.TabIndex = 5;
        btnOpenFolder.Text = "🗁";
        btnOpenFolder.UseVisualStyleBackColor = true;
        btnOpenFolder.Click += btnOpenFolder_Click;
        // 
        // Form1
        // 
        BackColor = Color.FromArgb(243, 243, 243);
        ClientSize = new Size(974, 641);
        Controls.Add(btnOpenFolder);
        Controls.Add(btnUpdateBin);
        Controls.Add(btnStartDownload);
        Controls.Add(btnClearAll);
        Controls.Add(grpDownloads);
        Controls.Add(grpParameters);
        Font = new Font("Segoe UI", 9.5F);
        Icon = (Icon)resources.GetObject("$this.Icon");
        MinimumSize = new Size(990, 680);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Gui YTDLP";
        Load += Form1_Load;
        grpParameters.ResumeLayout(false);
        grpParameters.PerformLayout();
        grpDownloads.ResumeLayout(false);
        contextMenu.ResumeLayout(false);
        ResumeLayout(false);
    }
    private Button btnUpdateBin;
    private ToolTip toolTip1;
    private Button btnOpenFolder;
    private CheckBox chkPlaylist;
    private Button BtnPaste;
    private Button btnClearLink;
}