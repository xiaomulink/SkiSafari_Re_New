using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class StoreManager_GooglePlay : StoreManager
{
	[CompilerGenerated]
	private sealed class _003CCanPurchaseProduct_003Ec__AnonStorey24
	{
		internal Item_Product product;
	}

	public string publicKey;

    private string m_currentProductId;

    public float purchaseProductDelay = 2f;

    public List<string> consumableProductIds = new List<string>();

	private bool m_billingSupported { get { return true; } }


	public override bool IsBillingSupported
	{
		get
		{
            if (m_billingSupported == true)
                return m_billingSupported;
            else
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
		if (m_billingSupported)
		{
		}
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
		/*_003CCanPurchaseProduct_003Ec__AnonStorey24 _003CCanPurchaseProduct_003Ec__AnonStorey = new _003CCanPurchaseProduct_003Ec__AnonStorey24();
		_003CCanPurchaseProduct_003Ec__AnonStorey.product = product;
		*/
		return true;
	}

	public override void PurchaseProduct(Item_Product product)
	{
        Debug.Log(m_billingSupported);
		if (m_billingSupported)
		{
            m_currentProductId = product.productId;
            Invoke("FinishPurchaseProduct", purchaseProductDelay);
        }
		else
		{

            //HandlePurchaseProductFailed("Billing not currently supported");
        }
    }

    private void FinishPurchaseProduct()
    {
        HandlePurchaseProductSucceeded(m_currentProductId);
    }

    public override void OnShopShow()
	{
	}

	public override void OnShopHide()
	{
	}

	private void OnBillingSupported()
	{
		//m_billingSupported = true;
	}

	private void OnBillingNotSupported(string reason)
	{
		//m_billingSupported = true;
	}



	private void OnQueryInventoryFailed(string reason)
	{
	}

	
	private void OnPurchaseFailed(string reason, int response)
	{
		if (reason.ToLower().Contains("cancel"))
		{
			reason = "Purchase cancelled";
		}
		HandlePurchaseProductFailed(reason);
	}


}
