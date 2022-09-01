using System;

public class MsgSaveText : MsgBase
{
	public string text = "";

	public int result;

	public MsgSaveText()
	{
		this.protoName = "MsgSaveText";
	}
}
