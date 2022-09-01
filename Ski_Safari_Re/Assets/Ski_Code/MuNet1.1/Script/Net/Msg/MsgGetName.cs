using System;

// Token: 0x020001A7 RID: 423
public class MsgGetName : MsgBase
{
	// Token: 0x0600080D RID: 2061 RVA: 0x000342F8 File Offset: 0x000324F8
	public MsgGetName()
	{
		this.protoName = "MsgGetName";
	}

	// Token: 0x0400091E RID: 2334
	public string name = "";

	// Token: 0x0400091F RID: 2335
	public string id = "";
}
