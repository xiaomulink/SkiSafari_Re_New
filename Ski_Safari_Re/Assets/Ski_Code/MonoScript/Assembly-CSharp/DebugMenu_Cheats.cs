using UnityEngine;

public class DebugMenu_Cheats : DebugMenu
{
	public override string Name
	{
		get
		{
			return "Cheats";
		}
	}

	public override bool IsAvailable()
	{
		return (!SkiGameManager.Instance || SkiGameManager.Instance.TitleScreenActive) && (!GUITutorials.Instance || !GUITutorials.Instance.AutoShow);
	}

	public override void Draw()
	{
		if ((bool)SkiGameManager.Instance)
		{
			if (GUILayout.Button("Complete All Challenges", GUILayout.Height(40f), GUILayout.Width(250f)))
			{
				ItemManager.Instance.MarkCurrentItemsUsed();
				AchievementManager.Instance.DebugCompleteAll();
			}
			GUILayout.Space(10f);
			if (GUILayout.Button("Complete Level " + LevelManager.Instance.CurrentLevel, GUILayout.Height(40f), GUILayout.Width(250f)))
			{
				ItemManager.Instance.MarkCurrentItemsUsed();
				AchievementManager.Instance.DebugCompleteLevel();
			}
			GUILayout.Space(10f);
			foreach (Achievement activeAchievement in AchievementManager.Instance.ActiveAchievements)
			{
				if (!activeAchievement.IsComplete && GUILayout.Button("Complete Challenge: " + activeAchievement.description, GUILayout.Height(40f), GUILayout.Width(400f)))
				{
					ItemManager.Instance.MarkCurrentItemsUsed();
					SkiGameManager.Instance.ShowAchievementsGUI = false;
					activeAchievement.Complete();
				}
			}
		}
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Give Small Coin Pack", GUILayout.Height(40f), GUILayout.Width(150f)))
		{
			GameState.IncrementCoinCount(10000);
		}
		if (GUILayout.Button("Give Large Coin Pack", GUILayout.Height(40f), GUILayout.Width(150f)))
		{
			GameState.IncrementCoinCount(25000);
		}
		if (GUILayout.Button("Give Mega Coin Pack", GUILayout.Height(40f), GUILayout.Width(150f)))
		{
			GameState.IncrementCoinCount(75000);
		}
		if (GUILayout.Button("Remove All Coins", GUILayout.Height(40f), GUILayout.Width(150f)))
		{
			GameState.IncrementCoinCount(-GameState.CoinCount);
		}
		GUILayout.EndHorizontal();
		if ((bool)ItemManager.Instance)
		{
			GUILayout.Space(10f);
			GUILayout.BeginHorizontal();
			Booster[] boosters = ItemManager.Instance.boosters;
			foreach (Booster booster in boosters)
			{
				if (GUILayout.Button("Give 5 " + booster.name, GUILayout.Height(40f), GUILayout.Width(250f)))
				{
					booster.CurrentCount += 5;
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.Space(10f);
		if (GUILayout.Button("Disable title buttons and tabs", GUILayout.Height(40f), GUILayout.Width(350f)))
		{
			SkiGameManager.Instance.HideTransitions(SkiGameManager.Instance.onTutorialsShowingHideTransitions);
			SkiGameManager.Instance.rolloutButton.Hide();
		}
	}
}
