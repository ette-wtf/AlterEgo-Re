namespace UnityEngine.Purchasing
{
	internal interface INativeTizenStore : INativeStore
	{
		void SetUnityPurchasingCallback(UnityNativePurchasingCallback AsyncCallback);

		void SetGroupId(string group);
	}
}
