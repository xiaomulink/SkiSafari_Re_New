using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;


internal class HttpNetmanager
{
    static bool isOpen = false;
    static Object o = new object();
    public static bool istest = false;
   
    static HttpListener listerner = new HttpListener();

    /// <summary>  
    /// 将传进来的文件转换成字符串  
    /// </summary>  
    /// <param name="FilePath">待处理的文件路径(本地或服务器)</param>  
    /// <returns></returns>
    public static string FileToBinary(string filePath)
    {
        //利用新传来的路径实例化一个FileStream对像  
        System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        //得到对象的大小
        int fileLength = Convert.ToInt32(fs.Length);
        //声明一个byte数组 
        byte[] fileByteArray = new byte[fileLength];
        //声明一个读取二进流的BinaryReader对像
        System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
        for (int i = 0; i < fileLength; i++)
        {
            //将数据读取出来放在数组中 
            br.Read(fileByteArray, 0, fileLength);
        }
        //装数组转换为String字符串
        string strData = Convert.ToBase64String(fileByteArray);
        br.Close();
        fs.Close();
        return strData;
    }
    /// <summary>  
    /// 将传进来的字符串保存为文件  
    /// </summary>  
    /// <param name="path">需要保存的位置路径</param>  
    /// <param name="binary">需要转换的字符串</param>  
    public static void BinaryToFile(string path, string binary)
    {
        System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
        //利用新传来的路径实例化一个FileStream对像  
        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
        //实例化一个用于写的BinaryWriter  
        bw.Write(Convert.FromBase64String(binary));
        bw.Close();
        fs.Close();
    }

    /// <summary>
    /// 将 Stream 写入文件
    /// </summary>
    public static void StreamToFile(Stream stream, string fileName)
    {
        // 把 Stream 转换成 byte[]
        byte[] bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        // 设置当前流的位置为流的开始
        stream.Seek(0, SeekOrigin.Begin);
        // 把 byte[] 写入文件
        FileStream fs = new FileStream(fileName, FileMode.Create);
        BinaryWriter bw = new BinaryWriter(fs);
        bw.Write(bytes);
        bw.Close();
        fs.Close();
    }
    /// <summary>
    /// 从文件读取 Stream
    /// </summary>
    public static Stream FileToStream(string fileName)
    {
        // 打开文件
        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        // 读取文件的 byte[]
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, bytes.Length);
        fileStream.Close();
        // 把 byte[] 转换成 Stream
        Stream stream = new MemoryStream(bytes);
        return stream;
    }

    public void StartLoop()
    {
        listerner = new HttpListener();
        //  while (true)
        {
            try
            {
                if (istest == true)
                {
                    listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;//指定身份验证 Anonymous匿名访问
                    listerner.Prefixes.Add("http://127.0.0.1:8880/Service/");
                    listerner.Start();
                    listerner.Prefixes.Add("http://127.0.0.1:8880/Service/get/");//添加这个就可以访问任何地方了，就很机车
                }
                else if (istest == false)
                {
                    listerner.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                    listerner.Prefixes.Add("http://+:8880/Service/");
                    listerner.Start();
                    listerner.Prefixes.Add("http://+:8880/Service/get/");
                    //Console.WriteLine("地址" + "http://" + ip + ":8880/Service/");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动失败..." + ex);
                // break;
            }
            Console.WriteLine("服务器启动成功.......");

            //线程池
            int minThreadNum;
            int portThreadNum;
            int maxThreadNum;
            ThreadPool.GetMaxThreads(out maxThreadNum, out portThreadNum);
            ThreadPool.GetMinThreads(out minThreadNum, out portThreadNum);
            Console.WriteLine("最大线程数：{0}", maxThreadNum);
            Console.WriteLine("最小空闲线程数：{0}", minThreadNum);
            //ThreadPool.QueueUserWorkItem(new WaitCallback(TaskProc1), x);

            Console.WriteLine("\n\n等待客户连接中。。。。");
            // while (true)
            {
                //等待请求连接
                //没有请求则GetContext处于阻塞状态
                try
                {
                    listerner.BeginGetContext(Proc, listerner);

                    //  ThreadPool.QueueUserWorkItem(new WaitCallback(Proc), ctx);
                }
                catch
                { }
                //Application.DoEvents();

            }

            //listerner.Stop();
        }
        //Application.DoEvents();
        // Console.ReadKey();

    }

    public void StopLoop()
    {
        if (listerner != null)
        {
            listerner.Close();
            Console.WriteLine("停止数据监听");
        }
    }


    


    public static List<string> webresult = new List<string>();

    static void Proc(IAsyncResult result)
    {
        //  listerner = result.AsyncState as HttpListener;
        try
        {
            if (listerner.IsListening)
            {
                // webresult = new List<string>();
                listerner.BeginGetContext(Proc, result);
                HttpListenerContext ctx = listerner.EndGetContext(result);
                HttpListenerRequest request = ctx.Request;
                ctx.Response.StatusCode = 200;//设置返回给客服端http状态代码

                string methodtext = request.RawUrl.ToString().Split('/')[2];

                ReadLog._ReadLog(methodtext);
                try
                {
                    methodtext = methodtext.Substring(0, methodtext.Length);
                    methodtext = methodtext.Split('=')[0];
                    object webobj = new object();

                    try
                    {
                        object[] parameters = new object[] { ctx };
                        Type type = Type.GetType(methodtext);
                        ReadLog._ReadLog("method:"+methodtext);
                        object obj = type.Assembly.CreateInstance(methodtext);
                        webobj = type.GetMethod("httpevent").Invoke(obj, parameters);
                    }
                    catch (Exception ex)
                    {
                        webobj = "你的请求错误！";
                        ReadLog._ReadLog("NullClass!" + ex);

                    }

                    //使用Writer输出http响应代码,UTF8格式
                    using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream, Encoding.UTF8))
                    {
                        try
                        {
                            writer.Write((string)webobj);
                            writer.Close();
                            ctx.Response.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                catch
                {
                    //使用Writer输出http响应代码,UTF8格式
                    using (StreamWriter writer = new StreamWriter(ctx.Response.OutputStream))
                    {
                        try
                        {
                            /*Console.WriteLine("hello");
                            writer.WriteLine("<html><head><title>The WebServer Test</title></head><body>");
                            writer.WriteLine("<div style=\"height:20px;color:blue;text-align:center;\"><p> 你好 {0}</p></div>", "mulin");
                            writer.WriteLine("<ul>");


                            foreach (string header in ctx.Request.Headers.Keys)
                            {
                                writer.WriteLine("<li><b>{0}:</b>{1}</li>", header, ctx.Request.Headers[header]);

                            }
                            writer.WriteLine("</ul>");
                            writer.WriteLine("</body></html>");
                            */
                            List<string> filelist = new List<string>();
                            StreamReader sr = new StreamReader(Environment.CurrentDirectory + "/Http/WebPage.html");
                            while (!sr.EndOfStream)//判断是否读完文件，EndOfStream表示是否是流文件的结尾
                            {
                                filelist.Add(sr.ReadLine());   // 按照行读取
                            }
                            foreach (string str in filelist)
                            {
                                writer.WriteLine(str);
                            }
                            writer.Close();
                            ctx.Response.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }



            }
        }
        catch
        {

        }
    }

    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            string size = fs.Length / 1024 + "";
            
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb + "|" + size;
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }
}

    

