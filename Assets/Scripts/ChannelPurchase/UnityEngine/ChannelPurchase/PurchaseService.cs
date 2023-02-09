namespace UnityEngine.ChannelPurchase
{
	public class PurchaseService
	{
		private static AndroidJavaClass serviceClass = new AndroidJavaClass("com.unity.channel.sdk.ChannelService");

		public static void Purchase(string productCode, string gameOrderId, IPurchaseListener listener, string developerPayload = null)
		{
			PurchaseForwardCallback purchaseForwardCallback = new PurchaseForwardCallback(listener);
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("com.unity.channel.sdk.PurchaseInfo");
			androidJavaObject.Set("productCode", productCode);
			androidJavaObject.Set("gameOrderId", gameOrderId);
			if (!string.IsNullOrEmpty(developerPayload))
			{
				androidJavaObject.Set("developerPayload", developerPayload);
			}
			serviceClass.CallStatic("purchase", androidJavaObject, purchaseForwardCallback);
		}

		public static void ValidateReceipt(string gameOrderId, IPurchaseListener listener)
		{
			PurchaseForwardCallback purchaseForwardCallback = new PurchaseForwardCallback(listener);
			serviceClass.CallStatic("validateReceipt", gameOrderId, purchaseForwardCallback);
		}

		public static void ConfirmPurchase(string gameOrderId, IPurchaseListener listener)
		{
			PurchaseForwardCallback purchaseForwardCallback = new PurchaseForwardCallback(listener);
			serviceClass.CallStatic("confirmPurchase", gameOrderId, purchaseForwardCallback);
		}
	}
}
