using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Netwolk_Battle
{
    public partial class Voicemix : Form
    {
        public Voicemix()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Baidu_Trans_C_Sharp AB = new Baidu_Trans_C_Sharp();
            string retString = AB.BaiduTrans(textBox1.Text);
            textBox2.Text = retString;

            Root rt = JsonConvert.DeserializeObject<Root>(retString);
            //由于这个JSON字符串的 public List data 是一个集合，所以我们需要遍历集合里面的所有数据
            for (int i = 0; i < rt.trans_result.Count; i++)
            {
                textBox3.Text = rt.trans_result[i].dst;
            }
            ExcuteDosCommand("\"C:\\softalk\\SofTalk.exe\" /R:C:\\"+textBox3.Text+".wav /W:" + textBox3.Text);
        }

        private void ExcuteDosCommand(string cmd)
        {
            /*if (System.IO.File.Exists(@"C:\_audios\"+textBox3.Text+".wav"))
            {
                ReadLog._ReadLog("文件存在");
                return;
                /*try
                {
                    System.IO.File.Delete(@"C:\_audios\test.wav");
                }
                catch
                {
                    ReadLog._ReadLog("无法删除文件");
                }*/

          /*  }
            else
            {
                ReadLog._ReadLog("文件不存在..");
            }
            */
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = "cmd";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;

                p.Start();
                StreamWriter cmdWriter = p.StandardInput;
                p.BeginOutputReadLine();
                if (!String.IsNullOrEmpty(cmd))
                {
                    cmdWriter.WriteLine(cmd);
                }
                cmdWriter.Close();
                Application.DoEvents();
                p.WaitForExit();
                p.Close();
            }
            catch
            {
                MessageBox.Show("执行命令失败，请检查输入的命令是否正确！");
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}

    

