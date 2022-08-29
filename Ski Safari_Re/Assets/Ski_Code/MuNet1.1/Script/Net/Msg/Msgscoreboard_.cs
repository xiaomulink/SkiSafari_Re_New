using System;

//比分
public class MsgScoreboardD : MsgBase
{
	public MsgScoreboardD()
	{
		this.protoName = "MsgScoreboardD";
	}


    public HumanInfo[] players;
}
