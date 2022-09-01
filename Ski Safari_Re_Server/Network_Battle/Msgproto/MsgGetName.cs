using System;

public class MsgGetName : MsgBase
{
	public string name = "";

	public string id = "";

	public MsgGetName()
	{
		this.protoName = "MsgGetName";
	}
}
