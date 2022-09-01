using System;

// Token: 0x020001AA RID: 426
public class MsgKick : MsgBase
{
	// Token: 0x06000810 RID: 2064 RVA: 0x0003436C File Offset: 0x0003256C
	public MsgKick()
	{
		this.protoName = "MsgKick";
	}

	// Token: 0x04000925 RID: 2341
	public int reason;
}
