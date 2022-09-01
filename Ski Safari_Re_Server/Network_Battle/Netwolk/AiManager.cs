using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Netwolk
{
	public class AiManager : Form
	{
		private IContainer components;

		private ListBox listBox1;

		public AiManager()
		{
			this.InitializeComponent();
		}

		private void AiManager_Load(object sender, EventArgs e)
		{
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
			this.listBox1 = new ListBox();
			base.SuspendLayout();
			this.listBox1.FormattingEnabled = true;
			this.listBox1.ItemHeight = 12;
			this.listBox1.Location = new Point(19, 33);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new Size(395, 316);
			this.listBox1.TabIndex = 0;
			base.AutoScaleDimensions = new SizeF(6f, 12f);
			base.ClientSize = new Size(426, 370);
			base.Controls.Add(this.listBox1);
			base.Name = "AiManager";
			this.Text = "AiManager";
			base.Load += new System.EventHandler(this.AiManager_Load);
			base.ResumeLayout(false);
		}
	}
}
