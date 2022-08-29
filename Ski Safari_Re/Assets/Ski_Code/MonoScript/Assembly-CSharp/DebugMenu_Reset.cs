using System;
using UnityEngine;

public class DebugMenu_Reset : DebugMenu
{
	public override string Name
	{
		get
		{
			return "Reset";
		}
	}

	public override void Draw()
	{
		if (GUILayout.Button("Reset Shop", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			ItemSet[] itemSets = ItemManager.Instance.itemSets;
			foreach (ItemSet itemSet in itemSets)
			{
				itemSet.DebugReset();
			}
			AchievementManager.Instance.ClearActiveAchievements();
			AchievementManager.Instance.DebugRefresh();
		}
		if (GUILayout.Button("Clear Cached Bundles", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			BundleManager.Instance.ClearCachedBundles();
		}
		if (GUILayout.Button("Dump Referenced Assets [T]", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			AssetManager.DumpReferencedAssets();
		}
		if (GUILayout.Button("Reset First Install Date", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			DateTime firstInstallDate = new DateTime(2012, 4, 26);
			GameState.PersistentProfile.FirstInstallDate = firstInstallDate;
			GameState.TempProfile.FirstInstallDate = firstInstallDate;
			GameState.Save();
		}
		if (GUILayout.Button("Reset Update Version", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			GameState.DeleteKey("last_update_version");
			GameState.DeleteKey("social_settings_shown");
			base.Active = false;
			SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.QuickTitle);
		}
		if (GUITutorials.Instance.updateNewsList.Length > 1 && GUILayout.Button("Set Update Version To Previous", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			if (GUITutorials.Instance.updateNewsList.Length > 1)
			{
				GameState.SetString("last_update_version", GUITutorials.Instance.updateNewsList[GUITutorials.Instance.updateNewsList.Length - 2].version);
			}
			else
			{
				GameState.DeleteKey("last_update_version");
			}
			SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.QuickTitle);
		}
		if (GUILayout.Button("Reset Rated Version", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			GameState.DeleteKey("last_rated_version");
			GameState.DeleteKey("next_rating_request_date");
			SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.QuickTitle);
		}
		GUILayout.Space(10f);
		if (GUILayout.Button("Reset Stats", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			GameState.ResetAllStats();
		}
		GUILayout.Space(10f);
		if (GUILayout.Button("Reset Comments", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			CommentManager.Instance.ResetComments();
		}
		GUILayout.Space(10f);
		if (GUILayout.Button("Show Snapshots", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
		}
		GUILayout.Space(10f);
		if (GUILayout.Button("Reset Player Prefs", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			PlayerPrefs.DeleteAll();
		}
		GUILayout.Space(10f);
		GUI.color = Color.red;
		if (GUILayout.Button("Reset Everything", GUILayout.Height(40f), GUILayout.Width(250f)))
		{
			GameState.DeleteAll();
			PlayerPrefs.DeleteAll();
			ItemManager.Instance.DebugReset();
			AchievementManager.Instance.ClearActiveAchievements();
			AchievementManager.Instance.DebugRefresh();
			CommentManager.Instance.LoadComments();
			LevelManager.Instance.Load();
		}
		GUI.color = Color.white;
	}

	public override void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			AssetManager.DumpReferencedAssets();
		}
	}
}
