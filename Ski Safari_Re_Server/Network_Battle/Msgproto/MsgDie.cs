using System;

public class MsgDie : MsgBase
{
	public int result;

	public string id = "";

	public string Killerid = "";

	public string name = "";

	public string CharName = "";

    public string CharSkinName = "";
    //ÎäÆ÷id
    public string Weapon = "";

    public int camp;

	public float time;

	public MsgDie()
	{
		this.protoName = "MsgDie";
	}
}
