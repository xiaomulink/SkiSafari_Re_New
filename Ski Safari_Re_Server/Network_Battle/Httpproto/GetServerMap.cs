using Netwolk_Battle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class GetServerMap
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

    public static void LoadDirs(string dirname)
    {
        string[] Files = Directory.GetFiles(@"" + dirname);

        foreach(string filePath in Files)
        {
            Console.WriteLine(filePath);
        }
    }
}
