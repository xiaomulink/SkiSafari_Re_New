using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class Test
{
    public static object httpevent(object obj)
    {

        string Request = "";
        HttpListenerContext ctx = (HttpListenerContext)obj;
        switch (ctx.Request.HttpMethod)
        {
            case "GET":
                {
                    Request = "你在使用Get请求！";
                }
                break;

            case "POST":
                {
                    Request = "你在使用POST请求！";
                }
                break;
        }
     
        return Request;
    }
}

