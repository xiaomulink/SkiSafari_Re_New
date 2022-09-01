using HslCommunication;
using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Netwolk_Battle
{
    public partial class CreateRoom : Form
    {
        public CreateRoom()
        {
            InitializeComponent();
        }

    

        private void CreateRoom_Load(object sender, EventArgs e)
        {

        }

        int roomid = 0;

        private void button1_Click_2(object sender, EventArgs e)
        {
            if (maxplayer.Text==""||  describe.Text == ""||textBox1.Text == "")
            {
                if (MessageBox.Show("参数不足！", "创建房间", MessageBoxButtons.OK) == DialogResult.OK)
                {
                    //delete
                }
            }
            else
            {
                Room room = RoomManager.AddRoom();
                room.maxPlayer = int.Parse(maxplayer.Text);
                room.describe = describe.Text;
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    // 点击开始上传，此处按照实际项目需求放到了后台线程处理，事实上这种耗时的操作就应该放到后台线程
                    System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart((ThreadUploadFile)));
                    thread.IsBackground = true;
                    thread.Start(textBox1.Text);
                    string[] filename =textBox1.Text.Split('\\');
                    string resultfilename = filename[filename.Length - 1];
                    ReadLog._ReadLog("Length"+filename.Length.ToString());
                    ReadLog._ReadLog("resultfile" + resultfilename.ToString());
                    Main.main.serverData.MapName = resultfilename;
                    roomid = room.id;
                }
            }    
        }


        private void ThreadUploadFile(object filename)
        {
            if (filename is string fileName)
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
                // 开始正式上传，关于三级分类，下面只是举个例子，上传成功后去服务器端寻找文件就能明白
                OperateResult result = Main.main.integrationFileClient.UploadFile(
                    fileName,                   // 需要上传的原文件的完整路径，上传成功还需要个条件，该文件不能被占用
                    fileInfo.Name,              // 在服务器存储的文件名，带后缀，一般设置为原文件的文件名
                    "Files",                    // 第一级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Personal",                 // 第二级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "Admin",                    // 第三级分类，指示文件存储的类别，对应在服务器端存储的路径不一致
                    "服务器地图文件",         // 这个文件的额外描述文本，可以为空（""）
                    "Admin",                     // 文件的上传人，当然你也可以不使用
                    UpdateReportProgress        // 文件上传时的进度报告，如果你不需要，指定为NULL就行，一般文件比较大，带宽比较小，都需要进度提示
                    );

                // 切换到UI前台显示结果
                Invoke(new Action<OperateResult>(operateResult =>
                {
                    if (result.IsSuccess)
                    {
                        MessageBox.Show("文件上传成功！");
                        if (MessageBox.Show("创建成功！\n房间名为" + roomid, "创建房间", MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            Close();
                        }
                    }
                    else
                    {
                        // 失败原因多半来自网络异常，还有文件不存在，分类名称填写异常
                        MessageBox.Show("文件上传失败：" + result.ToMessageShowString());
                    }
                }), result);
            }
        }

        /// <summary>
        /// 用于更新上传进度的方法，该方法是线程安全的
        /// </summary>
        /// <param name="sended">已经上传的字节数</param>
        /// <param name="totle">总字节数</param>
        private void UpdateReportProgress(long sended, long totle)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action<long, long>(UpdateReportProgress), sended, totle);
                return;
            }


            // 此处代码是安全的
            int value = (int)(sended * 100L / totle);
            progressBar1.Value = value;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // GameObject.FindWithTag("moviePlayer").GetComponent<VideoPlayer>().Pause();

            OpenFileName ofn = new OpenFileName();

            ofn.structSize = Marshal.SizeOf(ofn);

            ofn.filter = "THBMFPSORRPG地图文件\0*.thgamemap\0\0";

            ofn.file = new string(new char[256]);

            ofn.maxFile = ofn.file.Length;

            ofn.fileTitle = new string(new char[64]);

            ofn.maxFileTitle = ofn.fileTitle.Length;
            string path = Environment.CurrentDirectory;
            path = path.Replace('/', '\\');
            //默认路径  
            ofn.initialDir = path;
            //ofn.initialDir = "D:\\MyProject\\UnityOpenCV\\Assets\\StreamingAssets";  
            ofn.title = "Open Project";

            ofn.defExt = "TOUHOUBMMAP";//显示文件的类型  
                                       //注意 一下项目不一定要全选 但是0x00000008项不要缺少  
            ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR  

            if (WindowDll.GetOpenFileName(ofn))
            {
                // MessageBox.Show("Selected file with full path: {0}" + ofn.file);
                textBox1.Text = ofn.file;
            }
            //此处更改了大部分答案的协程方法，在这里是采用unity的videoplayer.url方法播放视频；
            //
            /*而且我认为大部分的其他答案，所给的代码并不全，所以，想要其他功能的人，可以仿照下面的代码，直接在此类中写功能。
            //*/


        }
    }
}
