using System;

// Token: 0x0200019E RID: 414
public class MsgBattleTimeout : MsgBase
{
	// Token: 0x06000804 RID: 2052 RVA: 0x000341D0 File Offset: 0x000323D0
	public MsgBattleTimeout()
	{
		this.protoName = "MsgBattleTimeout";
	}

	// Token: 0x04000908 RID: 2312
	public long time;

	// Token: 0x04000909 RID: 2313
	public int result;
}
