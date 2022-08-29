using System;

// Token: 0x020001B9 RID: 441
public class MsgSyncHuman : MsgBase
{
	// Token: 0x0600081F RID: 2079 RVA: 0x000344F8 File Offset: 0x000326F8
	public MsgSyncHuman()
	{
		this.protoName = "MsgSyncHuman";
	}

	// Token: 0x04000949 RID: 2377
	public float x;

	// Token: 0x0400094A RID: 2378
	public float y;

	// Token: 0x0400094B RID: 2379
	public float z;

	// Token: 0x0400094C RID: 2380
	public float ex;

	// Token: 0x0400094D RID: 2381
	public float ey;

	// Token: 0x0400094E RID: 2382
	public float ez;

	// Token: 0x0400094F RID: 2383
	public int turn;

	// Token: 0x04000950 RID: 2384
	public string id = "";
}
