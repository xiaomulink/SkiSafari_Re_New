using UnityEngine;

public class Stunt_RideAvalanche : Stunt
{
	public float rideDistance = 10f;

	public float escapeDistance = 10f;

	public float minRideDuration = 0.5f;

	public float score = 400f;

	public string description = "White Powder Rush!";

	public static Player.SimplePlayerDelegate OnRideAvalanche;

	private float m_rideTimer;

	protected void FixedUpdate()
	{
		Player instance = Player.Instance;
		if ((bool)instance && !(instance is PlayerSkierBellyslide) && !(instance is PlayerSkierSleeping) && (bool)Avalanche.Instance)
		{
			float num = Vector3.Distance(instance.transform.position, Avalanche.Instance.transform.position);
			if (num <= rideDistance)
			{
				m_rideTimer += Time.deltaTime;
				return;
			}
			if (num < escapeDistance)
			{
				return;
			}
			if (m_rideTimer >= minRideDuration)
			{
				base.Manager.AddScore(score, description);
				if (OnRideAvalanche != null)
				{
					OnRideAvalanche(instance);
				}
			}
		}
		m_rideTimer = 0f;
	}

	protected override void OnEnable()
	{
	}

	protected override void OnDisable()
	{
	}
}
