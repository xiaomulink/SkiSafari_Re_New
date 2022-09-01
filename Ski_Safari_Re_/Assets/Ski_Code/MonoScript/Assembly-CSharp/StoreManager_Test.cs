public class StoreManager_Test : StoreManager
{
	public float purchaseProductDelay = 2f;

	public float restoreTransactionsDelay = 2f;

	private string m_currentProductId;

	public override bool IsBillingSupported
	{
		get
		{
			return true;
		}
	}

	public override bool IsRestoreTransactionsSupported
	{
		get
		{
			return true;
		}
	}

	public override void LoadProductList(string[] productIds)
	{
	}

	public override string GetProductFormattedCost(Item_Product product)
	{
		if (product.cost > 0)
		{
			int num = product.cost / 100;
			return string.Format("{0}{1} ", num, product.cost - num * 100);
		}
		return string.Empty;
	}

	public override bool CanPurchaseProduct(Item_Product product)
	{
		return true;
	}

	public override void PurchaseProduct(Item_Product product)
	{
		m_currentProductId = product.productId;
		Invoke("FinishPurchaseProduct", purchaseProductDelay);
	}

	public override void RestoreTransactions()
	{
		Invoke("FinishRestoreTransactions", restoreTransactionsDelay);
	}

	private void FinishPurchaseProduct()
	{
		HandlePurchaseProductSucceeded(m_currentProductId);
	}

	private void FinishRestoreTransactions()
	{
		HandleRestoreTransactionsSucceeded();
		ItemSet[] itemSets = ItemManager.Instance.itemSets;
		foreach (ItemSet itemSet in itemSets)
		{
			Item[] items = itemSet.items;
			foreach (Item item in items)
			{
				Item_NonConsumableProduct item_NonConsumableProduct = item as Item_NonConsumableProduct;
				if ((bool)item_NonConsumableProduct)
				{
					HandlePurchaseProductSucceeded(item_NonConsumableProduct.productId);
				}
			}
		}
	}
}
