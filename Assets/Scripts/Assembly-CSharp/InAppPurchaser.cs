using System;
using System.Collections;
using App;
using UnityEngine;
using UnityEngine.Purchasing;

public class InAppPurchaser : MonoBehaviour, IStoreListener
{
	private static InAppPurchaser Instance;

	private IStoreController m_StoreController;

	private IExtensionProvider m_StoreExtensionProvider;

	private bool IsWaiting;

	private Action CallbackMethod;

	public static readonly string[] ProductIDList = PurchasingItem.ITEM_LIST;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			StartCoroutine(InitializePurchasing());
		}
	}

	public IEnumerator InitializePurchasing()
	{
		if (IsInitialized())
		{
			yield break;
		}
		if (!IsWaiting)
		{
			StandardPurchasingModule standardPurchasingModule = StandardPurchasingModule.Instance();
			standardPurchasingModule.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
			ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(standardPurchasingModule);
			string aPP_BUNDLE_ID = BuildInfo.APP_BUNDLE_ID;
			string[] productIDList = ProductIDList;
			foreach (string text in productIDList)
			{
				configurationBuilder.AddProduct(text, ProductType.NonConsumable, new IDs
				{
					{
						aPP_BUNDLE_ID + "." + text,
						"AppleAppStore"
					},
					{
						aPP_BUNDLE_ID + "." + text,
						"GooglePlay"
					}
				});
			}
			UnityPurchasing.Initialize(this, configurationBuilder);
			IsWaiting = true;
		}
		yield return null;
		float waitTime = 0f;
		while (!IsInitialized())
		{
			waitTime += Time.unscaledDeltaTime;
			if (waitTime > 60f)
			{
				break;
			}
			yield return null;
		}
	}

	private bool IsInitialized()
	{
		if (m_StoreController != null)
		{
			return m_StoreExtensionProvider != null;
		}
		return false;
	}

	public static IEnumerator BuyProductID(string productId, Action callback)
	{
		if (!(Instance == null))
		{
			Instance.CallbackMethod = callback;
			yield return Instance.InitializePurchasing();
			Product product = Instance.m_StoreController.products.WithID(productId);
			if (Instance.IsInitialized() && product != null && product.availableToPurchase)
			{
				Instance.m_StoreController.InitiatePurchase(product);
			}
			else
			{
				Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
			}
		}
	}

	public static IEnumerator RestorePurchases(Action callback)
	{
		if (Instance == null)
		{
			yield break;
		}
		Instance.CallbackMethod = callback;
		yield return Instance.InitializePurchasing();
		if (!Instance.IsInitialized())
		{
			Debug.Log("RestorePurchases FAIL. Not initialized.");
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			Instance.m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(delegate(bool result)
			{
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		else
		{
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log("OnInitialized: PASS");
		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;
		Product[] all = controller.products.all;
		foreach (Product product in all)
		{
			Debug.Log("product.id=" + product.definition.id + " has=" + product.hasReceipt + " available=" + product.availableToPurchase + " receipt=" + product.receipt);
			if (product.hasReceipt)
			{
				PurchasingItem.Set(product.definition.id, true);
			}
		}
		Callback();
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		Callback();
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		PurchasingItem.Set(args.purchasedProduct.definition.id, true);
		Callback();
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		Callback();
	}

	private void Callback()
	{
		if (CallbackMethod != null)
		{
			CallbackMethod();
		}
	}
}
