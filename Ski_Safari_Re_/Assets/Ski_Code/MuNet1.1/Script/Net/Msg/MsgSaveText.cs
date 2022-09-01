using System;

// Token: 0x020001AF RID: 431
public class MsgSaveText : MsgBase
{
	// Token: 0x06000815 RID: 2069 RVA: 0x00034418 File Offset: 0x00032618
	public MsgSaveText()
	{
		this.protoName = "MsgSaveText";
	}

	// Token: 0x0400092D RID: 2349
	public string text = "";

	// Token: 0x0400092E RID: 2350
	public int result;
}
