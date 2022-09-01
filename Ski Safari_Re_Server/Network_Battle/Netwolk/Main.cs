using HslCommunication.Enthernet;
using HttpShow;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

namespace Netwolk_Battle
{
	public class Main : Form
	{
        public static Main main;

		private static XmlDocument xmlDoc = new XmlDocument();

		public bool ison;

		private IContainer components;

		private MenuStrip menuStrip1;

		public Button button1;

		private Label label1;

		private Timer timer1;
        private CheckBox isTest;
        private ToolStripMenuItem 关于ToolStripMenuItem;
        private CheckBox checkBox1;
        private ToolStripMenuItem 消息显示ToolStripMenuItem;
        private Label label2;
        private Label label3;
        private Label IPtext;
        private Button button2;
        private Timer timer2;
        private Label label4;
        public string nowip = "";

        public ServerData serverData;

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
            nowip=GetGlobalIP();
            IPtext.Text = nowip;
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
        }

        public void button1_Click(object sender, EventArgs e)
		{
            HttpNetmanager http = new HttpNetmanager();

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
                    if (Httpison)
                    {
                        //开启
                        this.Httpison = false;
                        HttpTest.Close();

                        http.StopLoop();
                        ultimateFileServer.ServerClose();

                        this.ison = false;

                        return;
                    }
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
            // 点击了启动服务器端的文件引擎
            UltimateFileServerInitialization();
            IntegrationFileClientInitialization();
            //关闭
            this.Httpison = true;
            serverData = new ServerData();
            http.StartLoop();
            HttpTest.Start();
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
			//NetManager.versionsvalue = DbManager.Getvalue();
		}

		private void timer2_Tick(object sender, EventArgs e)
        { 

		}

		private void 人机管理ToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

        /// <summary>
        /// 从html中通过正则找到ipv4信息
        /// </summary>
        /// <param name="pageHtml"></param>
        /// <returns></returns>
        public static string GetGlobalIP()
        {
            string pageHtml = GetHtml("http://www.net.cn/static/customercare/yourip.asp", "gbk");
            //验证ipv4地址
            string reg = @"(?:(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))\.){3}(?:(25[0-5])|(2[0-4]\d)|((1\d{2})|([1-9]?\d)))";
            string ip = "none";
            Match m = Regex.Match(pageHtml, reg);
            if (m.Success)
            {
                ip = m.Value;
            }
            Console.WriteLine("公网ip：" + ip);
            return ip;
        }

        /// <summary>
        /// 获取页面html
        /// </summary>
        /// <param name="url">请求的地址</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string GetHtml(string url, string encoding)
        {
            string pageHtml = string.Empty;
            try
            {
                using (WebClient MyWebClient = new WebClient())
                {
                    Encoding encode = Encoding.GetEncoding(encoding);
                    MyWebClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.84 Safari/537.36");
                    MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
                    Byte[] pageData = MyWebClient.DownloadData(url); //从指定网站下载数据
                    pageHtml = encode.GetString(pageData);
                }
            }
            catch (Exception e)
            {
                return "访问 " + url + " 失败，请检查网络配置";
            }
            return pageHtml;
        }


        /*
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
        */
       
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
            this.消息显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.isTest = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.IPtext = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.消息显示ToolStripMenuItem,
            this.关于ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 25);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 消息显示ToolStripMenuItem
            // 
            this.消息显示ToolStripMenuItem.Name = "消息显示ToolStripMenuItem";
            this.消息显示ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.消息显示ToolStripMenuItem.Text = "消息显示";
            this.消息显示ToolStripMenuItem.Click += new System.EventHandler(this.消息显示ToolStripMenuItem_Click);
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
            this.label1.Location = new System.Drawing.Point(376, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 25);
            this.label1.TabIndex = 9;
            this.label1.Text = "总人数：000";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(519, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 14);
            this.label2.TabIndex = 21;
            this.label2.Text = "端口";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(519, 165);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 14);
            this.label3.TabIndex = 24;
            this.label3.Text = "IP";
            // 
            // IPtext
            // 
            this.IPtext.AutoSize = true;
            this.IPtext.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.IPtext.Location = new System.Drawing.Point(562, 165);
            this.IPtext.Name = "IPtext";
            this.IPtext.Size = new System.Drawing.Size(95, 14);
            this.IPtext.TabIndex = 25;
            this.IPtext.Text = "192.168.0.1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(381, 28);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 26;
            this.button2.Text = "查看房间";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(562, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 14);
            this.label4.TabIndex = 28;
            this.label4.Text = "10028";
            // 
            // Main
            // 
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.IPtext);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.isTest);
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


        bool Httpison = false;
        private void HttpServer_Click(object sender, EventArgs e)
        {
           
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

        private void button2_Click(object sender, EventArgs e)
        {
            room room = new room();
            try
            {
                room.id = 1;
            }
            catch
            {
            }
            room.Show();
        }
    }
}
