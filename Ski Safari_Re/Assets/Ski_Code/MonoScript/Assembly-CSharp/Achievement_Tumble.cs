using UnityEngine;

public class Achievement_Tumble : Achievement_Time
{
	private void Update()
	{
		PlayerSkierBellyslide playerSkierBellyslide = Player.Instance as PlayerSkierBellyslide;
		if ((bool)playerSkierBellyslide && playerSkierBellyslide.IsTumbling)
		{
			Increment(Time.deltaTime);
		}
	}
}
