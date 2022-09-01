using System;

public class MsgRegister : MsgBase
{
	public string id = "";

	public string pw = "";

	public string name = "";

	public int result;

	public MsgRegister()
	{
		this.protoName = "MsgRegister";
	}
}
