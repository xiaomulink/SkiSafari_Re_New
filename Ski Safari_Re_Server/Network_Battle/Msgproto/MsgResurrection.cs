using System;

public class MsgResurrection : MsgBase
{
	public int result;

	public string id = "";

	public string name = "";

	public string CharName = "";

    public string CharSkinName = "";
    //ÎäÆ÷id
    public string Weapon = "";

    public int camp;

	public MsgResurrection()
	{
		this.protoName = "MsgResurrection";
	}
}
