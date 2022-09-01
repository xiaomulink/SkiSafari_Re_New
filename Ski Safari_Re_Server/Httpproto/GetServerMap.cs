using Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class GetServerMap
{
    class ServerData
    {
        public string playernub;
        public string serverdescription;
        public string serverping;
    }
    public static object httpevent(object obj)
    {

        string Request = "";
        HttpListenerContext ctx = (HttpListenerContext)obj;
        switch (ctx.Request.HttpMethod)
        {
            case "GET":
                {
                    Request = Main.main.serverData.MapName;
                }
                break;
        }
        return Request;
    }

    public static List<string> LoadDirs(string dirname,bool isfull=false)
    {
        List<string> Filename = new List<string>();
        string[] Files = Directory.GetFiles(@"" + dirname);

        foreach (string filePath in Files)
        {
            string file = filePath;
            if (!isfull)
            {
                string[] files = filePath.Split('\\');
                file = files[files.Length - 1];
                Console.WriteLine(filePath);
            }
            Console.WriteLine(file);
            Filename.Add(file);
        }
        return Filename;
    }
}