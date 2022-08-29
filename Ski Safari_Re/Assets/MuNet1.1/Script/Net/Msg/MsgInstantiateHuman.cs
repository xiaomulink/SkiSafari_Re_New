using System;

// Token: 0x020001A3 RID: 419
public class MsgInstantiateHuman : MsgBase
{
	// Token: 0x06000809 RID: 2057 RVA: 0x0003425C File Offset: 0x0003245C
	public MsgInstantiateHuman()
	{
		this.protoName = "MsgInstantiateHuman";
	}

	// Token: 0x04000914 RID: 2324
	public string id = "";

	// Token: 0x04000915 RID: 2325
	public string name = "";

	// Token: 0x04000916 RID: 2326
	public int camp;

	// Token: 0x04000917 RID: 2327
	public string CharName = "";
}
