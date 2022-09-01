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

	public static void AddPlayer(string id, Player player)
	{
		try
		{
			PlayerManager.players.Add(id, player);
			bool flag = !PlayerManager.players.ContainsKey(id);
			if (flag)
			{
				ReadLog._ReadLog("null", null, true);
			}else
            {
                ReadLog._ReadLog("addPlayer", null, true);

            }
        }
		catch
		{
			ReadLog._ReadLog("注意,已添加此玩家", null, true);
		}
	}

	public static void RemovePlayer(string id)
	{
		PlayerManager.players.Remove(id);
	}
}
