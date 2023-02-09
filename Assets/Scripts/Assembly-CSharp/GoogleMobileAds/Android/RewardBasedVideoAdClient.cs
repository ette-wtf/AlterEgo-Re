using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	public class RewardBasedVideoAdClient : AndroidJavaProxy, IRewardBasedVideoAdClient
	{
		private AndroidJavaObject androidRewardBasedVideo;

		public event EventHandler<EventArgs> OnAdLoaded = delegate
		{
		};

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad = delegate
		{
		};

		public event EventHandler<EventArgs> OnAdOpening = delegate
		{
		};

		public event EventHandler<EventArgs> OnAdStarted = delegate
		{
		};

		public event EventHandler<EventArgs> OnAdClosed = delegate
		{
		};

		public event EventHandler<Reward> OnAdRewarded = delegate
		{
		};

		public event EventHandler<EventArgs> OnAdLeavingApplication = delegate
		{
		};

		public event EventHandler<EventArgs> OnAdCompleted = delegate
		{
		};

		public RewardBasedVideoAdClient()
			: base("com.google.unity.ads.UnityRewardBasedVideoAdListener")
		{
			AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			androidRewardBasedVideo = new AndroidJavaObject("com.google.unity.ads.RewardBasedVideo", @static, this);
		}

		public void CreateRewardBasedVideoAd()
		{
			androidRewardBasedVideo.Call("create");
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
			androidRewardBasedVideo.Call("loadAd", Utils.GetAdRequestJavaObject(request), adUnitId);
		}

		public bool IsLoaded()
		{
			return androidRewardBasedVideo.Call<bool>("isLoaded", Array.Empty<object>());
		}

		public void ShowRewardBasedVideoAd()
		{
			androidRewardBasedVideo.Call("show");
		}

		public void SetUserId(string userId)
		{
			androidRewardBasedVideo.Call("setUserId", userId);
		}

		public void DestroyRewardBasedVideoAd()
		{
			androidRewardBasedVideo.Call("destroy");
		}

		public string MediationAdapterClassName()
		{
			return androidRewardBasedVideo.Call<string>("getMediationAdapterClassName", Array.Empty<object>());
		}

		private void onAdLoaded()
		{
			if (this.OnAdLoaded != null)
			{
				this.OnAdLoaded(this, EventArgs.Empty);
			}
		}

		private void onAdFailedToLoad(string errorReason)
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

		private void onAdOpened()
		{
			if (this.OnAdOpening != null)
			{
				this.OnAdOpening(this, EventArgs.Empty);
			}
		}

		private void onAdStarted()
		{
			if (this.OnAdStarted != null)
			{
				this.OnAdStarted(this, EventArgs.Empty);
			}
		}

		private void onAdClosed()
		{
			if (this.OnAdClosed != null)
			{
				this.OnAdClosed(this, EventArgs.Empty);
			}
		}

		private void onAdRewarded(string type, float amount)
		{
			if (this.OnAdRewarded != null)
			{
				Reward e = new Reward
				{
					Type = type,
					Amount = amount
				};
				this.OnAdRewarded(this, e);
			}
		}

		private void onAdLeftApplication()
		{
			if (this.OnAdLeavingApplication != null)
			{
				this.OnAdLeavingApplication(this, EventArgs.Empty);
			}
		}

		private void onAdCompleted()
		{
			if (this.OnAdCompleted != null)
			{
				this.OnAdCompleted(this, EventArgs.Empty);
			}
		}
	}
}
