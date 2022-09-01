using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Profile
{
	private struct TimestampedString
	{
		public string str;

		public long date;
	}

	private class Stat
	{
		public int total;

		public int sessionMax;

		public int instanceMax;

		public int sessionTotal;

		public Stat()
		{
		}

		public Stat(int initialAmount)
		{
			total = (sessionMax = (instanceMax = (sessionTotal = initialAmount)));
		}

		public Stat(Stat other)
		{
			total = other.total;
			sessionMax = other.sessionMax;
			instanceMax = other.instanceMax;
			sessionTotal = other.sessionTotal;
		}
	}

	private class StatGroup
	{
		public Stat primary = new Stat();

		public Dictionary<string, Stat> variants = new Dictionary<string, Stat>();

		public StatGroup()
		{
		}

		public StatGroup(StatGroup other)
		{
			primary = new Stat(other.primary);
			foreach (KeyValuePair<string, Stat> variant in other.variants)
			{
				variants[variant.Key] = new Stat(variant.Value);
			}
		}
	}

	public class LeaderboardEntry : IComparable
	{
		public string name;

		public float score;

		public int rank;

		public bool isSubmitted;

		public bool isRecent;

		public int CompareTo(object obj)
		{
			LeaderboardEntry leaderboardEntry = obj as LeaderboardEntry;
			if (leaderboardEntry == null)
			{
				return 1;
			}
			if (leaderboardEntry.score > score)
			{
				return 1;
			}
			if (leaderboardEntry.score < score)
			{
				return -1;
			}
			int num = leaderboardEntry.name.CompareTo(name);
			if (num != 0)
			{
				return num;
			}
			return leaderboardEntry.rank.CompareTo(rank);
		}
	}

	public class Leaderboard
	{
		public const int MaxEntries = 5;

		public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

		public Leaderboard()
		{
		}

		public Leaderboard(Leaderboard other)
		{
			entries.AddRange(other.entries);
		}

		public void SortAndTrim()
		{
			entries.Sort();
			int num = 0;
			while (num < entries.Count - 1)
			{
				if (entries[num].CompareTo(entries[num + 1]) == 0)
				{
					entries.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
			if (entries.Count > 5)
			{
				entries.RemoveRange(5, entries.Count - 5);
			}
		}
	}

	private const int CurrentVersion = 1;

	private static DateTime MinNewInstallDate = new DateTime(2012, 10, 25);

	private string m_udid;

	private int m_coins;
	private int m_qiuqiubi;

	private Dictionary<string, int> m_intValues = new Dictionary<string, int>();

	private Dictionary<string, float> m_floatValues = new Dictionary<string, float>();

	private Dictionary<string, TimestampedString> m_stringValues = new Dictionary<string, TimestampedString>();

	private Dictionary<string, StatGroup> m_statGroups = new Dictionary<string, StatGroup>();

	private Dictionary<string, Leaderboard> m_leaderboards = new Dictionary<string, Leaderboard>();

	private List<string> m_otherProfileUDIDs = new List<string>();

	private int m_firstVersion;

	private DateTime m_firstInstallDate;

	private int m_modifications;
	private int m_modifications_qiuqiubi;

	private bool m_isLive;

	private DateTime m_cloudDate;

	public string UDID
	{
		get
		{
			return m_udid;
		}
		set
		{
			m_udid = value;
		}
	}

	public string SharedUUID
	{
		get
		{
			return GetString("shared_uuid");
		}
		set
		{
			SetString("shared_uuid", value);
		}
	}

	public DateTime FirstInstallDate
	{
		get
		{
			return m_firstInstallDate;
		}
		set
		{
			m_firstInstallDate = value;
		}
	}

	public bool IsModified
	{
		get
		{
			return m_modifications > 0;
		}
	}

	public bool IsLive
	{
		get
		{
			return m_isLive;
		}
		set
		{
			m_isLive = value;
		}
	}

	public DateTime Date
	{
		get
		{
			return m_cloudDate;
		}
		set
		{
			m_cloudDate = value;
		}
	}

	public int Coins
	{
		get
		{
			return m_coins;
		}
		set
		{
			m_coins = value;
			m_modifications++;
		}
	}

    public int Qiuqiubis
    {
        get
        {
            return m_qiuqiubi;
        }
        set
        {
            m_qiuqiubi = value;
            m_modifications++;
        }
    }

    public Dictionary<string, Leaderboard> Leaderboards
	{
		get
		{
			return m_leaderboards;
		}
	}

	public Profile()
	{
		m_udid = "badf000d";
	}

	public Profile(string udid)
	{
		m_udid = udid;
		m_firstVersion = 1;
		m_firstInstallDate = DateTime.UtcNow;
		if (m_firstInstallDate < MinNewInstallDate)
		{
			m_firstInstallDate = MinNewInstallDate;
		}
	}

	public Profile(Profile other)
	{
		m_udid = other.m_udid;
		foreach (KeyValuePair<string, int> intValue in other.m_intValues)
		{
			m_intValues[intValue.Key] = intValue.Value;
		}
		foreach (KeyValuePair<string, float> floatValue in other.m_floatValues)
		{
			m_floatValues[floatValue.Key] = floatValue.Value;
		}
		foreach (KeyValuePair<string, TimestampedString> stringValue in other.m_stringValues)
		{
			m_stringValues[stringValue.Key] = stringValue.Value;
		}
		foreach (KeyValuePair<string, StatGroup> statGroup in other.m_statGroups)
		{
			m_statGroups[statGroup.Key] = new StatGroup(statGroup.Value);
		}
		foreach (KeyValuePair<string, Leaderboard> leaderboard in other.m_leaderboards)
		{
			m_leaderboards[leaderboard.Key] = new Leaderboard(leaderboard.Value);
		}
		foreach (string otherProfileUDID in other.m_otherProfileUDIDs)
		{
			m_otherProfileUDIDs.Add(otherProfileUDID);
		}
		m_firstVersion = other.m_firstVersion;
		m_firstInstallDate = other.m_firstInstallDate;
		m_modifications = 0;
	}

	public void SetInt(string key, int value)
	{
		int value2;
		if (!m_intValues.TryGetValue(key, out value2) || value != value2)
		{
			m_intValues[key] = value;
			m_modifications++;
		}
	}

	public int GetInt(string key)
	{
		int value;
		m_intValues.TryGetValue(key, out value);
		return value;
	}

	public int GetInt(string key, int defaultValue)
	{
		int value;
		if (m_intValues.TryGetValue(key, out value))
		{
			return value;
		}
		return defaultValue;
	}

	public void SetFloat(string key, float value)
	{
		float value2;
		if (!m_floatValues.TryGetValue(key, out value2) || value != value2)
		{
			m_floatValues[key] = value;
			m_modifications++;
		}
	}

	public float GetFloat(string key)
	{
		float value = 0f;
		m_floatValues.TryGetValue(key, out value);
		return value;
	}

	public void SetString(string key, string value)
	{
		TimestampedString value2;
		if (m_stringValues.TryGetValue(key, out value2))
		{
			if (value != value2.str)
			{
				value2.date = DateTime.UtcNow.ToFileTimeUtc();
				value2.str = value;
				m_stringValues[key] = value2;
				m_modifications++;
			}
		}
		else
		{
			DateTime utcNow = DateTime.UtcNow;
			m_stringValues[key] = new TimestampedString
			{
				str = value,
				date = utcNow.ToFileTimeUtc()
			};
			m_modifications++;
		}
	}

	public string GetString(string key)
	{
		TimestampedString value;
		if (m_stringValues.TryGetValue(key, out value))
		{
			return value.str;
		}
		return string.Empty;
	}

	public bool HasKey(string key)
	{
		return m_intValues.ContainsKey(key) || m_floatValues.ContainsKey(key) || m_stringValues.ContainsKey(key);
	}

	public void DeleteKey(string key)
	{
		if (m_intValues.ContainsKey(key))
		{
			m_intValues.Remove(key);
			m_modifications++;
		}
		if (m_floatValues.ContainsKey(key))
		{
			m_floatValues.Remove(key);
			m_modifications++;
		}
		if (m_stringValues.ContainsKey(key))
		{
			m_stringValues.Remove(key);
			m_modifications++;
		}
	}

	public void IncrementStat(string key, string variant, int amount)
	{
		StatGroup value;
		if (m_statGroups.TryGetValue(key, out value))
		{
			IncrementStat(value.primary, amount);
		}
		else
		{
			value = new StatGroup();
			value.primary.total = amount;
			value.primary.sessionMax = amount;
			value.primary.instanceMax = amount;
			value.primary.sessionTotal = amount;
			m_statGroups[key] = value;
		}
		Stat value2;
		if (value.variants.TryGetValue(variant, out value2))
		{
			IncrementStat(value2, amount);
		}
		else
		{
			value2 = new Stat(amount);
			value.variants[variant] = value2;
		}
		m_modifications++;
	}

	public int GetStatTotal(string key, string variant)
	{
		Stat stat = GetStat(key, variant);
		if (stat != null)
		{
			return stat.total;
		}
		return 0;
	}

	public int GetStatSessionMax(string key, string variant)
	{
		Stat stat = GetStat(key, variant);
		if (stat != null)
		{
			return stat.sessionMax;
		}
		return 0;
	}

	public int GetStatInstanceMax(string key, string variant)
	{
		Stat stat = GetStat(key, variant);
		if (stat != null)
		{
			return stat.instanceMax;
		}
		return 0;
	}

	public void ResetAllStats()
	{
		m_statGroups.Clear();
	}

	public void ResetStatSessionTotals()
	{
		foreach (KeyValuePair<string, StatGroup> statGroup in m_statGroups)
		{
			statGroup.Value.primary.sessionTotal = 0;
			foreach (KeyValuePair<string, Stat> variant in statGroup.Value.variants)
			{
				variant.Value.sessionTotal = 0;
			}
		}
	}

	private void IncrementStat(Stat stat, int amount)
	{
		if (int.MaxValue - stat.total <= amount)
		{
			stat.total = int.MaxValue;
		}
		else
		{
			stat.total += amount;
		}
		stat.sessionTotal += amount;
		if (stat.sessionTotal > stat.sessionMax)
		{
			stat.sessionMax = stat.sessionTotal;
		}
		if (amount > stat.instanceMax)
		{
			stat.instanceMax = amount;
		}
	}

	private Stat GetStat(string key, string variant)
	{
		StatGroup value;
		if (m_statGroups.TryGetValue(key, out value))
		{
			if (string.IsNullOrEmpty(variant))
			{
				return value.primary;
			}
			Stat value2;
			if (value.variants.TryGetValue(variant, out value2))
			{
				return value2;
			}
		}
		return null;
	}

	public bool IsHighScore(string id, float score)
	{
		Leaderboard value;
		if (!m_leaderboards.TryGetValue(id, out value))
		{
			return true;
		}
		if (value.entries.Count < 5)
		{
			return true;
		}
		foreach (LeaderboardEntry entry in value.entries)
		{
			if (score > entry.score)
			{
				return true;
			}
		}
		return false;
	}

	public void AddHighScore(string id, LeaderboardEntry entry)
	{
		Leaderboard value;
		if (!m_leaderboards.TryGetValue(id, out value))
		{
			value = new Leaderboard();
			m_leaderboards[id] = value;
		}
		value.entries.Add(entry);
		value.SortAndTrim();
		m_modifications++;
	}

	public LeaderboardEntry AddHighScore(string id, string name, float score, int rank)
	{
		LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
		leaderboardEntry.name = name;
		leaderboardEntry.score = score;
		leaderboardEntry.rank = rank;
		leaderboardEntry.isSubmitted = false;
		LeaderboardEntry leaderboardEntry2 = leaderboardEntry;
		AddHighScore(id, leaderboardEntry2);
		return leaderboardEntry2;
	}

	public List<LeaderboardEntry> GetHighScoreEntries(string id)
	{
		Leaderboard value;
		if (m_leaderboards.TryGetValue(id, out value))
		{
			return value.entries;
		}
		return null;
	}

	public void LinkToOtherProfile(string udid)
	{
		if (!m_otherProfileUDIDs.Contains(udid))
		{
			m_otherProfileUDIDs.Add(udid);
			m_modifications++;
		}
	}

	public bool IsLinkedToOtherProfile(string udid)
	{
		return m_otherProfileUDIDs.Contains(udid);
	}

	public void DeleteAll()
	{
		Coins = 0;
		Qiuqiubis = 0;
        PlayerPrefs.SetInt("Qiuqiubis",0);
		m_intValues.Clear();
		m_floatValues.Clear();
		m_stringValues.Clear();
		m_statGroups.Clear();
	}

	public void MigrateInt(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			SetInt(key, PlayerPrefs.GetInt(key));
			PlayerPrefs.DeleteKey(key);
		}
	}

	public void MigrateFloat(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			SetFloat(key, PlayerPrefs.GetFloat(key));
			PlayerPrefs.DeleteKey(key);
		}
	}

	public void MigrateString(string key)
	{
		if (PlayerPrefs.HasKey(key))
		{
			SetString(key, PlayerPrefs.GetString(key));
			PlayerPrefs.DeleteKey(key);
		}
	}

	public void UnmigrateString(string key)
	{
		TimestampedString value;
		if (m_stringValues.TryGetValue(key, out value))
		{
			PlayerPrefs.SetString(key, value.str);
		}
	}

	public void MigrateAchievement(Achievement achievement)
	{
		string viewedKey = achievement.ViewedKey;
		if (PlayerPrefs.HasKey(viewedKey))
		{
			SetInt(viewedKey, 2);
			PlayerPrefs.DeleteKey(viewedKey);
		}
		achievement.MigrateToProfile(this);
	}

	public void MigrateStartingSkierAndSlope(ItemManager itemManager)
	{
		ItemSet itemSet = itemManager.GetItemSet("starting_skier");
		int num = Mathf.Clamp(PlayerPrefs.GetInt("CurrentStartingSkier"), 0, itemSet.items.Length);
		PlayerPrefs.DeleteKey("CurrentStartingSkier");
		PlayerPrefs.SetString(itemSet.CurrentKey, itemSet.items[num].name);
		ItemSet itemSet2 = itemManager.GetItemSet("slope");
		int num2 = Mathf.Clamp(PlayerPrefs.GetInt("CurrentTrail"), 0, itemSet2.items.Length);
		PlayerPrefs.SetString(itemSet2.CurrentKey, itemSet2.items[num2].name);
		PlayerPrefs.DeleteKey("CurrentTrail");
		SetInt("StartingSkier_Sleeping_viewed", 1);
		SetInt("StartingSkier_Sleeping_used", 1);
		SetInt("Slope_01_viewed", 1);
		SetInt("Slope_01_used", 1);
		MigrateStartingSkierOrSlope("StartingSkier_Penguin");
		MigrateStartingSkierOrSlope("StartingSkier_Yeti");
		MigrateStartingSkierOrSlope("StartingSkier_Eagle");
		MigrateStartingSkierOrSlope("StartingSkier_Snowmobile");
		MigrateStartingSkierOrSlope("StartingSkier_Wolf");
		MigrateStartingSkierOrSlope("StartingSkier_Rocket");
		MigrateStartingSkierOrSlope("slope_02", "Slope_02");
	}

	private void MigrateStartingSkierOrSlope(string itemName)
	{
		MigrateStartingSkierOrSlope(itemName, itemName);
	}

	private void MigrateStartingSkierOrSlope(string oldPlayedKey, string itemName)
	{
		if (PlayerPrefs.GetInt(oldPlayedKey, GetInt(oldPlayedKey)) > 0)
		{
			SetInt(itemName + "_viewed", 1);
			SetInt(itemName + "_used", 1);
		}
	}

	public void MigrateLeaderboards()
	{
		MigrateLeaderboard("highscores", "high_scores");
		MigrateLeaderboard("high_scores", "high_scores");
		MigrateLeaderboard("high_scores_2", "high_scores_2");
		Save();
	}

	private void MigrateLeaderboard(string oldId, string newId)
	{
		try
		{
			string text = Path.Combine(Application.persistentDataPath, oldId + ".txt");
			if (!File.Exists(text))
			{
				return;
			}
			Leaderboard_old leaderboard_old = new Leaderboard_old();
			leaderboard_old.Load(oldId);
			Leaderboard value;
			if (!m_leaderboards.TryGetValue(newId, out value))
			{
				value = new Leaderboard();
				m_leaderboards[newId] = value;
			}
			foreach (Leaderboard_old.Entry entry in leaderboard_old.Entries)
			{
				value.entries.Add(new LeaderboardEntry
				{
					name = entry.name,
					score = entry.score,
					rank = entry.rank,
					isSubmitted = true
				});
			}
			value.SortAndTrim();
			m_modifications++;
			string text2 = Path.Combine(Application.persistentDataPath, oldId + ".bak");
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.Move(text, text2);
		}
		catch (Exception)
		{
		}
	}

	public static Profile Load(string udid, bool preferLocal)
	{
		string fileName = udid + ".profile";
		try
		{
			DateTime cloudDate;
			bool isCloudFile;
			byte[] array = FileManager.Instance.ReadAllBytes(fileName, preferLocal, out cloudDate, out isCloudFile);
			if (array != null)
			{
				return Load(udid, array, cloudDate, isCloudFile);
			}
		}
		catch (Exception)
		{
		}
		return null;
	}

	public static Profile Load(string udid, byte[] bytes, DateTime cloudDate, bool isLive)
	{
		//Discarded unreachable code: IL_0065, IL_0077
		Decrypt(udid, bytes);
		using (MemoryStream memoryStream = new MemoryStream(bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				string @string = Encoding.UTF8.GetString(binaryReader.ReadBytes(udid.Length));
				if (udid != @string)
				{
					return null;
				}
				Profile profile = new Profile(udid);
				ReadProfile(profile, binaryReader, memoryStream);
				profile.m_cloudDate = cloudDate;
				profile.m_isLive = isLive;
				return profile;
			}
		}
	}

	private static void ReadProfile(Profile profile, BinaryReader reader, MemoryStream stream)
	{
		profile.m_firstVersion = 0;
		profile.m_firstInstallDate = new DateTime(2012, 4, 26);
		profile.m_coins = reader.ReadInt32();
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string key = reader.ReadString();
			int value = reader.ReadInt32();
			profile.m_intValues[key] = value;
		}
		int num2 = reader.ReadInt32();
		for (int j = 0; j < num2; j++)
		{
			string key2 = reader.ReadString();
			float value2 = reader.ReadSingle();
			profile.m_floatValues[key2] = value2;
		}
		int num3 = reader.ReadInt32();
		for (int k = 0; k < num3; k++)
		{
			string key3 = reader.ReadString();
			string str = reader.ReadString();
			long date = reader.ReadInt64();
			profile.m_stringValues[key3] = new TimestampedString
			{
				str = str,
				date = date
			};
		}
		try
		{
			if (stream.Length - stream.Position < 4)
			{
				return;
			}
			int num4 = reader.ReadInt32();
			for (int l = 0; l < num4; l++)
			{
				string key4 = reader.ReadString();
				StatGroup statGroup = new StatGroup();
				statGroup.primary.total = reader.ReadInt32();
				statGroup.primary.sessionMax = reader.ReadInt32();
				statGroup.primary.instanceMax = reader.ReadInt32();
				int num5 = reader.ReadInt32();
				for (int m = 0; m < num5; m++)
				{
					string key5 = reader.ReadString();
					int total = reader.ReadInt32();
					int sessionMax = reader.ReadInt32();
					int instanceMax = reader.ReadInt32();
					statGroup.variants[key5] = new Stat
					{
						total = total,
						sessionMax = sessionMax,
						instanceMax = instanceMax
					};
				}
				profile.m_statGroups[key4] = statGroup;
			}
		}
		catch (Exception)
		{
		}
		try
		{
			if (stream.Length - stream.Position < 4)
			{
				return;
			}
			int num6 = reader.ReadInt32();
			for (int n = 0; n < num6; n++)
			{
				string key6 = reader.ReadString();
				Leaderboard leaderboard = new Leaderboard();
				int num7 = reader.ReadInt32();
				for (int num8 = 0; num8 < num7; num8++)
				{
					string name = reader.ReadString();
					float score = reader.ReadSingle();
					int rank = reader.ReadInt32();
					bool isSubmitted = reader.ReadBoolean();
					leaderboard.entries.Add(new LeaderboardEntry
					{
						name = name,
						score = score,
						rank = rank,
						isSubmitted = isSubmitted
					});
				}
				profile.m_leaderboards[key6] = leaderboard;
			}
		}
		catch (Exception)
		{
		}
		try
		{
			if (stream.Length - stream.Position < 4)
			{
				return;
			}
			int num9 = reader.ReadInt32();
			for (int num10 = 0; num10 < num9; num10++)
			{
				string item = reader.ReadString();
				profile.m_otherProfileUDIDs.Add(item);
			}
		}
		catch (Exception)
		{
		}
		try
		{
			if (stream.Length - stream.Position >= 4)
			{
				profile.m_firstVersion = reader.ReadInt32();
				if (stream.Length - stream.Position >= 8)
				{
					long fileTime = reader.ReadInt64();
					profile.m_firstInstallDate = DateTime.FromFileTimeUtc(fileTime);
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public void Merge(Profile other)
	{
		foreach (KeyValuePair<string, int> intValue in other.m_intValues)
		{
			int value;
			if (m_intValues.TryGetValue(intValue.Key, out value))
			{
				if (intValue.Value > value)
				{
					m_intValues[intValue.Key] = intValue.Value;
					m_modifications++;
				}
			}
			else
			{
				m_intValues[intValue.Key] = intValue.Value;
				m_modifications++;
			}
		}
		foreach (KeyValuePair<string, float> floatValue in other.m_floatValues)
		{
			float value2;
			if (m_floatValues.TryGetValue(floatValue.Key, out value2))
			{
				if (floatValue.Value > value2)
				{
					m_floatValues[floatValue.Key] = floatValue.Value;
					m_modifications++;
				}
			}
			else
			{
				m_floatValues[floatValue.Key] = floatValue.Value;
				m_modifications++;
			}
		}
		foreach (KeyValuePair<string, TimestampedString> stringValue in other.m_stringValues)
		{
			TimestampedString value3;
			if (m_stringValues.TryGetValue(stringValue.Key, out value3))
			{
				if (stringValue.Value.date > value3.date)
				{
					m_stringValues[stringValue.Key] = stringValue.Value;
					m_modifications++;
				}
			}
			else
			{
				m_stringValues[stringValue.Key] = stringValue.Value;
				m_modifications++;
			}
		}
		foreach (KeyValuePair<string, StatGroup> statGroup in other.m_statGroups)
		{
			StatGroup value4;
			if (!m_statGroups.TryGetValue(statGroup.Key, out value4))
			{
				value4 = new StatGroup();
				m_statGroups[statGroup.Key] = value4;
			}
			MergeStat(value4.primary, statGroup.Value.primary);
			foreach (KeyValuePair<string, Stat> variant in statGroup.Value.variants)
			{
				Stat value5;
				if (!value4.variants.TryGetValue(variant.Key, out value5))
				{
					value5 = new Stat();
					value4.variants[variant.Key] = value5;
				}
				MergeStat(value5, variant.Value);
			}
		}
		foreach (KeyValuePair<string, Leaderboard> leaderboard in other.m_leaderboards)
		{
			Leaderboard value6;
			if (!m_leaderboards.TryGetValue(leaderboard.Key, out value6))
			{
				value6 = new Leaderboard();
				m_leaderboards[leaderboard.Key] = value6;
			}
			value6.entries.AddRange(leaderboard.Value.entries);
			value6.SortAndTrim();
		}
		foreach (string otherProfileUDID in other.m_otherProfileUDIDs)
		{
			if (otherProfileUDID != m_udid && !m_otherProfileUDIDs.Contains(otherProfileUDID))
			{
				m_otherProfileUDIDs.Add(otherProfileUDID);
				m_modifications++;
			}
		}
		if (other.m_firstInstallDate < m_firstInstallDate)
		{
			m_firstInstallDate = other.m_firstInstallDate;
			m_modifications++;
		}
		if (other.m_firstVersion < m_firstVersion)
		{
			m_firstVersion = other.m_firstVersion;
			m_modifications++;
		}
	}

	private void MergeStat(Stat currentStat, Stat otherStat)
	{
		if (otherStat.total > 0)
		{
		}
		if (otherStat.sessionMax > currentStat.sessionMax)
		{
			currentStat.sessionMax = otherStat.sessionMax;
			m_modifications++;
		}
		if (otherStat.instanceMax > currentStat.instanceMax)
		{
			currentStat.instanceMax = otherStat.instanceMax;
			m_modifications++;
		}
	}

	public void Save()
	{
		Save(true);
	}

	public void Save(bool saveToCloud)
	{
		try
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				WriteToStreamAndEncrypt(memoryStream);
				string fileName = m_udid + ".profile";
				FileManager.Instance.WriteAllBytes(fileName, memoryStream.GetBuffer(), saveToCloud, ref m_cloudDate, out m_isLive);
				m_modifications = 0;
				m_isLive = saveToCloud;
			}
		}
		catch (Exception)
		{
		}
	}

	public void WriteToStreamAndEncrypt(MemoryStream stream)
	{
		long num = WriteToStream(stream);
		byte[] buffer = stream.GetBuffer();
		Encrypt(m_udid, buffer, (int)num);
	}

	private long WriteToStream(MemoryStream stream)
	{
		//Discarded unreachable code: IL_02c7, IL_03dc, IL_045d, IL_0493, IL_04a5
		using (BinaryWriter binaryWriter = new BinaryWriter(stream))
		{
			binaryWriter.Write(Encoding.UTF8.GetBytes(m_udid));
			binaryWriter.Write(m_coins);
			binaryWriter.Write(m_intValues.Count);
			foreach (KeyValuePair<string, int> intValue in m_intValues)
			{
				binaryWriter.Write(intValue.Key);
				binaryWriter.Write(intValue.Value);
			}
			binaryWriter.Write(m_floatValues.Count);
			foreach (KeyValuePair<string, float> floatValue in m_floatValues)
			{
				binaryWriter.Write(floatValue.Key);
				binaryWriter.Write(floatValue.Value);
			}
			binaryWriter.Write(m_stringValues.Count);
			foreach (KeyValuePair<string, TimestampedString> stringValue in m_stringValues)
			{
				binaryWriter.Write(stringValue.Key);
				binaryWriter.Write((!string.IsNullOrEmpty(stringValue.Value.str)) ? stringValue.Value.str : string.Empty);
				binaryWriter.Write(stringValue.Value.date);
			}
			try
			{
				binaryWriter.Write(m_statGroups.Count);
				foreach (KeyValuePair<string, StatGroup> statGroup in m_statGroups)
				{
					binaryWriter.Write(statGroup.Key);
					Stat primary = statGroup.Value.primary;
					binaryWriter.Write(primary.total);
					binaryWriter.Write(primary.sessionMax);
					binaryWriter.Write(primary.instanceMax);
					binaryWriter.Write(statGroup.Value.variants.Count);
					foreach (KeyValuePair<string, Stat> variant in statGroup.Value.variants)
					{
						binaryWriter.Write(variant.Key);
						primary = variant.Value;
						binaryWriter.Write(primary.total);
						binaryWriter.Write(primary.sessionMax);
						binaryWriter.Write(primary.instanceMax);
					}
				}
			}
			catch (Exception)
			{
				return stream.Length;
			}
			try
			{
				binaryWriter.Write(m_leaderboards.Count);
				foreach (KeyValuePair<string, Leaderboard> leaderboard in m_leaderboards)
				{
					binaryWriter.Write(leaderboard.Key);
					binaryWriter.Write(leaderboard.Value.entries.Count);
					foreach (LeaderboardEntry entry in leaderboard.Value.entries)
					{
						binaryWriter.Write((!string.IsNullOrEmpty(entry.name)) ? entry.name : string.Empty);
						binaryWriter.Write(entry.score);
						binaryWriter.Write(entry.rank);
						binaryWriter.Write(entry.isRecent);
					}
				}
			}
			catch (Exception)
			{
				return stream.Length;
			}
			try
			{
				binaryWriter.Write(m_otherProfileUDIDs.Count);
				foreach (string otherProfileUDID in m_otherProfileUDIDs)
				{
					binaryWriter.Write((!string.IsNullOrEmpty(otherProfileUDID)) ? otherProfileUDID : string.Empty);
				}
			}
			catch (Exception)
			{
				return stream.Length;
			}
			try
			{
				binaryWriter.Write(m_firstVersion);
				binaryWriter.Write(m_firstInstallDate.ToFileTimeUtc());
			}
			catch (Exception)
			{
				return stream.Length;
			}
			return stream.Length;
		}
	}

	private static void Obfuscate(string udid, byte[] data, int dataLength)
	{
		int length = udid.Length;
		int num = length * 3 / 4;
		int num2 = data.Length;
		int num3 = 0;
		while (num3 < num2)
		{
			data[num3] = (byte)(data[num3] ^ udid[num]);
			num3++;
			num++;
			if (++num >= length)
			{
				num = 0;
			}
		}
	}

	public static void Encrypt(string udid, byte[] rawData, int rawDataLength)
	{
		Obfuscate(udid, rawData, rawDataLength);
	}

	public static void Decrypt(string udid, byte[] encryptedData)
	{
		Obfuscate(udid, encryptedData, encryptedData.Length);
	}

	public string ToShortString()
	{
		string udid = m_udid;
		udid += ((!m_isLive) ? "\n" : (" [live @ " + m_cloudDate.ToString() + "]\n"));
		string text = udid;
		udid = text + "Coins: " + m_coins + "\n";
		text = udid;
		udid = text + "Installed: " + m_firstInstallDate.ToString() + " (" + m_firstVersion + ")\n";
		if (SharedUUID.Length > 0)
		{
			udid = udid + "SharedID: " + SharedUUID.ToString() + "\n";
		}
		return udid + "Model: " + GetString("device_model") + "\n";
	}

	public override string ToString()
	{
		return m_udid;
	}
}
