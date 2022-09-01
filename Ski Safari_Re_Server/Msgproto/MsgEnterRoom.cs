using System;

public class MsgEnterRoom : MsgBase
{
	public int id;

	public int result;

	public MsgEnterRoom()
	{
		this.protoName = "MsgEnterRoom";
	}
}
