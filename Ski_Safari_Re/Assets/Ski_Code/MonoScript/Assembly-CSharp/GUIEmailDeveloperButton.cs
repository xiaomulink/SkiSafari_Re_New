using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIEmailDeveloperButton : GUIButton
{
	public bool sendFullReport = true;

	private static Dictionary<char, string> s_encodedTokens;

	private static int MaxBodyLength = 9800;

	private string UrlEncode(string str)
	{
		if (s_encodedTokens == null)
		{
			s_encodedTokens = new Dictionary<char, string>();
			s_encodedTokens[' '] = "%20";
			s_encodedTokens['!'] = "%21";
			s_encodedTokens['"'] = "%22";
			s_encodedTokens['#'] = "%23";
			s_encodedTokens['$'] = "%24";
			s_encodedTokens['%'] = "%25";
			s_encodedTokens['&'] = "%26";
			s_encodedTokens['\''] = "%27";
			s_encodedTokens['('] = "%28";
			s_encodedTokens[')'] = "%29";
			s_encodedTokens['*'] = "%2A";
			s_encodedTokens['+'] = "%2B";
			s_encodedTokens[','] = "%2C";
			s_encodedTokens['.'] = "%2E";
			s_encodedTokens['/'] = "%2F";
			s_encodedTokens[':'] = "%3A";
			s_encodedTokens[';'] = "%3B";
			s_encodedTokens['<'] = "%3C";
			s_encodedTokens['='] = "%3D";
			s_encodedTokens['>'] = "%3E";
			s_encodedTokens['?'] = "%3F";
			s_encodedTokens['@'] = "%40";
			s_encodedTokens['['] = "%5B";
			s_encodedTokens[']'] = "%5D";
			s_encodedTokens['^'] = "%5E";
			s_encodedTokens['>'] = "%3E";
			s_encodedTokens['{'] = "%7B";
			s_encodedTokens['}'] = "%7D";
			s_encodedTokens['|'] = "%7C";
			s_encodedTokens['\\'] = "%5C";
			s_encodedTokens['~'] = "%7E";
			s_encodedTokens['`'] = "%60";
			s_encodedTokens['\n'] = "%0A";
		}
		string text = string.Empty;
		foreach (char c in str)
		{
			text = ((!s_encodedTokens.ContainsKey(c)) ? (text + c) : (text + s_encodedTokens[c]));
		}
		return text;
	}

	public override void Click(Vector3 position)
	{
		string empty = string.Empty;
		string text = string.Empty;
		if (sendFullReport)
		{
			empty = string.Format("Ski Safari Problem Report ({0} {1})", AppInfo.PlatformName, AppInfo.Version);
			if (string.IsNullOrEmpty(PlayerPrefs.GetString("gift_id")))
			{
				int num = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
				PlayerPrefs.SetString("gift_id", string.Format("{0:X}", num));
			}
			text = text + "\n\n--------------------\nReport ID: " + PlayerPrefs.GetString("gift_id");
			text = text + "\nPlayer Rank: " + LevelManager.Instance.CurrentLevel;
			text = text + "\nCoins: " + GameState.CoinCount;
			text = text + "\nCloud Enabled: " + FileManager.Instance.CloudEnabled;
			text += "\n-----\nPrimary: ";
			text += GameState.PersistentProfile.ToShortString();
			foreach (KeyValuePair<string, Profile> otherProfile in GameState.OtherProfiles)
			{
				text += "-----\nSecondary: ";
				text += otherProfile.Value.ToShortString();
			}
			foreach (KeyValuePair<string, Profile> invalidProfile in GameState.InvalidProfiles)
			{
				text += "------\nInvalid: ";
				text += invalidProfile.Value.ToShortString();
			}
			if (LogManager.Logs.Count > 0)
			{
				text += "-----";
				foreach (string log in LogManager.Logs)
				{
					text += "\n";
					text += log;
				}
			}
			if (text.Length > MaxBodyLength)
			{
				string text2 = "<report truncated>";
				text = text.Substring(0, MaxBodyLength - text2.Length);
				text += text2;
			}
			text += "\n--------------------\n\n";
		}
		else
		{
			empty = string.Format("Ski Safari Feedback ({0} {1})", AppInfo.PlatformName, AppInfo.Version);
		}
		string url = "mailto:defiant@defiantdev.com?subject=" + Uri.EscapeDataString(empty) + "&body=" + Uri.EscapeDataString(text);
		GoTweenConfig config = new GoTweenConfig().scale(1.25f, true).setEaseType(GoEaseType.ElasticPunch);
		Go.to(base.gameObject.transform, 0.5f, config);
		Application.OpenURL(url);
		base.Click(position);
	}

	protected override void OnActivate()
	{
		base.transform.localScale = Vector3.one;
		base.OnActivate();
	}
}
