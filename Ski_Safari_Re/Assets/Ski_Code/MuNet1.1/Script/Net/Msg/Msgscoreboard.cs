using System;

//比分
public class Msgscoreboard : MsgBase
{
	public Msgscoreboard()
	{
		this.protoName = "Msgscoreboard";
	}

	// Token: 0x04000901 RID: 2305
	public int aim;

	// Token: 0x04000902 RID: 2306
	public string id = "";

    public string Killerid = "";

    // Token: 0x04000903 RID: 2307
    public string name = "";

	// Token: 0x04000904 RID: 2308
	public int camp;

	// Token: 0x04000905 RID: 2309
	public float time;

	// Token: 0x04000906 RID: 2310
	public int bule;

	// Token: 0x04000907 RID: 2311
	public int red;
}
