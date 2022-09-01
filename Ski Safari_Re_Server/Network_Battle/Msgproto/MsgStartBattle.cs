using System;

public class MsgStartBattle : MsgBase
{
	public int result;

	public MsgStartBattle()
	{
		this.protoName = "MsgStartBattle";
	}
}
