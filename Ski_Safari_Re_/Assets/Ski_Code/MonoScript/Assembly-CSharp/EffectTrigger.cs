using UnityEngine;

public class EffectTrigger : PlayerTrigger
{
	public delegate void OnPlayerTriggerEffectDelegate(Player player, EffectTrigger effectTrigger);

	public string effectCategory = "snowman_hat";

	public GameObject effectPrefab;

	public static OnPlayerTriggerEffectDelegate OnPlayerTriggerEffect;

	private bool m_triggered;

	protected override void OnPlayerTrigger(Player player)
	{
		m_triggered = true;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_triggered = false;
	}

	protected override void FixedUpdate()
	{
		if (m_triggered)
		{
			Pool.Spawn(effectPrefab, base.transform.position, base.transform.rotation);
			if (OnPlayerTriggerEffect != null && Player.Instance != null)
			{
				OnPlayerTriggerEffect(Player.Instance, this);
			}
			base.gameObject.SetActive(false);
		}
		base.FixedUpdate();
	}
}
