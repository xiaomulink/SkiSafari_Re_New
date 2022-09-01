using System;

// Token: 0x020001AB RID: 427
public class MsgName : MsgBase
{
	// Token: 0x06000811 RID: 2065 RVA: 0x00034380 File Offset: 0x00032580
	public MsgName()
	{
		this.protoName = "MsgName";
	}

	// Token: 0x04000926 RID: 2342
	public string Name = "";

	// Token: 0x04000927 RID: 2343
	public int result;
}
