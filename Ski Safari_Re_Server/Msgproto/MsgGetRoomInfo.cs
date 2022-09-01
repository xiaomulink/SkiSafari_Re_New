using System;

public class MsgGetRoomInfo : MsgBase
{
    public MsgGetRoomInfo()
    {
        this.protoName = "MsgGetRoomInfo";
    }
    public string serverdescription;

    public Server_PlayerInfo[] players;

}
