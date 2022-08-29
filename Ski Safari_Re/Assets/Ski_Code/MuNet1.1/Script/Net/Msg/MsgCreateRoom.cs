using System;

// Token: 0x020001B3 RID: 435
public class MsgCreateRoom : MsgBase
{
	// Token: 0x06000819 RID: 2073 RVA: 0x00034468 File Offset: 0x00032668
	public MsgCreateRoom()
	{
		this.protoName = "MsgCreateRoom";
	}

	// Token: 0x04000937 RID: 2359
	public string id = "";

	// Token: 0x04000938 RID: 2360
	public int maxplayer;

	// Token: 0x04000939 RID: 2361
	public int Gamemode;

	// Token: 0x0400093A RID: 2362
	public int roomid;

	// Token: 0x0400093B RID: 2363
	public int result;
}
