using System;

// Token: 0x020001A1 RID: 417
public class MsgEnterBattle : MsgBase
{
	// Token: 0x06000807 RID: 2055 RVA: 0x0003422C File Offset: 0x0003242C
	public MsgEnterBattle()
	{
		this.protoName = "MsgEnterBattle";
	}

	// Token: 0x04000911 RID: 2321
	public HumanInfo[] humans;

    public int time;

    // Token: 0x04000912 RID: 2322
    public int mapId = 1;
}
