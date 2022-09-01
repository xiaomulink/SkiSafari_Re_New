using System;

public class MsgEnterBattle : MsgBase
{
	public HumanInfo[] humans;

	public int mapId = 1;

    public float time;

    public MsgEnterBattle()
	{
		this.protoName = "MsgEnterBattle";
	}
}
