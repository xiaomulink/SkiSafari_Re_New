using UnityEngine;

public class Hazard : MonoBehaviour
{
	public enum OnHitBehaviour
	{
		Nothing = 0,
		Disable = 1,
		Despawn = 2
	}

	public string category = string.Empty;

	public GameObject hitEffect;

	public float speedImpact = 5f;

	public OnHitBehaviour onHit = OnHitBehaviour.Disable;

	public int count = 1;

	public GameObject replacementObject;

	public void OnHitByPlayer(Player player)
	{
		if ((bool)hitEffect)
		{
			TransformUtils.Instantiate(hitEffect, base.transform, false, true);
		}
		switch (onHit)
		{
		case OnHitBehaviour.Disable:
			base.gameObject.SetActive(false);
			break;
		case OnHitBehaviour.Despawn:
			Pool.Despawn(base.gameObject);
			break;
		}
		if ((bool)replacementObject)
		{
			replacementObject.SetActive(true);
		}
	}

	private void OnEnable()
	{
		if ((bool)replacementObject)
		{
			replacementObject.SetActive(false);
		}
	}
}
