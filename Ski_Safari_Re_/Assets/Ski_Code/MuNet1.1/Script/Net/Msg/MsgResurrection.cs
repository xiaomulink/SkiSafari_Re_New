using System;

// Token: 0x0200019C RID: 412
public class MsgResurrection : MsgBase
{
	public MsgResurrection()
	{
		this.protoName = "MsgResurrection";
	}

	// Token: 0x040008FC RID: 2300
	public int result;

	// Token: 0x040008FD RID: 2301
	public string id = "";

	// Token: 0x040008FE RID: 2302
	public string name = "";

	// Token: 0x040008FF RID: 2303
	public string CharName = "";

	public string CharSkinName = "";

    public string Weapon;

    // Token: 0x04000900 RID: 2304
    public int camp;
}
