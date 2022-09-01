using System;

public class MsgJoinAi : MsgBase
{
	public int ison = 1;

	public int id;

	public int result;

	public MsgJoinAi()
	{
		this.protoName = "MsgJoinAi";
	}
}
