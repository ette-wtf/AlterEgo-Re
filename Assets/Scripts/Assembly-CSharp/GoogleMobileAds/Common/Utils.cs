using System;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	internal class Utils
	{
		public static void CheckInitialization()
		{
			if (!MobileAdsEventExecutor.IsActive())
			{
				Debug.Log("You intitialized an ad object but have not yet called MobileAds.Initialize(). We highly recommend you call MobileAds.Initialize() before interacting with the Google Mobile Ads SDK.");
			}
			MobileAdsEventExecutor.Initialize();
		}

		public static Texture2D GetTexture2DFromByteArray(byte[] img)
		{
			Texture2D texture2D = new Texture2D(1, 1);
			if (!texture2D.LoadImage(img))
			{
				throw new InvalidOperationException("Could not load custom native template\n                        image asset as texture");
			}
			return texture2D;
		}
	}
}
