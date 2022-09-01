using System;

public class MsgKick : MsgBase
{
	public int reason;

	public MsgKick()
	{
		this.protoName = "MsgKick";
	}
}
