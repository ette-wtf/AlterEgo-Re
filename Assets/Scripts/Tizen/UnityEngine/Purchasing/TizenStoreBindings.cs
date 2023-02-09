using System;

namespace UnityEngine.Purchasing
{
	internal class TizenStoreBindings : INativeTizenStore, INativeStore
	{
		public void SetGroupId(string group)
		{
			throw new NotImplementedException();
		}

		public void SetUnityPurchasingCallback(UnityNativePurchasingCallback AsyncCallback)
		{
			throw new NotImplementedException();
		}

		public void RetrieveProducts(string json)
		{
			throw new NotImplementedException();
		}

		public void Purchase(string productJSON, string developerPayload)
		{
			throw new NotImplementedException();
		}

		public void FinishTransaction(string productJSON, string transactionId)
		{
			throw new NotImplementedException();
		}

		public void RestoreTransactions()
		{
			throw new NotImplementedException();
		}

		public void RefreshAppReceipt()
		{
			throw new NotImplementedException();
		}

		public void AddTransactionObserver()
		{
			throw new NotImplementedException();
		}
	}
}
