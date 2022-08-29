using UnityEngine;

public class Booster_Mount : Booster
{
	public PlayerSkierMounted playerPrefab;

	public float minSpeed = 10f;

	public bool startIgnited;

	public float minPitch = -60f;

	public float maxPitch = 60f;

	public string[] excludedCategories;

	public override bool CanUse
	{
		get
		{
			Player instance = Player.Instance;
			if (!instance)
			{
				return false;
			}
			if (base.CurrentCount <= 0)
			{
				return false;
			}
			if (instance.IgnitionLevel == FlamePowerup.Level.SuperBlue)
			{
				return false;
			}
			string category = instance.Category;
			string[] array = excludedCategories;
			foreach (string text in array)
			{
				if (text == category)
				{
					return false;
				}
			}
			return true;
		}
	}

	protected override void OnUse()
	{
		Player instance = Player.Instance;
		Vector3 eulerAngles = instance.transform.eulerAngles;
		if (eulerAngles.z > 180f)
		{
			eulerAngles.z = Mathf.Max(eulerAngles.z, 360f + minPitch);
		}
		else
		{
			eulerAngles.z = Mathf.Min(eulerAngles.z, maxPitch);
		}
		Quaternion quaternion = Quaternion.Euler(eulerAngles);
		Vector3 zero = Vector3.zero;
		if (minSpeed > 0f)
		{
			Vector3 lhs = quaternion * Vector3.right;
			float num = Vector3.Dot(lhs, instance.Collider.velocity);
			if (num < minSpeed)
			{
				zero.x = minSpeed - num;
			}
		}
		instance.Mount(playerPrefab, instance.transform.position, quaternion, startIgnited, zero, null, false, true);
	}
}
