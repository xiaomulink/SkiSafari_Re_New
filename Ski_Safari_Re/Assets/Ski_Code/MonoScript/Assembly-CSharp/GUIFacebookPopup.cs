using System;
using UnityEngine;

public class GUIFacebookPopup : MonoBehaviour
{
	public GUITransitionAnimator transitionAnimator;

	public GUIButton linkButton;

	public Transform pulseNode;

	public Sound[] showSounds;

	public string appUrl = "fb://profile/100363946788083";

	public string webUrl = "http://www.facebook.com/skisafarigame";

	private float m_pulseTimer;

	private bool m_showSoundPlayed;

	private void OpenFacebookURL()
	{
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");

			using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass(Application.identifier + ".FacebookLink"))
			{
				androidJavaClass2.CallStatic("open", @static, appUrl, webUrl);
			}
		}
	}

	protected void OnEnable()
	{
		m_pulseTimer = 0f;
		m_showSoundPlayed = false;
		pulseNode.localScale = Vector3.one;
	}

	protected void Awake()
	{
		GUIButton gUIButton = linkButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(OpenFacebookURL));
	}

	protected void Update()
	{
		m_pulseTimer += Time.deltaTime;
		float num = 1f + Mathf.Sin(m_pulseTimer * 3f) * 0.1f;
		pulseNode.localScale = new Vector3(num, num, num);
		if (!m_showSoundPlayed && m_pulseTimer >= transitionAnimator.transitionInTime)
		{
			m_showSoundPlayed = true;
			SoundManager.Instance.PlayRandomSound(showSounds);
		}
	}
}
