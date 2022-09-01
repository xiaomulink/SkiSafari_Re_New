using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
	[Serializable]
	public class Item
	{
		public SpawnParams spawnParams;

		public int weighting = 1;
	}

	public Item[] items;

	public SpawnManager.SpawnFlags spawnFlags = SpawnManager.SpawnFlags.IgnoreAll;

	private List<Item> m_shuffledItems;

	private void OnEnable()
	{
		if (!SkiGameManager.Instance || !SkiGameManager.Instance.Started)
		{
			return;
		}
		if (m_shuffledItems == null)
		{
			m_shuffledItems = new List<Item>();
			Item[] array = items;
			foreach (Item item in array)
			{
				for (int j = 0; j < item.weighting; j++)
				{
					m_shuffledItems.Add(item);
				}
			}
		}
		m_shuffledItems.Shuffle();
		foreach (Item shuffledItem in m_shuffledItems)
		{
			GameObject gameObject = SpawnManager.ForegroundInstance.ManualSpawn(shuffledItem.spawnParams, base.transform.position, base.transform.rotation, spawnFlags);
			if ((bool)gameObject)
			{
				CircleCollider component = gameObject.GetComponent<CircleCollider>();
				if ((bool)component)
				{
					component.enabled = false;
				}
				Skier component2 = gameObject.GetComponent<Skier>();
				if ((bool)component2)
				{
					component2.enabled = false;
				}
				break;
			}
		}
	}
}
