using System;

// Token: 0x02000113 RID: 275
public class MsgAddChar : MsgBase
{
	// Token: 0x0600058C RID: 1420 RVA: 0x00028EB8 File Offset: 0x000270B8
	public MsgAddChar()
	{
		this.protoName = "MsgAddChar";
	}

	// Token: 0x04000726 RID: 1830
	public string id = "";

	// Token: 0x04000727 RID: 1831
	public string CharName = "";

	// Token: 0x04000728 RID: 1832
	public int result;
}
