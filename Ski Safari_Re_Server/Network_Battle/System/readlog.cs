using System;
using System.IO;
using System.Windows.Forms;

namespace System
{
	public static class ReadLog
	{
		public static void _ReadLog(string log, string logtext = null, bool isshow = true)
		{
			bool flag = logtext == null;
			if (flag)
			{
				logtext = "log.txt";
			}
			string str = Convert.ToString(DateTime.Now);
            
            bool flag2 = File.Exists(logtext);
			if (flag2)
			{
				try
				{
					StreamWriter streamWriter = new StreamWriter(logtext, true);
					streamWriter.WriteLine(str + "   " + log);
					streamWriter.Close();
					goto IL_6B;
				}
				catch
				{
					goto IL_6B;
				}
			}
			StreamWriter streamWriter2 = File.AppendText(logtext);
			streamWriter2.WriteLine(str + "   " + log);
			streamWriter2.Close();
			IL_6B:
			if (isshow)
			{
				Console.WriteLine(str + "  " + log);
			}
		}

		public static void ReadRoomLog(string log, int roomid)
		{
			bool flag = Application.OpenForms["Room" + roomid] != null;
			if (flag)
			{
				Form form = Application.OpenForms["Room" + roomid];
				Control[] array = form.Controls.Find("textBox1", true);
				bool flag2 = array.Length != 0;
				if (flag2)
				{
					TextBox textBox = (TextBox)array[0];
					TextBox textBox2 = textBox;
					textBox2.Text = textBox2.Text + log + "\r\n";
				}
			}
		}
	}
}
