using System;

// Token: 0x020001A5 RID: 421
public class MsgRegister : MsgBase
{
	// Token: 0x0600080B RID: 2059 RVA: 0x000342B0 File Offset: 0x000324B0
	public MsgRegister()
	{
		this.protoName = "MsgRegister";
	}

	// Token: 0x04000919 RID: 2329
	public string id = "";

	// Token: 0x0400091A RID: 2330
	public string pw = "";

	// Token: 0x0400091B RID: 2331
	public string name = "";

	// Token: 0x0400091C RID: 2332
	public int result;
}
