using UnityEngine;

public abstract class Achievement : MonoBehaviour
{
	public string description;

	public float descriptionTextScale = 1f;

	public AchievementFlags requiredFlags;

	public int requiredLevel;

	public Item requiredItem;

	public Item[] otherRequiredItems;

	public string ViewedKey
	{
		get
		{
			return base.name + "_viewed";
		}
	}

	public AchievementManager Manager { get; set; }

	public bool IsNew
	{
		get
		{
			return !GameState.HasKey(ViewedKey);
		}
		set
		{
			if (value)
			{
				GameState.DeleteKey(ViewedKey);
			}
			else if (IsNew)
			{
				GameState.SetInt(ViewedKey, 1);
			}
		}
	}

	public bool IsDone
	{
		get
		{
			return GameState.GetInt(ViewedKey) > 1;
		}
		set
		{
			if (value)
			{
				GameState.SetInt(ViewedKey, 2);
			}
			else if (IsDone)
			{
				GameState.SetInt(ViewedKey, 1);
			}
		}
	}

	public virtual bool HasBasicRequirements
	{
		get
		{
			if (LevelManager.Instance.CurrentLevel < requiredLevel)
			{
				return false;
			}
			if (requiredFlags != 0 && (requiredFlags & AchievementManager.Instance.AchievementFlags) != requiredFlags)
			{
				return false;
			}
			if ((bool)requiredItem && !ItemManager.Instance.IsItemCurrent(requiredItem))
			{
				return false;
			}
			if (otherRequiredItems.Length > 0)
			{
				Item[] array = otherRequiredItems;
				foreach (Item item in array)
				{
					if (!ItemManager.Instance.IsItemCurrent(item))
					{
						return false;
					}
				}
			}
			return true;
		}
	}

	public abstract bool IsComplete { get; }

	public void Reset()
	{
		GameState.DeleteKey(base.name);
	}

	public void Load()
	{
		OnLoad();
	}

	public void Save()
	{
		IsNew = false;
		OnSave();
	}

	public virtual void Complete()
	{
		OnComplete();
		Save();
		if (base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(false);
			if ((bool)Manager)
			{
				Manager.OnComplete(this);
			}
		}
	}

	public abstract void MigrateToProfile(Profile profile);

	protected abstract void OnComplete();

	protected abstract void OnLoad();

	protected abstract void OnSave();

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected static bool CheckMountCategory(Player player, string requiredMountCategory)
	{
		if (string.IsNullOrEmpty(requiredMountCategory))
		{
			return true;
		}
		return player.Category == requiredMountCategory;
	}

	protected static bool CheckMountCategory(PlayerInteractionTrigger trigger, string requiredMountCategory)
	{
		if (string.IsNullOrEmpty(requiredMountCategory))
		{
			return true;
		}
		return trigger.category == requiredMountCategory;
	}

	protected static bool CheckAttachmentCategory(Player player, string requiredAttachmentCategory)
	{
		if (string.IsNullOrEmpty(requiredAttachmentCategory))
		{
			return true;
		}
		return player.HasAttachment(requiredAttachmentCategory);
	}

	protected static bool CheckPlayerType(Player player, AchievementPlayerType requiredPlayerType)
	{
		switch (requiredPlayerType)
		{
		case AchievementPlayerType.Any:
			return true;
		case AchievementPlayerType.Snowball:
			return player is PlayerSnowball;
		default:
			return false;
		}
	}
}
