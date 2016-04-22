/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.ServiceRunners
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.txtLog = new System.Windows.Forms.TextBox();
			this.btnStartServer = new System.Windows.Forms.Button();
			this.btnStopServer = new System.Windows.Forms.Button();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.checkedListBox = new System.Windows.Forms.CheckedListBox();
			this.cbxSelectedAll = new System.Windows.Forms.CheckBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtLog
			// 
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtLog.Location = new System.Drawing.Point(0, 0);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(504, 407);
			this.txtLog.TabIndex = 1;
			this.txtLog.WordWrap = false;
			// 
			// btnStartServer
			// 
			this.btnStartServer.Enabled = false;
			this.btnStartServer.Location = new System.Drawing.Point(12, 13);
			this.btnStartServer.Name = "btnStartServer";
			this.btnStartServer.Size = new System.Drawing.Size(86, 25);
			this.btnStartServer.TabIndex = 2;
			this.btnStartServer.Text = "开始";
			this.btnStartServer.UseVisualStyleBackColor = true;
			this.btnStartServer.Click += new System.EventHandler(this.StartServerClick);
			// 
			// btnStopServer
			// 
			this.btnStopServer.Enabled = false;
			this.btnStopServer.Location = new System.Drawing.Point(113, 13);
			this.btnStopServer.Name = "btnStopServer";
			this.btnStopServer.Size = new System.Drawing.Size(83, 25);
			this.btnStopServer.TabIndex = 3;
			this.btnStopServer.Text = "停止";
			this.btnStopServer.UseVisualStyleBackColor = true;
			this.btnStopServer.Click += new System.EventHandler(this.StopServerClick);
			// 
			// notifyIcon
			// 
			this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.notifyIcon.BalloonTipText = "服务运行程序";
			this.notifyIcon.BalloonTipTitle = "服务运行提示";
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Text = "服务运行";
			this.notifyIcon.Visible = true;
			this.notifyIcon.Click += new System.EventHandler(this.NotifyIconDoubleClick);
			this.notifyIcon.DoubleClick += new System.EventHandler(this.NotifyIconDoubleClick);
			this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconMouseClick);
			// 
			// checkedListBox
			// 
			this.checkedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBox.FormattingEnabled = true;
			this.checkedListBox.HorizontalScrollbar = true;
			this.checkedListBox.Location = new System.Drawing.Point(12, 68);
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.Size = new System.Drawing.Size(184, 319);
			this.checkedListBox.TabIndex = 4;
			// 
			// cbxSelectedAll
			// 
			this.cbxSelectedAll.AutoSize = true;
			this.cbxSelectedAll.Location = new System.Drawing.Point(12, 44);
			this.cbxSelectedAll.Name = "cbxSelectedAll";
			this.cbxSelectedAll.Size = new System.Drawing.Size(50, 17);
			this.cbxSelectedAll.TabIndex = 5;
			this.cbxSelectedAll.Text = "全选";
			this.cbxSelectedAll.UseVisualStyleBackColor = true;
			this.cbxSelectedAll.CheckedChanged += new System.EventHandler(this.SelectedAllCheckedChanged);
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.AutoScroll = true;
			this.splitContainer.Panel1.Controls.Add(this.btnStartServer);
			this.splitContainer.Panel1.Controls.Add(this.checkedListBox);
			this.splitContainer.Panel1.Controls.Add(this.cbxSelectedAll);
			this.splitContainer.Panel1.Controls.Add(this.btnStopServer);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.txtLog);
			this.splitContainer.Size = new System.Drawing.Size(710, 407);
			this.splitContainer.SplitterDistance = 202;
			this.splitContainer.TabIndex = 6;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(710, 407);
			this.Controls.Add(this.splitContainer);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "服务运行";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Resize += new System.EventHandler(this.MainForm_Resize);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel1.PerformLayout();
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.Panel2.PerformLayout();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.Button btnStartServer;
		private System.Windows.Forms.Button btnStopServer;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.CheckedListBox checkedListBox;
		private System.Windows.Forms.CheckBox cbxSelectedAll;
		private System.Windows.Forms.SplitContainer splitContainer;
	}
}

