using System.Collections.Generic;

namespace UnityEngine.UDP
{
	public class Inventory
	{
		private readonly Dictionary<string, PurchaseInfo> _purchaseDictionary = new Dictionary<string, PurchaseInfo>();

		private readonly Dictionary<string, ProductInfo> _productDictionary = new Dictionary<string, ProductInfo>();

		public PurchaseInfo GetPurchaseInfo(string productId)
		{
			PurchaseInfo value = new PurchaseInfo();
			if (_purchaseDictionary.TryGetValue(productId, out value))
			{
				return value;
			}
			return null;
		}

		public ProductInfo GetProductInfo(string productId)
		{
			ProductInfo value;
			if (_productDictionary.TryGetValue(productId, out value))
			{
				return value;
			}
			return null;
		}

		public bool HasPurchase(string productId)
		{
			return _purchaseDictionary.ContainsKey(productId);
		}

		public bool HasProduct(string productId)
		{
			return _productDictionary.ContainsKey(productId);
		}

		public IDictionary<string, PurchaseInfo> GetPurchaseDictionary()
		{
			return new Dictionary<string, PurchaseInfo>(_purchaseDictionary);
		}

		public IDictionary<string, ProductInfo> GetProductDictionary()
		{
			return new Dictionary<string, ProductInfo>(_productDictionary);
		}

		public IList<ProductInfo> GetProductList()
		{
			List<ProductInfo> list = new List<ProductInfo>();
			foreach (KeyValuePair<string, ProductInfo> item in _productDictionary)
			{
				list.Add(item.Value);
			}
			return list;
		}

		public List<PurchaseInfo> GetPurchaseList()
		{
			List<PurchaseInfo> list = new List<PurchaseInfo>();
			foreach (KeyValuePair<string, PurchaseInfo> item in _purchaseDictionary)
			{
				list.Add(item.Value);
			}
			return list;
		}

		internal void AddProduct(ProductInfo productInfo)
		{
			_productDictionary.Add(productInfo.ProductId, productInfo);
		}

		internal void AddPurchase(PurchaseInfo purchaseInfo)
		{
			_purchaseDictionary.Add(purchaseInfo.ProductId, purchaseInfo);
		}
	}
}
