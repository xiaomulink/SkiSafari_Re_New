using UnityEngine;

public class GUICreditsButton : GUIButton
{
	public override void Click(Vector3 position)
	{
		Deactivate();
		SkiGameManager.Instance.StartSkiing();
		CreditsSpawner.Instance.EnableSpawning = true;
	}
}
