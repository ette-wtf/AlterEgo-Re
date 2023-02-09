using System.Collections.Generic;

namespace GoogleMobileAds.Common
{
	public interface ICustomNativeTemplateClient
	{
		string GetTemplateId();

		byte[] GetImageByteArray(string key);

		List<string> GetAvailableAssetNames();

		string GetText(string key);

		void PerformClick(string assetName);

		void RecordImpression();
	}
}
