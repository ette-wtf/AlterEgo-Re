using App;
using UnityEngine;

public class RemoteSettingsReceiver : MonoBehaviour
{
	private void Start()
	{
		ReadRemoteSettings();
		RemoteSettings.Completed += ReadRemoteSettings;
	}

	private void OnDestroy()
	{
		RemoteSettings.Completed -= ReadRemoteSettings;
	}

	private void ReadRemoteSettings(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse)
	{
		ReadRemoteSettings();
	}

	private void ReadRemoteSettings()
	{
		string saveDataID = Settings.SaveDataID;
		if (string.IsNullOrEmpty(saveDataID))
		{
			return;
		}
		string @string = RemoteSettings.GetString(saveDataID);
		string[] iTEM_LIST = PurchasingItem.ITEM_LIST;
		foreach (string value in iTEM_LIST)
		{
			if (@string.Contains(value))
			{
				PurchasingItem.Set(value, true);
			}
		}
	}
}
