using System;

public class MsgLogin : MsgBase
{
	public string id = "";

	public string pw = "";

	public string name = "";

	public int win = 0;

	public int lost = 0;

	public int coin = 0;

	public int result;

	public MsgLogin()
	{
		this.protoName = "MsgLogin";
	}
}
