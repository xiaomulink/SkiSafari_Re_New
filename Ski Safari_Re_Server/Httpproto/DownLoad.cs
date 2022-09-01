using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class DownLoad
{
    public static object httpevent(object obj)
    {

        string downloadname = "";
        HttpListenerContext ctx = (HttpListenerContext)obj;
        switch (ctx.Request.HttpMethod)
        {
            case "GET":
                {
                    string str6 = AppDomain.CurrentDomain.BaseDirectory;
                    Console.WriteLine("1" );
                  
                    string info = string.Format("当前目录：{0}", str6);
                    Console.WriteLine(info);
                    //使用path获取当前应用程序集的执行的上级目录
                    string dir1 = Path.GetFullPath("..//..//..");
                    string info1 = string.Format("上上上级目录：{0}", dir1);
                    Console.WriteLine(info1);
                    Console.WriteLine(dir1 + "\\upload\\" + ctx.Request.QueryString["downloadname"]);
                    //接收Get参数
                  
                    string resPath = dir1+ "\\upload\\" + ctx.Request.QueryString["downloadname"];
                   
                    if (File.Exists(resPath))
                    {
                        
                        try
                        {
                            downloadname =  HttpNetmanager.FileToBinary(resPath);
                        }
                        catch
                        {
                            downloadname = "处理失败";
                        }
                    }
                    else
                    {
                        downloadname = "处理失败，没有此路径";
                    }
                    
                }
                break;
        }
     
        return downloadname;
    }
}

