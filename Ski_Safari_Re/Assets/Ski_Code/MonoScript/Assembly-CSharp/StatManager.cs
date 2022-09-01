using System;
using UnityEngine;

public class StatManager : MonoBehaviour
{
	public static StatManager Instance;

	public StatDescriptor[] statDescriptors;

	private Player m_travellingPlayer;

	private Vector3 m_travelStartPos;

	private Vector3 m_frozenTravelStartPos;

	private int m_transferCount;

	private Player m_jumpingPlayer;

	private bool m_jumping;

	private Vector3 m_jumpStartPos;

	private Vector3 m_jumpEndPos;

	private void OnStateChanged(SkiGameManager.State state)
	{
		switch (state)
		{
		case SkiGameManager.State.Spawning:
		{
			GameState.ResetStatSessionTotals();
			StatDescriptor[] array2 = statDescriptors;
			foreach (StatDescriptor statDescriptor2 in array2)
			{
				statDescriptor2.OnSessionStarted();
			}
			m_travellingPlayer = null;
			m_travelStartPos = SkiGameManager.Instance.StartPos;
			m_frozenTravelStartPos = m_travelStartPos;
			m_transferCount = 0;
			m_jumpingPlayer = null;
			m_jumping = false;
			m_jumpStartPos = (m_jumpEndPos = Vector3.zero);
			break;
		}
		case SkiGameManager.State.Restarting:
		{
			if ((bool)m_travellingPlayer)
			{
				TrackPlayerDistance(m_travellingPlayer);
			}
			StatDescriptor[] array = statDescriptors;
			foreach (StatDescriptor statDescriptor in array)
			{
				if (statDescriptor.Value > statDescriptor.SessionStartValue)
				{
				}
			}
			break;
		}
		}
	}

	private void OnPlayerSpawn(Player player)
	{
		PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
		if ((bool)playerSkierMounted)
		{
			GameState.IncrementStat("mount", playerSkierMounted.mountCategory);
		}
		m_travellingPlayer = player;
		m_travelStartPos = player.transform.position;
	}

	private void OnDieByAvalanche(Player player)
	{
		int amount = Mathf.FloorToInt(SkiGameManager.Instance.CurrentDistance - m_travelStartPos.x);
		GameState.IncrementStat("travel", m_travellingPlayer.Category, amount);
		if (m_travellingPlayer is PlayerSkierFrozen)
		{
			int amount2 = Mathf.FloorToInt(SkiGameManager.Instance.CurrentDistance - m_frozenTravelStartPos.x);
			GameState.IncrementStat("frozen_travel", m_travellingPlayer.Category, amount2);
		}
	}

	private void OnPlayerMount(Player previousPlayer, Player mountedPlayer)
	{
		GameState.IncrementStat("mount", mountedPlayer.Category);
		string category = previousPlayer.Category;
		if (category != "skier")
		{
			string category2 = mountedPlayer.Category;
			if (category == category2)
			{
				m_transferCount++;
			}
			else
			{
				m_transferCount = 1;
			}
			GameState.IncrementStat("transfer", category + "_" + mountedPlayer.Category, m_transferCount);
		}
		else
		{
			m_transferCount = 0;
		}
		TrackPlayerDistance(mountedPlayer);
	}

	private void OnPlayerDismount(PlayerSkierMounted previousPlayer, Player player)
	{
		m_transferCount = 0;
	}

	private void OnPlayerAttach(Player player, Attachment attachment)
	{
		GameState.IncrementStat("attach", attachment.attachmentCategory);
	}

	private void OnPlayerTakeDamage(Player previousPlayer, Player player)
	{
		TrackPlayerDistance(player);
		if (player is PlayerPiledrive)
		{
			GameState.IncrementStat("piledrive", string.Empty);
		}
	}

	private void OnPlayerBreakLine(Player player, GeometryUtils.ContactInfo contactInfo)
	{
		GameState.IncrementStat("breakline", contactInfo.collider.category);
	}

	private void OnPlayerHitHazard(Player previousPlayer, Player player, Hazard hazard)
	{
		GameState.IncrementStat("hithazard", hazard.category);
	}

	private void OnPlayerFreeze(Player previousPlayer, PlayerSkierFrozen player)
	{
		GameState.IncrementStat("freeze", player.mountCategory);
	}

	private void OnPlayerGetUp(PlayerSkierBellyslide previousPlayer, Player player)
	{
		TrackPlayerDistance(player);
	}

	private void OnPlayerGetUpFromPiledrive(PlayerPiledrive previousPlayer, Player player)
	{
		TrackPlayerDistance(player);
	}

	private void OnBackflipLanded(Player player, int consecutiveCount)
	{
		GameState.IncrementStat("backflip", player.Category, consecutiveCount);
	}

	private void OnCollectableBoost(Player player, Collectable collectable)
	{
		GameState.IncrementStat("collectable_boost", collectable.category);
	}

	private void OnGlideIgnite(Player player, FlamePowerup.Level glideLevel)
	{
		GameState.IncrementStat("glide_boost", player.Category);
	}

	private void OnLineSlide(Player player, LineCollider lineCollider)
	{
		GameState.IncrementStat("slide", lineCollider.category);
	}

	private void OnRideAvalanche(Player player)
	{
		GameState.IncrementStat("ride_avalanche", player.Category);
	}

	private void OnPlayerOpenDoor(Player player, DoorTrigger doorTrigger)
	{
		if (SkiGameManager.Instance.CurrentDistance >= 10f)
		{
			GameState.IncrementStat("passthrough", player.Category);
		}
	}

	private void TrackPlayerDistance(Player player)
	{
		Vector3 position = player.transform.position;
		int amount = Mathf.FloorToInt(position.x - m_travelStartPos.x);
		GameState.IncrementStat("travel", m_travellingPlayer.Category, amount);
		if (player.Category != m_travellingPlayer.Category)
		{
			m_travelStartPos = position;
		}
		if (player is PlayerSkierFrozen)
		{
			if (!m_travellingPlayer || !(m_travellingPlayer is PlayerSkierFrozen))
			{
				m_frozenTravelStartPos = position;
			}
		}
		else if ((bool)m_travellingPlayer && m_travellingPlayer is PlayerSkierFrozen)
		{
			int amount2 = Mathf.FloorToInt(position.x - m_frozenTravelStartPos.x);
			GameState.IncrementStat("frozen_travel", m_travellingPlayer.Category, amount2);
		}
		m_travellingPlayer = player;
	}

	private void StartJump(Player player)
	{
		m_jumping = true;
		m_jumpStartPos = (m_jumpEndPos = player.transform.position);
		m_jumpingPlayer = player;
	}

	private void FinishJump()
	{
		m_jumping = false;
		int num = Mathf.FloorToInt(m_jumpEndPos.x - m_jumpStartPos.x);
		if (num > 0)
		{
			GameState.IncrementStat("jump", m_jumpingPlayer.Category, num);
		}
		m_jumpingPlayer = null;
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Combine(instance.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnStateChanged));
		SkiGameManager instance2 = SkiGameManager.Instance;
		instance2.OnPlayerSpawn = (Player.SimplePlayerDelegate)Delegate.Combine(instance2.OnPlayerSpawn, new Player.SimplePlayerDelegate(OnPlayerSpawn));
		SkiGameManager instance3 = SkiGameManager.Instance;
		instance3.OnDieByAvalanche = (Player.SimplePlayerDelegate)Delegate.Combine(instance3.OnDieByAvalanche, new Player.SimplePlayerDelegate(OnDieByAvalanche));
		Player.OnPlayerMount = (Player.OnMountDelegate)Delegate.Combine(Player.OnPlayerMount, new Player.OnMountDelegate(OnPlayerMount));
		Player.OnPlayerDismount = (Player.OnDisountDelegate)Delegate.Combine(Player.OnPlayerDismount, new Player.OnDisountDelegate(OnPlayerDismount));
		Player.OnPlayerAttach = (Player.OnAttachDelegate)Delegate.Combine(Player.OnPlayerAttach, new Player.OnAttachDelegate(OnPlayerAttach));
		Player.OnPlayerTakeDamage = (Player.OnTakeDamageDelegate)Delegate.Combine(Player.OnPlayerTakeDamage, new Player.OnTakeDamageDelegate(OnPlayerTakeDamage));
		Player.OnPlayerBreakLine = (Player.OnCollisionDelegate)Delegate.Combine(Player.OnPlayerBreakLine, new Player.OnCollisionDelegate(OnPlayerBreakLine));
		Player.OnPlayerHitHazard = (Player.OnHitHazardDelegate)Delegate.Combine(Player.OnPlayerHitHazard, new Player.OnHitHazardDelegate(OnPlayerHitHazard));
		Player.OnPlayerFreeze = (Player.OnFreezeDelegate)Delegate.Combine(Player.OnPlayerFreeze, new Player.OnFreezeDelegate(OnPlayerFreeze));
		PlayerSkierBellyslide.OnPlayerGetUp = (PlayerSkierBellyslide.OnPlayerGetUpDelegate)Delegate.Combine(PlayerSkierBellyslide.OnPlayerGetUp, new PlayerSkierBellyslide.OnPlayerGetUpDelegate(OnPlayerGetUp));
		PlayerPiledrive.OnPlayerGetUp = (PlayerPiledrive.OnPlayerGetUpDelegate)Delegate.Combine(PlayerPiledrive.OnPlayerGetUp, new PlayerPiledrive.OnPlayerGetUpDelegate(OnPlayerGetUpFromPiledrive));
		PlayerSkierFrozen.OnPlayerBreakOut = (Player.SimplePlayerDelegate)Delegate.Combine(PlayerSkierFrozen.OnPlayerBreakOut, new Player.SimplePlayerDelegate(TrackPlayerDistance));
		Stunt_Backflip.OnBackflipLanded = (Stunt_Backflip.OnBackflipLandedDelegate)Delegate.Combine(Stunt_Backflip.OnBackflipLanded, new Stunt_Backflip.OnBackflipLandedDelegate(OnBackflipLanded));
		Stunt_Collect.OnCollectableBoost = (Stunt_Collect.OnCollectableBoostDelegate)Delegate.Combine(Stunt_Collect.OnCollectableBoost, new Stunt_Collect.OnCollectableBoostDelegate(OnCollectableBoost));
		Stunt_Glide.OnGlideIgnite = (Stunt_Glide.OnGlideIgniteDelegate)Delegate.Combine(Stunt_Glide.OnGlideIgnite, new Stunt_Glide.OnGlideIgniteDelegate(OnGlideIgnite));
		Stunt_LineSlide.OnLineSlide = (Stunt_LineSlide.OnLineSlideDelegate)Delegate.Combine(Stunt_LineSlide.OnLineSlide, new Stunt_LineSlide.OnLineSlideDelegate(OnLineSlide));
		Stunt_RideAvalanche.OnRideAvalanche = (Player.SimplePlayerDelegate)Delegate.Combine(Stunt_RideAvalanche.OnRideAvalanche, new Player.SimplePlayerDelegate(OnRideAvalanche));
		DoorTrigger.OnPlayerOpenDoor = (DoorTrigger.OnPlayerOpenDoorDelegate)Delegate.Combine(DoorTrigger.OnPlayerOpenDoor, new DoorTrigger.OnPlayerOpenDoorDelegate(OnPlayerOpenDoor));
	}

	private void FixedUpdate()
	{
		Player instance = Player.Instance;
		if (!instance)
		{
			if (m_jumping)
			{
				FinishJump();
			}
		}
		else if (!m_jumpingPlayer)
		{
			if (!instance.Collider.OnGround)
			{
				StartJump(instance);
			}
		}
		else if (m_jumpingPlayer.Category != instance.Category)
		{
			if (m_jumping)
			{
				FinishJump();
			}
			if (!instance.Collider.OnGround)
			{
				StartJump(instance);
			}
		}
		else
		{
			if (!instance)
			{
				return;
			}
			if (m_jumping)
			{
				if (!instance.Collider.OnGround)
				{
					m_jumpEndPos = instance.transform.position;
				}
				else
				{
					FinishJump();
				}
			}
			else if (!instance.Collider.OnGround)
			{
				StartJump(instance);
			}
		}
	}
}
