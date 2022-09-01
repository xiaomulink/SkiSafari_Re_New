using System;

public class MsgGetRoomInfo : MsgBase
{
	public Server_PlayerInfo[] players;

    public string serverdescription;
    public MsgGetRoomInfo()
	{
		this.protoName = "MsgGetRoomInfo";
	}
}
