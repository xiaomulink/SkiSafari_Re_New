using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIOtherGames : MonoBehaviour
{
	[Serializable]
	public class GameInfo
	{
		public string name;

		public Sound sound;

		public string textureName;

		public string breadcrumbTextureName;

		public string iOSUrl;

		public string iOSProductId;

		public string googlePlayUrl;

		public string amazonUrl;

		public string nookUrl;

		public string URL
		{
			get
			{
				return googlePlayUrl;
			}
		}

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(URL);
			}
		}
	}

	public GameInfo[] games;

	public int initialGameIndex;

	public Renderer image;

	public GUIButton[] linkButtons;

	public GUIButton leftButton;

	public GUIButton rightButton;

	public float soundDelay = 1f;

	public GUITransitionAnimator transitionAnimator;

	private int m_gameIndex = -1;

	private List<GameInfo> m_availableGames = new List<GameInfo>();

	private GoTweenConfig m_tweenConfig = new GoTweenConfig().scale(0.5f, true).setEaseType(GoEaseType.ElasticPunch);

	public string GetNextBreadcrumbTextureName()
	{
		foreach (GameInfo availableGame in m_availableGames)
		{
			if (!string.IsNullOrEmpty(availableGame.breadcrumbTextureName) && GameState.GetInt(availableGame.name + "_banner_viewed") == 0)
			{
				return availableGame.breadcrumbTextureName;
			}
		}
		return null;
	}

	private void MoveLeft()
	{
		m_gameIndex--;
		if (m_gameIndex < 0)
		{
			m_gameIndex = m_availableGames.Count - 1;
		}
		Go.to(leftButton.gameObject.transform, 0.5f, m_tweenConfig);
		Refresh();
	}

	private void MoveRight()
	{
		m_gameIndex++;
		if (m_gameIndex >= m_availableGames.Count)
		{
			m_gameIndex = 0;
		}
		Go.to(rightButton.gameObject.transform, 0.5f, m_tweenConfig);
		Refresh();
	}

	private void OpenLink()
	{
		GameInfo gameInfo = m_availableGames[m_gameIndex];
		AnalyticsManager.Instance.SendEvent("games_banner_click", "game", gameInfo.name);
		Application.OpenURL(gameInfo.URL);
	}

	private void PlaySound()
	{
		SoundManager.Instance.PlaySound(m_availableGames[m_gameIndex].sound);
	}

	private void Refresh()
	{
		AssetManager.UpdateTexture(image, m_availableGames[m_gameIndex].textureName);
		CancelInvoke("PlaySound");
		Invoke("PlaySound", soundDelay);
		GameState.SetInt(m_availableGames[m_gameIndex].name + "_banner_viewed", 1);
	}

	private void Awake()
	{
		GUIButton gUIButton = leftButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(MoveLeft));
		GUIButton gUIButton2 = rightButton;
		gUIButton2.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton2.OnClick, new GUIButton.OnClickDelegate(MoveRight));
		GUIButton[] array = linkButtons;
		foreach (GUIButton gUIButton3 in array)
		{
			gUIButton3.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton3.OnClick, new GUIButton.OnClickDelegate(OpenLink));
		}
		GameInfo[] array2 = games;
		foreach (GameInfo gameInfo in array2)
		{
			if (gameInfo.IsValid)
			{
				m_availableGames.Add(gameInfo);
			}
		}
	}

	private void Start()
	{
		m_gameIndex = Mathf.Clamp(initialGameIndex, 0, m_availableGames.Count - 1);
	}

	private void OnEnable()
	{
		if (m_gameIndex >= 0)
		{
			Refresh();
		}
	}

	private void OnDisable()
	{
		CancelInvoke("PlaySound");
		AssetManager.UnloadTexture(image);
	}
}
