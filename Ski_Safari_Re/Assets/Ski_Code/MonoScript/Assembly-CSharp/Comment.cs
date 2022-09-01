using UnityEngine;

public class Comment : MonoBehaviour
{
	public GUICommentator.Message[] messages;

	public int minRunsForReplay = 2;

	public int maxRunsForReplay = 10;

	public int maxLevel = 15;

	public bool CanActivate
	{
		get
		{
			return PlayerPrefs.GetInt(base.name) <= 0 && (maxLevel < 0 || LevelManager.Instance.CurrentLevel <= maxLevel);
		}
	}

	public void Reset()
	{
		PlayerPrefs.SetInt(base.name, 0);
	}

	public void IncrementRuns()
	{
		PlayerPrefs.SetInt(base.name, PlayerPrefs.GetInt(base.name) - 1);
	}

	protected void ShowMessagesAndComplete()
	{
		if (!CreditsSpawner.Instance.EnableSpawning)
		{
			GUICommentator.Message[] array = messages;
			foreach (GUICommentator.Message message in array)
			{
				CommentManager.Instance.ShowMessage(message);
			}
			Complete();
		}
	}

	protected void Complete()
	{
		PlayerPrefs.SetInt(base.name, Random.Range(minRunsForReplay, maxRunsForReplay + 1));
		CommentManager.Instance.OnComplete(this);
	}
}
