using System.Collections.Generic;
using UnityEngine.UDP.Analytics;

namespace UnityEngine.UDP
{
	public class TransactionEventHandler : IPurchaseListener
	{
		private PurchaseInfo _purchaseInfo;

		private int retryTime;

		public TransactionEventHandler(PurchaseInfo purchaseInfo)
		{
			_purchaseInfo = purchaseInfo;
		}

		public void OnPurchase(PurchaseInfo purchaseInfo)
		{
		}

		public void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo)
		{
		}

		public void OnPurchaseRepeated(string productId)
		{
		}

		public void OnPurchaseConsume(PurchaseInfo purchaseInfo)
		{
		}

		public void OnPurchaseConsumeFailed(string message, PurchaseInfo purchaseInfo)
		{
		}

		public void OnQueryInventory(Inventory inventory)
		{
			ProductInfo productInfo = inventory.GetProductInfo(_purchaseInfo.ProductId);
			UdpAnalytics.Transaction(_purchaseInfo.ProductId, productInfo.Price, productInfo.Currency, _purchaseInfo.StorePurchaseJsonString, _purchaseInfo.GameOrderId);
		}

		public void OnQueryInventoryFailed(string message)
		{
			if (retryTime < Utils.RETRY_WAIT_TIME.Length)
			{
				retryTime++;
				MainThreadDispatcher.DispatchDelayJob(Utils.RETRY_WAIT_TIME[retryTime], delegate
				{
					StoreService.QueryInventory(new List<string> { _purchaseInfo.ProductId }, this);
				});
			}
		}
	}
}
