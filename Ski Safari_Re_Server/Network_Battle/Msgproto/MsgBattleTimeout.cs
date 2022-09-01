using System;

public class MsgBattleTimeout : MsgBase
{
	public long time;

	public int result;

	public MsgBattleTimeout()
	{
		this.protoName = "MsgBattleTimeout";
	}
}
