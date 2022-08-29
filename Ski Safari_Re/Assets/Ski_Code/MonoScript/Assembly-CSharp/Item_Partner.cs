using System;
using UnityEngine;

public class Item_Partner : Item
{
	public Player partnerPrefab;

	public float initialSpawnDistance = 50f;

	public Player[] catchupPrefabs;

	public float catchupDistance = 100f;

	public float teleportDistance = 150f;

	public float dismountDistance = 100f;

	private Player m_currentCatchupInstance;

	private void OnStateChanged(SkiGameManager.State newState)
	{
		switch (newState)
		{
		case SkiGameManager.State.Playing:
			if (!PlayerManager.GetPlayer(PlayerManager.PlayerType.AI_1))
			{
				float height = 0f;
				if (Terrain.GetTerrainForLayer(TerrainLayer.Game).GetHeight(initialSpawnDistance, ref height))
				{
                        Debug.LogError("Partner");
					PlayerManager.Spawn(partnerPrefab, new Vector3(initialSpawnDistance, height, 0f), Quaternion.identity, PlayerManager.PlayerType.AI_1);
				}
			}
			break;
		case SkiGameManager.State.Restarting:
			PlayerManager.RemovePlayer(PlayerManager.PlayerType.AI_1);
			break;
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Combine(instance.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnStateChanged));
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		SkiGameManager instance = SkiGameManager.Instance;
		instance.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Remove(instance.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnStateChanged));
	}

	private void Update()
	{
		Player player = PlayerManager.GetPlayer(PlayerManager.PlayerType.AI_1);
		if (!player || !player.gameObject.activeInHierarchy)
		{
			return;
		}
		float num = player.transform.position.x;
		float x = FollowCamera.Instance.transform.position.x;
		float num2 = x - num;
		if (num2 >= teleportDistance)
		{
			num = x - catchupDistance;
			num2 = catchupDistance;
			float height = 0f;
			if (Terrain.GetTerrainForLayer(TerrainLayer.Game).GetHeight(num, ref height))
			{
				player.Collider.Teleport(new Vector3(num, height + player.Collider.radius, 0f));
			}
		}
		if (num2 >= catchupDistance)
		{
			if (!m_currentCatchupInstance || m_currentCatchupInstance.name != player.name)
			{
				Vector3 position = player.transform.position;
				Quaternion rotation = player.transform.rotation;
				Pool.Despawn(player.gameObject);
				Player newPlayerPrefab = catchupPrefabs[UnityEngine.Random.Range(0, catchupPrefabs.Length)];
				m_currentCatchupInstance = PlayerManager.SpawnReplacement(player, newPlayerPrefab, position, rotation);
			}
			return;
		}
		if (0f - num2 > dismountDistance)
		{
			PlayerSkierMounted playerSkierMounted = player as PlayerSkierMounted;
			if ((bool)playerSkierMounted)
			{
				playerSkierMounted.Dismount(null, false, false);
				return;
			}
		}
		Skier component = player.GetComponent<Skier>();
		if (!component)
		{
			return;
		}
		float liftInput = 0f;
		if (!player.Collider.OnGround)
		{
			float height2 = 0f;
			Vector3 normal = Vector3.up;
			if (Terrain.GetTerrainForLayer(TerrainLayer.Game).GetHeightAndNormal(player.transform.position, ref height2, ref normal) && Vector3.Dot(player.transform.right, normal) < 0f)
			{
				liftInput = 1f;
			}
		}
		else
		{
			Hazard_Circle[] array = UnityEngine.Object.FindObjectsOfType(typeof(Hazard_Circle)) as Hazard_Circle[];
			if (array != null)
			{
				Hazard_Circle[] array2 = array;
				foreach (Hazard_Circle hazard_Circle in array2)
				{
					float num3 = hazard_Circle.transform.position.x - num;
					if (num3 > 0f && num3 < 10f)
					{
						liftInput = 1f;
						break;
					}
				}
			}
		}
		component.LiftInput = liftInput;
	}
}
