using UnityEngine;

public abstract class Booster : MonoBehaviour
{
	public string buttonTextureName;

	private int m_usesSinceGameStarted;

	public int CurrentCount
	{
		get
		{
			return GameState.GetInt(base.name);
		}
		set
		{
			GameState.SetInt(base.name, value);
		}
	}

	public abstract bool CanUse { get; }

	protected abstract void OnUse();

	public void Use()
	{
		if (CanUse)
		{
			OnUse();
			CurrentCount--;
			m_usesSinceGameStarted++;
		}
	}

	public void OnGameStarted()
	{
		m_usesSinceGameStarted = 0;
	}

	public void OnGameStopped()
	{
		if (m_usesSinceGameStarted > 0)
		{
			AnalyticsManager.Instance.SendEvent("session_used_" + base.name, "count", m_usesSinceGameStarted.ToString(), "slope", Slope.Instance.name);
			m_usesSinceGameStarted = 0;
		}
	}
}
