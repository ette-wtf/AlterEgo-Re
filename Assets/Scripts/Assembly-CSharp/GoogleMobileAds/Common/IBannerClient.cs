using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	public interface IBannerClient
	{
		event EventHandler<EventArgs> OnAdLoaded;

		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		event EventHandler<EventArgs> OnAdOpening;

		event EventHandler<EventArgs> OnAdClosed;

		event EventHandler<EventArgs> OnAdLeavingApplication;

		void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position);

		void CreateBannerView(string adUnitId, AdSize adSize, int x, int y);

		void LoadAd(AdRequest request);

		void ShowBannerView();

		void HideBannerView();

		void DestroyBannerView();

		float GetHeightInPixels();

		float GetWidthInPixels();

		void SetPosition(AdPosition adPosition);

		void SetPosition(int x, int y);

		string MediationAdapterClassName();
	}
}
