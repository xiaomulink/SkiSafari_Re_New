using System;

// Token: 0x020001B2 RID: 434
public class MsgGetRoomList : MsgBase
{
	// Token: 0x06000818 RID: 2072 RVA: 0x00034454 File Offset: 0x00032654
	public MsgGetRoomList()
	{
		this.protoName = "MsgGetRoomList";
	}

	// Token: 0x04000936 RID: 2358
	public RoomInfo[] rooms;
}
