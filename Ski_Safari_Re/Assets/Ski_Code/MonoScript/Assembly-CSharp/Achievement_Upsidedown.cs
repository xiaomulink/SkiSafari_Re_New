using UnityEngine;

public class Achievement_Upsidedown : Achievement_Time
{
	public string requiredMountCategory;

	private bool CheckAngle(Player player)
	{
		float z = player.transform.eulerAngles.z;
		return z >= 135f && z <= 225f;
	}

	private void Update()
	{
		Player instance = Player.Instance;
		if ((bool)instance && Achievement.CheckMountCategory(instance, requiredMountCategory) && CheckAngle(instance))
		{
			Increment(Time.deltaTime);
		}
	}
}
