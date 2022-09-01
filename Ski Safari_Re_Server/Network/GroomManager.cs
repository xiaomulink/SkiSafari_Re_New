using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Network
{
	public class GroomManager : Form
	{
		private IContainer components;

		private ListBox listBox1;
        private Button button1;
        private Button button2;
        private Timer timer1;

		public GroomManager()
		{
			this.InitializeComponent();
		}

		private void Groom_Load(object sender, EventArgs e)
		{
			foreach (Room current in RoomManager.rooms.Values)
			{
				int num = 0;
				int count = RoomManager.rooms.Count;
				RoomInfo[] array = new RoomInfo[count];
				array[num] = new RoomInfo
				{
					id = current.id,
					count = current.playerIds.Count,
					//status = (int)current.status
				};
				this.listBox1.Items.Add(array[num].id.ToString());
				this.timer1.Interval = 1000;
				this.timer1.Start();
				num++;
				Application.DoEvents();
			}
		}

		private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			int num = this.listBox1.IndexFromPoint(e.Location);
			if (num != -1)
			{
				room room = new room();
				try
				{
					room.id = int.Parse(this.listBox1.Items[num].ToString());
				}
				catch
				{
				}
				room.Show();
			}
		}

		public void Gupdate()
		{
			foreach (Room current in RoomManager.rooms.Values)
			{
				int num = 0;
				int count = RoomManager.rooms.Count;
				RoomInfo[] array = new RoomInfo[count];
				array[num] = new RoomInfo
				{
					id = current.id,
					count = current.playerIds.Count,
					//status = (int)current.status
				};
				foreach (string b in this.listBox1.Items)
				{
					bool flag = array[num].id.ToString() == b;
					if (flag)
					{
						return;
					}
				}
				this.listBox1.Items.Add(array[num].id.ToString());
				num++;
				Application.DoEvents();
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.Gupdate();
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(12, 57);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(342, 328);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "创建房间";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(104, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "删除房间";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // GroomManager
            // 
            this.ClientSize = new System.Drawing.Size(367, 399);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Name = "GroomManager";
            this.Text = "房间";
            this.Load += new System.EventHandler(this.Groom_Load);
            this.ResumeLayout(false);

		}

        private void button1_Click(object sender, EventArgs e)
        {
            CreateRoom cr = new CreateRoom();
            cr.Show();
        }
    }
}
