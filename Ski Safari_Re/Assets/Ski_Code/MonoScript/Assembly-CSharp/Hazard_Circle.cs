using UnityEngine;

public class Hazard_Circle : Hazard
{
	public float radius = 1f;

	private void Update()
	{
		foreach (Player player in PlayerManager.GetPlayers(PlayerManager.PlayerGroup.All))
		{
			if (((Vector2)(base.transform.position - player.transform.position)).magnitude < radius + player.Collider.radius)
			{
				player.Hit(this);
				break;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
