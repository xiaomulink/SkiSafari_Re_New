using System;

public class MsgCreateRoom : MsgBase
{
	public string id = "";

    public string describe = "";

    public int maxplayer;

	public int Gamemode;

	public int roomid;

	public int result;

	public MsgCreateRoom()
	{
		this.protoName = "MsgCreateRoom";
	}
}
