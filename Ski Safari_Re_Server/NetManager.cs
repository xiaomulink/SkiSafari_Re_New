using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PreciseTimer;

internal class NetManager
{
	public static bool isclose;

    public static long pingInterval;

    public static bool islog;

    public static Socket listenfd;

	public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

	private static List<Socket> checkRead = new List<Socket>();

	public static int TotalPlayer = 0;

	public static float versionsvalue = 0f;

    static TimeController timeController;

    public static void StartLoop(int listenPort)
	{
		NetManager.listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		try
		{
			NetManager.listenfd.Close();
			NetManager.listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}
		catch
		{
		}
		IPAddress address = IPAddress.Parse("0.0.0.0");
		IPEndPoint localEP = new IPEndPoint(address, listenPort);
		NetManager.listenfd.Bind(localEP);
		NetManager.listenfd.Listen(0);
		ReadLog._ReadLog("[服务器]启动成功", null, true);
        //timeController = new TimeController(loop);
        loop(0.1f);
    }

	public static void loop(double ElapsedTime)
	{
        while (true)
        {
            if (!NetManager.isclose)
            {
                NetManager.ResetCheckRead();
                try
                {
                    Socket.Select(NetManager.checkRead, null, null, 1000);
                }
                catch
                {

                }
                for (int i = NetManager.checkRead.Count - 1; i >= 0; i--)
                {
                    Socket socket = NetManager.checkRead[i];
                    bool flag = socket == NetManager.listenfd;
                    if (flag)
                    {
                        NetManager.ReadListenfd(socket);
                    }
                    else
                    {
                        NetManager.ReadClientfd(socket);
                    }
                }
                NetManager.Timer();
                Application.DoEvents();
            }else
            {
                return;
            }
        }
	}

	public static void ResetCheckRead()
	{
		NetManager.checkRead.Clear();
		NetManager.checkRead.Add(NetManager.listenfd);
		foreach (ClientState current in NetManager.clients.Values)
		{
			NetManager.checkRead.Add(current.socket);
		}
	}

	public static async void ReadListenfd(Socket listenfd)
	{
		try
		{
			if (!NetManager.isclose)
			{
				Socket socket = listenfd.Accept();
                ReadLog._ReadLog("Accept " + socket.RemoteEndPoint.ToString(), null, true);
                NetManager.TotalPlayer++;
                
                ReadLog._ReadLog(socket.RemoteEndPoint.ToString(), "ip.txt", false);

                ClientState clientState = new ClientState();
				clientState.socket = socket;
                string[] str = socket.RemoteEndPoint.ToString().Split(':');

                clientState.ip = str[0];
                clientState.post = int.Parse(str[1]);
                clientState.lastPingTime = NetManager.GetTimeStamp();
				NetManager.clients.Add(socket, clientState);
                MsgWelcome msg = new MsgWelcome();
                Random n = new Random();
                msg.key = n.Next(0,200);
                clientState.randomkey = msg.key;
                NetManager.Send(clientState, msg);
                ReadLog._ReadLog("sendwelcome");
                await Task.Delay(1000);
               //MsgWelcome(socket, clientState);

            }
		}
		catch (SocketException ex)
		{
			ReadLog._ReadLog("Accept fail" + ex.ToString(), null, true);
		}
	}

    public static void MsgWelcome(Socket soc, ClientState cli)
    {
        if (!cli.iswel)
        {
            Close(cli);
            ReadLog._ReadLog("No Welcome");
        }else
        {
            ReadLog._ReadLog("Welcome");
        }
    }

    public static int GetNumberInt(string str)
	{
		int result = 0;
		bool flag = str != null && str != string.Empty;
		if (flag)
		{
			str = Regex.Replace(str, "[^\\d.\\d]", "");
			bool flag2 = Regex.IsMatch(str, "^[+-]?\\d*[.]?\\d*$");
			if (flag2)
			{
				bool flag3 = str == "";
				if (flag3)
				{
					return 0;
				}
				result = int.Parse(str);
			}
		}
		return result;
	}

	public static string RemoveNumber(string key)
	{
		return Regex.Replace(key, "\\d", "");
	}

	public static void Close(ClientState state)
	{
		try
		{
			MethodInfo method = typeof(global::EventHandler).GetMethod("OnDisconnect");
			object[] parameters = new object[]
			{
				state
			};
			method.Invoke(null, parameters);
			state.socket.Close();
			NetManager.clients.Remove(state.socket);
			NetManager.TotalPlayer--;
		}
		catch
		{
		}
	}

	public static void ReadClientfd(Socket clientfd)
	{
		ClientState clientState = NetManager.clients[clientfd];
		ByteArray readBuff = clientState.readBuff;
		int num = 0;
		bool flag = readBuff.remain <= 0;
		if (flag)
		{
			NetManager.OnReceiveData(clientState);
			readBuff.MoveBytes();
		}
		bool flag2 = readBuff.remain <= 0;
		if (flag2)
		{
			ReadLog._ReadLog("接收失败，可能是消息长度大于缓冲容量", null, true);
			NetManager.Close(clientState);
			return;
		}
		try
		{
			try
			{
				num = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, SocketFlags.None);
			}
			catch (SocketException ex)
			{
				ReadLog._ReadLog("接收Socket异常 " + ex.ToString(), null, true);
				NetManager.Close(clientState);
				return;
			}
			bool flag3 = num <= 0;
			if (flag3)
			{

				ReadLog._ReadLog("Socket Close " + clientfd.RemoteEndPoint.ToString(), null, true);
				NetManager.Close(clientState);
			}
			else
			{
				readBuff.writeIdx += num;
				NetManager.OnReceiveData(clientState);
				readBuff.CheckAndMoveBytes();
			}
		}
		catch
		{
		}
	}

    public static void OnReceiveData(ClientState state)
	{
		ByteArray readBuff = state.readBuff;
		if (readBuff.length > 2)
		{
			short num = readBuff.ReadInt16();
			if (readBuff.length >= (int)num)
			{
				int num2 = 0;
				string text = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out num2);
				bool flag = text == "";
				if (flag)
				{
					ReadLog._ReadLog("OnReceiveData MsgBase.DecodeName失败 协议名为:"+text, null, true);
					NetManager.Close(state);
					return;
				}
                if(text == "null"&&text== "Notenough")
                {
                    return;
                }
				readBuff.readIdx += num2;
				int num3 = (int)num - num2;
				MsgBase msgBase = MsgBase.Decode(text, readBuff.bytes, readBuff.readIdx, num3);
                if (msgBase == null)
                    Close(state);
				readBuff.readIdx += num3;
				readBuff.CheckAndMoveBytes();
				MethodInfo method = typeof(MsgHandler).GetMethod(text);
              
                object[] parameters = new object[]
				{
					state,
					msgBase
				};
				if (!(text == "MsgPing") && !(text == "MsgSyncPlayer"))
				{
                    //if(islog)
					ReadLog._ReadLog("Receive " + text, null, true);
				}
				bool flag2 = method != null;
				if (flag2)
				{
					method.Invoke(null, parameters);
				}
				else
				{
					ReadLog._ReadLog("OnReceiveData Invoke fail " + text, null, true);
				}
				bool flag3 = readBuff.length > 2;
				if (flag3)
				{
					NetManager.OnReceiveData(state);
				}
			}
            else
            {
                ReadLog._ReadLog("(readBuff.length < (int)num");
                Close(state);
            }
		}
        else
        {
            ReadLog._ReadLog("Length < 2");
        }
	}

	public static void Send(ClientState cs, MsgBase msg)
	{
		if (cs != null && cs.socket.Connected)
		{
			byte[] array = MsgBase.EncodeName(msg);
			byte[] array2 = MsgBase.Encode(msg);
			int num = array.Length + array2.Length;
			byte[] array3 = new byte[2 + num];
			array3[0] = (byte)(num % 256);
			array3[1] = (byte)(num / 256);
			Array.Copy(array, 0, array3, 2, array.Length);
			Array.Copy(array2, 0, array3, 2 + array.Length, array2.Length);
			try
			{
				cs.socket.BeginSend(array3, 0, array3.Length, SocketFlags.None, null, null);
			}
			catch (SocketException ex)
			{
				ReadLog._ReadLog("Socket Close on BeginSend" + ex.ToString(), null, true);
			}
		}
	}

	private static void Timer()
	{
		MethodInfo method = typeof(global::EventHandler).GetMethod("OnTimer");
		object[] parameters = new object[0];
		method.Invoke(null, parameters);
	}

	public static long GetTimeStamp()
	{
		return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
	}
}
