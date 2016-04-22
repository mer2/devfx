/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Windows.Forms;
using HTB.DevFx.Log;
using HTB.DevFx.Services;

namespace HTB.DevFx.ServiceRunners
{
	public partial class MainForm : Form
	{
		public MainForm() {
			InitializeComponent();
			CheckForIllegalCrossThreadCalls = false;
		}

		private void MainForm_Load(object sender, EventArgs e) {
			this.notifyIcon.Text = this.Text;
			LogService.LogWriting += this.LogHelperLogEvent;

			try {
				this.checkedListBox.DataSource = ServiceRunnerHost.Current.ServiceRunners;
				this.cbxSelectedAll.Checked = true;
				this.btnStartServer.Enabled = true;
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Application.Exit();
			}
		}

		private void LogHelperLogEvent(object sender, LogEventArgs[] args) {
			foreach (var e in args) {
				var log = string.Format("[{0}][{1}][{2}]\r\n{3}\r\n------------------\r\n", e.LogTime, e.Sender, e.Level, e.Message);
				if(this.Visible && this.txtLog != null) {
					if(this.txtLog.TextLength >= this.txtLog.MaxLength) {
						this.txtLog.Clear();
					}
					this.txtLog.AppendText(log);
				}
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
			try {
				ServiceRunnerHost.Current.Stop();
				LogService.LogWriting -= this.LogHelperLogEvent;
			} catch(Exception ex) {
				MessageBox.Show(ex.Message, "致命错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void NotifyIconDoubleClick(object sender, EventArgs e) {
			this.ShowInTaskbar = true;
			this.Show();
			this.WindowState = FormWindowState.Normal;
			this.Activate();
		}

		private void MainForm_Resize(object sender, EventArgs e) {
			if(this.WindowState == FormWindowState.Minimized) {
				this.Hide();
			}
		}

		private void NotifyIconMouseClick(object sender, MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				this.notifyIcon.ShowBalloonTip(1000);
			}
		}

		private void StartServerClick(object sender, EventArgs e) {
			var count = this.checkedListBox.CheckedItems.Count;
			if(count <= 0) {
				this.txtLog.AppendText("请选择需要运行的服务\r\n");
				return;
			}
			this.btnStartServer.Enabled = false;
			this.checkedListBox.Enabled = false;
			this.cbxSelectedAll.Enabled = false;
			var services = new RunnerWrap[count];
			this.checkedListBox.CheckedItems.CopyTo(services, 0);
			ServiceRunnerHost.Current.StartServices(services);
			this.btnStopServer.Enabled = true;
		}

		private void StopServerClick(object sender, EventArgs e) {
			var count = this.checkedListBox.CheckedItems.Count;
			if (count <= 0) {
				return;
			}
			this.btnStopServer.Enabled = false;
			var services = new RunnerWrap[count];
			this.checkedListBox.CheckedItems.CopyTo(services, 0);
			ServiceRunnerHost.Current.StopServices(services);
			this.checkedListBox.Enabled = true;
			this.cbxSelectedAll.Enabled = true;
			this.btnStartServer.Enabled = true;
		}

		private void SelectedAllCheckedChanged(object sender, EventArgs e) {
			for(var i = 0; i < this.checkedListBox.Items.Count; i++) {
				this.checkedListBox.SetItemChecked(i, this.cbxSelectedAll.Checked);
			}
		}
	}
}