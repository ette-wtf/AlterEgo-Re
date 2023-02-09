namespace UnityEngine.Purchasing.Default
{
	public interface IWindowsIAPCallback
	{
		void OnProductListReceived(WinProductDescription[] winProducts);

		void OnProductListError(string message);

		void OnPurchaseSucceeded(string productId, string receipt, string transactionId);

		void OnPurchaseFailed(string productId, string error);

		void logError(string error);

		void log(string message);
	}
}
