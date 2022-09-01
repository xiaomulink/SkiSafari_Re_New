using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

public class EventHandler
{
	public static void OnDisconnect(ClientState c)
	{
		ReadLog._ReadLog("Close", null, true);
		bool flag = c.player != null;
		if (flag)
		{
			int roomId = c.player.roomId;
			bool flag2 = roomId >= 0;
			if (flag2)
			{
				Room room = RoomManager.GetRoom(roomId);
				room.RemovePlayer(c.player.id);
			}
           //PlayerData pd = DbManager.GetPlayerData(c.player.id);

            //DbManager.UpdatePlayerData(c.player.id, pd);
			PlayerManager.RemovePlayer(c.player.id);
		}
	}

	public static void OnTimer()
	{
		global::EventHandler.CheckPing();
		RoomManager.Update();
	}

	public static void CheckPing()
	{
		long timeStamp = NetManager.GetTimeStamp();
		foreach (ClientState current in NetManager.clients.Values)
		{
			bool flag = timeStamp - current.lastPingTime > NetManager.pingInterval;
			if (flag)
			{
				if (current.isping)
				{
                   
                    Ping ping = new Ping();
                    PingReply pingReply = ping.Send(current.ip);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        current.isonline = true;
                        //Console.WriteLine("��ǰ���ߣ���pingͨ��");
                    }
                    else
                    {
                        current.isonline = false;
                        //Console.WriteLine("�����ߣ�ping��ͨ��");
                        ReadLog._ReadLog("Ping Close " + current.socket.RemoteEndPoint.ToString(), null, true);
                        NetManager.Close(current);
                    }

                  
                    break;
				}else
                {
                    current.lastPingTime = NetManager.GetTimeStamp();
                }
                break;
            }
        }
	}

    /// <summary>
    /// telnet port 
    /// </summary>
    /// <param name="_ip"></param>
    /// <param name="_port"></param>
    /// <returns></returns>
    private bool checkPortEnable(string _ip, int _port)
    {
        //��IP�Ͷ˿��滻��Ϊ��Ҫ����
        string ipAddress = _ip;
        int portNum = _port;
        IPAddress ip = IPAddress.Parse(ipAddress);
        IPEndPoint point = new IPEndPoint(ip, portNum);

        bool _portEnable = false;
        try
        {
            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Connect(point);
                //Console.WriteLine("����{0}�ɹ�!", point);
                sock.Close();

                _portEnable = true;
            }
        }
        catch (SocketException e)
        {
            //Console.WriteLine("����{0}ʧ��", point);
            _portEnable = false;
        }
        return _portEnable;
    }

}
