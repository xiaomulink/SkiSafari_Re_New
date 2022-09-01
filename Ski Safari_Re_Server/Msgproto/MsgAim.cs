using System;

public class MsgAim : MsgBase
{
	public float x;

	public float y;

	public float z;

	public float w;

	public string id = "";

	public MsgAim()
	{
		this.protoName = "MsgAim";
	}
}
