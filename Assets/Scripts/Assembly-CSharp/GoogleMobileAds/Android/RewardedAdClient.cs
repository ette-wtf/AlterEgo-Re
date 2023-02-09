using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class RewardedAdClient : AndroidJavaProxy, IRewardedAdClient
	{
		private AndroidJavaObject androidRewardedAd;

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<Reward> OnUserEarnedReward;

		public event EventHandler<EventArgs> OnAdClosed;

		public RewardedAdClient()
			: base("com.google.unity.ads.UnityRewardedAdCallback")
		{
			AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			androidRewardedAd = new AndroidJavaObject("com.google.unity.ads.UnityRewardedAd", @static, this);
		}

		public void CreateRewardedAd(string adUnitId)
		{
			androidRewardedAd.Call("create", adUnitId);
		}

		public void LoadAd(AdRequest request)
		{
			androidRewardedAd.Call("loadAd", Utils.GetAdRequestJavaObject(request));
		}

		public bool IsLoaded()
		{
			return androidRewardedAd.Call<bool>("isLoaded", Array.Empty<object>());
		}

		public void Show()
		{
			androidRewardedAd.Call("show");
		}

		public void DestroyRewardBasedVideoAd()
		{
			androidRewardedAd.Call("destroy");
		}

		public string MediationAdapterClassName()
		{
			return androidRewardedAd.Call<string>("getMediationAdapterClassName", Array.Empty<object>());
		}

		private void onRewardedAdLoaded()
		{
			if (this.OnAdLoaded != null)
			{
				this.OnAdLoaded(this, EventArgs.Empty);
			}
		}

		private void onRewardedAdFailedToLoad(string errorReason)
		{
			if (this.OnAdFailedToLoad != null)
			{
				AdErrorEventArgs e = new AdErrorEventArgs
				{
					Message = errorReason
				};
				this.OnAdFailedToLoad(this, e);
			}
		}

		private void onRewardedAdFailedToShow(string errorReason)
		{
			if (this.OnAdFailedToLoad != null)
			{
				AdErrorEventArgs e = new AdErrorEventArgs
				{
					Message = errorReason
				};
				this.OnAdFailedToShow(this, e);
			}
		}

		private void onRewardedAdOpened()
		{
			if (this.OnAdOpening != null)
			{
				this.OnAdOpening(this, EventArgs.Empty);
			}
		}

		private void onRewardedAdClosed()
		{
			if (this.OnAdClosed != null)
			{
				this.OnAdClosed(this, EventArgs.Empty);
			}
		}

		private void onUserEarnedReward(string type, float amount)
		{
			if (this.OnUserEarnedReward != null)
			{
				Reward e = new Reward
				{
					Type = type,
					Amount = amount
				};
				this.OnUserEarnedReward(this, e);
			}
		}
	}
}
