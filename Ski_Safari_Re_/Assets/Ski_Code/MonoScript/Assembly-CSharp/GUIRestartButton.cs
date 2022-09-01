using UnityEngine;

public class GUIRestartButton : GUIButton
{
	public override void Click(Vector3 position)
	{
		base.Click(position);
		Deactivate();
		SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.QuickSki);
	}
}
