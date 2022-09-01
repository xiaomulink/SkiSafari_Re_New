using Fleck;
using Netwolk_Battle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HttpShow
{
    public class HttpTest
    {
        //客户端url以及其对应的Socket对象字典
        public static IDictionary<string, IWebSocketConnection> dic_Sockets;
        public static WebSocketServer server;
       
        
        

        public static void Start()
        {
            //客户端url以及其对应的Socket对象字典
            dic_Sockets = new Dictionary<string, IWebSocketConnection>();
            //创建
            server = new WebSocketServer("ws://0.0.0.0:30000");//监听所有的的地址
            server.RestartAfterListenError = true;       //出错后进行重启
            HttpTest.Loop();
        }

        public static void Loop()
        {
            //开始监听
            server.Start(socket =>
                {
                    socket.OnOpen = () =>   //连接建立事件
                    {
                        //获取客户端网页的url
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        dic_Sockets.Add(clientUrl, socket);
                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 建立WebSock连接！");
                    };
                    socket.OnClose = () =>  //连接关闭事件
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        //如果存在这个客户端,那么对这个socket进行移除
                        if (dic_Sockets.ContainsKey(clientUrl))
                        {
                            //注:Fleck中有释放
                            //关闭对象连接 
                            //if (dic_Sockets[clientUrl] != null)
                            //{
                            //dic_Sockets[clientUrl].Close();
                            //}
                            dic_Sockets.Remove(clientUrl);
                        }
                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:和客户端网页:" + clientUrl + " 断开WebSock连接！");
                    };
                    socket.OnMessage = message =>  //接受客户端网页消息事件
                    {
                        string clientUrl = socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort;
                        switch (message)
                        {

                            case "开启服务器":
                                {
                                    if (Main.main.ison == false)
                                    {
                                        Send("已开启服务器");
                                        Send("", true);
                                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                                        Main.main.StartServer();
                                    }else
                                    {
                                        Send("服务器已开启!");
                                        Send("", true);
                                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);

                                    }
                                }
                                break;
                            case "关闭服务器":
                                {
                                    if (Main.main.ison == true)
                                    {
                                        Send("已关闭服务器");
                                        Send("", true);
                                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                                        Main.main.CloseServer();
                                    }
                                    else
                                    {
                                        Send("服务器已关闭!");
                                        Send("", true);
                                        Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                                    }
                                }
                                break;
                            case "刷新页面":
                                {
                                    Send("", true);
                                    Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                                    Main.main.CloseServer();
                                }
                                break;
                            default:
                                Send(message);
                                Console.WriteLine(DateTime.Now.ToString() + "|服务器:【收到】来客户端网页:" + clientUrl + "的信息：\n" + message);
                                break;
                        }
                    };
                });

               
           





        }
        
        public static void Send(string str="",bool Pagerefresh=false)
        {
            try
            {
                foreach (var item in dic_Sockets.Values)
                {
                    if (item.IsAvailable == true)
                    {
                        if (Pagerefresh == false)
                        {
                            item.Send("服务器消息：" + str + "   " + DateTime.Now.ToString());
                        }
                        else
                        {
                            item.Send("Page Refresh");
                        }
                    }
                }
            }catch
            {

            }
        }
        public static void Close()
        {
            //关闭与客户端的所有的连接
            foreach (var item in dic_Sockets.Values)
            {
                if (item != null)
                {
                    item.Close();
                }
            }
        }
        
    }
}
