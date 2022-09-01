using System;

public class MsgInstantiateHuman : MsgBase
{
	public string id = "";

	public string name = "";

	public int camp;

	public string CharName = "";

    public string CharSkinName = "";
    //ÎäÆ÷id
    public string Weapon = "";

    public MsgInstantiateHuman()
	{
		this.protoName = "MsgInstantiateHuman";
	}
}
