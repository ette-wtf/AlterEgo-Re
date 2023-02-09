using GoogleMobileAds.Android;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
	public class GoogleMobileAdsClientFactory
	{
		public static IBannerClient BuildBannerClient()
		{
			return new BannerClient();
		}

		public static IInterstitialClient BuildInterstitialClient()
		{
			return new InterstitialClient();
		}

		public static IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
		{
			return new RewardBasedVideoAdClient();
		}

		public static IRewardedAdClient BuildRewardedAdClient()
		{
			return new RewardedAdClient();
		}

		public static IAdLoaderClient BuildAdLoaderClient(AdLoader adLoader)
		{
			return new AdLoaderClient(adLoader);
		}

		public static IMobileAdsClient MobileAdsInstance()
		{
			return MobileAdsClient.Instance;
		}
	}
}
