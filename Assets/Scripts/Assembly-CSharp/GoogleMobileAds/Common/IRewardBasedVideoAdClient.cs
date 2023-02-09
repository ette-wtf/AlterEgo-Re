using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	public interface IRewardBasedVideoAdClient
	{
		event EventHandler<EventArgs> OnAdLoaded;

		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		event EventHandler<EventArgs> OnAdOpening;

		event EventHandler<EventArgs> OnAdStarted;

		event EventHandler<Reward> OnAdRewarded;

		event EventHandler<EventArgs> OnAdClosed;

		event EventHandler<EventArgs> OnAdLeavingApplication;

		event EventHandler<EventArgs> OnAdCompleted;

		void CreateRewardBasedVideoAd();

		void LoadAd(AdRequest request, string adUnitId);

		bool IsLoaded();

		string MediationAdapterClassName();

		void ShowRewardBasedVideoAd();

		void SetUserId(string userId);
	}
}
