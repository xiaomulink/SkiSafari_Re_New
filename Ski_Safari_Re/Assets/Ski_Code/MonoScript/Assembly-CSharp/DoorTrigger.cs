using UnityEngine;

public class DoorTrigger : PlayerTrigger
{
	private enum State
	{
		Closed = 0,
		Opening = 1,
		Open = 2
	}

	public delegate void OnPlayerOpenDoorDelegate(Player player, DoorTrigger doorTrigger);

	public string doorCategory = "cabin";

	public Transform doorPivot;

	public float doorStartYaw = -45f;

	public float doorEndYaw;

	public float doorOpeningDuration = 0.5f;

	public AudioSourceSettings audioSourceSettings;

	public Sound openSound;

	public Sound reopenSound;

	public Sound[] randomSounds;

	public float randomSoundChance = 0.25f;

	public GameObject openEffectPrefab;

	public bool igniteOnTrigger;

	public string[] explodeMountCategories;

	public AvalancheDestroyable ownerDestroyable;

	public static OnPlayerOpenDoorDelegate OnPlayerOpenDoor;

	private AudioSource m_audio;

	private State m_state;

	private float m_stateTimer;

	private int m_randomSoundIndex;

	protected override void OnPlayerTrigger(Player player)
	{
		OpenDoor();
		if (igniteOnTrigger && !(player is PlayerSkierBellyslide))
		{
			player.Ignite(FlamePowerup.Level.Red);
		}
		PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
		if ((bool)playerSkierMounted)
		{
			string[] array = explodeMountCategories;
			foreach (string text in array)
			{
				if (playerSkierMounted.mountCategory == text)
				{
					player.TakeDamage(0f, Player.DamageFlags.None);
					ownerDestroyable.Destroy();
					break;
				}
			}
		}
		if (OnPlayerOpenDoor != null && PlayerManager.IsHumanPlayer(player))
		{
			OnPlayerOpenDoor(player, this);
		}
	}

	private void OpenDoor()
	{
		if (SoundManager.Instance.SFXEnabled)
		{
			if (m_state == State.Closed)
			{
				m_audio.loop = false;
				m_audio.PlayOneShot(openSound.clip);
			}
			else if (m_state == State.Open)
			{
				m_audio.loop = false;
				m_audio.PlayOneShot(reopenSound.clip);
			}
			if (Random.value <= randomSoundChance)
			{
				m_audio.loop = false;
				m_audio.PlayOneShot(randomSounds[m_randomSoundIndex].clip);
				m_randomSoundIndex = (m_randomSoundIndex + 1) % randomSounds.Length;
			}
		}
		if (m_state == State.Closed)
		{
			TransformUtils.Instantiate(openEffectPrefab, base.transform, false, true);
		}
		m_state = State.Opening;
		m_stateTimer = 0f;
	}

	protected override void Awake()
	{
		base.Awake();
		m_audio = audioSourceSettings.CreateSource(base.gameObject);
		m_randomSoundIndex = Random.Range(0, randomSounds.Length);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		m_state = State.Closed;
		m_stateTimer = 0f;
		doorPivot.localEulerAngles = new Vector3(0f, 180f, 0f);
	}

	private void Update()
	{
		if (m_state == State.Closed)
		{
			foreach (Skier allSkier in Skier.AllSkiers)
			{
				if (allSkier.Collider.terrainLayer == TerrainLayer.Game && ((Vector2)(base.transform.position - allSkier.transform.position)).sqrMagnitude < m_sqrRadius + allSkier.Collider.SqrRadius && CameraUtils.IsPointVisible(allSkier.transform.position))
				{
					OpenDoor();
					break;
				}
			}
		}
		if (m_state == State.Opening)
		{
			m_stateTimer += Time.deltaTime;
			if (m_stateTimer > doorOpeningDuration)
			{
				doorPivot.localEulerAngles = new Vector3(0f, doorEndYaw, 0f);
				m_state = State.Open;
			}
			else
			{
				float t = m_stateTimer / doorOpeningDuration;
				doorPivot.localEulerAngles = new Vector3(0f, Mathf.Lerp(doorStartYaw, doorEndYaw, t), 0f);
			}
		}
	}
}
