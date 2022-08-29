using System;

// Token: 0x020001B0 RID: 432
public class MsgGetAchieve : MsgBase
{
	// Token: 0x06000816 RID: 2070 RVA: 0x00034438 File Offset: 0x00032638
	public MsgGetAchieve()
	{
		this.protoName = "MsgGetAchieve";
	}

	// Token: 0x0400092F RID: 2351
	public int win;

	// Token: 0x04000930 RID: 2352
	public int lost;
}
