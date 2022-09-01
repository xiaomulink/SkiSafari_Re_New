using System;

public class MsgHit : MsgBase
{
	public string targetId = "";

	public float x;

	public float y;

	public float z;

	public string id = "";

	public int hp;

	public int damage;

	public MsgHit()
	{
		this.protoName = "MsgHit";
	}
}
