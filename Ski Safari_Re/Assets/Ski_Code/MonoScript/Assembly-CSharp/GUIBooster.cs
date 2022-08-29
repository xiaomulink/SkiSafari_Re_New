using System;
using UnityEngine;

public class GUIBooster : MonoBehaviour
{
	public Booster booster;

	public GUITransitionAnimator content;

	public GameObject activeSprite;

	public GameObject inactiveSprite;

	public TextMesh countText;

	public GUIButton button;

	private bool m_usedThisRun;

	private void UseBooster()
	{
		if (!m_usedThisRun)
		{
			booster.Use();
			m_usedThisRun = true;
		}
	}

	private void UpdateActive()
	{
		if (!m_usedThisRun && SkiGameManager.Instance.Playing && booster.CurrentCount > 0 && !GUIAchievementList.Instance.IsShowing)
		{
			bool canUse = booster.CanUse;
			activeSprite.SetActive(canUse);
			inactiveSprite.SetActive(!canUse);
			if (!content.IsShowing)
			{
				content.Show();
				activeSprite.GetComponent<Renderer>().material.mainTexture = AssetManager.LoadAsset<Texture>(booster.buttonTextureName);
				inactiveSprite.GetComponent<Renderer>().material.mainTexture = activeSprite.GetComponent<Renderer>().material.mainTexture;
			}
		}
		else if (content.IsShowing)
		{
			content.SnapHide();
			AssetManager.UnloadTexture(activeSprite.GetComponent<Renderer>());
		}
	}

	private void OnStateChanged(SkiGameManager.State state)
	{
		if (state == SkiGameManager.State.Spawning)
		{
			m_usedThisRun = false;
		}
	}

	private void Awake()
	{
		GUIButton gUIButton = button;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(UseBooster));
	}

	private void Start()
	{
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Combine(instance.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnStateChanged));
	}

	private void OnEnable()
	{
		UpdateActive();
	}

	private void Update()
	{
		UpdateActive();
	}
}
