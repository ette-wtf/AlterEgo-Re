using System.Collections.Generic;
using GoogleMobileAds.Common;
using UnityEngine;

namespace GoogleMobileAds.Api
{
	public class CustomNativeTemplateAd
	{
		private ICustomNativeTemplateClient client;

		internal CustomNativeTemplateAd(ICustomNativeTemplateClient client)
		{
			this.client = client;
		}

		public List<string> GetAvailableAssetNames()
		{
			return client.GetAvailableAssetNames();
		}

		public string GetCustomTemplateId()
		{
			return client.GetTemplateId();
		}

		public Texture2D GetTexture2D(string key)
		{
			byte[] imageByteArray = client.GetImageByteArray(key);
			if (imageByteArray == null)
			{
				return null;
			}
			return Utils.GetTexture2DFromByteArray(imageByteArray);
		}

		public string GetText(string key)
		{
			return client.GetText(key);
		}

		public void PerformClick(string assetName)
		{
			client.PerformClick(assetName);
		}

		public void RecordImpression()
		{
			client.RecordImpression();
		}
	}
}
