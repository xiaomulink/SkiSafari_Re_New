public class StoreManager_None : StoreManager
{
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
			return false;
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
		HandlePurchaseProductFailed("IAP is not supported on this device");
	}
}
