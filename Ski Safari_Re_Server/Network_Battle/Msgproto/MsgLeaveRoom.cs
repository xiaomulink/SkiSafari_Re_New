using System;

public class MsgLeaveRoom : MsgBase
{
	public int result;

	public MsgLeaveRoom()
	{
		this.protoName = "MsgLeaveRoom";
	}
}
