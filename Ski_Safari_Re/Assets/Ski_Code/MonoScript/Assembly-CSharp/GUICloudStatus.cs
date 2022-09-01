using UnityEngine;

public class GUICloudStatus : GUIButton
{
	private enum State
	{
		None = 0,
		Refresh = 1,
		Synced = 2,
		Error = 3
	}

	public GUIDropShadowText statusText;

	public GameObject cloudIcon;

	public GameObject syncedIcon;

	public GameObject refreshIcon;

	public GameObject errorIcon;

	private State m_state;

	public override void Click(Vector3 position)
	{
		base.Click(position);
	}

	private GameObject GetStateIcon(State state)
	{
		switch (m_state)
		{
		case State.Synced:
			return syncedIcon;
		case State.Refresh:
			return refreshIcon;
		case State.Error:
			return errorIcon;
		default:
			return null;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		cloudIcon.SetActive(false);
		statusText.gameObject.SetActive(false);
		m_state = State.None;
	}
}
