using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	public interface IInterstitialClient
	{
		event EventHandler<EventArgs> OnAdLoaded;

		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		event EventHandler<EventArgs> OnAdOpening;

		event EventHandler<EventArgs> OnAdClosed;

		event EventHandler<EventArgs> OnAdLeavingApplication;

		void CreateInterstitialAd(string adUnitId);

		void LoadAd(AdRequest request);

		bool IsLoaded();

		void ShowInterstitial();

		void DestroyInterstitial();

		string MediationAdapterClassName();
	}
}
