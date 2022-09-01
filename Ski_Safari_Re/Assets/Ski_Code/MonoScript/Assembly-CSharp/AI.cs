using UnityEngine;

public class AI : MonoBehaviour
{
	protected virtual void FixedUpdate()
	{
		if (SkiGameManager.Instance.Started)
		{
			Avalanche instance = Avalanche.Instance;
			if ((bool)instance && instance.Contains(base.transform.position))
			{
				Pool.Despawn(base.gameObject);
			}
		}
	}
}
