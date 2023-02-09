using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	public class RewardedAdDummyClient : IRewardedAdClient
	{
		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnUserEarnedReward;

		public void CreateRewardedAd(string adUnitId)
		{
		}

		public void LoadAd(AdRequest request)
		{
		}

		public bool IsLoaded()
		{
			return true;
		}

		public void Show()
		{
		}

		public string MediationAdapterClassName()
		{
			return null;
		}
	}
}
