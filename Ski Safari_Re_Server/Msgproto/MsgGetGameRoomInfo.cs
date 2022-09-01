using System;


public class MsgGetGameRoomInfo : MsgBase
{

    public MsgGetGameRoomInfo()
    {
        this.protoName = "MsgGetGameRoomInfo";
    }
    public string serverdescription;

    // Token: 0x04000946 RID: 2374
    public Server_PlayerInfo[] players;
}
