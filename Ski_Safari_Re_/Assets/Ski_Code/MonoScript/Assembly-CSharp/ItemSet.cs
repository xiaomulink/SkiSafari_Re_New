using System;
using UnityEngine;

[Serializable]
public class ItemSet
{
	public string name;

	public string displayName;

	public string displayNamePrefix;

	public Item[] items;

	public int defaultIndex;

	public bool canEquipItems = true;

	private int m_currentIndex;

	private Item m_currentItem;

	private bool m_playInstanceSound;

	private int m_instanceIndex = -1;

	private Item m_instance;

	public string CurrentKey
	{
		get
		{
			return "current_" + name;
		}
	}

	public Item CurrentItem
	{
		get
		{
			return m_currentItem;
		}
	}

	public int CurrentIndex
	{
		get
		{
			return m_currentIndex;
		}
	}

	public bool CanBuyAnyItem
	{
		get
		{
			int coinCount = GameState.CoinCount;
			Item[] array = items;
			foreach (Item item in array)
			{
				if (item.CurrentCost > 0 && item.CurrentCost <= coinCount && !item.Unlocked)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool HasNewSelectableItem
	{
		get
		{
			int coinCount = GameState.CoinCount;
			Item[] array = items;
			foreach (Item item in array)
			{
				if (item.CurrentCost > 0 && (item.Unlocked || item.CurrentCost <= coinCount) && item.IsNew)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool HasNewUnlockedItem
	{
		get
		{
			Item[] array = items;
			foreach (Item item in array)
			{
				if (item.Unlocked && item.IsNew)
				{
					return true;
				}
			}
			return false;
		}
	}

	public int UnlockedItemCount
	{
		get
		{
			int num = 0;
			Item[] array = items;
			foreach (Item item in array)
			{
				if (item.Unlocked)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int UnusedItemCount
	{
		get
		{
			int num = 0;
			Item[] array = items;
			foreach (Item item in array)
			{
				if (!item.HasBeenUsed && item.Unlocked)
				{
					num++;
				}
			}
			return num;
		}
	}

	public bool SelectNextItem(bool playSound)
	{
		int num = (m_currentIndex + 1) % items.Length;
		while ((!items[num].Unlocked || !items[num].IsCached) && num != m_currentIndex)
		{
			num = (num + 1) % items.Length;
		}
		if (num != m_currentIndex)
		{
			SelectItem(num, playSound);
			return true;
		}
		return false;
	}

	public void SelectItem(int index, bool playSound)
	{
		if (m_currentIndex == index)
		{
			return;
		}
		int num = Mathf.Clamp(index, -1, items.Length - 1);
		Item item = ((num < 0) ? null : items[num]);
		if (canEquipItems)
		{
			m_currentIndex = num;
			m_currentItem = item;
			if ((bool)m_currentItem)
			{
				m_currentItem.IsNew = false;
				PlayerPrefs.SetString(CurrentKey, m_currentItem.name);
			}
			else
			{
				PlayerPrefs.DeleteKey(CurrentKey);
				ItemManager.Instance.HandleItemSelected(null);
			}
			m_playInstanceSound = playSound;
			LoadItem();
		}
		else
		{
			ItemManager.Instance.HandleItemSelected(item);
		}
	}

	private void LoadItem()
	{
		if (m_currentIndex == m_instanceIndex)
		{
			return;
		}
		m_instanceIndex = m_currentIndex;
        if ((bool)m_currentItem)
        {
          

            if ((bool)SkiGameManager.Instance)
            {
                SkiGameManager.Instance.AddInputLock(m_currentItem.name);
            }
			m_currentItem.PreloadAssets(OnItemPreloaded);
		}
		else
		{
			DestroyInstance();
		}
	}

	public void DestroyInstance()
	{
		if ((bool)m_instance)
		{
			m_instance.UnloadAssets();
			UnityEngine.Object.Destroy(m_instance.gameObject);
			m_instance = null;
		}
	}

	private void CreateInstance()
	{
		m_currentItem.LoadAssets();
		m_instance = UnityEngine.Object.Instantiate(m_currentItem);
		m_instance.name = m_currentItem.name;
		m_instance.transform.parent = ItemManager.Instance.transform;
		if (m_playInstanceSound)
		{
			SoundManager.Instance.PlaySound(m_currentItem.selectSound);
		}
		ItemManager.Instance.HandleItemSelected(m_currentItem);
	}

	private void OnItemPreloaded(bool success, string error)
	{
		if ((bool)SkiGameManager.Instance)
		{
			SkiGameManager.Instance.RemoveInputLock(m_currentItem.name);
		}
		if (success)
		{
			DestroyInstance();
			CreateInstance();
		}
		else
		{
			Item currentItem = m_currentItem;
			SelectNextItem(m_playInstanceSound);
			ItemManager.Instance.HandleItemDownloadFailed(currentItem, error);
		}
	}

	public void Load()
	{
		if (canEquipItems)
		{
			string @string = PlayerPrefs.GetString(CurrentKey);
			m_currentIndex = defaultIndex;
			for (int i = 0; i < items.Length; i++)
			{
				if (items[i].name == @string)
				{
                    if (items[i].Unlocked && items[i].IsCached)
                    {
                        if (items[i].name.Contains("Slope") && SkiGameManager.Instance.isOnline)
                        {
                            if ((bool)SkiGameManager.Instance)
                            {
                                if (SkiGameManager.Instance.isOnline)
                                {
                                    ItemManager.Instance.itemSets[7].defaultIndex = 8;
                                    /*
                                    m_currentIndex = 9;
                                    m_currentItem = SkiGameManager.Instance.OnlineSlope;*/
                                }
                            }
                        }else
                        {
                            m_currentIndex = i;
                            m_currentItem = items[i];
                        }
                    }
                   
					
					break;
				}
			}
                m_currentItem = ((m_currentIndex < 0) ? null : items[m_currentIndex]);
                LoadItem();
           
		}
		Item[] array = items;
		foreach (Item item in array)
		{
			item.Init();
		}
	}

	public int GetUnlockedItemCount()
	{
		int num = 0;
		Item[] array = items;
		foreach (Item item in array)
		{
			if (item.Unlocked)
			{
				num++;
			}
		}
		return num;
	}

	public void RefreshCurrentCosts()
	{
		Item[] array = items;
		foreach (Item item in array)
		{
			item.RefreshCurrentCost();
		}
	}

	public void SelectDependentItems(Item item)
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i].requiredItem == item)
			{
				SelectItem(i, false);
				break;
			}
		}
	}

	public void DebugReset()
	{
		Item[] array = items;
		foreach (Item item in array)
		{
			item.Unlocked = false;
		}
		m_currentIndex = defaultIndex;
		m_currentItem = ((defaultIndex < 0) ? null : items[defaultIndex]);
	}

	public void MigrateToProfile(Profile profile)
	{
		Item[] array = items;
		foreach (Item item in array)
		{
			profile.MigrateInt(item.ViewedKey);
			profile.MigrateInt(item.name);
			Achievement[] achievements = item.achievements;
			foreach (Achievement achievement in achievements)
			{
				achievement.MigrateToProfile(profile);
			}
		}
	}
}
