using System;

// Token: 0x020001BC RID: 444
public class MsgHit : MsgBase
{
	// Token: 0x06000822 RID: 2082 RVA: 0x00034558 File Offset: 0x00032758
	public MsgHit()
	{
		this.protoName = "MsgHit";
	}

	
	public string targetId = "";

	// Token: 0x04000960 RID: 2400
	public float x;

	// Token: 0x04000961 RID: 2401
	public float y;

	// Token: 0x04000962 RID: 2402
	public float z;

	
	public string id = "";

	// Token: 0x04000964 RID: 2404
	public int hp;

	// Token: 0x04000965 RID: 2405
	public int damage;
}
