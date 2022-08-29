using System;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	public int cost;

	public int discountCost = -1;

	public int discountYear = -1;

	public int discountMonthStart;

	public int discountDayStart;

	public int discountMonthEnd;

	public int discountDayEnd;

	public int requiredLevel;

	public Item requiredItem;

	public bool lockedUntilUpdate;

	public string displayName;

	public string description;

	public float descriptionTextScale = 1f;

	public string iconTextureName = "gui_item_";

	public string bannerTextureName;

	public string iOSBannerUrl;

	public string iOSProductId;

	public string googlePlayBannerUrl;

	public string amazonBannerUrl;

	public string nookBannerUrl;

	public Sound selectSound;

	public Achievement[] achievements;

	public AchievementFlags achievementFlags;

	public GUIReward guiReward;

	public GUIShopPopup guiShopPopup;

	public List<Bundle> bundles;

	private int m_currentCost;

	public string ViewedKey
	{
		get
		{
			return base.name + "_viewed";
		}
	}

	public string UsedKey
	{
		get
		{
			return base.name + "_used";
		}
	}

	public string NotificationKey
	{
		get
		{
			return base.name + "_notification";
		}
	}

	public int CurrentCost
	{
		get
		{
			return m_currentCost;
		}
	}

	public bool IsDiscounted
	{
		get
		{
			return m_currentCost == discountCost;
		}
	}

	public bool IsNew
	{
		get
		{
			return GameState.GetInt(ViewedKey, -1) < achievements.Length;
		}
		set
		{
			if (value)
			{
				GameState.DeleteKey(ViewedKey);
			}
			else if (IsNew)
			{
				GameState.SetInt(ViewedKey, achievements.Length);
			}
		}
	}

	public bool HasBeenUsed
	{
		get
		{
			return GameState.HasKey(UsedKey);
		}
		set
		{
			if (value)
			{
				GameState.SetInt(UsedKey, 1);
			}
			else if (HasBeenUsed)
			{
				GameState.DeleteKey(UsedKey);
			}
		}
	}

	public virtual bool Unlocked
	{
		get
		{
			if (lockedUntilUpdate)
			{
				return false;
			}
			if (GameState.GetInt(base.name) != 0)
			{
				return true;
			}
			if (CurrentCost > 0)
			{
				return false;
			}
			if (requiredLevel > LevelManager.Instance.CurrentLevel)
			{
				return false;
			}
			if ((bool)requiredItem && !requiredItem.Unlocked)
			{
				return false;
			}
			return true;
		}
		set
		{
			if (value)
			{
				GameState.SetInt(base.name, 1);
			}
			else
			{
				GameState.DeleteKey(base.name);
			}
		}
	}

	public virtual bool IsCached
	{
		get
		{
			return true;
		}
	}

	public string BannerURL
	{
		get
		{
			switch (Application.platform)
			{
			default:
				return iOSBannerUrl;
			case RuntimePlatform.Android:
				return googlePlayBannerUrl;
			}
		}
	}

	public void RefreshCurrentCost()
	{
		m_currentCost = cost;
		if (Unlocked)
		{
			if (discountCost >= 0)
			{
				ClearDiscountNotification();
			}
		}
		else
		{
			if (discountCost < 0)
			{
				return;
			}
			ClearDiscountNotification();
			DateTime now = DateTime.Now;
			int num = ((discountYear != -1) ? discountYear : now.Year);
			int year = num;
			if (discountMonthStart > discountMonthEnd || (discountMonthStart == discountMonthEnd && discountDayStart > discountDayEnd))
			{
				year = num + 1;
			}
			DateTime dateTime = new DateTime(num, discountMonthStart, discountDayStart);
			DateTime dateTime2 = new DateTime(year, discountMonthEnd, discountDayEnd);
			if (now > dateTime2)
			{
				if (discountYear != -1)
				{
					ClearDiscountNotification();
					return;
				}
				dateTime = dateTime.AddYears(1);
				dateTime2 = dateTime2.AddYears(1);
			}
			if (now < dateTime)
			{
				ScheduleDiscountNotification(dateTime, now);
			}
			else
			{
				m_currentCost = discountCost;
			}
		}
	}

	public virtual void DownloadAssets(Action<bool, string> callback)
	{
		callback(true, string.Empty);
	}

	public virtual void PreloadAssets(Action<bool, string> callback)
	{
		callback(true, string.Empty);
	}

	public virtual void LoadAssets()
	{
	}

	public virtual void UnloadAssets()
	{
	}

	public virtual void Init()
	{
		m_currentCost = cost;
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	private void ClearDiscountNotification()
	{
		string notificationKey = NotificationKey;
		if (PlayerPrefs.HasKey(notificationKey))
		{
			int @int = PlayerPrefs.GetInt(notificationKey);
			PlayerPrefs.DeleteKey(notificationKey);
		}
	}

	private void ScheduleDiscountNotification(DateTime discountDateStart, DateTime currentDate)
	{
		PlayerPrefs.SetInt(NotificationKey, 12);
	}
}
