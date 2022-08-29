using System;
using UnityEngine;

public class FlamePowerup : MonoBehaviour
{
    Transform PlayerTrans;
    public Vector3 Pos =new Vector3(-2.2f,1.5f,0);
	public enum Level
	{
		None = 0,
		Yellow = 1,
		Red = 2,
		Blue = 3,
		SuperBlue = 4
	}

	[Serializable]
	public class Flame
	{
		public bool available = true;

		public float duration = 3f;

		public float speedIncrease = 5f;

		public float minSpeedRatioOnIgnite = 0.65f;

		public float lateralDragIncrease;

		public ParticleSystem fireParticlesPrefab;

		public FXTrail fireTrailPrefab;

		public Sound igniteSound;

		public ParticleSystem m_fireParticles;

		private FXTrail m_fireTrail;

		private AudioSource m_audio;

		private bool m_isFlaming;

		public bool IsFlaming
		{
			get
			{
				return m_isFlaming;
			}
		}

		public void Init(FlamePowerup flamePowerup, Transform pivot)
		{
			if (available)
			{
                //GameObject go = Instantiate(fireParticlesPrefab.gameObject, GameObject.Find("GamePool").transform);
                //fireParticlesPrefab = go.GetComponent<FlamePowerup>().stage1ParticlesPrefab;
                //fireTrailPrefab = go.GetComponent<FlamePowerup>().;

                m_fireParticles = TransformUtils.Instantiate(fireParticlesPrefab, pivot);
				m_fireTrail = TransformUtils.Instantiate(fireTrailPrefab, pivot);
				m_fireTrail.transform.localPosition += flamePowerup.flameTrailOffset;
				m_audio = m_fireTrail.GetComponent<AudioSource>();
			}
		}

		public void Reset()
		{
			if (available)
			{
				m_isFlaming = false;
				m_fireParticles.enableEmission = false;
				m_fireParticles.Clear();
				m_fireTrail.gameObject.SetActive(false);
			}
		}

		public void SetRatio(float ratio)
		{
			if (ratio >= 1f)
			{
				Ignite();
			}
			else if (m_isFlaming)
			{
				UpdateFlames(ratio);
			}
		}

		private void Ignite()
		{
			m_fireParticles.enableEmission=true;
			m_fireTrail.gameObject.SetActive(true);
			if (SoundManager.Instance.SFXEnabled)
			{
				m_audio.loop = false;
				m_audio.PlayOneShot(igniteSound.clip);
				m_audio.loop = true;
			}
			UpdateFlames(1f);
			m_isFlaming = true;
		}

		private void UpdateFlames(float ratio)
		{
			if (ratio > 0f)
			{
				float num = 1f - ratio;
				m_fireTrail.StartWidth = fireTrailPrefab.startWidth * (1f - num * num);
				m_fireTrail.StartColor = new Color(1f, 1f, 1f, Mathf.Lerp(0.3f, 1f, ratio));
				m_audio.volume = ratio;
			}
			else
			{
				m_fireParticles.enableEmission = false;
				m_fireTrail.gameObject.SetActive(false);
				m_audio.Stop();
				m_isFlaming = false;
			}
		}
	}

	public Vector3 pivotOffset;

	public Vector3 flameTrailOffset;

	public Flame yellowFlame;

	public Flame redFlame;

	public Flame blueFlame;

	public Flame superBlueFlame;

	public float stage1Ratio = 0.33f;

	public ParticleSystem stage1ParticlesPrefab;

	public float stage2Ratio = 0.66f;

	public ParticleSystem stage2ParticlesPrefab;

	public float directionFilter = 4f;

	private Skier m_skier;

	private CircleCollider m_collider;

	private Transform m_pivot;

	private float m_normalMaxSpeed;

	private float m_normalMinLateralDrag;

	private float m_normalMaxLateralDrag;

	private ParticleSystem m_stage1Particles;

	private ParticleSystem m_stage2Particles;

	private Level m_activeLevel;

	private Flame m_activeFlame;

	private Flame m_nextFlame;

	private float m_flameTimer;

	private bool m_wasEmittingFlameEffects;

	private bool m_touchedGroundAfterIgnite;

	private Vector3 m_filteredDir;

	private float m_lastRatio;

	public Skier Skier
	{
		get
		{
			return m_skier;
		}
		set
		{
			m_skier = value;
			m_normalMinLateralDrag = m_skier.minLateralDrag;
			m_normalMaxLateralDrag = m_skier.maxLateralDrag;
		}
	}

	public CircleCollider Collider
	{
		get
		{
			return m_collider;
		}
		set
		{
			m_collider = value;
			m_normalMaxSpeed = m_collider.maxSpeed;
		}
	}

	public bool IsIgnited
	{
		get
		{
			return m_flameTimer > 0f;
		}
	}

	public bool IsEmitting
	{
		get
		{
			return m_stage1Particles.enableEmission || m_stage2Particles.enableEmission;
		}
	}

	public bool IsDwindling
	{
		get
		{
			return m_touchedGroundAfterIgnite;
		}
	}

	public Level IgnitionLevel
	{
		get
		{
			return m_activeLevel;
		}
	}

	public bool CanIgniteToLevel(Level level)
	{
		if (level == m_activeLevel && m_touchedGroundAfterIgnite)
		{
			return true;
		}
		return GetFlame(level) != null;
	}

	public void PreIgnite(float ratio)
	{
		if (m_nextFlame != null)
		{
			m_nextFlame.SetRatio(ratio);
		}
		if (ratio > stage1Ratio)
		{
			if (m_lastRatio <= stage1Ratio)
			{
				m_stage1Particles.enableEmission = true;
				m_stage1Particles.Clear();
			}
		}
		else if (m_lastRatio > stage1Ratio)
		{
			m_stage1Particles.enableEmission = false;
			m_stage1Particles.Clear();
		}
		if (ratio > stage2Ratio)
		{
			if (m_lastRatio <= stage2Ratio)
			{
				m_stage2Particles.enableEmission = true;
				m_stage2Particles.Clear();
			}
		}
		else if (m_lastRatio > stage2Ratio)
		{
			m_stage2Particles.enableEmission = false;
			m_stage2Particles.Clear();
           
		}
		m_lastRatio = ratio;
	}

	public bool Ignite(Level minLevel)
	{
		if (m_activeLevel == Level.SuperBlue)
		{
			RefillActiveLevel();
			return true;
		}
		Level level = minLevel;
		if (m_activeLevel >= minLevel)
		{
			level = ((m_activeLevel >= Level.Blue) ? m_activeLevel : (m_activeLevel + 1));
		}
		if (level == m_activeLevel)
		{
			RefillActiveLevel();
			return true;
		}
		Flame flame = GetFlame(level);
		if (flame == null)
		{
			if (m_activeFlame != null)
			{
				RefillActiveLevel();
				return true;
			}
			return false;
		}
		if (m_activeFlame != null)
		{
			m_activeFlame.SetRatio(0f);
		}
		m_activeLevel = level;
		m_activeFlame = flame;
		RefillActiveLevel();
		if (level < Level.Blue)
		{
			level++;
			m_nextFlame = GetFlame(level);
		}
		else
		{
			m_nextFlame = null;
		}
		return true;
	}

	public void Douse()
	{
		m_touchedGroundAfterIgnite = true;
	}

	private Flame GetFlame(Level level)
	{
		switch (level)
		{
		case Level.Yellow:
			return (!yellowFlame.available) ? null : yellowFlame;
		case Level.Red:
			return (!redFlame.available) ? null : redFlame;
		case Level.Blue:
			return (!blueFlame.available) ? null : blueFlame;
		case Level.SuperBlue:
			return (!superBlueFlame.available) ? null : superBlueFlame;
		default:
			return null;
		}
	}

	private void RefillActiveLevel()
	{
		m_stage1Particles.enableEmission = false;
		m_stage2Particles.Clear();
		m_stage2Particles.enableEmission = false;
		m_stage2Particles.Clear();
		m_lastRatio = 1f;
		m_activeFlame.SetRatio(1f);
		m_flameTimer = m_activeFlame.duration;
		m_touchedGroundAfterIgnite = false;
		m_collider.maxSpeed = m_normalMaxSpeed + m_activeFlame.speedIncrease;
		if ((bool)m_skier)
		{
			m_skier.minLateralDrag = m_normalMinLateralDrag + m_activeFlame.lateralDragIncrease;
			m_skier.maxLateralDrag = m_normalMaxLateralDrag + m_activeFlame.lateralDragIncrease;
		}
		float num = m_collider.maxSpeed * m_activeFlame.minSpeedRatioOnIgnite;
		if (m_collider.OnGround)
		{
			Vector3 normal = m_collider.GroundContactInfo.normal;
			Vector3 vector = new Vector3(normal.y, 0f - normal.x);
			float num2 = Vector3.Dot(m_collider.velocity, vector);
			if (num2 < num)
			{
				m_collider.velocity = vector * num;
			}
		}
		else if (Vector3.Dot(m_collider.velocity, Vector3.right) >= 0f - Mathf.Epsilon)
		{
			float magnitude = m_collider.velocity.magnitude;
			if (magnitude < num)
			{
				m_collider.velocity *= num / magnitude;
			}
		}
		else if (Vector3.Dot(m_collider.transform.right, Vector3.right) >= 0f - Mathf.Epsilon)
		{
			float num3 = Vector3.Dot(m_collider.velocity, m_collider.transform.right);
			if (num3 < num)
			{
				m_collider.velocity += m_collider.transform.right * (num - num3);
			}
		}
		else
		{
			m_collider.velocity = Vector3.right * num;
		}
	}

	private void RevertToNormal()
	{
		m_collider.maxSpeed = m_normalMaxSpeed;
		if ((bool)m_skier)
		{
			m_skier.minLateralDrag = m_normalMinLateralDrag;
			m_skier.maxLateralDrag = m_normalMaxLateralDrag;
		}
	}

	private void Awake()
	{
        Pos = new Vector3(-2.2f, 1.5f, 0);
        GameObject gameObject = new GameObject("Pivot");
		m_pivot = gameObject.transform;
		m_pivot.parent = base.transform;
		m_pivot.localPosition = pivotOffset;
		m_pivot.localRotation = Quaternion.identity;

		m_stage1Particles = TransformUtils.Instantiate(stage1ParticlesPrefab, m_pivot);
		m_stage2Particles = TransformUtils.Instantiate(stage2ParticlesPrefab, m_pivot);
		yellowFlame.Init(this, m_pivot);
		redFlame.Init(this, m_pivot);
		blueFlame.Init(this, m_pivot);
		superBlueFlame.Init(this, m_pivot);
	}
    private void Start()
    {
        PlayerTrans = transform.parent.Find("SkierSprite");

    }
    private void OnEnable()
	{
       
		m_activeLevel = Level.None;
		m_activeFlame = null;
		m_nextFlame = yellowFlame;
		m_flameTimer = 0f;
		m_wasEmittingFlameEffects = false;
		m_touchedGroundAfterIgnite = false;
		m_filteredDir = Vector3.right;
		m_lastRatio = 0f;
		m_stage1Particles.enableEmission = false;
		m_stage1Particles.Clear();
		m_stage2Particles.enableEmission = false;
		m_stage2Particles.Clear();
		yellowFlame.Reset();
		redFlame.Reset();
		blueFlame.Reset();
		superBlueFlame.Reset();
		if ((bool)m_skier)
		{
			RevertToNormal();
		}
	}

	private void FixedUpdate()
	{
		if (!(m_flameTimer > 0f) || (!m_touchedGroundAfterIgnite && !m_collider.OnGround))
		{
			return;
		}
		if (!m_touchedGroundAfterIgnite)
		{
			if (m_nextFlame != null)
			{
				m_nextFlame.SetRatio(0f);
			}
			m_touchedGroundAfterIgnite = true;
		}
		m_flameTimer -= Time.fixedDeltaTime;
		if (m_flameTimer <= 0f)
		{
			m_flameTimer = 0f;
			RevertToNormal();
			if (m_nextFlame != null)
			{
				m_nextFlame.SetRatio(0f);
			}
			m_activeFlame.SetRatio(0f);
			m_activeLevel = Level.None;
			m_activeFlame = null;
			m_nextFlame = yellowFlame;
		}
		else
		{
			m_activeFlame.SetRatio(m_flameTimer / m_activeFlame.duration);
		}

        transform.localPosition = PlayerTrans.localPosition+ Pos;
        transform.localEulerAngles = PlayerTrans.transform.localEulerAngles;
    }

	private void LateUpdate()
	{
		if (!m_collider.OnGround && IsEmitting)
		{
			if (!m_wasEmittingFlameEffects)
			{
				m_filteredDir = m_collider.velocity.normalized;
			}
			else
			{
				m_filteredDir = Vector3.Lerp(m_filteredDir, m_collider.velocity.normalized, directionFilter * Time.deltaTime);
			}
			m_wasEmittingFlameEffects = true;
		}
		else
		{
			m_filteredDir = m_collider.velocity.normalized;
			m_wasEmittingFlameEffects = false;
		}
		Vector3 normalized = Vector3.Cross(Vector3.forward, m_filteredDir).normalized;
		Quaternion rotation = Quaternion.LookRotation(Vector3.forward, normalized);
		m_pivot.rotation = rotation;
	}
}
