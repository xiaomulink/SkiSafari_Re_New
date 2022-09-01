using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Network
{
	public class room : Form
	{
		public int id;

		public static TextBox text;

		private IContainer components;

		private ListBox listBox1;

		public TextBox textBox1;

		private Label label1;

		private Label label2;

		private Label label3;

		private Label label4;

		private Label label5;

		private Label label6;

		private Label label7;

		private Timer timer1;

		public room()
		{
			this.InitializeComponent();
		}
       
		private void Groom_Load(object sender, EventArgs e)
		{
			
			TextBox textBox = this.textBox1;
			textBox.Text = (textBox.Text ?? "");
			Room room = RoomManager.GetRoom(this.id);
			RoomInfo roomInfo = new RoomInfo();
			roomInfo.id = room.id;
			this.Text = "Room" + room.id.ToString();
			base.Name = "Toom" + room.id.ToString();
			roomInfo.count = room.playerIds.Count;
		//	roomInfo.status = (int)room.status;
			this.label7.Text = room.id.ToString();
			bool flag = room.status == Room.Status.PREPARE;
			if (flag)
			{
				this.label6.Text = "准备中...";
			}
			else
			{
				this.label6.Text = "已开始";
			}
			this.label5.Text = room.ownerId.ToString();
			foreach (KeyValuePair<string, bool> current in room.playerIds)
			{
				this.listBox1.Items.Add(current.Key);
			}
			this.timer1.Interval = 1000;
			this.timer1.Start();
			Application.DoEvents();
		}

		public void GUpdate()
		{
			try
			{
				Room room = RoomManager.GetRoom(this.id);
				RoomInfo roomInfo = new RoomInfo();
				this.Text = "Room" + room.id.ToString();
				base.Name = "Toom" + room.id.ToString();
				roomInfo.id = room.id;
				roomInfo.count = room.playerIds.Count;
			//	roomInfo.status = (int)room.status;
				this.label7.Text = room.id.ToString();
				bool flag = room.status == Room.Status.PREPARE;
				if (flag)
				{
					this.label6.Text = "准备中...";
				}
				else
				{
					this.label6.Text = "已开始";
				}
				this.label5.Text = room.ownerId.ToString();
				TextBox textBox = this.textBox1;
				textBox.Text = (textBox.Text ?? "");
				foreach (KeyValuePair<string, bool> current in room.playerIds)
				{
					foreach (string b in this.listBox1.Items)
					{
						bool flag2 = current.Key == b;
						if (flag2)
						{
							return;
						}
					}
					this.listBox1.Items.Add(current.Key);
				}
				Application.DoEvents();
			}
			catch
			{
				DialogResult dialogResult = MessageBox.Show("此房间已经退出或不存在！", "房间提示", MessageBoxButtons.OKCancel);
				bool flag3 = dialogResult == DialogResult.OK;
				if (flag3)
				{
					base.Close();
				}
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.GUpdate();
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
            this.components = new System.ComponentModel.Container();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(12, 102);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(282, 316);
            this.listBox1.TabIndex = 0;

            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.MenuText;
            this.textBox1.ForeColor = System.Drawing.Color.Lime;
            this.textBox1.Location = new System.Drawing.Point(516, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(272, 222);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(205, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "房间序号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(264, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 12);
            this.label2.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(217, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "状态";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(217, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "房主Id";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(265, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "888";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(265, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "准备中";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(265, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(23, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "000";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // room
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Name = "room";
            this.Text = "room";
            this.Load += new System.EventHandler(this.Groom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

   
    }
}
