using System;

public class Comment_Intro : Comment
{
	public float delay = 2f;

	public Item requiredItem;

	public bool noSoundOnly;

	private void OnGameStateChanged(SkiGameManager.State state)
	{
		switch (state)
		{
		case SkiGameManager.State.Spawning:
			if ((!requiredItem || ItemManager.Instance.IsItemCurrent(requiredItem)) && (!noSoundOnly || (!SoundManager.Instance.SFXEnabled && !SoundManager.Instance.MusicEnabled)))
			{
				Invoke("ShowDelayedMessage", delay);
			}
			break;
		case SkiGameManager.State.Restarting:
			CancelInvoke("ShowDelayedMessage");
			break;
		}
	}

	private void ShowDelayedMessage()
	{
		ShowMessagesAndComplete();
	}

	private void OnEnable()
	{
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Combine(instance.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnGameStateChanged));
	}

	private void OnDisable()
	{
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Remove(instance.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnGameStateChanged));
	}
}
