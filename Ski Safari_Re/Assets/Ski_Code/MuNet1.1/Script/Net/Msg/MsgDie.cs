using System;

// Token: 0x0200019F RID: 415
public class MsgDie : MsgBase
{
	// Token: 0x06000805 RID: 2053 RVA: 0x000341E4 File Offset: 0x000323E4
	public MsgDie()
	{
		this.protoName = "MsgDie";
	}

	// Token: 0x0400090A RID: 2314
	public int result;

	// Token: 0x0400090B RID: 2315
	public float time;

	// Token: 0x0400090C RID: 2316
	public string name = "";

	// Token: 0x0400090D RID: 2317
	public int camp;

	// Token: 0x0400090E RID: 2318
	public string CharName = "";

	// Token: 0x0400090F RID: 2319
	public string id = "";

	public string Killerid = "";
}
