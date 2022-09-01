using System;

public class MsgGetAchieve : MsgBase
{
	public int win;

	public int lost;

	public MsgGetAchieve()
	{
		this.protoName = "MsgGetAchieve";
	}
}
