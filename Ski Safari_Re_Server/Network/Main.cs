using HslCommunication.Core;
using HslCommunication.Enthernet;
using HttpShow;
using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace Network
{
	public class Main : Form
	{
        public static Main main;

		private static XmlDocument xmlDoc = new XmlDocument();

		public bool ison;

		private IContainer components;

		private MenuStrip menuStrip1;

		private ToolStripMenuItem 开始ToolStripMenuItem;

		private ToolStripMenuItem 房间ToolStripMenuItem;

		private ToolStripMenuItem 消息显示ToolStripMenuItem;

		public Button button1;

		private ToolStripMenuItem 查看房间ToolStripMenuItem;

		private Label label1;

		private Timer timer1;

		private ToolStripMenuItem 人机管理ToolStripMenuItem;

		private TextBox textBox1;

		private Label label2;

		private Button button2;
        private Button HttpServer;
        private CheckBox isTest;
        private ToolStripMenuItem 工具ToolStripMenuItem;
        private ToolStripMenuItem 语言生成ToolStripMenuItem;
        private ToolStripMenuItem 字符转文件ToolStripMenuItem;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private CheckBox checkBox1;
        private ToolStripMenuItem 查看渠道房间ToolStripMenuItem;
        private Timer timer2;

        public ServerData serverData;

        public List<string> Maps;
        public List<string> FullMaps;

        public static void stopNamedProcess(string name)
		{
			Process[] processesByName = Process.GetProcessesByName(name);
			for (int i = 0; i < processesByName.Length; i++)
			{
				Process process = processesByName[i];
				try
				{
					process.Kill();
					process.WaitForExit();
				}
				catch (Exception ex)
				{
					ReadLog._ReadLog(ex.Message, null, true);
					EventLog.WriteEntry("AlchemySearch:KillProcess", ex.Message, EventLogEntryType.Error);
				}
			}
		}

		public Main()
		{
			this.InitializeComponent();
            main = this;
        }

		public void open()
		{
			this.InitializeComponent();
		}

		private void 消息显示ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			news news = new news();
			news.Show();
		}

        #region 服务器端代码

        private UltimateFileServer ultimateFileServer;                                            // 引擎对象

        private void UltimateFileServerInitialization()
        {
            ultimateFileServer = new UltimateFileServer();                                        // 实例化对象
            ultimateFileServer.KeyToken = new Guid("A8826745-84E1-4ED4-AE2E-D3D70A9725B5");       // 指定一个令牌
            ultimateFileServer.LogNet = new HslCommunication.LogNet.LogNetSingle(Application.StartupPath + @"\Logs\log.txt");
            ultimateFileServer.FilesDirectoryPath = Application.StartupPath + @"\UltimateFile";   // 所有文件存储的基础路径
            ultimateFileServer.ServerStart(34567);                                                // 启动一个端口的引擎
            ReadLog._ReadLog("文件服务器已开启");

            // 订阅一个目录的信息，使用文件集容器实现
            GroupFileContainer container = ultimateFileServer.GetGroupFromFilePath(Application.StartupPath + @"\UltimateFile\Files\Personal\Map");
            container.FileCountChanged += Container_FileCountChanged;                         // 当文件数量发生变化时触发
        }

        private void Container_FileCountChanged(int obj)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(Container_FileCountChanged), obj);
                return;
            }
            //  label1.Text = "文件数量：" + obj.ToString();
        }



        #endregion


        #region 客户端核心引擎

        public IntegrationFileClient integrationFileClient;                 // 客户端的核心引擎

        private void IntegrationFileClientInitialization()
        {
            // 定义连接服务器的一些属性，超时时间，IP及端口信息
            integrationFileClient = new IntegrationFileClient()
            {
                ConnectTimeout = 5000,
                ServerIpEndPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 34567),
                KeyToken = new Guid("A8826745-84E1-4ED4-AE2E-D3D70A9725B5")                                         // 指定一个令牌
            };

            // 创建本地文件存储的路径
            string path = Application.StartupPath + @"\Files";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }

        #endregion

        public void StartServer()
        {
            Start start = new Start();
            bool flag = this.ison;
            this.ison = true;
            this.button1.Text = "关闭服务器";
            start.start();
        }

        public void CloseServer()
        {
            Start start = new Start();
            bool flag = this.ison;
            if (flag)
            {
                DialogResult dialogResult = MessageBox.Show("你确定要关闭服务器吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                bool flag_ = dialogResult == DialogResult.OK;
                if (flag_)
                {
                    this.button1.Text = "开启服务器";
                    start.stop();
                    this.ison = false;
                }
                try
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        bool flag2 = form.Name == "Form2";
                        if (flag2)
                        {
                            form.Close();
                        }
                    }
                    return;
                }
                catch
                {
                    return;
                }
            }
        }

        public void button1_Click(object sender, EventArgs e)
		{
			Start start = new Start();
			bool flag = this.ison;
			if (flag)
			{
                DialogResult dialogResult = MessageBox.Show("你确定要关闭服务器吗？", "版本号修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                bool flag_ = dialogResult == DialogResult.OK;
                if (flag_)
                {
                    this.button1.Text = "开启服务器";
                    start.stop();
                    this.ison = false;
                }
				try
				{
					foreach (Form form in Application.OpenForms)
					{
						bool flag2 = form.Name == "Form2";
						if (flag2)
						{
							form.Close();
						}
					}
					return;
				}
				catch
				{
					return;
				}
			}
            //关闭
            // 点击了启动服务器端的文件引擎
            UltimateFileServerInitialization();
            IntegrationFileClientInitialization();
            this.ison = true;
            this.button1.Text = "关闭服务器";
            start.start();
        }

		private void Form1_Load(object sender, EventArgs e)
		{
			this.timer2.Interval = 7200000;
			this.timer2.Start();
			this.timer1.Interval = 2000;
			this.timer1.Start();
        }

		private void 查看房间ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GroomManager groomManager = new GroomManager();
			groomManager.Show();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			this.label1.Text = "总人数：" + NetManager.TotalPlayer.ToString();
			this.textBox1.Text = NetManager.versionsvalue.ToString();
			NetManager.versionsvalue = DbManager.Getvalue();
		}

		private void timer2_Tick(object sender, EventArgs e)
		{
			bool flag = !DbManager.Connect("game_fps+rpg", "127.0.0.1", 3306, "root", "", "name");
		}

		private void 人机管理ToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			DialogResult dialogResult = MessageBox.Show("你确定要修改版本号吗？", "版本号修改", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			bool flag = dialogResult == DialogResult.OK;
			if (flag)
			{
				DbManager.versionsvalue(float.Parse(this.textBox1.Text));
				MessageBox.Show("修改成功！", "版本号修改");
			}
		}

		private void timer2_Tick_1(object sender, EventArgs e)
		{
			bool flag = !DbManager.Connect("game", "127.0.0.1", 3306, "root", "", "name");
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.开始ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.人机管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.房间ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看房间ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.消息显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.语言生成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.字符转文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.HttpServer = new System.Windows.Forms.Button();
            this.isTest = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.查看渠道房间ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.开始ToolStripMenuItem,
            this.房间ToolStripMenuItem,
            this.消息显示ToolStripMenuItem,
            this.工具ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 开始ToolStripMenuItem
            // 
            this.开始ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.人机管理ToolStripMenuItem});
            this.开始ToolStripMenuItem.Name = "开始ToolStripMenuItem";
            this.开始ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.开始ToolStripMenuItem.Text = "开始";
            // 
            // 人机管理ToolStripMenuItem
            // 
            this.人机管理ToolStripMenuItem.Name = "人机管理ToolStripMenuItem";
            this.人机管理ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.人机管理ToolStripMenuItem.Text = "人机管理";
            this.人机管理ToolStripMenuItem.Click += new System.EventHandler(this.人机管理ToolStripMenuItem_Click);
            // 
            // 房间ToolStripMenuItem
            // 
            this.房间ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.查看房间ToolStripMenuItem,
            this.查看渠道房间ToolStripMenuItem});
            this.房间ToolStripMenuItem.Name = "房间ToolStripMenuItem";
            this.房间ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.房间ToolStripMenuItem.Text = "房间";
            // 
            // 查看房间ToolStripMenuItem
            // 
            this.查看房间ToolStripMenuItem.Name = "查看房间ToolStripMenuItem";
            this.查看房间ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.查看房间ToolStripMenuItem.Text = "查看房间";
            this.查看房间ToolStripMenuItem.Click += new System.EventHandler(this.查看房间ToolStripMenuItem_Click);
            // 
            // 消息显示ToolStripMenuItem
            // 
            this.消息显示ToolStripMenuItem.Name = "消息显示ToolStripMenuItem";
            this.消息显示ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.消息显示ToolStripMenuItem.Text = "消息显示";
            this.消息显示ToolStripMenuItem.Click += new System.EventHandler(this.消息显示ToolStripMenuItem_Click);
            // 
            // 工具ToolStripMenuItem
            // 
            this.工具ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.语言生成ToolStripMenuItem,
            this.字符转文件ToolStripMenuItem});
            this.工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            this.工具ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.工具ToolStripMenuItem.Text = "工具";
            // 
            // 语言生成ToolStripMenuItem
            // 
            this.语言生成ToolStripMenuItem.Name = "语言生成ToolStripMenuItem";
            this.语言生成ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.语言生成ToolStripMenuItem.Text = "语言生成";
            this.语言生成ToolStripMenuItem.Click += new System.EventHandler(this.语言生成ToolStripMenuItem_Click);
            // 
            // 字符转文件ToolStripMenuItem
            // 
            this.字符转文件ToolStripMenuItem.Name = "字符转文件ToolStripMenuItem";
            this.字符转文件ToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.字符转文件ToolStripMenuItem.Text = "字符转文件";
            this.字符转文件ToolStripMenuItem.Click += new System.EventHandler(this.字符转文件ToolStripMenuItem_Click);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.关于ToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(525, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(114, 69);
            this.button1.TabIndex = 7;
            this.button1.Text = "开启服务器";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(177, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 25);
            this.label1.TabIndex = 9;
            this.label1.Text = "总人数：000";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(390, 185);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(178, 21);
            this.textBox1.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(255, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 27);
            this.label2.TabIndex = 12;
            this.label2.Text = "版本号：";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(574, 185);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 21);
            this.button2.TabIndex = 13;
            this.button2.Text = "修改";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick_1);
            // 
            // HttpServer
            // 
            this.HttpServer.Location = new System.Drawing.Point(656, 28);
            this.HttpServer.Name = "HttpServer";
            this.HttpServer.Size = new System.Drawing.Size(107, 69);
            this.HttpServer.TabIndex = 15;
            this.HttpServer.Text = "开启Web服务器";
            this.HttpServer.UseVisualStyleBackColor = true;
            this.HttpServer.Click += new System.EventHandler(this.HttpServer_Click);
            // 
            // isTest
            // 
            this.isTest.AutoSize = true;
            this.isTest.Location = new System.Drawing.Point(674, 103);
            this.isTest.Name = "isTest";
            this.isTest.Size = new System.Drawing.Size(72, 16);
            this.isTest.TabIndex = 17;
            this.isTest.Text = "本地测试";
            this.isTest.UseVisualStyleBackColor = true;
            this.isTest.CheckedChanged += new System.EventHandler(this.isTest_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(674, 125);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(66, 16);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "显示Log";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // 查看渠道房间ToolStripMenuItem
            // 
            this.查看渠道房间ToolStripMenuItem.Name = "查看渠道房间ToolStripMenuItem";
            this.查看渠道房间ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.查看渠道房间ToolStripMenuItem.Text = "查看渠道房间";
            // 
            // Main
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.isTest);
            this.Controls.Add(this.HttpServer);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.Text = "服务器主程序";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        bool Httpison = false;
        private void HttpServer_Click(object sender, EventArgs e)
        {
            HttpNetmanager http = new HttpNetmanager();
            if (Httpison)
            {
                
                this.HttpServer.Text = "开启Web服务器";
                this.Httpison = false;
                
                http.StopLoop();
                return;
            }
            this.Httpison = true;
          
            http.StartLoop();
            HttpTest.Start();
            this.HttpServer.Text = "关闭Web服务器";
        }
      
        private void isTest_CheckedChanged(object sender, EventArgs e)
        {
            if (isTest.Checked == true)
            {
                HttpNetmanager.istest = true;
               
            }else
            {
                HttpNetmanager.istest = false;
               
            }
        }

        private void 语言生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Voicemix voicemix = new Voicemix();
            voicemix.Show();
        }

        private void 字符转文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BinaryToFile binaryToFile = new BinaryToFile();
            binaryToFile.Show();
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.Show();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (isTest.Checked == true)
            {
                NetManager.islog = true;

            }
            else
            {
                NetManager.islog = false;
            }
        }
    }
}
