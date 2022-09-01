using System;

public class MsgGetText : MsgBase
{
	public string text = "";

	public MsgGetText()
	{
		this.protoName = "MsgGetText";
	}
}
