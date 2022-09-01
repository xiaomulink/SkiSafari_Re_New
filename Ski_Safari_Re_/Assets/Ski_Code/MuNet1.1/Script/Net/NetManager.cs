using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

// Token: 0x02000158 RID: 344
public static class NetManager
{
    public static string _ip = "";

	public static Action ConnectCallbackAction;

    // Token: 0x040007F7 RID: 2039
    private static Socket socket;

    // Token: 0x040007F8 RID: 2040
    private static ByteArray readBuff;

    // Token: 0x040007F9 RID: 2041
    private static Queue<ByteArray> writeQueue;

    public static List<MsgBase> sendMsgList =new List<MsgBase>();


    // Token: 0x040007FA RID: 2042
    public static bool isConnecting = false;

    // Token: 0x040007FB RID: 2043
    private static bool isClosing = false;

    // Token: 0x040007FC RID: 2044
    private static List<MsgBase> msgList = new List<MsgBase>();

    // Token: 0x040007FD RID: 2045
    private static int msgCount = 0;
    private static int msgSendCount = 0;

    // Token: 0x040007FE RID: 2046
    private static readonly int MAX_MESSAGE_FIRE = 10;
    private static readonly int MAX_MESSAGE_SEND = 10;

    // Token: 0x040007FF RID: 2047
    public static float pingInterval = 0.1f;

    // Token: 0x04000800 RID: 2048
    private static float lastPingTime = 0f;

    // Token: 0x04000801 RID: 2049
    public static float lastPongTime = 0f;

    public static float MsgSendUpateInterval = 0.0333333333333333f * 2;

    //下一次更新的时间
    private static float lastSendTime=0f;


    // Token: 0x04000802 RID: 2050
    private static Dictionary<NetManager.NetEvent, NetManager.EventListener> eventListeners = new Dictionary<NetManager.NetEvent, NetManager.EventListener>();

    // Token: 0x04000803 RID: 2051
    private static Dictionary<string, NetManager.MsgListener> msgListeners = new Dictionary<string, NetManager.MsgListener>();

    // Token: 0x04000804 RID: 2052
    private static string ip;

    // Token: 0x04000805 RID: 2053
    private static int port;

    // Token: 0x04000806 RID: 2054
    public static bool isture = false;

    // Token: 0x02000159 RID: 345
    public enum NetEvent
    {
        // Token: 0x04000808 RID: 2056
        ConnectSucc = 1,
        // Token: 0x04000809 RID: 2057
        ConnectFail,
        // Token: 0x0400080A RID: 2058
        Close
    }

    // Token: 0x0200015A RID: 346
    // (Invoke) Token: 0x060006C1 RID: 1729
    public delegate void EventListener(string err);

    // Token: 0x0200015B RID: 347
    // (Invoke) Token: 0x060006C5 RID: 1733
    public delegate void MsgListener(MsgBase msgBase);

    // Token: 0x060006AF RID: 1711 RVA: 0x0002D7D8 File Offset: 0x0002B9D8
    public static void AddEventListener(NetManager.NetEvent netEvent, NetManager.EventListener listener)
	{
		if (NetManager.eventListeners.ContainsKey(netEvent))
		{
			Dictionary<NetManager.NetEvent, NetManager.EventListener> dictionary = NetManager.eventListeners;
			dictionary[netEvent] = (NetManager.EventListener)Delegate.Combine(dictionary[netEvent], listener);
			return;
		}
		NetManager.eventListeners[netEvent] = listener;
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x0002D820 File Offset: 0x0002BA20
	public static void RemoveEventListener(NetManager.NetEvent netEvent, NetManager.EventListener listener)
	{
		if (NetManager.eventListeners.ContainsKey(netEvent))
		{
			Dictionary<NetManager.NetEvent, NetManager.EventListener> dictionary = NetManager.eventListeners;
			dictionary[netEvent] = (NetManager.EventListener)Delegate.Remove(dictionary[netEvent], listener);
		}
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x0002D85C File Offset: 0x0002BA5C
	private static void FireEvent(NetManager.NetEvent netEvent, string err)
	{
		if (NetManager.eventListeners.ContainsKey(netEvent))
		{
			NetManager.eventListeners[netEvent](err);
		}
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x0002D87C File Offset: 0x0002BA7C
	public static void AddMsgListener(string msgName, NetManager.MsgListener listener)
	{
		if (NetManager.msgListeners.ContainsKey(msgName))
		{
			Dictionary<string, NetManager.MsgListener> dictionary = NetManager.msgListeners;
			dictionary[msgName] = (NetManager.MsgListener)Delegate.Combine(dictionary[msgName], listener);
		}
		else
		{
			NetManager.msgListeners[msgName] = listener;
		}
		Debug.Log("Add:" + msgName);
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0002D8D8 File Offset: 0x0002BAD8
	public static void RemoveMsgListener(string msgName, NetManager.MsgListener listener)
	{
		if (NetManager.msgListeners.ContainsKey(msgName))
		{
			Dictionary<string, NetManager.MsgListener> dictionary = NetManager.msgListeners;
			dictionary[msgName] = (NetManager.MsgListener)Delegate.Remove(dictionary[msgName], listener);
		}
        Debug.Log("Remove:" + msgName);
    }

    public static void ClearMsgList()
    {
        Debug.LogError("MsgClear");
        NetManager.msgListeners.Clear();
    }

    // Token: 0x060006B4 RID: 1716 RVA: 0x0002D914 File Offset: 0x0002BB14
    private static void FireMsg(string msgName, MsgBase msgBase)
	{
		if (NetManager.msgListeners.ContainsKey(msgName))
		{
			NetManager.msgListeners[msgName](msgBase);
		}
	}

	// Token: 0x060006B5 RID: 1717 RVA: 0x0002D934 File Offset: 0x0002BB34
	public static void Connect(string ip,int _post=10021, Action Callback=null)
	{
        Debug.LogError("IpConnect");

        _ip = ip;
        Debug.LogError("Ip:"+ip+"：PingIP:" + _ip);

        //ip = "106.13.54.122";
        NetManager.port = _post;
		try
		{
			if (NetManager.socket != null && NetManager.socket.Connected)
			{
				Debug.Log("连接失败或已连接！");
			}
			else if (NetManager.isConnecting)
			{
				Debug.Log("已经在连接");
			}
			else
			{
				NetManager.InitState();
				NetManager.socket.NoDelay = true;
				NetManager.isConnecting = true;
               
                    ConnectCallbackAction = Callback;
                
                NetManager.socket.BeginConnect(ip, NetManager.port, new AsyncCallback(NetManager.ConnectCallback), NetManager.socket);

                NetManager.AddMsgListener("MsgPing", OnMsgPing);

            }

        }
        catch (SocketException ex)
		{
			Debug.LogError("连接失败，原因：" + ex.ToString());
		}
	}

    // Token: 0x060006B5 RID: 1717 RVA: 0x0002D934 File Offset: 0x0002BB34
    public static void TestConnect()
    {

        NetManager.port = 10001;
        try
        {
            if (NetManager.socket != null && NetManager.socket.Connected)
            {
                Debug.Log("连接失败或已连接！");
            }
            else if (NetManager.isConnecting)
            {
                Debug.Log("已经在连接");
            }
            else
            {
                NetManager.InitState();
                NetManager.socket.NoDelay = true;
                NetManager.isConnecting = true;
                NetManager.socket.BeginConnect("127.0.0.1", NetManager.port, new AsyncCallback(NetManager.ConnectCallback), NetManager.socket);
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError("连接失败，原因：" + ex.ToString());
        }
    }

    // Token: 0x060006B6 RID: 1718 RVA: 0x0002D9E8 File Offset: 0x0002BBE8
    private static void InitState()
	{
        Debug.LogError("初始化中。。。");

        try
        {
			NetManager.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			NetManager.readBuff = new ByteArray(1024*4);
			NetManager.writeQueue = new Queue<ByteArray>();
			NetManager.isConnecting = false;
			NetManager.isClosing = false;
			NetManager.msgList = new List<MsgBase>();
			NetManager.msgCount = 0;
            NetManager.sendMsgList = new List<MsgBase>();


            NetManager.lastPingTime = Time.time;
		}
		catch
		{
			Debug.LogError("初始化失败！");
		}
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x0002DA6C File Offset: 0x0002BC6C
	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			NetManager.FireEvent(NetManager.NetEvent.ConnectSucc, "");
			Debug.Log("Socket Connect Succ ");
            try
            {
                ConnectCallbackAction.Invoke();
            }
            catch
            {

            }
            NetManager.isConnecting = false;
			socket.BeginReceive(NetManager.readBuff.bytes, NetManager.readBuff.writeIdx, NetManager.readBuff.remain, SocketFlags.None, new AsyncCallback(NetManager.ReceiveCallback), socket);
			
        }
		catch (SocketException ex)
		{
			Debug.Log("Socket Connect fail " + ex.ToString());
			NetManager.FireEvent(NetManager.NetEvent.ConnectFail, ex.ToString());
			NetManager.isConnecting = false;
		}
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x0002DB18 File Offset: 0x0002BD18
	public static void Close()
	{
		if (NetManager.socket == null || !NetManager.socket.Connected)
		{
			return;
		}
		if (NetManager.isConnecting)
		{
			return;
		}
		if (NetManager.writeQueue.Count > 0)
		{
			NetManager.isClosing = true;
			return;
		}
		NetManager.socket.Close();
		NetManager.FireEvent(NetManager.NetEvent.Close, "");
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0002DB6C File Offset: 0x0002BD6C
	public static void Send(MsgBase msg)
	{
        if(MyGameInfo.roomMode == MyGameInfo.RoomMode.Room3RD)
        {
            NetWebServerManager.Send(msg);
            return;
        }
		if (NetManager.socket == null || !NetManager.socket.Connected)
		{
			return;
		}
		if (NetManager.isConnecting)
		{
			return;
		}
		if (NetManager.isClosing)
		{
			return;
		}
        sendMsgList.Add(msg);
        NetManager.msgSendCount++;
    }

    public static void SendMsg(MsgBase msg)
    {
        if (NetManager.socket == null || !NetManager.socket.Connected)
        {
            if (msg.protoName == "MsgChat")
            {
                Debug.LogError("1");
            }
            return;
        }
        if (NetManager.isConnecting)
        {
            if (msg.protoName == "MsgChat")
            {
                Debug.LogError("2");
            }
            return;
        }
        if (NetManager.isClosing)
        {
            if (msg.protoName == "MsgChat")
            {
                Debug.LogError("3");
            }
            return;
        }
        byte[] array = MsgBase.EncodeName(msg);
        byte[] array2 = MsgBase.Encode(msg);
        int num = array.Length + array2.Length;
        byte[] array3 = new byte[2 + num];
        array3[0] = (byte)(num % 256);
        array3[1] = (byte)(num / 256);
        Array.Copy(array, 0, array3, 2, array.Length);
        Array.Copy(array2, 0, array3, 2 + array.Length, array2.Length);
        ByteArray item = new ByteArray(array3);
        int num2 = 0;
        Queue<ByteArray> obj = NetManager.writeQueue;
        lock (obj)
        {
            NetManager.writeQueue.Enqueue(item);
            num2 = NetManager.writeQueue.Count;
        }
       
        if (num2 == 1)
        {
            NetManager.socket.BeginSend(array3, 0, array3.Length, SocketFlags.None, new AsyncCallback(NetManager.SendCallback), NetManager.socket);
        }else
        {
            if (msg.protoName == "MsgChat")
            {
                Debug.LogError("4");
            }
        }
        Debug.LogWarning(msg.protoName);
        if (msg.protoName == null || msg.protoName == "MsgPing" || msg.protoName == "MsgSyncHuman" || msg.protoName == "MsgUseSkill")
        {
            if (msg.protoName == "MsgChat")
            {
                Debug.LogError("5");
            }
            return;
        }else
        {
            
        }
        //PingLoadManager.AddPanel(msg.protoName);
        NetManager.lastPingTime = Time.time;
    }

    // Token: 0x060006BA RID: 1722 RVA: 0x0002DCC8 File Offset: 0x0002BEC8

    public static void SendCallback(IAsyncResult ar)
	{
		Socket socket = (Socket)ar.AsyncState;
		if (socket == null || !socket.Connected)
		{
			return;
		}
		int num = socket.EndSend(ar);
		Queue<ByteArray> obj = NetManager.writeQueue;
		ByteArray byteArray;
		lock (obj)
		{
			byteArray = NetManager.writeQueue.First<ByteArray>();
		}
		byteArray.readIdx += num;
		if (byteArray.length == 0)
		{
			obj = NetManager.writeQueue;
			lock (obj)
			{
				NetManager.writeQueue.Dequeue();
				byteArray = NetManager.writeQueue.First<ByteArray>();
			}
		}
		if (byteArray != null)
		{
			socket.BeginSend(byteArray.bytes, byteArray.readIdx, byteArray.length, SocketFlags.None, new AsyncCallback(NetManager.SendCallback), socket);
			return;
		}
		if (NetManager.isClosing)
		{
			socket.Close();
		}
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x0002DDBC File Offset: 0x0002BFBC
	public static void ReceiveCallback(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			int num = socket.EndReceive(ar);
			NetManager.readBuff.writeIdx += num;
			NetManager.OnReceiveData();
			if (NetManager.readBuff.remain < 8)
			{
				NetManager.readBuff.MoveBytes();
				NetManager.readBuff.ReSize(NetManager.readBuff.length * 2);
			}
			socket.BeginReceive(NetManager.readBuff.bytes, NetManager.readBuff.writeIdx, NetManager.readBuff.remain, SocketFlags.None, new AsyncCallback(NetManager.ReceiveCallback), socket);
		}
		catch (SocketException ex)
		{
            NetManager.Close();
			Debug.Log("Socket Receive fail" + ex.ToString());
		}
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x0002DE80 File Offset: 0x0002C080
	public static void OnReceiveData()
	{
		if (NetManager.readBuff.length <= 2)
		{
			return;
		}
		int readIdx = NetManager.readBuff.readIdx;
		byte[] bytes = NetManager.readBuff.bytes;
		short num = (short)((int)bytes[readIdx + 1] << 8 | (int)bytes[readIdx]);
		if (NetManager.readBuff.length < (int)num)
		{
			return;
		}
		NetManager.readBuff.readIdx += 2;
		int num2 = 0;
		string text = MsgBase.DecodeName(NetManager.readBuff.bytes, NetManager.readBuff.readIdx, out num2);
		if (text == "")
		{
			Debug.Log("OnReceiveData MsgBase.DecodeName fail");
			return;
		}
		NetManager.readBuff.readIdx += num2;
		int num3 = (int)num - num2;
		MsgBase item = MsgBase.Decode(text, NetManager.readBuff.bytes, NetManager.readBuff.readIdx, num3);
		NetManager.readBuff.readIdx += num3;
		NetManager.readBuff.CheckAndMoveBytes();
		List<MsgBase> obj = NetManager.msgList;
		lock (obj)
		{
			NetManager.msgList.Add(item);
			NetManager.msgCount++;
		}
		if (NetManager.readBuff.length > 2)
		{
			NetManager.OnReceiveData();
		}
	}

    public static float delayTime;
    public static float time;
    public static float timer =1;
    // Token: 0x060006BD RID: 1725 RVA: 0x0002DFC4 File Offset: 0x0002C1C4
    public static void Update()
	{
		NetManager.MsgUpdate();
		NetManager.MsgSendUpdate();
        PingUpDate();
    }
    // Token: 0x060006BE RID: 1726 RVA: 0x0002DFCC File Offset: 0x0002C1CC
    public static void MsgSendUpdate()
    {
        if (Time.time - lastSendTime < MsgSendUpateInterval)
        {
            return;
        }
        lastSendTime = Time.time;
        for (int i = 0; i < NetManager.MAX_MESSAGE_SEND; i++)
        {
            MsgBase msgBase = null;
            List<MsgBase> obj = NetManager.sendMsgList;
            lock (obj)
            {
                if (NetManager.sendMsgList.Count > 0)
                {
                    msgBase = NetManager.sendMsgList[0];
                    NetManager.sendMsgList.RemoveAt(0);
                    NetManager.msgSendCount--;
                }
            }
            if (msgBase == null)
            {
                break;
            }
            
            try
            {
                Debug.LogError("Send:"+msgBase.protoName);
                //处理消息
                NetManager.SendMsg(msgBase);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }

    public static void MsgUpdate()
	{
		if (NetManager.msgCount == 0)
		{
			return;
		}
		for (int i = 0; i < NetManager.MAX_MESSAGE_FIRE; i++)
		{
			MsgBase msgBase = null;
			List<MsgBase> obj = NetManager.msgList;
			lock (obj)
			{
				if (NetManager.msgList.Count > 0)
				{
					msgBase = NetManager.msgList[0];
					NetManager.msgList.RemoveAt(0);
					NetManager.msgCount--;
				}
			}
			if (msgBase == null)
			{
				break;
			}
			try
			{
				PingLoadManager.RePanel(msgBase.protoName);
			}
			catch
			{
			}
            try
            {
                //处理消息
                NetManager.FireMsg(msgBase.protoName, msgBase);
            }catch
            {

            }
		}
	}

    public static float pinglasttime;
    public static float pinginterval=2;

    public static void PingUpDate()
    {
        if(pinglasttime-Time.time> pinginterval)
        {
            MsgPing msg = new MsgPing();
            msg.return_ = "aaa bbb ccc";
            Send(msg);
        }

        if (pinglasttime - Time.time > pinginterval*2)
        {
            Debug.LogError("无法连接到服务器");
            Close();
            PanelManager.Open<Relink>();
        }
    }
    public static void OnMsgPing(MsgBase msgbase)
    {
        MsgPing msg = (MsgPing)msgbase;
        pinglasttime = Time.time;

    }
}

