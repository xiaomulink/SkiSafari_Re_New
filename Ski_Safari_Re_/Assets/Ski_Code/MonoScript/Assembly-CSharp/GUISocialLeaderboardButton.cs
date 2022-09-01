using System;
using UnityEngine;

public class GUISocialLeaderboardButton : GUIButton
{
	public Sound failSound;

	public override void Click(Vector3 position)
	{
		base.Click(position);
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Combine(instance.OnAuthenticationFinished, new Action<bool>(ShowLeaderboardIfAuthenticated));
		SocialManager.Instance.Authenticate(true);
	}

	private void ShowLeaderboardIfAuthenticated(bool success)
	{
		Go.killAllTweensWithTarget(base.transform);
		base.transform.localRotation = Quaternion.identity;
		if (success)
		{
			SocialManager.Instance.ShowLeaderboard(GUILeaderboard.Instance.SelectedSlope.leaderboardId);
		}
		else
		{
			Vector3 shakeMagnitude = new Vector3(0f, 0f, 25f);
			float duration = 0.3f;
			Go.to(base.transform, duration, new GoTweenConfig().shake(shakeMagnitude, GoShakeType.Eulers));
			SoundManager.Instance.PlaySound(failSound);
		}
		SocialManager instance = SocialManager.Instance;
		instance.OnAuthenticationFinished = (Action<bool>)Delegate.Remove(instance.OnAuthenticationFinished, new Action<bool>(ShowLeaderboardIfAuthenticated));
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		Go.killAllTweensWithTarget(base.transform);
		base.transform.localRotation = Quaternion.identity;
	}
}
