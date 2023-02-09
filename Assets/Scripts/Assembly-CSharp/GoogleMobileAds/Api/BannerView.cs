using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	public class BannerView
	{
		private IBannerClient client;

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public BannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildBannerClient", BindingFlags.Static | BindingFlags.Public);
			client = (IBannerClient)method.Invoke(null, null);
			client.CreateBannerView(adUnitId, adSize, position);
			ConfigureBannerEvents();
		}

		public BannerView(string adUnitId, AdSize adSize, int x, int y)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildBannerClient", BindingFlags.Static | BindingFlags.Public);
			client = (IBannerClient)method.Invoke(null, null);
			client.CreateBannerView(adUnitId, adSize, x, y);
			ConfigureBannerEvents();
		}

		public void LoadAd(AdRequest request)
		{
			client.LoadAd(request);
		}

		public void Hide()
		{
			client.HideBannerView();
		}

		public void Show()
		{
			client.ShowBannerView();
		}

		public void Destroy()
		{
			client.DestroyBannerView();
		}

		public float GetHeightInPixels()
		{
			return client.GetHeightInPixels();
		}

		public float GetWidthInPixels()
		{
			return client.GetWidthInPixels();
		}

		public void SetPosition(AdPosition adPosition)
		{
			client.SetPosition(adPosition);
		}

		public void SetPosition(int x, int y)
		{
			client.SetPosition(x, y);
		}

		private void ConfigureBannerEvents()
		{
			client.OnAdLoaded += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLoaded != null)
				{
					this.OnAdLoaded(this, args);
				}
			};
			client.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
			client.OnAdOpening += delegate(object sender, EventArgs args)
			{
				if (this.OnAdOpening != null)
				{
					this.OnAdOpening(this, args);
				}
			};
			client.OnAdClosed += delegate(object sender, EventArgs args)
			{
				if (this.OnAdClosed != null)
				{
					this.OnAdClosed(this, args);
				}
			};
			client.OnAdLeavingApplication += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLeavingApplication != null)
				{
					this.OnAdLeavingApplication(this, args);
				}
			};
		}

		public string MediationAdapterClassName()
		{
			return client.MediationAdapterClassName();
		}
	}
}
