using UnityEngine.UI;

namespace UnityEngine.Analytics
{
	public class DataPrivacyButton : Button
	{
		private bool urlOpened;

		private DataPrivacyButton()
		{
			base.onClick.AddListener(OpenDataPrivacyUrl);
		}

		private void OnFailure(string reason)
		{
			base.interactable = true;
			Debug.LogWarning(string.Format("Failed to get data privacy url: {0}", reason));
		}

		private void OpenUrl(string url)
		{
			base.interactable = true;
			urlOpened = true;
			Application.OpenURL(url);
		}

		private void OpenDataPrivacyUrl()
		{
			base.interactable = false;
			DataPrivacy.FetchPrivacyUrl(OpenUrl, OnFailure);
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (hasFocus && urlOpened)
			{
				urlOpened = false;
				RemoteSettings.ForceUpdate();
			}
		}
	}
}
