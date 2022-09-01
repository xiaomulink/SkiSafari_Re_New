using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
	[Serializable]
	public class Preallocation
	{
		public GameObject prefab;

		public int count;
	}

	public static Pool Instance;

	public Preallocation[] preallocations;

	[SerializeField]
	private List<PoolTable> items = new List<PoolTable>();

	private Dictionary<GameObject, PoolTable> poolTables = new Dictionary<GameObject, PoolTable>();

	private Dictionary<GameObject, PoolTable> overriddenPoolTables = new Dictionary<GameObject, PoolTable>();

	private Dictionary<GameObject, PoolInstance> poolInstances = new Dictionary<GameObject, PoolInstance>();

	public List<PoolTable> Items
	{
		get
		{
			return items;
		}
	}

	private PoolTable GetOrCreateTable(GameObject prefab)
	{
		PoolTable value;
		if (!poolTables.TryGetValue(prefab, out value))
		{
			value = new PoolTable(prefab);
			poolTables[prefab] = value;
			items.Add(value);
		}
		return value;
	}

	private void DoPreallocate(GameObject prefab, int count)
	{
		GetOrCreateTable(prefab).Preallocate(count);
	}

	private GameObject DoSpawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		base.enabled = true;
		PoolTable orCreateTable = GetOrCreateTable(prefab);
		GameObject gameObject = orCreateTable.Spawn(position, rotation);
		poolInstances[gameObject] = new PoolInstance(gameObject, true, orCreateTable);
		return gameObject;
	}

	private void DoDespawn(GameObject obj)
	{
		PoolInstance value;
		if (poolInstances.TryGetValue(obj, out value))
		{
			PoolTable table = value.Table;
			if (table != null)
			{
				value.InUse = false;
				table.Despawn(obj);
				base.enabled = true;
				return;
			}
		}
		UnityEngine.Object.Destroy(obj);
	}

	private void DoDespawnAll()
	{
		foreach (KeyValuePair<GameObject, PoolInstance> poolInstance in poolInstances)
		{
			PoolInstance value = poolInstance.Value;
			if (value.Table != null)
			{
				value.InUse = false;
				value.Table.Despawn(poolInstance.Key);
			}
		}
		base.enabled = true;
	}

	private void DoReplace(GameObject prefab, GameObject otherPrefab)
	{
		PoolTable value;
		if (!poolTables.TryGetValue(prefab, out value) || value.Prefab == otherPrefab)
		{
			return;
		}
		foreach (KeyValuePair<GameObject, PoolInstance> poolInstance in poolInstances)
		{
			if (poolInstance.Value.Table == value)
			{
				poolInstance.Value.InUse = false;
				value.Despawn(poolInstance.Key);
			}
		}
		Instance.enabled = true;
		PoolTable value2;
		if (overriddenPoolTables.TryGetValue(otherPrefab, out value2))
		{
			overriddenPoolTables.Remove(otherPrefab);
		}
		else
		{
			value2 = new PoolTable(otherPrefab);
			items.Add(value2);
			value2.Preallocate(value.ActiveCount + value.FreeCount);
		}
		overriddenPoolTables[value.Prefab] = value;
		poolTables[prefab] = value2;
	}

	private void Awake()
	{
		Instance = this;
		Preallocation[] array = preallocations;
		foreach (Preallocation preallocation in array)
		{
			DoPreallocate(preallocation.prefab, preallocation.count);
		}
	}

	private void LateUpdate()
	{
		foreach (PoolTable item in items)
		{
			item.Update();
		}
		base.enabled = false;
	}

	public static PoolTable GetTable(GameObject prefab)
	{
		return (!Instance) ? null : Instance.GetOrCreateTable(prefab);
	}

	public static GameObject Spawn(GameObject prefab)
	{
		return (!Instance) ? null : Instance.DoSpawn(prefab, Vector3.zero, Quaternion.identity);
	}

	public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
	{
		return (!Instance) ? null : Instance.DoSpawn(prefab, position, rotation);
	}

	public static void Despawn(GameObject obj)
	{
		if ((bool)Instance)
		{
			Instance.DoDespawn(obj);
		}
	}

	public static T Spawn<T>(T prefab) where T : Component
	{
		if ((bool)Instance)
		{
			GameObject gameObject = Instance.DoSpawn(prefab.gameObject, Vector3.zero, Quaternion.identity);
			if ((bool)gameObject)
			{
				return gameObject.GetComponent<T>();
			}
		}
		return (T)default(T);
	}

	public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
	{
		if ((bool)Instance)
		{
			GameObject gameObject = Instance.DoSpawn(prefab.gameObject, position, rotation);
			if ((bool)gameObject)
			{
				return gameObject.GetComponent<T>();
			}
		}
		return (T)default(T);
	}

	public static void Despawn<T>(T obj) where T : Component
	{
		if ((bool)Instance)
		{
			Instance.DoDespawn(obj.gameObject);
		}
	}

	public static void DespawnAll()
	{
		if ((bool)Instance)
		{
			Instance.DoDespawnAll();
		}
	}

	public static void Replace(GameObject prefab, GameObject otherPrefab)
	{
		if ((bool)Instance)
		{
			Instance.DoReplace(prefab, otherPrefab);
		}
	}

	public static void Revert(GameObject prefab)
	{
		if ((bool)Instance)
		{
			Instance.DoReplace(prefab, prefab);
		}
	}

	public int GetActiveCount(GameObject prefab)
	{
		PoolTable value;
		if (poolTables.TryGetValue(prefab, out value))
		{
			return value.ActiveCount;
		}
		return 0;
	}

	public PoolInstance GetPoolInstance(GameObject obj)
	{
		PoolInstance value;
		poolInstances.TryGetValue(obj, out value);
		return value;
	}
}
