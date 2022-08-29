using System;
using UnityEngine;

public abstract class StoreManager : MonoBehaviour
{
	public static StoreManager Instance;

	public Action<string> OnPurchaseProductSucceeded;

	public Action<string> OnPurchaseProductFailed;

	public Action OnRestoreTransactionsSucceeded;

	public Action<string> OnRestoreTransactionsFailed;

	public abstract bool IsBillingSupported {
        get;
    }

	public abstract bool IsRestoreTransactionsSupported { get; }

	public abstract void LoadProductList(string[] productIds);

	public abstract string GetProductFormattedCost(Item_Product product);

	public abstract bool CanPurchaseProduct(Item_Product product);

	public abstract void PurchaseProduct(Item_Product product);

	public virtual void RestoreTransactions()
	{
		HandleRestoreTransactionsFailed("Not implemented");
	}

	public virtual void OnShopShow()
	{
	}

	public virtual void OnShopHide()
	{
	}

	public void Init()
	{
		if ((bool)GUIShop.Instance)
		{
			GUIShop instance = GUIShop.Instance;
			instance.OnShow = (Action)Delegate.Combine(instance.OnShow, new Action(OnShopShow));
			GUIShop instance2 = GUIShop.Instance;
			instance2.OnHide = (Action)Delegate.Combine(instance2.OnHide, new Action(OnShopHide));
		}
	}

	protected void HandlePurchaseProductSucceeded(string productId)
	{
		if (OnPurchaseProductSucceeded != null)
		{
			OnPurchaseProductSucceeded(productId);
		}
	}

	protected void HandlePurchaseProductFailed(string reason)
	{
		if (OnPurchaseProductFailed != null)
		{
			OnPurchaseProductFailed(reason);
		}
	}

	protected void HandleRestoreTransactionsSucceeded()
	{
		if (OnRestoreTransactionsSucceeded != null)
		{
			OnRestoreTransactionsSucceeded();
		}
	}

	protected void HandleRestoreTransactionsFailed(string reason)
	{
		if (OnRestoreTransactionsFailed != null)
		{
			OnRestoreTransactionsFailed(reason);
		}
	}

	private void Awake()
	{
		Instance = this;
	}
}
