using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Netwolk_Battle
{
	public class news : Form
	{
		public static news ns;

		private IContainer components;

		private RichTextBox DebugText;

		private Timer timer1;

		public news()
		{
			this.InitializeComponent();
			Application.DoEvents();
		}

		public static void i(string log)
		{
			news.ns = new news();
			ReadLog._ReadLog(log, null, true);
		}

		private void news_Load(object sender, EventArgs e)
		{
			ReadLog._ReadLog("窗口初始化成功", null, true);
			StreamReader streamReader = new StreamReader("log.txt");
			this.DebugText.Text = streamReader.ReadToEnd();
			streamReader.Close();
			this.timer1.Interval = 1000;
			this.timer1.Start();
			Application.DoEvents();
		}

		private void newsupdate()
		{
			StreamReader streamReader = new StreamReader("log.txt");
			this.DebugText.Text = streamReader.ReadToEnd();
			streamReader.Close();
			Application.DoEvents();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.newsupdate();
		}

		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			if (flag)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			this.DebugText = new RichTextBox();
			this.timer1 = new Timer(this.components);
			base.SuspendLayout();
			this.DebugText.BackColor = SystemColors.MenuText;
			this.DebugText.ForeColor = Color.Lime;
			this.DebugText.Location = new Point(22, 43);
			this.DebugText.Name = "DebugText";
			this.DebugText.Size = new Size(394, 324);
			this.DebugText.TabIndex = 0;
			this.DebugText.Text = "";
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.ClientSize = new Size(450, 373);
			base.Controls.Add(this.DebugText);
			base.Name = "news";
			this.Text = "news";
			base.Load += new System.EventHandler(this.news_Load);
			base.ResumeLayout(false);
		}
	}
}
