using System;

public class MsgFire : MsgBase
{
    public int aumo;

    public double x;

	public double y;

	public double z;

    public double ex;

    public double ey;

    public double ez;

	public string id = "";

	public int ShootMode;

	public MsgFire()
	{
		this.protoName = "MsgFire";
	}
}
