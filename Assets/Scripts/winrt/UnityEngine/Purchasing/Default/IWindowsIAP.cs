using System.Collections.Generic;

namespace UnityEngine.Purchasing.Default
{
	public interface IWindowsIAP
	{
		void BuildDummyProducts(List<WinProductDescription> products);

		void Initialize(IWindowsIAPCallback callback);

		void RetrieveProducts(bool retryIfOffline);

		void Purchase(string productId);

		void FinaliseTransaction(string transactionId);
	}
}
