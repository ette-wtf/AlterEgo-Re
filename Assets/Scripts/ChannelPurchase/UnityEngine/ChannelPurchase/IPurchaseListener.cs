namespace UnityEngine.ChannelPurchase
{
	public interface IPurchaseListener
	{
		void OnPurchase(PurchaseInfo purchaseInfo);

		void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo);

		void OnPurchaseRepeated(string productCode);

		void OnReceiptValidate(ReceiptInfo receiptInfo);

		void OnReceiptValidateFailed(string gameOrderId, string message);

		void OnPurchaseConfirm(string gameOrderId);

		void OnPurchaseConfirmFailed(string gameOrderId, string message);
	}
}
