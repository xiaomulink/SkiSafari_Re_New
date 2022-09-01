using System;

public class MsgGetRoomList : MsgBase
{
	public RoomInfo[] rooms;

	public MsgGetRoomList()
	{
		this.protoName = "MsgGetRoomList";
	}
}
