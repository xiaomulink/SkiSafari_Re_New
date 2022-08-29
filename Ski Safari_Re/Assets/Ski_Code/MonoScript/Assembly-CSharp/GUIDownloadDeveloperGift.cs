using UnityEngine;

public class GUIDownloadDeveloperGift : GUIButton
{
	private enum State
	{
		Disabled = 0,
		LoadingUrl = 1,
		WaitingForClick = 2,
		ShowResponse = 3,
		HideResponse = 4
	}

	public GameObject sprite;

	public GameObject successSprite;

	public GameObject coinSprite;

	public TextMesh coinCount;

	public GameObject challengeSprite;

	public TextMesh challengeCount;

	public TextMesh text;

	public TextMesh infoText;

	public string baseUrl;

	public Sound errorSound;

	public Sound successSound;

	public GameObject successEffect;

	public GameObject[] disableObjects;

	private State m_state;

	private float m_stateTimer;

	private WWW m_request;

	private GoTweenConfig m_tweenConfig = new GoTweenConfig().scale(0.25f, true).setEaseType(GoEaseType.ElasticPunch);

	private void CheckForGift()
	{
		if (string.IsNullOrEmpty(PlayerPrefs.GetString("gift_id")))
		{
			Disable();
		}
		else
		{
			Load();
		}
	}

	private void Load()
	{
		Disable();
		string url = baseUrl + PlayerPrefs.GetString("gift_id");
		m_request = new WWW(url);
		SetState(State.LoadingUrl);
	}

	private void Disable()
	{
		sprite.gameObject.SetActive(false);
		text.gameObject.SetActive(false);
		successSprite.SetActive(false);
		infoText.gameObject.SetActive(false);
		coinSprite.SetActive(false);
		coinCount.gameObject.SetActive(false);
		challengeSprite.SetActive(false);
		challengeCount.gameObject.SetActive(false);
		GameObject[] array = disableObjects;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(true);
		}
		SetState(State.Disabled);
	}

	public bool WaitingForClick()
	{
		return m_state == State.WaitingForClick;
	}

	public override void Click(Vector3 position)
	{
		if (m_state != State.WaitingForClick)
		{
			return;
		}
		int num = GameState.CoinCount;
		int completedAchievementCount = AchievementManager.Instance.CompletedAchievementCount;
		bool flag = GameState.MergeGiftProfile(m_request.bytes);
		m_request = null;
		if (flag)
		{
			SoundManager.Instance.PlaySound(successSound);
			PlayerPrefs.DeleteKey("gift_id");
			GameState.Save();
			int num2 = GameState.CoinCount - num;
			if (num2 > 0)
			{
				coinSprite.SetActive(true);
				coinCount.gameObject.SetActive(true);
				coinCount.text = num2.ToString("N0");
				coinCount.transform.localScale = Vector3.one;
				Go.to(coinCount.gameObject.transform, 0.5f, m_tweenConfig);
			}
			int num3 = AchievementManager.Instance.CompletedAchievementCount - completedAchievementCount;
			if (num3 > 0)
			{
				challengeSprite.SetActive(true);
				challengeCount.gameObject.SetActive(true);
				challengeCount.text = num3.ToString();
				challengeCount.transform.localScale = Vector3.one;
				Go.to(challengeCount.gameObject.transform, 0.5f, m_tweenConfig);
			}
			sprite.gameObject.SetActive(false);
			text.gameObject.SetActive(false);
			infoText.gameObject.SetActive(true);
			infoText.text = "Gift received!";
			infoText.GetComponent<Renderer>().material.color = Color.white;
			successSprite.SetActive(true);
			successSprite.transform.localScale = Vector3.one;
			Go.to(successSprite.gameObject.transform, 0.5f, m_tweenConfig);
			TransformUtils.Instantiate(successEffect, sprite.transform, false);
		}
		else
		{
			SoundManager.Instance.PlaySound(errorSound);
			sprite.gameObject.SetActive(false);
			text.gameObject.SetActive(false);
			infoText.gameObject.SetActive(true);
			infoText.text = "Unable to process gift";
			infoText.GetComponent<Renderer>().material.color = Color.white;
		}
		SetState(State.ShowResponse);
		base.Click(position);
	}

	private void SetState(State state)
	{
		m_state = state;
		m_stateTimer = 0f;
	}

	protected override void OnActivate()
	{
		CheckForGift();
		base.OnActivate();
	}

	private void Update()
	{
		m_stateTimer += Time.deltaTime;
		switch (m_state)
		{
		case State.Disabled:
			if (m_stateTimer >= 5f)
			{
				CheckForGift();
			}
			break;
		case State.LoadingUrl:
			if (m_request == null)
			{
				Disable();
			}
			else
			{
				if (!m_request.isDone)
				{
					break;
				}
				if (string.IsNullOrEmpty(m_request.error) && m_request.size > 0 && (string.IsNullOrEmpty(m_request.text) || !m_request.text.Contains("<html>")))
				{
					sprite.gameObject.SetActive(true);
					text.gameObject.SetActive(true);
					Go.to(successSprite.gameObject.transform, 0.5f, m_tweenConfig);
					GameObject[] array = disableObjects;
					foreach (GameObject gameObject in array)
					{
						gameObject.SetActive(false);
					}
					SetState(State.WaitingForClick);
				}
				else
				{
					Disable();
				}
			}
			break;
		case State.ShowResponse:
			if (m_stateTimer >= 4f)
			{
				Go.to(infoText.GetComponent<Renderer>().material, 0.5f, new GoTweenConfig().materialColor(new Color(1f, 1f, 1f, 0f)));
				Go.to(successSprite.transform, 0.5f, new GoTweenConfig().scale(Vector3.zero));
				SetState(State.HideResponse);
			}
			break;
		case State.HideResponse:
			if (m_stateTimer >= 0.5f)
			{
				Go.killAllTweensWithTarget(infoText.GetComponent<Renderer>().material);
				Go.killAllTweensWithTarget(successSprite.transform);
				Disable();
			}
			break;
		case State.WaitingForClick:
			break;
		}
	}
}
