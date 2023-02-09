namespace UnityEngine.UDP
{
	public interface IPurchaseListener
	{
		void OnPurchase(PurchaseInfo purchaseInfo);

		void OnPurchaseFailed(string message, PurchaseInfo purchaseInfo);

		void OnPurchaseRepeated(string productId);

		void OnPurchaseConsume(PurchaseInfo purchaseInfo);

		void OnPurchaseConsumeFailed(string message, PurchaseInfo purchaseInfo);

		void OnQueryInventory(Inventory inventory);

		void OnQueryInventoryFailed(string message);
	}
}
