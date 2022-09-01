using System;

// Token: 0x0200019A RID: 410
public class MsgJoinAi : MsgBase
{
	// Token: 0x06000800 RID: 2048 RVA: 0x00034148 File Offset: 0x00032348
	public MsgJoinAi()
	{
		this.protoName = "MsgJoinAi";
	}

	// Token: 0x040008F8 RID: 2296
	public int ison;

	// Token: 0x040008F9 RID: 2297
	public int id;

	// Token: 0x040008FA RID: 2298
	public int result;
}
