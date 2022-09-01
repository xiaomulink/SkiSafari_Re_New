using System;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace Netwolk_Battle
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += new ThreadExceptionEventHandler(Program.Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.CurrentDomain_UnhandledException);
                Control.CheckForIllegalCrossThreadCalls = false;
                Application.EnableVisualStyles();
                Console.WriteLine("Run");
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main());
            }
            catch (Exception ex)
            {
                string exceptionMsg = Program.GetExceptionMsg(ex, string.Empty);
                MessageBox.Show(exceptionMsg, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            string exceptionMsg = Program.GetExceptionMsg(e.Exception, e.ToString());
            MessageBox.Show(exceptionMsg, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            string exceptionMsg = Program.GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
            MessageBox.Show(exceptionMsg, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }

        public static void Time_Event(object source, ElapsedEventArgs e)
        {
        }

        private static string GetExceptionMsg(Exception ex, string backStr)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("****************************异常文本****************************");
            stringBuilder.AppendLine("【出现时间】：" + DateTime.Now.ToString());
            if (ex != null)
            {
                stringBuilder.AppendLine("【异常类型】：" + ex.GetType().Name);
                stringBuilder.AppendLine("【异常信息】：" + ex.Message);
                stringBuilder.AppendLine("【堆栈调用】：" + ex.StackTrace);
            }
            else
            {
                stringBuilder.AppendLine("【未处理异常】：" + backStr);
            }
            stringBuilder.AppendLine("***************************************************************");
            return stringBuilder.ToString();
        }
    }
}
