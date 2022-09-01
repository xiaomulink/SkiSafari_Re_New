using System;
using System.Collections.Generic;

public class RoomManager
{
	private static int maxId = 0;

	public static Dictionary<int, Room> rooms = new Dictionary<int, Room>();

	public static Room AddRoom()
	{
		RoomManager.maxId++;
		Room room = new Room();
        room.lasttime = NetManager.GetTimeStamp();
        room.id = RoomManager.maxId;
		RoomManager.rooms.Add(room.id, room);
		return room;
	}

	public static bool RemoveRoom(int id)
	{
		RoomManager.rooms.Remove(id);
		return true;
	}

	public static Room GetRoom(int id)
	{
		bool flag = RoomManager.rooms.ContainsKey(id);
		Room result;
		if (flag)
		{
			result = RoomManager.rooms[id];
		}
		else
		{
			result = null;
		}
		return result;
	}

	public static MsgBase ToMsg()
	{
		MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
		int count = RoomManager.rooms.Count;
		msgGetRoomList.rooms = new RoomInfo[count];
		int num = 0;
		foreach (Room current in RoomManager.rooms.Values)
		{
			RoomInfo roomInfo = new RoomInfo();
			roomInfo.id = current.id;
		//	roomInfo.gamemode = current.Roomgamemode;
            roomInfo.describe = current.describe;
			roomInfo.maxplayer = current.maxPlayer;
			roomInfo.count = current.playerIds.Count;
		//	roomInfo.status = (int)current.status;
			msgGetRoomList.rooms[num] = roomInfo;
			num++;
		}
		return msgGetRoomList;
	}

	public static void Update()
	{
		foreach (Room current in RoomManager.rooms.Values)
		{
			current.Update();
		}
	}
}
