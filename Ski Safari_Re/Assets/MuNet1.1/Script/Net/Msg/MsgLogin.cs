using System;

// Token: 0x020001A9 RID: 425
public class MsgLogin : MsgBase
{
	// Token: 0x0600080F RID: 2063 RVA: 0x00034338 File Offset: 0x00032538
	public MsgLogin()
	{
		this.protoName = "MsgLogin";
	}

	// Token: 0x04000921 RID: 2337
	public string id = "";

	// Token: 0x04000922 RID: 2338
	public string pw = "";

	// Token: 0x04000923 RID: 2339
	public string name = "";

    public int win = 0;

    public int lost= 0;

    public int coin= 0;

    // Token: 0x04000924 RID: 2340
    public int result;
}
