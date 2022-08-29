using UnityEngine;

public class PoolInstance
{
	private GameObject instance;

	private bool inUse;

	private PoolTable table;

	public GameObject Instance
	{
		get
		{
			return instance;
		}
	}

	public PoolTable Table
	{
		get
		{
			return table;
		}
	}

	public bool InUse
	{
		get
		{
			return inUse;
		}
		set
		{
			inUse = value;
		}
	}

	public PoolInstance(GameObject instance, bool inUse, PoolTable table)
	{
		this.instance = instance;
		this.inUse = inUse;
		this.table = table;
	}
}
