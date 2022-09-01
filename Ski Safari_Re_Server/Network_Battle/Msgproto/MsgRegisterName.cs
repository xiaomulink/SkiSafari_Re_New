using System;

public class MsgRegisterName : MsgBase
{
	public string name = "";

	public int result;

	public MsgRegisterName()
	{
		this.protoName = "MsgRegisterName";
	}
}
