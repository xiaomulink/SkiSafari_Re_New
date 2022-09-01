using System;
using System.Net.Sockets;

public class ClientState
{
	public Socket socket;

    public int randomkey;

    public bool iswel;

    public ByteArray readBuff = new ByteArray(1024*4);

	public bool isping =false;
	public bool isonline =true;
    public string ip="";
    public int post=0;
	public long lastPingTime;

	public Player player;
}
