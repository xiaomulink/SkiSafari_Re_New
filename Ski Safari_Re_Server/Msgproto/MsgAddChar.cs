using System;

public class MsgAddChar : MsgBase
{
	public string id = "";

	public string CharName = "";

	public int result;

	public MsgAddChar()
	{
		this.protoName = "MsgAddChar";
	}
}
