using System.Collections.Generic;
using UnityEngine;

public class DebugMenu_Profiles : DebugMenu
{
	private string m_selectedUDID;

	private Vector3 m_scrollPos;

	public override string Name
	{
		get
		{
			return "Profiles";
		}
	}

	public override bool IsAvailable()
	{
		if (!GameState.IsInstantiated || ((bool)SkiGameManager.Instance && !SkiGameManager.Instance.TitleScreenActive && !base.Active) || ((bool)GUITutorials.Instance && GUITutorials.Instance.AutoShow))
		{
			return false;
		}
		return true;
	}

	private void DrawProfileToggle(Profile profile, string category)
	{
		string text = profile.UDID.Substring(0, Mathf.Min(10, profile.UDID.Length)) + "... [" + ((!profile.IsLive) ? "cached" : "live") + " " + category + " " + profile.Date.ToString() + "]: " + profile.Coins + profile.Qiuqiubis;
		if (GUILayout.Toggle(m_selectedUDID == profile.UDID, text, GUILayout.Height(30f)))
		{
			m_selectedUDID = profile.UDID;
		}
	}

	public override void Draw()
	{
		if (!FileManager.Instance.CloudEnabled)
		{
			GUI.color = Color.red;
			GUILayout.Label("Cloud files not available");
			GUI.color = Color.white;
		}
		GUILayout.BeginHorizontal();
		GUI.color = ((!GameState.PersistentProfile.IsModified) ? Color.white : Color.green);
		if (GUILayout.Button("Save", GUILayout.Height(40f)))
		{
			GameState.Save();
		}
		GUI.color = ((!GameState.IsLoadRequested) ? Color.white : Color.green);
		if (GUILayout.Button("Synchronise", GUILayout.Height(40f)))
		{
			GameState.Synchronise();
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if (GameState.PersistentProfile == GameState.TempProfile && string.IsNullOrEmpty(m_selectedUDID))
		{
			m_selectedUDID = GameState.PersistentProfile.UDID;
		}
		DrawProfileToggle(GameState.PersistentProfile, "primary");
		foreach (KeyValuePair<string, Profile> otherProfile in GameState.OtherProfiles)
		{
			DrawProfileToggle(otherProfile.Value, "other");
		}
		if (GameState.PersistentProfile != GameState.TempProfile && GUILayout.Toggle(string.IsNullOrEmpty(m_selectedUDID), "Merged: " + GameState.CoinCount, GUILayout.Height(30f)))
		{
			m_selectedUDID = string.Empty;
		}
		GUI.color = Color.red;
		foreach (KeyValuePair<string, Profile> invalidProfile in GameState.InvalidProfiles)
		{
			DrawProfileToggle(invalidProfile.Value, "invalid");
		}
		GUI.color = Color.white;
		GUILayout.Space(10f);
		m_scrollPos = GUILayout.BeginScrollView(m_scrollPos, "box");
		Profile value = null;
		if (string.IsNullOrEmpty(m_selectedUDID))
		{
			value = GameState.TempProfile;
		}
		else if (!GameState.OtherProfiles.TryGetValue(m_selectedUDID, out value) && !GameState.InvalidProfiles.TryGetValue(m_selectedUDID, out value))
		{
			value = GameState.PersistentProfile;
		}
		GUILayout.Label(value.ToString());
		GUILayout.EndScrollView();
	}
}
