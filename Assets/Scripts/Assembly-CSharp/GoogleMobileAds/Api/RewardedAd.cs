using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	public class RewardedAd
	{
		private IRewardedAdClient client;

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnUserEarnedReward;

		public RewardedAd(string adUnitId)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildRewardedAdClient", BindingFlags.Static | BindingFlags.Public);
			client = (IRewardedAdClient)method.Invoke(null, null);
			client.CreateRewardedAd(adUnitId);
			client.OnAdLoaded += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLoaded != null)
				{
					this.OnAdLoaded(this, args);
				}
			};
			client.OnAdFailedToLoad += delegate(object sender, AdErrorEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
			client.OnAdFailedToShow += delegate(object sender, AdErrorEventArgs args)
			{
				if (this.OnAdFailedToShow != null)
				{
					this.OnAdFailedToShow(this, args);
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
			client.OnUserEarnedReward += delegate(object sender, Reward args)
			{
				if (this.OnUserEarnedReward != null)
				{
					this.OnUserEarnedReward(this, args);
				}
			};
		}

		public void LoadAd(AdRequest request)
		{
			client.LoadAd(request);
		}

		public bool IsLoaded()
		{
			return client.IsLoaded();
		}

		public void Show()
		{
			client.Show();
		}

		public string MediationAdapterClassName()
		{
			return client.MediationAdapterClassName();
		}
	}
}
