using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{
	public enum DamageFlags
	{
		None = 0,
		HitHazard = 1,
		IgnoreInvulnerability = 2,
		Piledrive = 4,
		SpeedOnly = 8
	}

	public delegate void SimplePlayerDelegate(Player player);

	public delegate void OnMountDelegate(Player previousPlayer, Player mountedPlayer);

	public delegate void OnDisountDelegate(PlayerSkierMounted mountedPlayer, Player player);

	public delegate void OnHitHazardDelegate(Player previousPlayer, Player player, Hazard hazard);

	public delegate void OnWaterDelegate(Player player, Water water);

	public delegate void OnCollisionDelegate(Player player, GeometryUtils.ContactInfo contactInfo);

	public delegate void OnAttachDelegate(Player player, Attachment attachment);

	public delegate void OnTakeDamageDelegate(Player previousPlayer, Player player);

	public delegate void OnFreezeDelegate(Player previousPlayer, PlayerSkierFrozen frozenPlayer);

	public delegate void OnSnowballDelegate(Player previousPlayer, PlayerSnowball snowballPlayer);

	public delegate void OnIgniteDelegate(Player player, FlamePowerup.Level ignitionLevel);

	public static Player Instance;

	public float initialInvulnerabilityTime = 1f;

	public bool alwaysInvulnerable;

	public string[] hazardImmunities;

	public bool immuneToAvalanche;

	public FlamePowerup flamePowerupPrefab;

	public Sound[] igniteSounds;

	public float igniteSoundChance = 0.25f;

	public FlamePowerup.Level ignitionLevelOverride;

	public AttachNode[] attachNodes;

	public float kickOnDetach = 10f;

	public float reattachDelay = 1f;

	public Sound[] attachSounds;

	public bool preventNewAttachments;

	public bool allowInstantRemount;

	public string[] allowedMountCategories;

	public List<Player> allowedLeaders = new List<Player>();

	public Follower followerPrefab;

	public float minFollowPositionHistorySeparation = 0.5f;

	public float maxFollowDistance;

	public bool useCameraOffsetOverride;

	public Vector3 cameraOffsetOverride = Vector3.zero;

	public static SimplePlayerDelegate OnPlayerLand;

	public static SimplePlayerDelegate OnPlayerHardLanding;

	public static OnCollisionDelegate OnPlayerContact;

	public static OnCollisionDelegate OnPlayerBreakLine;

	public static SimplePlayerDelegate OnPlayerTakeoff;

	public static OnTakeDamageDelegate OnPlayerTakeDamage;

	public static OnFreezeDelegate OnPlayerFreeze;

	public static SimplePlayerDelegate OnPlayerIgnoreFreeze;

	public static OnSnowballDelegate OnPlayerSnowball;

	public static SimplePlayerDelegate OnPlayerBackflip;

	public static OnMountDelegate OnPlayerMount;

	public static OnDisountDelegate OnPlayerDismount;

	public static SimplePlayerDelegate OnPlayerFollow;

	public static OnHitHazardDelegate OnPlayerHitHazard;

	public static SimplePlayerDelegate OnPlayerIgniteFromGlide;

	public static OnAttachDelegate OnPlayerAttach;

	public static OnIgniteDelegate OnPlayerIgnite;

	private CircleCollider m_collider;

	private FlamePowerup m_flamePowerup;

	public Vector3[] m_positionHistory;

	private float m_minFollowPositionHistorySeparationSqr;

	private float m_invulnerabilityTimer;

	private float m_attachTimer;

	private bool m_mounting;

	private List<Follower> m_followers = new List<Follower>();

	private int m_positionHistoryIndex;

	private int m_positionHistoryCount;

	private static List<Player> s_allPlayers = new List<Player>();

	[CompilerGenerated]
	private static Comparison<Attachment> _003C_003Ef__am_0024cache33;

	public static List<Player> AllPlayers
	{
		get
		{
			return s_allPlayers;
		}
	}

	public virtual string Category
	{
		get
		{
			return "none";
		}
	}

	public virtual CircleCollider Collider
	{
		get
		{
			return m_collider;
		}
	}

	public FlamePowerup FlamePowerup
	{
		get
		{
			return m_flamePowerup;
		}
	}

	public virtual Vector3 LookAtPos
	{
		get
		{
			if (m_followers.Count > 0)
			{
				return m_followers[m_followers.Count - 1].transform.position;
			}
			return base.transform.position;
		}
	}

	public virtual VOSource VOSource
	{
		get
		{
			return null;
		}
	}

	public bool IsDisabling { get; private set; }

	public virtual bool IsGliding
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsInvulnerable
	{
		get
		{
			return alwaysInvulnerable || m_invulnerabilityTimer > 0f || ((bool)m_flamePowerup && m_flamePowerup.IsIgnited);
		}
	}

	public float InvulnerabilityTimer
	{
		get
		{
			return m_invulnerabilityTimer;
		}
		set
		{
			m_invulnerabilityTimer = value;
		}
	}

	public virtual FlamePowerup.Level IgnitionLevel
	{
		get
		{
			return (!m_flamePowerup) ? ignitionLevelOverride : m_flamePowerup.IgnitionLevel;
		}
	}

	public virtual float LiftInput
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public List<Follower> Followers
	{
		get
		{
			return m_followers;
		}
	}

	public bool UseCameraOffsetOverride
	{
		get
		{
			return useCameraOffsetOverride;
		}
	}

	public Vector3 CameraOffsetOverride
	{
		get
		{
			return cameraOffsetOverride;
		}
	}

	public virtual bool CanIgniteToLevel(FlamePowerup.Level ignitionLevel)
	{
		return (bool)m_flamePowerup && m_flamePowerup.CanIgniteToLevel(ignitionLevel);
	}

	public virtual void PreIgnite(float ratio)
	{
		if ((bool)m_flamePowerup)
		{
			m_flamePowerup.PreIgnite(ratio);
		}
	}

	private void PlayIgniteVO()
	{
		for (int num = attachNodes.Length - 1; num >= 0; num--)
		{
			if ((bool)attachNodes[num].Attachment && (bool)attachNodes[num].Attachment.igniteSound)
			{
				VOSource.PlayVO(attachNodes[num].Attachment.igniteSound);
				return;
			}
		}
		VOSource.PlayRandomVO(igniteSounds);
	}

	public virtual bool Ignite(FlamePowerup.Level minIgnitionLevel)
	{
		if ((bool)m_flamePowerup && m_flamePowerup.Ignite(minIgnitionLevel))
		{
			if (InvulnerabilityTimer <= 0f && (bool)VOSource && igniteSounds.Length > 0 && UnityEngine.Random.value <= igniteSoundChance)
			{
				PlayIgniteVO();
			}
			InvulnerabilityTimer = 0f;
			if (OnPlayerIgnite != null && PlayerManager.IsHumanPlayer(this))
			{
				OnPlayerIgnite(this, m_flamePowerup.IgnitionLevel);
			}
			return true;
		}
		return false;
	}

	public bool Ignite()
	{
		return Ignite(FlamePowerup.Level.Yellow);
	}

	public virtual void Douse()
	{
		if ((bool)m_flamePowerup)
		{
			m_flamePowerup.Douse();
		}
	}

	public virtual void TakeDamage(float speedImpact, DamageFlags damageFlags)
	{
	}

	public void Freeze()
	{
		if (!IsDisabling)
		{
			OnFreeze();
		}
	}

	protected virtual bool OnFreeze()
	{
		return false;
	}

	public bool Snowball(SnowballTrigger snowballTrigger)
	{
		if (IsDisabling)
		{
			return false;
		}
		return OnSnowball(snowballTrigger);
	}

	protected virtual bool OnSnowball(SnowballTrigger snowballTrigger)
	{
		return false;
	}

	protected virtual bool OnPreCollision(GeometryUtils.ContactInfo contactInfo)
	{
		if (IsDisabling)
		{
			return true;
		}
		if ((bool)contactInfo.collider && !Collider.OnGround && 0f - contactInfo.normalSpeed > contactInfo.collider.maxSpeed)
		{
			contactInfo.collider.BreakLine(contactInfo, Collider);
			if (OnPlayerBreakLine != null && PlayerManager.IsHumanPlayer(this))
			{
				OnPlayerBreakLine(this, contactInfo);
			}
			Hazard component = contactInfo.collider.GetComponent<Hazard>();
			if ((bool)component)
			{
				Hit(component);
			}
			return false;
		}
		return true;
	}

	protected virtual void OnCollision(GeometryUtils.ContactInfo contactInfo)
	{
		if (!IsDisabling && OnPlayerContact != null && PlayerManager.IsHumanPlayer(this))
		{
			OnPlayerContact(this, contactInfo);
		}
	}

	public void Hit(Hazard hazard)
	{
		if (IsDisabling)
		{
			return;
		}
		PlayerManager.PlayerType playerType = PlayerManager.GetPlayerType(this);
		if (OnHit(hazard))
		{
			Player player = PlayerManager.GetPlayer(playerType);
			if (OnPlayerHitHazard != null && PlayerManager.IsHumanPlayer(player))
			{
				OnPlayerHitHazard(this, player, hazard);
			}
			hazard.OnHitByPlayer(player);
		}
	}

	public bool IsImmuneToHazard(Hazard hazard)
	{
		for (int i = 0; i < hazardImmunities.Length; i++)
		{
			if (hazardImmunities[i] == hazard.category)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual bool OnHit(Hazard hazard)
	{
		if (IsImmuneToHazard(hazard))
		{
			return true;
		}
		if (hazard.category == "freezing_cloud")
		{
			Freeze();
			return true;
		}
		TakeDamage(hazard.speedImpact, DamageFlags.HitHazard);
		return true;
	}

	public virtual bool CanDieByAvalanche()
	{
		return !immuneToAvalanche && Collider.OnGround && !Collider.LastContactInfo.collider && PlayerManager.GetPlayer(PlayerManager.PlayerType.Human_1) == this;
	}

	private void TryReattach(AttachNode node)
	{
		AttachNode[] array = attachNodes;
		foreach (AttachNode attachNode in array)
		{
			if (attachNode != node && attachNode.CanAttach(node.Attachment))
			{
				attachNode.Attach(node.Pop());
				return;
			}
		}
		node.Detach();
	}

	public Attachment TryAttach(Attachment attachment, bool sendEvent)
	{
		if (m_mounting || m_attachTimer > 0f || (sendEvent && preventNewAttachments))
		{
			return null;
		}
		Attachment attachment2 = TryAttach(attachment, sendEvent, attachNodes);
		if ((bool)attachment2)
		{
			return attachment2;
		}
		foreach (Follower follower in m_followers)
		{
			attachment2 = TryAttach(attachment, sendEvent, follower.attachNodes);
			if ((bool)attachment2)
			{
				return attachment2;
			}
		}
		return null;
	}

	private Attachment TryAttach(Attachment attachment, bool sendEvent, AttachNode[] nodes)
	{
		foreach (AttachNode attachNode in nodes)
		{
			if (!attachNode.CanAttach(attachment))
			{
				continue;
			}
			if ((bool)attachNode.Attachment)
			{
				TryReattach(attachNode);
			}
			attachNode.Attach(attachment);
			if (sendEvent)
			{
				SoundManager.Instance.PlaySound(attachment.attachSound);
				if ((bool)VOSource && attachment.attachmentCategory != "hat")
				{
					VOSource.PlayRandomVO(attachSounds);
				}
				if (OnPlayerAttach != null && PlayerManager.IsHumanPlayer(this))
				{
					OnPlayerAttach(this, attachment);
				}
			}
			return attachNode.Attachment;
		}
		return null;
	}

	protected bool DetachFirstPassenger()
	{
		for (int num = attachNodes.Length - 1; num >= 0; num--)
		{
			AttachNode attachNode = attachNodes[num];
			if ((bool)attachNode.Attachment && (bool)attachNode.Attachment.detachedPrefab && attachNode.Attachment.attachmentCategory != "skier")
			{
				if ((bool)attachNode.Attachment.detachSound && SoundManager.Instance.SFXEnabled)
				{
					GetComponent<AudioSource>().PlayOneShot(attachNode.Attachment.detachSound.clip);
				}
				TransformUtils.Instantiate(attachNode.Attachment.detachEffectPrefab, attachNode.transform, true, true);
				attachNode.Detach();
				m_attachTimer = reattachDelay;
				m_invulnerabilityTimer = reattachDelay;
				LiftInput = 1f;
				Collider.velocity += base.transform.up * kickOnDetach;
				return true;
			}
		}
		return false;
	}

	public void DetachAll()
	{
		AttachNode[] array = attachNodes;
		foreach (AttachNode attachNode in array)
		{
			attachNode.Detach();
		}
		foreach (Follower follower in m_followers)
		{
			AttachNode[] array2 = follower.attachNodes;
			foreach (AttachNode attachNode2 in array2)
			{
				attachNode2.Detach();
			}
		}
		while (m_followers.Count > 0)
		{
			Pool.Despawn(m_followers[0].gameObject);
			m_followers.RemoveAt(0);
		}
	}

	public List<Attachment> PopAttachments()
	{
		List<Attachment> list = new List<Attachment>();
		AttachNode[] array = attachNodes;
		foreach (AttachNode attachNode in array)
		{
			if ((bool)attachNode.Attachment)
			{
				list.Add(attachNode.Pop());
			}
		}
		foreach (Follower follower in m_followers)
		{
			AttachNode[] array2 = follower.attachNodes;
			foreach (AttachNode attachNode2 in array2)
			{
				if ((bool)attachNode2.Attachment)
				{
					list.Add(attachNode2.Pop());
				}
			}
		}
		return list;
	}

	public void PushAttachments(List<Attachment> attachments)
	{
		foreach (Attachment attachment in attachments)
		{
			if (!TryAttach(attachment, false))
			{
				attachment.Detach(base.transform);
			}
		}
	}

	public bool HasAttachment(string attachmentCategory)
	{
		AttachNode[] array = attachNodes;
		foreach (AttachNode attachNode in array)
		{
			if ((bool)attachNode.Attachment && attachNode.Attachment.attachmentCategory == attachmentCategory)
			{
				return true;
			}
		}
		foreach (Follower follower in m_followers)
		{
			if (follower.followerCategory == attachmentCategory)
			{
				return true;
			}
			AttachNode[] array2 = follower.attachNodes;
			foreach (AttachNode attachNode2 in array2)
			{
				if ((bool)attachNode2.Attachment && attachNode2.Attachment.attachmentCategory == attachmentCategory)
				{
					return true;
				}
			}
		}
		return false;
	}

	public Player TryFollow(PlayerInteractionTrigger trigger, Transform sourceTransform)
	{
		if (m_mounting || m_attachTimer > 0f)
		{
			return null;
		}
		Player[] leaderPrefabs = trigger.followConfig.leaderPrefabs;
		foreach (Player leaderPrefab in leaderPrefabs)
		{
			Player player = TryFollow(leaderPrefab, sourceTransform);
			if ((bool)player)
			{
				if ((bool)player.VOSource)
				{
					player.VOSource.PlayVO(trigger.followConfig.followSound, true);
				}
				return player;
			}
		}
		return null;
	}

	private Player TryFollow(Player leaderPrefab, Transform sourceTransform)
	{
		if ((bool)followerPrefab && (bool)leaderPrefab && allowedLeaders.Contains(leaderPrefab))
		{
			Vector3 lookAtPos = LookAtPos;
			Vector3 velocity = Collider.velocity;
			FlamePowerup.Level ignitionLevel = IgnitionLevel;
			List<Attachment> attachments = PopAttachments();
			List<Follower> followers = PopFollowers();
			Pool.Despawn(base.gameObject);
			Player player = PlayerManager.SpawnReplacement(this, leaderPrefab, sourceTransform.position, sourceTransform.rotation);
			player.m_followers.Add(Pool.Spawn(followerPrefab, base.transform.position, base.transform.rotation));
			player.Collider.velocity = velocity;
			if (ignitionLevel > FlamePowerup.Level.None)
			{
				player.Ignite(ignitionLevel);
			}
			if (player.m_positionHistory.Length > 0 && m_positionHistoryCount > 0)
			{
				int num = m_positionHistoryIndex - m_positionHistoryCount + 1;
				if (num < 0)
				{
					num = m_positionHistory.Length + num;
				}
				while (m_positionHistoryCount > 0)
				{
					player.AddPositionToHistory(m_positionHistory[num]);
					m_positionHistoryCount--;
					if (++num >= m_positionHistory.Length)
					{
						num = 0;
					}
				}
			}
			player.PushAttachments(attachments);
			player.PushFollowers(followers);
			player.Collider.FixedUpdate();
			FollowCamera.Instance.AddPlayerChangeOffset(player.LookAtPos - lookAtPos);
			if (OnPlayerFollow != null && PlayerManager.IsHumanPlayer(player))
			{
				OnPlayerFollow(player);
			}
			return player;
		}
		return null;
	}

	protected virtual void OnDespawnAsLeader()
	{
	}

	protected Player TryLoseLeader()
	{
		if (m_followers.Count == 0)
		{
			return null;
		}
		Follower follower = m_followers[0];
		if (!follower.leaderPrefab)
		{
			return null;
		}
		Vector3 lookAtPos = LookAtPos;
		Vector3 velocity = Collider.velocity;
		List<Attachment> attachments = PopAttachments();
		List<Follower> list = PopFollowers();
		list.RemoveAt(0);
		Pool.Despawn(base.gameObject);
		OnDespawnAsLeader();
		Pool.Despawn(follower.gameObject);
		Player player = PlayerManager.SpawnReplacement(this, follower.leaderPrefab, follower.transform.position, follower.transform.rotation);
		player.Collider.velocity = velocity;
		player.PushAttachments(attachments);
		player.PushFollowers(list);
		player.m_attachTimer = reattachDelay;
		player.m_invulnerabilityTimer = reattachDelay;
		player.Collider.FixedUpdate();
		FollowCamera.Instance.AddPlayerChangeOffset(player.LookAtPos - lookAtPos);
		return player;
	}

	protected List<Follower> PopFollowers()
	{
		List<Follower> followers = m_followers;
		m_followers = new List<Follower>();
		return followers;
	}

	protected void DropAllFollowers()
	{
		while (m_followers.Count > 0)
		{
			Follower follower = m_followers[0];
			if ((bool)follower.detachedPrefab)
			{
				Pool.Spawn(follower.detachedPrefab, follower.transform.position, follower.transform.rotation);
			}
			Pool.Despawn(follower.gameObject);
			m_followers.RemoveAt(0);
		}
	}

	protected void PushFollowers(List<Follower> followers)
	{
		m_followers.AddRange(followers);
	}

	public virtual bool CanMount(PlayerInteractionTrigger trigger, bool forceInstantRemount)
	{
		if (!IsDisabling && !m_mounting && (forceInstantRemount || trigger.mountConfig.instantMount || allowInstantRemount || InvulnerabilityTimer <= 0f) && trigger.IsMountable(this))
		{
			string[] array = allowedMountCategories;
			foreach (string text in array)
			{
				if (text == trigger.category)
				{
					return true;
				}
			}
			return false;
		}
		return false;
	}

	public Player TryMount(PlayerInteractionTrigger trigger, bool sendEvents)
	{
		if (CanMount(trigger, false))
		{
			m_mounting = true;
			return trigger.Mount(this, sendEvents);
		}
		return null;
	}

	public Player TryMount(List<Attachment> attachments, bool sendEvents)
	{
		if (_003C_003Ef__am_0024cache33 == null)
		{
			_003C_003Ef__am_0024cache33 = _003CTryMount_003Em__1C;
		}
		attachments.Sort(_003C_003Ef__am_0024cache33);
		foreach (Attachment attachment in attachments)
		{
			if ((bool)attachment.detachedPrefab)
			{
				PlayerInteractionTrigger component = attachment.detachedPrefab.GetComponent<PlayerInteractionTrigger>();
				if ((bool)component && CanMount(component, true))
				{
					m_mounting = true;
					attachments.Remove(attachment);
					GameObject gameObject = Pool.Spawn(attachment.detachedPrefab, base.transform.position, base.transform.rotation);
					PlayerInteractionTrigger component2 = gameObject.GetComponent<PlayerInteractionTrigger>();
					return component2.Mount(this, sendEvents);
				}
			}
		}
		return this;
	}

	public Player Mount(Player skierPrefab, Vector3 position, Quaternion rotation, bool startIgnited, Vector3 velocityOverride, CircleCollider sourceCollider, bool usePlayerPosition, bool sendEvents)
	{
		Vector3 lookAtPos = LookAtPos;
		Vector3 velocity = Collider.velocity;
		float radius = Collider.radius;
		Vector3 vector = (usePlayerPosition ? ((!Collider.OnGround) ? Vector3.up : Collider.GroundContactInfo.normal) : ((!sourceCollider) ? base.transform.up : ((!sourceCollider.OnGround) ? Vector3.up : sourceCollider.GroundContactInfo.normal)));
		FlamePowerup.Level level = IgnitionLevel;
		if (level == FlamePowerup.Level.None && startIgnited)
		{
			level = FlamePowerup.Level.Yellow;
		}
		List<Attachment> attachments = PopAttachments();
		Pool.Despawn(base.gameObject);
		if (usePlayerPosition)
		{
			float num = skierPrefab.GetComponent<CircleCollider>().radius - radius;
			Vector3 vector2 = vector * num;
			position += vector2;
		}
		else if ((bool)sourceCollider)
		{
			float num2 = skierPrefab.GetComponent<CircleCollider>().radius - sourceCollider.radius;
			Vector3 vector3 = vector * num2;
			position += vector3;
		}
		Player player = PlayerManager.SpawnReplacement(this, skierPrefab, position, rotation);
		if (velocityOverride != Vector3.zero)
		{
			player.Collider.velocity = rotation * velocityOverride;
		}
		else
		{
			player.Collider.velocity = velocity;
		}
		if (level > FlamePowerup.Level.None)
		{
			player.Ignite(level);
		}
		player.PushAttachments(attachments);
		player = OnMount(player);
		if (sendEvents && OnPlayerMount != null && PlayerManager.IsHumanPlayer(player))
		{
			OnPlayerMount(this, player);
		}
		player.Collider.FixedUpdate();
		FollowCamera.Instance.AddPlayerChangeOffset(player.LookAtPos - lookAtPos);
		return player;
	}

	protected virtual Player OnMount(Player player)
	{
		return player;
	}

	private void AddPositionToHistory(Vector3 position)
	{
		m_positionHistoryIndex++;
		if (m_positionHistoryIndex >= m_positionHistory.Length)
		{
			m_positionHistoryIndex = 0;
		}
		m_positionHistory[m_positionHistoryIndex] = position;
		if (m_positionHistoryCount < m_positionHistory.Length)
		{
			m_positionHistoryCount++;
		}
	}

	protected virtual void Awake()
	{
		IsDisabling = false;
		m_collider = GetComponent<CircleCollider>();
		CircleCollider collider = m_collider;
		collider.OnPreCollision = (CircleCollider.OnPreCollisionDelegate)Delegate.Combine(collider.OnPreCollision, new CircleCollider.OnPreCollisionDelegate(OnPreCollision));
		CircleCollider collider2 = m_collider;
		collider2.OnCollision = (CircleCollider.OnCollisionDelegate)Delegate.Combine(collider2.OnCollision, new CircleCollider.OnCollisionDelegate(OnCollision));
		if ((bool)flamePowerupPrefab)
		{

			m_flamePowerup = TransformUtils.Instantiate(flamePowerupPrefab, base.transform);
			m_flamePowerup.Collider = m_collider;
		}
		m_positionHistory = new Vector3[Mathf.CeilToInt(maxFollowDistance / minFollowPositionHistorySeparation)];
		m_minFollowPositionHistorySeparationSqr = minFollowPositionHistorySeparation * minFollowPositionHistorySeparation;
	}

	protected virtual void OnEnable()
	{
		s_allPlayers.Add(this);
		IsDisabling = false;
		m_attachTimer = 0f;
		m_mounting = false;
		m_invulnerabilityTimer = initialInvulnerabilityTime;
		m_positionHistoryCount = 0;
		m_positionHistoryIndex = 0;
	}

	protected virtual void OnDisable()
	{
		s_allPlayers.Remove(this);
		IsDisabling = true;
		DetachAll();
	}

	protected virtual void OnDespawn()
	{
		PopAttachments();
	}

	protected virtual void Start()
	{
	}

	protected virtual void FixedUpdate()
	{
		m_invulnerabilityTimer = Mathf.Max(0f, m_invulnerabilityTimer - Time.fixedDeltaTime);
		m_attachTimer = Mathf.Max(0f, m_attachTimer - Time.fixedDeltaTime);
		if (CanDieByAvalanche())
		{
			Avalanche instance = Avalanche.Instance;
			if ((bool)instance && instance.Contains(base.transform.position))
			{
				SkiGameManager.Instance.OnPlayerHitByAvalanche(this);
			}
		}
	}

	protected virtual void LateUpdate()
	{
		if (m_positionHistory.Length <= 0)
		{
			return;
		}
		if (m_positionHistoryCount > 0)
		{
			float num = Vector3.SqrMagnitude(base.transform.position - m_positionHistory[m_positionHistoryIndex]);
			if (num >= m_minFollowPositionHistorySeparationSqr)
			{
				AddPositionToHistory(base.transform.position);
			}
		}
		else
		{
			m_positionHistoryIndex = 0;
			m_positionHistoryCount = 1;
			m_positionHistory[0] = base.transform.position;
			int num2 = m_positionHistory.Length - 1;
			foreach (Follower follower in m_followers)
			{
				m_positionHistory[num2--] = follower.transform.position;
				m_positionHistoryCount++;
			}
		}
		if (m_followers.Count <= 0 || m_positionHistoryCount <= 0)
		{
			return;
		}
		Vector3 position = base.transform.position;
		Vector3 vector = position;
		float num3 = 0f;
		int num4 = 0;
		int num5 = m_positionHistoryIndex;
		foreach (Follower follower2 in m_followers)
		{
			while (num4 < m_positionHistoryCount)
			{
				float num6 = Vector3.Distance(m_positionHistory[num5], vector);
				if (num3 + num6 >= follower2.followDistance)
				{
					float t = Mathf.Max(0f, (follower2.followDistance - num3) / num6);
					follower2.transform.position = Vector3.Lerp(vector, m_positionHistory[num5], t);
					Vector3 vector2 = m_positionHistory[num5] - vector;
					Vector3 upwards = new Vector3(vector2.y, 0f - vector2.x);
					upwards.Normalize();
					follower2.transform.rotation = Quaternion.LookRotation(Vector3.forward, upwards);
					if ((bool)follower2.chainVisualNode)
					{
						follower2.chainVisualNode.position = (position + follower2.transform.position) * 0.5f;
					}
					num3 = 0f;
					position = follower2.transform.position;
					vector = position;
					break;
				}
				num3 += num6;
				vector = m_positionHistory[num5];
				num4++;
				num5--;
				if (num5 < 0)
				{
					num5 = m_positionHistory.Length - 1;
				}
			}
		}
	}

	protected virtual void OnDrawGizmos()
	{
		if (m_positionHistoryCount <= 0)
		{
			return;
		}
		Gizmos.color = Color.green;
		int num = m_positionHistoryIndex;
		for (int i = 1; i < m_positionHistoryCount; i++)
		{
			int num2 = num - 1;
			if (num2 < 0)
			{
				num2 = m_positionHistory.Length - 1;
			}
			Gizmos.DrawLine(m_positionHistory[num], m_positionHistory[num2]);
			Gizmos.DrawSphere(m_positionHistory[num], 0.1f);
			num = num2;
		}
	}

	[CompilerGenerated]
	private static int _003CTryMount_003Em__1C(Attachment x, Attachment y)
	{
		return y.attachmentGroup.CompareTo(x.attachmentGroup);
	}
}
