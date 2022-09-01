using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[Serializable]
public class PoolTable
{
	[SerializeField]
	private string name;

	[SerializeField]
	private GameObject prefab;

	[SerializeField]
	private List<GameObject> inUse = new List<GameObject>();

	[SerializeField]
	private List<GameObject> free = new List<GameObject>();

	[SerializeField]
	private List<GameObject> newObjects = new List<GameObject>();

	private List<GameObject> despawnQueue = new List<GameObject>();

	[CompilerGenerated]
	private static Predicate<GameObject> _003C_003Ef__am_0024cache6;

	public GameObject Prefab
	{
		get
		{
			return prefab;
		}
	}

	public int ActiveCount
	{
		get
		{
			return inUse.Count;
		}
	}

	public int FreeCount
	{
		get
		{
			return free.Count;
		}
	}

	public List<GameObject> ActiveObjects
	{
		get
		{
			return inUse;
		}
	}

	public PoolTable(GameObject prefab)
	{
		name = prefab.name;
		this.prefab = prefab;
	}

	private GameObject CreateNew(Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(prefab, position, rotation);
		gameObject.name = prefab.name;
		if (gameObject.GetComponentInChildren<ParticleSystem>() != null)
		{
			PooledParticles.Apply(gameObject);
		}
		return gameObject;
	}

	public void Preallocate(int count)
	{
		count -= inUse.Count;
		for (count -= free.Count; count > 0; count--)
		{
			GameObject gameObject = CreateNew(Vector3.zero, Quaternion.identity);
			gameObject.SetActive(false);
			gameObject.transform.parent = null;
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			free.Add(gameObject);
		}
	}

	public GameObject Spawn(Vector3 position, Quaternion rotation)
	{
		GameObject gameObject = null;
		if (free.Count == 0)
		{
			gameObject = CreateNew(position, rotation);
		}
		else
		{
			gameObject = free[0];
			free.RemoveAt(0);
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
			gameObject.SetActiveRecursively(true);
			gameObject.hideFlags = HideFlags.None;
			newObjects.Add(gameObject);
		}
		inUse.Add(gameObject);
		return gameObject;
	}

	public void Update()
	{
		while (despawnQueue.Count > 0)
		{
			GameObject gameObject = despawnQueue[0];
			despawnQueue.RemoveAt(0);
			Rigidbody component = gameObject.GetComponent<Rigidbody>();
			if ((bool)component)
			{
				component.velocity = Vector3.zero;
				component.angularVelocity = Vector3.zero;
			}
			inUse.Remove(gameObject);
			free.Add(gameObject);
		}
		List<GameObject> list = newObjects;
		if (_003C_003Ef__am_0024cache6 == null)
		{
			_003C_003Ef__am_0024cache6 = _003CUpdate_003Em__12;
		}
		list.RemoveAll(_003C_003Ef__am_0024cache6);
		newObjects.Clear();
	}

	public void Despawn(GameObject obj)
	{
		if (!despawnQueue.Contains(obj) && obj != null)
		{
			obj.SendMessage("OnDespawn", SendMessageOptions.DontRequireReceiver);
			if ((bool)obj.transform.parent && obj.transform.parent.gameObject.activeInHierarchy)
			{
				obj.transform.parent = null;
			}
			obj.SetActiveRecursively(false);
			obj.hideFlags = HideFlags.HideInHierarchy;
			despawnQueue.Add(obj);
		}
	}

	public void SetPrefab(GameObject newPrefab)
	{
		if (!(prefab == newPrefab))
		{
			prefab = newPrefab;
		}
	}

	public override string ToString()
	{
		return name;
	}

	[CompilerGenerated]
	private static bool _003CUpdate_003Em__12(GameObject m)
	{
		return m == null;
	}
}
