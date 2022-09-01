using System;
using System.Collections.Generic;

public class PlayerManager
{
	private static Dictionary<string, Player> players = new Dictionary<string, Player>();

	public static bool IsOnline(string id)
	{
		return PlayerManager.players.ContainsKey(id);
	}

	public static Player GetPlayer(string id)
	{
		bool flag = PlayerManager.players.ContainsKey(id);
		Player result;
		if (flag)
		{
			result = PlayerManager.players[id];
		}
		else
		{
			result = null;
		}
		return result;
	}

	public static bool AddPlayer(string id, Player player)
	{
		try
		{
			PlayerManager.players.Add(id, player);
			bool flag = !PlayerManager.players.ContainsKey(id);
			if (flag)
			{
				ReadLog._ReadLog("null", null, true);
                return false;
            }
            else
            {
                ReadLog._ReadLog("addPlayer", null, true);
                return true;
            }
        }
		catch
		{
			ReadLog._ReadLog("注意,已添加此玩家", null, true);
            return false;
        }
    }
    public static void Broadcast(MsgBase msg)
    {
        foreach (string current in players.Keys)
        {
            Player player = PlayerManager.GetPlayer(current);
            player.Send(msg);
        }
    }
    public static void RemovePlayer(string id)
	{
		PlayerManager.players.Remove(id);
	}
}
