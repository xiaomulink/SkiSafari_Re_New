using System;

// Token: 0x02000112 RID: 274
public class MsgUseSkill : MsgBase
{
	// Token: 0x0600058B RID: 1419 RVA: 0x00028EA4 File Offset: 0x000270A4
	public MsgUseSkill()
	{
		this.protoName = "MsgUseSkill";
	}

	// Token: 0x04000723 RID: 1827
	public string id;

	// Token: 0x04000724 RID: 1828
	public string charname;

	// Token: 0x04000725 RID: 1829
	public int Skillnub;
}
