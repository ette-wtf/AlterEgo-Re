using UnityEngine.Store;

namespace UnityEngine.ChannelPurchase
{
	public class PurchaseForwardCallback : AndroidJavaProxy
	{
		private IPurchaseListener purchaseListener;

		public PurchaseForwardCallback(IPurchaseListener purchaseListener)
			: base("com.unity.channel.sdk.PurchaseCallback")
		{
			this.purchaseListener = purchaseListener;
		}

		public void onPurchaseFinished(int resultCode, AndroidJavaObject jo)
		{
			PurchaseInfo purchaseInfo = new PurchaseInfo();
			if (jo == null)
			{
				purchaseInfo = null;
			}
			else
			{
				purchaseInfo.productCode = jo.Call<string>("getProductCode", new object[0]);
				purchaseInfo.gameOrderId = jo.Call<string>("getGameOrderId", new object[0]);
				purchaseInfo.orderQueryToken = jo.Call<string>("getOrderQueryToken", new object[0]);
				purchaseInfo.developerPayload = jo.Call<string>("getDeveloperPayload", new object[0]);
			}
			if (purchaseListener == null)
			{
				return;
			}
			if (resultCode == ResultCode.SDK_PURCHASE_SUCCESS)
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnPurchase(purchaseInfo);
				});
			}
			else if (resultCode == ResultCode.SDK_PURCHASE_REPEAT)
			{
				string productCode = jo.Call<string>("getProductCode", new object[0]);
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnPurchaseRepeated(productCode);
				});
			}
			else
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnPurchaseFailed("Purchase Failed: " + resultCode, purchaseInfo);
				});
			}
		}

		public void onReceiptValidated(int resultCode, AndroidJavaObject jo)
		{
			if (purchaseListener == null)
			{
				return;
			}
			if (resultCode == ResultCode.SDK_RECEIPT_VALIDATE_SUCCESS)
			{
				ReceiptInfo receiptInfo = new ReceiptInfo();
				receiptInfo.gameOrderId = jo.Call<string>("getGameOrderId", new object[0]);
				receiptInfo.signData = jo.Call<string>("getSignData", new object[0]);
				receiptInfo.signature = jo.Call<string>("getSignature", new object[0]);
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnReceiptValidate(receiptInfo);
				});
			}
			else
			{
				string orderId = null;
				if (jo != null)
				{
					orderId = jo.Call<string>("getGameOrderId", new object[0]);
				}
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnReceiptValidateFailed(orderId, "Receipt Validate Failed: " + resultCode);
				});
			}
		}

		public void onPurchaseConfirmed(int resultCode, string gameOrderId)
		{
			if (purchaseListener == null)
			{
				return;
			}
			if (resultCode == ResultCode.SDK_CONFIRM_PURCHASE_SUCCESS)
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnPurchaseConfirm(gameOrderId);
				});
			}
			else
			{
				MainThreadDispatcher.RunOnMainThread(delegate
				{
					purchaseListener.OnPurchaseConfirmFailed(gameOrderId, "Purchase Confirm Failed: " + resultCode);
				});
			}
		}
	}
}
