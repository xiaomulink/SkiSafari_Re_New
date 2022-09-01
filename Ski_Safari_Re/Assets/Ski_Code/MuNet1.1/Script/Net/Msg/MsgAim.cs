using System;

// Token: 0x020001BB RID: 443
public class MsgAim : MsgBase
{
	// Token: 0x06000821 RID: 2081 RVA: 0x00034538 File Offset: 0x00032738
	public MsgAim()
	{
		this.protoName = "MsgAim";
	}

	// Token: 0x0400095A RID: 2394
	public float x;

	// Token: 0x0400095B RID: 2395
	public float y;

	// Token: 0x0400095C RID: 2396
	public float z;

	// Token: 0x0400095D RID: 2397
	public float w;

	// Token: 0x0400095E RID: 2398
	public string id = "";
}
