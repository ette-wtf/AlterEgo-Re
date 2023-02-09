using System;
using System.Collections.Generic;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	public class AdLoader
	{
		public class Builder
		{
			internal string AdUnitId { get; private set; }

			internal HashSet<NativeAdType> AdTypes { get; private set; }

			internal HashSet<string> TemplateIds { get; private set; }

			internal Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateClickHandlers { get; private set; }

			public Builder(string adUnitId)
			{
				AdUnitId = adUnitId;
				AdTypes = new HashSet<NativeAdType>();
				TemplateIds = new HashSet<string>();
				CustomNativeTemplateClickHandlers = new Dictionary<string, Action<CustomNativeTemplateAd, string>>();
			}

			public Builder ForCustomNativeAd(string templateId)
			{
				TemplateIds.Add(templateId);
				AdTypes.Add(NativeAdType.CustomTemplate);
				return this;
			}

			public Builder ForCustomNativeAd(string templateId, Action<CustomNativeTemplateAd, string> callback)
			{
				TemplateIds.Add(templateId);
				CustomNativeTemplateClickHandlers[templateId] = callback;
				AdTypes.Add(NativeAdType.CustomTemplate);
				return this;
			}

			public AdLoader Build()
			{
				return new AdLoader(this);
			}
		}

		private IAdLoaderClient adLoaderClient;

		public Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateClickHandlers { get; private set; }

		public string AdUnitId { get; private set; }

		public HashSet<NativeAdType> AdTypes { get; private set; }

		public HashSet<string> TemplateIds { get; private set; }

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		private AdLoader(Builder builder)
		{
			AdUnitId = string.Copy(builder.AdUnitId);
			CustomNativeTemplateClickHandlers = new Dictionary<string, Action<CustomNativeTemplateAd, string>>(builder.CustomNativeTemplateClickHandlers);
			TemplateIds = new HashSet<string>(builder.TemplateIds);
			AdTypes = new HashSet<NativeAdType>(builder.AdTypes);
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildAdLoaderClient", BindingFlags.Static | BindingFlags.Public);
			adLoaderClient = (IAdLoaderClient)method.Invoke(null, new object[1] { this });
			Utils.CheckInitialization();
			adLoaderClient.OnCustomNativeTemplateAdLoaded += delegate(object sender, CustomNativeEventArgs args)
			{
				this.OnCustomNativeTemplateAdLoaded(this, args);
			};
			adLoaderClient.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
		}

		public void LoadAd(AdRequest request)
		{
			adLoaderClient.LoadAd(request);
		}
	}
}
