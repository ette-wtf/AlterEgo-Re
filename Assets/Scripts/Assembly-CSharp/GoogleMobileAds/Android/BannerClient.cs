using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class BannerClient : AndroidJavaProxy, IBannerClient
	{
		private AndroidJavaObject bannerView;

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public BannerClient()
			: base("com.google.unity.ads.UnityAdListener")
		{
			AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			bannerView = new AndroidJavaObject("com.google.unity.ads.Banner", @static, this);
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			bannerView.Call("create", adUnitId, Utils.GetAdSizeJavaObject(adSize), (int)position);
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, int x, int y)
		{
			bannerView.Call("create", adUnitId, Utils.GetAdSizeJavaObject(adSize), x, y);
		}

		public void LoadAd(AdRequest request)
		{
			bannerView.Call("loadAd", Utils.GetAdRequestJavaObject(request));
		}

		public void ShowBannerView()
		{
			bannerView.Call("show");
		}

		public void HideBannerView()
		{
			bannerView.Call("hide");
		}

		public void DestroyBannerView()
		{
			bannerView.Call("destroy");
		}

		public float GetHeightInPixels()
		{
			return bannerView.Call<float>("getHeightInPixels", Array.Empty<object>());
		}

		public float GetWidthInPixels()
		{
			return bannerView.Call<float>("getWidthInPixels", Array.Empty<object>());
		}

		public void SetPosition(AdPosition adPosition)
		{
			bannerView.Call("setPosition", (int)adPosition);
		}

		public void SetPosition(int x, int y)
		{
			bannerView.Call("setPosition", x, y);
		}

		public string MediationAdapterClassName()
		{
			return bannerView.Call<string>("getMediationAdapterClassName", Array.Empty<object>());
		}

		public void onAdLoaded()
		{
			if (this.OnAdLoaded != null)
			{
				this.OnAdLoaded(this, EventArgs.Empty);
			}
		}

		public void onAdFailedToLoad(string errorReason)
		{
			if (this.OnAdFailedToLoad != null)
			{
				AdFailedToLoadEventArgs e = new AdFailedToLoadEventArgs
				{
					Message = errorReason
				};
				this.OnAdFailedToLoad(this, e);
			}
		}

		public void onAdOpened()
		{
			if (this.OnAdOpening != null)
			{
				this.OnAdOpening(this, EventArgs.Empty);
			}
		}

		public void onAdClosed()
		{
			if (this.OnAdClosed != null)
			{
				this.OnAdClosed(this, EventArgs.Empty);
			}
		}

		public void onAdLeftApplication()
		{
			if (this.OnAdLeavingApplication != null)
			{
				this.OnAdLeavingApplication(this, EventArgs.Empty);
			}
		}
	}
}
