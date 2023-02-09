using System;
using System.Collections.Generic;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Android
{
	internal class CustomNativeTemplateClient : ICustomNativeTemplateClient
	{
		private AndroidJavaObject customNativeAd;

		public CustomNativeTemplateClient(AndroidJavaObject customNativeAd)
		{
			this.customNativeAd = customNativeAd;
		}

		public List<string> GetAvailableAssetNames()
		{
			return new List<string>(customNativeAd.Call<string[]>("getAvailableAssetNames", Array.Empty<object>()));
		}

		public string GetTemplateId()
		{
			return customNativeAd.Call<string>("getTemplateId", Array.Empty<object>());
		}

		public byte[] GetImageByteArray(string key)
		{
			byte[] array = customNativeAd.Call<byte[]>("getImage", new object[1] { key });
			if (array.Length == 0)
			{
				return null;
			}
			return array;
		}

		public string GetText(string key)
		{
			string text = customNativeAd.Call<string>("getText", new object[1] { key });
			if (text.Equals(string.Empty))
			{
				return null;
			}
			return text;
		}

		public void PerformClick(string assetName)
		{
			customNativeAd.Call("performClick", assetName);
		}

		public void RecordImpression()
		{
			customNativeAd.Call("recordImpression");
		}
	}
}
