using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	public delegate void OnBuyProductResponseDelegate(Item_Product product, bool purchased, string error);

	public static ItemManager Instance;

	public ItemSet[] itemSets;

	public Booster[] boosters;

	public string googlePlayPublicKey;

	public static OnBuyProductResponseDelegate OnBuyProductResponse;

	public static Action<Item> OnItemDownloadStarted;

	public static Action<Item> OnItemDownloadCompleted;

	public static Action<Item, string> OnItemDownloadFailed;

	public static Action<Item> OnItemSelected;

	private Item_Product m_processingProduct;

	public bool CanBuyAnyItem
	{
		get
		{
			ItemSet[] array = itemSets;
			foreach (ItemSet itemSet in array)
			{
				if (itemSet.CanBuyAnyItem)
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
			ItemSet[] array = itemSets;
			foreach (ItemSet itemSet in array)
			{
				if (itemSet.HasNewSelectableItem)
				{
					return true;
				}
			}
			return false;
		}
	}

	public ItemSet GetItemSet(string itemSetName)
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			if (itemSet.name == itemSetName)
			{
				return itemSet;
			}
		}
		return null;
	}

	public bool IsItemCurrent(Item item)
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			if (itemSet.CurrentItem == item)
			{
				return true;
			}
		}
		return false;
	}

	public void MarkCurrentItemsUsed()
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			if ((bool)itemSet.CurrentItem)
			{
				itemSet.CurrentItem.IsNew = false;
				itemSet.CurrentItem.HasBeenUsed = true;
			}
		}
	}

	public int CountPurchasedItems()
	{
		int num = 0;
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			Item[] items = itemSet.items;
			foreach (Item item in items)
			{
				if (item.CurrentCost > 0 && item.Unlocked && !(item is Item_ConsumableProduct))
				{
					num++;
				}
			}
		}
		return num;
	}

	public void DebugReset()
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			itemSet.SelectItem(itemSet.defaultIndex, false);
		}
	}

	public void BuyProduct(Item_Product product)
	{
		m_processingProduct = product;
		StoreManager.Instance.PurchaseProduct(product);
	}

	private Item_Product FindProduct(string productId)
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			Item[] items = itemSet.items;
			foreach (Item item in items)
			{
				if (item is Item_Product)
				{
					Item_Product item_Product = item as Item_Product;
					if (item_Product.productId == productId)
					{
						return item_Product;
					}
				}
			}
		}
		return null;
	}

	private void FinishBuyProduct(Item_Product product, bool success, string error)
	{
		m_processingProduct = null;
		if (OnBuyProductResponse != null)
		{
			OnBuyProductResponse(product, success, error);
		}
	}

	private void OnPurchaseProductSucceeded(string productId)
	{
		Item_Product item_Product = FindProduct(productId);
		if ((bool)item_Product)
		{
			FinishBuyProduct(item_Product, true, string.Empty);
		}
	}

	private void OnPurchaseProductFailed(string reason)
	{
		FinishBuyProduct(m_processingProduct, false, reason);
	}

	public void Load()
	{
		int num = 0;
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			itemSet.Load();
			num += itemSet.GetUnlockedItemCount();
		}
		GameState.SetInt("unlocked_item_count", num);
	}

	public void RefreshCurrentCosts()
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			itemSet.RefreshCurrentCosts();
		}
	}

	public void SelectDependentItems(Item item)
	{
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			itemSet.SelectDependentItems(item);
		}
	}

	public void HandleItemSelected(Item item)
	{
		if (OnItemSelected != null)
		{
			OnItemSelected(item);
		}
	}

	public void HandleItemDownloadFailed(Item item, string error)
	{
		if (OnItemDownloadFailed != null)
		{
			OnItemDownloadFailed(item, error);
		}
	}

	private string[] GetProductIds()
	{
		List<string> list = new List<string>();
		ItemSet[] array = itemSets;
		foreach (ItemSet itemSet in array)
		{
			Item[] items = itemSet.items;
			foreach (Item item in items)
			{
				Item_Product item_Product = item as Item_Product;
				if ((bool)item_Product && !string.IsNullOrEmpty(item_Product.productId))
				{
					list.Add(item_Product.productId);
				}
			}
		}
		return list.ToArray();
	}

	private void OnStateChanged(SkiGameManager.State state)
	{
		switch (state)
		{
		case SkiGameManager.State.Spawning:
		{
			Booster[] array2 = boosters;
			foreach (Booster booster2 in array2)
			{
				booster2.OnGameStarted();
			}
			break;
		}
		case SkiGameManager.State.Restarting:
		{
			Booster[] array = boosters;
			foreach (Booster booster in array)
			{
				booster.OnGameStopped();
			}
			break;
		}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause)
		{
			StoreManager.Instance.LoadProductList(GetProductIds());
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		StoreManager instance = StoreManager.Instance;
		if ((bool)instance)
		{
			instance.OnPurchaseProductSucceeded = (Action<string>)Delegate.Combine(instance.OnPurchaseProductSucceeded, new Action<string>(OnPurchaseProductSucceeded));
			instance.OnPurchaseProductFailed = (Action<string>)Delegate.Combine(instance.OnPurchaseProductFailed, new Action<string>(OnPurchaseProductFailed));
			instance.Init();
			instance.LoadProductList(GetProductIds());
			if ((bool)SkiGameManager.Instance)
			{
				SkiGameManager instance2 = SkiGameManager.Instance;
				instance2.OnStateChanged = (SkiGameManager.OnStateChangedDelegate)Delegate.Combine(instance2.OnStateChanged, new SkiGameManager.OnStateChangedDelegate(OnStateChanged));
			}
		}
	}
}
