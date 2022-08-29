using UnityEngine;

public class PlayerPiledrive : Player
{
	public delegate void OnPlayerGetUpDelegate(PlayerPiledrive previousPlayer, Player player);

	public PlayerSkier skierPrefab;

	public float recoverDelay = 3f;

	public float recoverKickForwardSpeed = 20f;

	public float recoverKickUpSpeed = 10f;

	public float delayDecayPerTap = 0.1f;

	public float delayBetweenTaps = 0.2f;

	public GameObject recoverEffect;

	public Sound recoverSound;

	public static OnPlayerGetUpDelegate OnPlayerGetUp;

	private float m_recoverTimer;

	private float m_recoverTimerOnTap;

	private bool m_tapping;

	public override string Category
	{
		get
		{
			return "piledrive";
		}
	}

	public override bool IsInvulnerable
	{
		get
		{
			return true;
		}
	}

	public override float LiftInput
	{
		get
		{
			return base.LiftInput;
		}
		set
		{
			base.LiftInput = value;
			if (value == 1f)
			{
				if (m_tapping)
				{
					return;
				}
				m_tapping = true;
				float num = m_recoverTimerOnTap - m_recoverTimer;
				if (num >= delayBetweenTaps)
				{
					m_recoverTimer -= delayDecayPerTap;
					m_recoverTimerOnTap = m_recoverTimer;
					if (m_recoverTimer <= 0f)
					{
						GetUp();
					}
				}
			}
			else
			{
				m_tapping = false;
			}
		}
	}

	public override bool CanDieByAvalanche()
	{
		return true;
	}

	private void GetUp()
	{
		if (base.enabled)
		{
			if ((bool)recoverEffect)
			{
				Pool.Spawn(recoverEffect, base.transform.position, base.transform.rotation);
			}
			Vector3 velocity = recoverKickForwardSpeed * base.transform.right + recoverKickUpSpeed * base.transform.up;
			Vector3 normalized = velocity.normalized;
			Quaternion rotation = Quaternion.LookRotation(upwards: new Vector3(0f - normalized.y, normalized.x, 0f), forward: Vector3.forward);
			Vector3 position = base.transform.position + new Vector3(0f, 0.1f, 0f);
			Pool.Despawn(base.gameObject);
			Player player = PlayerManager.SpawnReplacement(this, skierPrefab, position, rotation);
			player.Collider.velocity = velocity;
			player.LiftInput = 1f;
			if ((bool)recoverSound && SoundManager.Instance.SFXEnabled)
			{
				player.GetComponent<AudioSource>().loop = false;
				player.GetComponent<AudioSource>().PlayOneShot(recoverSound.clip);
			}
			if (OnPlayerGetUp != null && PlayerManager.IsHumanPlayer(player))
			{
				OnPlayerGetUp(this, player);
			}
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_recoverTimer = recoverDelay;
		m_recoverTimerOnTap = recoverDelay;
		m_tapping = false;
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!base.IsDisabling && m_recoverTimer > 0f)
		{
			m_recoverTimer -= Time.deltaTime;
			if (m_recoverTimer <= 0f)
			{
				GetUp();
			}
		}
	}
}
