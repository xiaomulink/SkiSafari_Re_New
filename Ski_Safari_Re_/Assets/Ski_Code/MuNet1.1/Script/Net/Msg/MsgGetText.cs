using System;

// Token: 0x020001AC RID: 428
public class MsgGetText : MsgBase
{
	// Token: 0x06000812 RID: 2066 RVA: 0x000343A0 File Offset: 0x000325A0
	public MsgGetText()
	{
		this.protoName = "MsgGetText";
	}

	// Token: 0x04000928 RID: 2344
	public string text = "";
}
