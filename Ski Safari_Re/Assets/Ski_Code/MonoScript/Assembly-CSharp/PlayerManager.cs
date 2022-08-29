using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public enum PlayerType
	{
		Human_1 = 0,
		Human_2 = 3,
		Human_3 = 4,
		Human_4 = 5,
		AI_1 = 1,
		COUNT = 2,
		Unregistered = 2
	}

	public enum PlayerGroup
	{
		Human = 0,
		All = 1
	}

	private static Dictionary<PlayerType, Player> s_players = new Dictionary<PlayerType, Player>();

	private static Dictionary<PlayerType, Attachment> s_attachments = new Dictionary<PlayerType, Attachment>();

	private static List<Player> s_humanPlayers = new List<Player>();

	public static Player Spawn(Player newPlayerPrefab, Vector3 position, Quaternion rotation, PlayerType playerType)
	{
		Player player = Pool.Spawn(newPlayerPrefab, position, rotation);
		RegisterPlayer(player, playerType);
		return player;
	}

	public static T SpawnReplacement<T>(Player oldPlayer, T newPlayerPrefab, Vector3 position, Quaternion rotation) where T : Player
	{
		T val = Pool.Spawn(newPlayerPrefab, position, rotation);
		ReplacePlayer(oldPlayer, val);
		return val;
	}

	public static void RegisterPlayer(Player player, PlayerType type)
	{
		s_players[type] = player;
		if (type == PlayerType.Human_1)
		{
			s_humanPlayers.Add(player);
			Player.Instance = player;
		}
	}

	public static Player GetPlayer(PlayerType type)
	{
		Player value;
		if (s_players.TryGetValue(type, out value))
		{
			return value;
		}
		return null;
	}

	public static PlayerType GetPlayerType(Player player)
	{
		foreach (KeyValuePair<PlayerType, Player> s_player in s_players)
		{
			if (s_player.Value == player)
			{
				return s_player.Key;
			}
		}
		return PlayerType.COUNT;
	}

	public static bool IsHumanPlayer(Player player)
	{
		return player == Player.Instance;
	}

	public static ICollection<Player> GetPlayers(PlayerGroup group)
	{
		if (group == PlayerGroup.Human)
		{
			return s_humanPlayers;
		}
		return s_players.Values;
	}

	private static void ReplacePlayer(Player oldPlayer, Player newPlayer)
	{
		bool flag = false;
		foreach (KeyValuePair<PlayerType, Player> s_player in s_players)
		{
			if (s_player.Value == oldPlayer)
			{
				s_players[s_player.Key] = newPlayer;
				flag = true;
				break;
			}
		}
		if (!flag)
		{
		}
		for (int i = 0; i < s_humanPlayers.Count; i++)
		{
			if (s_humanPlayers[i] == oldPlayer)
			{
				s_humanPlayers[i] = newPlayer;
			}
		}
		if (Player.Instance == oldPlayer)
		{
			Player.Instance = newPlayer;
		}
	}

	public static void ReplacePlayer(Player player, Attachment attachment)
	{
		foreach (KeyValuePair<PlayerType, Player> s_player in s_players)
		{
			if (s_player.Value == player)
			{
				s_attachments[s_player.Key] = attachment;
				s_players.Remove(s_player.Key);
				if (s_player.Key == PlayerType.Human_1)
				{
					s_humanPlayers.Remove(player);
				}
				break;
			}
		}
	}

	public static void ReplacePlayer(Attachment attachment, Player player)
	{
		foreach (KeyValuePair<PlayerType, Attachment> s_attachment in s_attachments)
		{
			if (s_attachment.Value == attachment)
			{
				s_players[s_attachment.Key] = player;
				s_attachments.Remove(s_attachment.Key);
				if (s_attachment.Key == PlayerType.Human_1)
				{
					s_humanPlayers.Add(player);
				}
				break;
			}
		}
	}

	public static void RemovePlayer(PlayerType type)
	{
		if (s_players.ContainsKey(type))
		{
			Player player = s_players[type];
			Pool.Despawn(player);
			s_players.Remove(type);
			if (type == PlayerType.Human_1)
			{
				s_humanPlayers.Remove(player);
				Player.Instance = null;
			}
		}
	}
}
