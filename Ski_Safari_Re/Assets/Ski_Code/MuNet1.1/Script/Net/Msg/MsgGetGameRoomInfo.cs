using System;

// Token: 0x020001B6 RID: 438
public class MsgGetGameRoomInfo : MsgBase
{
	// Token: 0x0600081C RID: 2076 RVA: 0x000344BC File Offset: 0x000326BC
	public MsgGetGameRoomInfo()
	{
		this.protoName = "MsgGetGameRoomInfo";
	}
    public string serverdescription;

    // Token: 0x04000946 RID: 2374
    public PlayerDataInfo[] players;
}
