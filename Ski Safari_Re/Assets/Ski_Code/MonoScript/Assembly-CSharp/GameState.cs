using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameState : MonoBehaviour
{
	public delegate void SimpleDelegate();

	[CompilerGenerated]
	private sealed class _003CAddHighScore_003Ec__AnonStorey20
	{
		internal Profile.LeaderboardEntry entry;

		internal void _003C_003Em__17(bool success)
		{
			if (success)
			{
				entry.isSubmitted = true;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003CSubmitScores_003Ec__AnonStorey21
	{
		internal Profile.LeaderboardEntry entryToSubmit;

		internal void _003C_003Em__18(bool success)
		{
			if (success)
			{
				entryToSubmit.isSubmitted = true;
			}
		}
	}

	private const string m_devicesKey = "devices";

	public ItemManager itemManager;

	public AchievementManager achievementManager;

	public static SimpleDelegate OnLoadRequested;

	public static SimpleDelegate OnSynchronise;

	private static GameState s_instance;

	private static Profile s_persistentProfile;

	private static Profile s_tempProfile;

	private static Dictionary<string, Profile> s_otherProfiles = new Dictionary<string, Profile>();

	private static Dictionary<string, Profile> s_invalidProfiles = new Dictionary<string, Profile>();

	private static int s_otherCoins = 0;
	private static int s_otherQiuqiubis = 0;

	private static bool s_isLoadRequested = true;

	public static bool IsInstantiated
	{
		get
		{
			return s_persistentProfile != null;
		}
	}

	public static bool IsLoadRequested
	{
		get
		{
			return s_isLoadRequested;
		}
	}

	public static Profile PersistentProfile
	{
		get
		{
			return s_persistentProfile;
		}
		set
		{
			s_persistentProfile = value;
		}
	}

	public static Profile TempProfile
	{
		get
		{
			return s_tempProfile;
		}
	}

	public static Dictionary<string, Profile> OtherProfiles
	{
		get
		{
			return s_otherProfiles;
		}
	}

	public static int OtherProfileCount
	{
		get
		{
			return s_otherProfiles.Count;
		}
	}

	public static Dictionary<string, Profile> InvalidProfiles
	{
		get
		{
			return s_invalidProfiles;
		}
	}

	public static int InvalidProfileCount
	{
		get
		{
			return s_invalidProfiles.Count;
		}
	}

	public static int CoinCount
	{
		get
		{
            if (!SkiGameManager.Instance.custom.InfiniteCoinToggle)
			    return s_persistentProfile.Coins + s_otherCoins;
            else
                return s_persistentProfile.Coins + s_otherCoins+114514;
        }
    }
    public static int QiuqiubiCount
    {
        get
        {
            return s_persistentProfile.Qiuqiubis + s_otherQiuqiubis;
        }
    }

    public static void IncrementCoinCount(int amount)
	{
		s_persistentProfile.Coins += amount;
	}
    
    public static void IncrementQiuqiubiCount(int amount)
    {
        s_persistentProfile.Qiuqiubis += amount;
        PlayerPrefs.SetInt("Qiuqiubis", s_persistentProfile.Qiuqiubis);
    }

    public static void SetInt(string key, int value)
	{
		s_persistentProfile.SetInt(key, value);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.SetInt(key, value);
		}
	}

	public static int GetInt(string key)
	{
		return s_tempProfile.GetInt(key);
	}

	public static int GetInt(string key, int defaultValue)
	{
		return s_tempProfile.GetInt(key, defaultValue);
	}

	public static void SetFloat(string key, float value)
	{
		s_persistentProfile.SetFloat(key, value);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.SetFloat(key, value);
		}
	}

	public static float GetFloat(string key)
	{
		return s_tempProfile.GetFloat(key);
	}

	public static void SetString(string key, string value)
	{
		s_persistentProfile.SetString(key, value);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.SetString(key, value);
		}
	}

	public static string GetString(string key)
	{
		return s_tempProfile.GetString(key);
	}

	public static bool HasKey(string key)
	{
		return s_tempProfile.HasKey(key);
	}

	public static void DeleteKey(string key)
	{
		s_persistentProfile.DeleteKey(key);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.DeleteKey(key);
		}
	}

	public static void IncrementStat(string key, string variant, int amount = 1)
	{
		s_persistentProfile.IncrementStat(key, variant, amount);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.IncrementStat(key, variant, amount);
		}
	}

	public static int GetStatTotal(string key, string variant)
	{
		return s_tempProfile.GetStatTotal(key, variant);
	}

	public static int GetStatSessionMax(string key, string variant)
	{
		return s_tempProfile.GetStatSessionMax(key, variant);
	}

	public static int GetStatInstanceMax(string key, string variant)
	{
		return s_tempProfile.GetStatInstanceMax(key, variant);
	}

	public static void ResetAllStats()
	{
		s_persistentProfile.ResetAllStats();
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.ResetAllStats();
		}
	}

	public static void ResetStatSessionTotals()
	{
		s_persistentProfile.ResetStatSessionTotals();
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.ResetStatSessionTotals();
		}
	}

	public static bool IsHighScore(string id, float score)
	{
		return s_tempProfile.IsHighScore(id, score);
	}

	public static Profile.LeaderboardEntry AddHighScore(string id, string name, float score, int rank)
	{
		_003CAddHighScore_003Ec__AnonStorey20 _003CAddHighScore_003Ec__AnonStorey = new _003CAddHighScore_003Ec__AnonStorey20();
		_003CAddHighScore_003Ec__AnonStorey.entry = s_persistentProfile.AddHighScore(id, name, score, rank);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.AddHighScore(id, _003CAddHighScore_003Ec__AnonStorey.entry);
		}
		if (SocialManager.Instance.IsAuthenticated)
		{
			SocialManager.Instance.SubmitScore(id, Mathf.FloorToInt(_003CAddHighScore_003Ec__AnonStorey.entry.score), _003CAddHighScore_003Ec__AnonStorey._003C_003Em__17);
		}
		return _003CAddHighScore_003Ec__AnonStorey.entry;
	}

	public static List<Profile.LeaderboardEntry> GetHighScoreEntries(string id)
	{
		return s_tempProfile.GetHighScoreEntries(id);
	}

	public static void SubmitScores()
	{
		if (!SocialManager.Instance.IsAuthenticated)
		{
			return;
		}
		foreach (KeyValuePair<string, Profile.Leaderboard> leaderboard in s_persistentProfile.Leaderboards)
		{
			Profile.Leaderboard value = leaderboard.Value;
			bool flag = false;
			foreach (Profile.LeaderboardEntry entry in value.entries)
			{
				if (!entry.isSubmitted && !flag)
				{
					_003CSubmitScores_003Ec__AnonStorey21 _003CSubmitScores_003Ec__AnonStorey = new _003CSubmitScores_003Ec__AnonStorey21();
					_003CSubmitScores_003Ec__AnonStorey.entryToSubmit = entry;
					flag = true;
					SocialManager.Instance.SubmitScore(leaderboard.Key, Mathf.FloorToInt(_003CSubmitScores_003Ec__AnonStorey.entryToSubmit.score), _003CSubmitScores_003Ec__AnonStorey._003C_003Em__18);
				}
				else
				{
					entry.isSubmitted = true;
				}
			}
		}
	}

	private static void DeleteFiles(string path)
	{
		string[] files = Directory.GetFiles(path);
		string[] array = files;
		foreach (string path2 in array)
		{
			File.Delete(Path.Combine(path, path2));
		}
	}

	public static void DeleteAll()
	{
		s_persistentProfile.DeleteAll();
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.DeleteAll();
		}
		s_otherCoins = 0;
		s_otherQiuqiubis = 0;
		s_otherProfiles.Clear();
		s_invalidProfiles.Clear();
		s_isLoadRequested = false;
		FileManager.Instance.DeleteAllFiles();
	}

	public static void Synchronise()
	{
		Save();
		if (s_isLoadRequested)
		{
			Load();
			if (s_tempProfile.IsModified)
			{
				if (OnSynchronise != null)
				{
					OnSynchronise();
				}
				if ((bool)ItemManager.Instance)
				{
					ItemManager.Instance.Load();
				}
				if ((bool)AchievementManager.Instance)
				{
					AchievementManager.Instance.RefreshAfterSynchronise();
				}
			}
		}
		Save();
	}

	private static void SetupAndroidSharedID()
	{
        try
        {
            using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

                using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass(Application.identifier + ".UserUUID"))
                {
                    string text = androidJavaClass2.CallStatic<string>("generate", new object[1] { @static });
                    if (!string.IsNullOrEmpty(text) && s_persistentProfile.SharedUUID != text)
                    {
                        s_persistentProfile.SharedUUID = text;
                        s_persistentProfile.Save();
                    }
                }
            }
        }
        catch
        {
            s_persistentProfile.SharedUUID = "generate64646494676449";

            s_persistentProfile.Save();
        }

	}

	private static void Load()
	{
		if (!s_isLoadRequested)
		{
			return;
		}
		if (s_persistentProfile == null)
		{
			string uniqueDeviceId = UUIDUtils.GetUniqueDeviceId();
			s_persistentProfile = Profile.Load(uniqueDeviceId, true);
			if (s_persistentProfile == null)
			{
				s_persistentProfile = new Profile(uniqueDeviceId);
			}
			SetupAndroidSharedID();
			s_persistentProfile.SetString("device_model", SystemInfo.deviceModel);
			MigrationUtils.MigrateAndroid1to3(s_persistentProfile, s_instance.itemManager);
			s_tempProfile = s_persistentProfile;
		}
		List<FileManager.FileInfo> fileList = FileManager.Instance.GetFileList("*.profile");
		if (fileList != null)
		{
			foreach (FileManager.FileInfo item in fileList)
			{
				ProcessProfile(item);
			}
		}
		s_isLoadRequested = false;
	}

	public static void Save()
	{
		if (s_persistentProfile != null && s_persistentProfile.IsModified)
		{
			s_persistentProfile.Save();
		}
	}

	private static bool IsOtherProfileValid(Profile otherProfile)
	{
		return true;
	}

	public static bool MergeGiftProfile(byte[] bytes)
	{
		Profile profile = Profile.Load(s_persistentProfile.UDID, bytes, DateTime.Now, true);
		if (profile != null)
		{
			s_persistentProfile.Merge(profile);
			if (s_persistentProfile != s_tempProfile)
			{
				s_tempProfile.Merge(profile);
			}
			s_persistentProfile.Coins += profile.Coins;
			s_persistentProfile.Qiuqiubis += profile.Qiuqiubis;
			if ((bool)ItemManager.Instance)
			{
				ItemManager.Instance.Load();
			}
			if ((bool)AchievementManager.Instance)
			{
				AchievementManager.Instance.RefreshAfterSynchronise();
			}
			return true;
		}
		return false;
	}

	private static void ProcessProfile(FileManager.FileInfo profileInfo)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(profileInfo.path);
		if (fileNameWithoutExtension == s_persistentProfile.UDID)
		{
			try
			{
				if (profileInfo.date > s_persistentProfile.Date)
				{
					Profile profile = Profile.Load(fileNameWithoutExtension, false);
					if (profile != null)
					{
						s_persistentProfile.Merge(profile);
						if (s_persistentProfile != s_tempProfile)
						{
							s_tempProfile.Merge(profile);
						}
						s_persistentProfile.Coins = Mathf.Max(s_persistentProfile.Coins, profile.Coins);
                        s_persistentProfile.Qiuqiubis = PlayerPrefs.GetInt("Qiuqiubis");
                        UnityEngine.Debug.Log(PlayerPrefs.GetInt("Qiuqiubis") +":"+profile.Qiuqiubis);
						s_persistentProfile.Qiuqiubis = Mathf.Max(s_persistentProfile.Qiuqiubis, profile.Qiuqiubis);
						s_persistentProfile.Date = profileInfo.date.AddMilliseconds(1.0);
					}
				}
				return;
			}
			catch (Exception)
			{
				return;
			}
		}
		Profile value;
		if (s_otherProfiles.TryGetValue(fileNameWithoutExtension, out value) && profileInfo.date <= value.Date)
		{
			return;
		}
		Profile profile2 = Profile.Load(fileNameWithoutExtension, false);
		if (profile2 == null)
		{
			return;
		}
		if (profile2.IsLive)
		{
			profile2.Save(false);
		}
		if (!IsOtherProfileValid(profile2))
		{
			s_invalidProfiles[fileNameWithoutExtension] = profile2;
			return;
		}
		if (s_tempProfile == s_persistentProfile)
		{
			s_tempProfile = new Profile(s_persistentProfile);
		}
		s_tempProfile.Merge(profile2);
        UnityEngine.Debug.Log(fileNameWithoutExtension);
		if (s_otherProfiles.ContainsKey(fileNameWithoutExtension))
		{
			s_otherCoins -= s_otherProfiles[fileNameWithoutExtension].Coins;
		}
        if (s_otherProfiles.ContainsKey(fileNameWithoutExtension))
        {
            s_otherQiuqiubis -= s_otherProfiles[fileNameWithoutExtension].Qiuqiubis;
        }
        if (s_invalidProfiles.ContainsKey(fileNameWithoutExtension))
		{
			s_invalidProfiles.Remove(fileNameWithoutExtension);
		}
		s_otherProfiles[fileNameWithoutExtension] = profile2;
		s_otherCoins += profile2.Coins;
		s_otherQiuqiubis += profile2.Qiuqiubis;
		s_persistentProfile.LinkToOtherProfile(profile2.UDID);
		if (s_tempProfile != s_persistentProfile)
		{
			s_tempProfile.LinkToOtherProfile(profile2.UDID);
		}
	}

	private void RequestLoad(bool sendEvent)
	{
		if (!s_isLoadRequested)
		{
			s_isLoadRequested = true;
			if (sendEvent && OnLoadRequested != null)
			{
				OnLoadRequested();
			}
		}
	}

    public static void UpdateQuiquibi()
    {
        GameState.s_persistentProfile.Qiuqiubis = PlayerPrefs.GetInt("Qiuqiubis");
    }

	private void OnFileUpdated(string fileName, DateTime cloudDate)
	{

       
        if (s_persistentProfile == null)
		{
			return;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
		if (fileNameWithoutExtension == s_persistentProfile.UDID)
		{
			if (cloudDate > s_persistentProfile.Date)
			{
				RequestLoad(true);
			}
			return;
		}
		Profile value = null;
		if (s_otherProfiles.TryGetValue(fileNameWithoutExtension, out value))
		{
			if (cloudDate > value.Date)
			{
				RequestLoad(true);
				value.IsLive = false;
			}
		}
		else if (s_invalidProfiles.TryGetValue(fileNameWithoutExtension, out value))
		{
			RequestLoad(false);
		}
		else
		{
			RequestLoad(true);
		}
	}

	private void OnFilePendingDownload(string fileName)
	{
	}

	private void Awake()
	{
		s_instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		FileManager.OnFileUpdated = (Action<string, DateTime>)Delegate.Combine(FileManager.OnFileUpdated, new Action<string, DateTime>(OnFileUpdated));
		FileManager.OnFilePendingDownload = (Action<string>)Delegate.Combine(FileManager.OnFilePendingDownload, new Action<string>(OnFilePendingDownload));
	}

	[Conditional("LOCAL_DEBUG")]
	private static void LocalLog(string log)
	{
	}
    private void Update()
    {

    }
}
