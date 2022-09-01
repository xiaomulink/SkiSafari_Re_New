using System;

// Token: 0x020001BA RID: 442
public class MsgFire : MsgBase
{
	// Token: 0x06000820 RID: 2080 RVA: 0x00034518 File Offset: 0x00032718
	public MsgFire()
	{
		this.protoName = "MsgFire";
	}

    public int aumo;

	// Token: 0x04000951 RID: 2385
	public double x;

	// Token: 0x04000952 RID: 2386
	public double y;

	// Token: 0x04000953 RID: 2387
	public double z;

	// Token: 0x04000954 RID: 2388
	public double ex;

	// Token: 0x04000955 RID: 2389
	public double ey;

	public double ez;


	// Token: 0x04000958 RID: 2392
	public int ShootMode;

	// Token: 0x04000959 RID: 2393
	public string id = "";
}
