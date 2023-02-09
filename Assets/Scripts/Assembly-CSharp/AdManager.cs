using System;
using System.Collections.Generic;
using System.Linq;
using App;
using UnityEngine;

public class AdManager : MonoBehaviour
{
	private static AdManager Instance;

	private bool IsActive;

	private GameObject BannerBG;

	private AndroidJavaObject AndroidUtil;

	private List<AdModule.AdEventArgs> SuspendEvent = new List<AdModule.AdEventArgs>();

	private IDictionary<string, AdModule> AdList = new Dictionary<string, AdModule>();

	private Action<bool> AppCallback;

	private void Awake()
	{
		Instance = this;
		IsActive = true;
		SetActive(AdInfo.ENABLE);
		AndroidUtil = new AndroidJavaObject("com.caracolu.appcommon.Util");
		AndroidUtil.Call("SetUIChangeListener");
	}

	public static void SetActive(bool active)
	{
		Debug.Log("AdManager#SetActive " + active);
		foreach (AdModule value in Instance.AdList.Values)
		{
			value.Destroy();
		}
		Instance.AdList.Clear();
		UnityEngine.Object.Destroy(Instance.BannerBG);
		Instance.BannerBG = null;
		if (active)
		{
			Instance.RestoreModule();
		}
		else
		{
			Instance.AddModule("SkipReward", null);
		}
		SafeAreaAdjuster.UpdateLayout();
	}

	private void RestoreModule()
	{
		foreach (string key in AdInfo.AD_ID.Keys)
		{
			AddModule(key, AdInfo.AD_ID[key]);
		}
		GameObject gameObject = Resources.Load<GameObject>("AdCanvas");
		if (gameObject != null)
		{
			BannerBG = UnityEngine.Object.Instantiate(gameObject, base.transform);
			BannerBG.SetActive(false);
		}
	}

	private void AddModule(string key, string[] values)
	{
		if (AdList.ContainsKey(key))
		{
			AdList[key].Destroy();
			AdList[key].OnAdEvent -= OnAdEvent;
			AdList.Remove(key);
		}
		AdModule adModule = ((!key.Contains("UnityAds")) ? new AdModule(this, key, values) : new AdModuleUnityAds());
		AdList.Add(key, adModule);
		adModule.OnAdEvent += OnAdEvent;
	}

	private void OnAdEvent(object sender, AdModule.AdEventArgs args)
	{
		Debug.Log("AdManager#OnEvent(" + args.Type + " " + args.Message + ")");
		if (!IsActive)
		{
			SuspendEvent.Add(args);
			return;
		}
		switch (args.Message)
		{
		case "OnClosed":
			break;
		case "OnFailed(Play)":
			break;
		case "OnReward":
			AppCallback(true);
			break;
		case "NoReward":
			AppCallback(false);
			break;
		case "NavigationBar:IsVisible":
		case "NavigationBar:NotVisible":
			if (AdList.ContainsKey("AdMobBanner") && AdList["AdMobBanner"].IsShowing)
			{
				AdList["AdMobBanner"].Hide();
				AdList["AdMobBanner"].Show();
				SafeAreaAdjuster.UpdateLayout();
			}
			break;
		case "OnChangedSafeInsetArea":
		{
			SafeAreaAdjuster.SafeAreaHeight = AndroidUtil.GetStatic<int>("sSafeInsetHeight");
			string[] array = AdList.Keys.ToArray();
			foreach (string text in array)
			{
				if (text.Contains("Rectangle"))
				{
					AddModule(text, AdInfo.AD_ID[text]);
				}
			}
			break;
		}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		Debug.Log("AdManager#OnApplicationPause " + pause);
		IsActive = !pause;
		AndroidUtil.SetStatic("sIsActive", IsActive);
		if (!IsActive)
		{
			return;
		}
		foreach (AdModule.AdEventArgs item in SuspendEvent)
		{
			OnAdEvent(null, item);
		}
		SuspendEvent.Clear();
		SafeAreaAdjuster.UpdateLayout();
	}

	private void OnDestroy()
	{
		foreach (AdModule value in AdList.Values)
		{
			value.OnAdEvent -= OnAdEvent;
			value.Destroy();
		}
		Instance = null;
		AndroidUtil.Dispose();
	}

	public static bool IsReady(string type)
	{
		if (Instance == null)
		{
			return false;
		}
		foreach (string key in Instance.AdList.Keys)
		{
			if (key.Contains(type) && Instance.AdList[key].IsReady())
			{
				return true;
			}
		}
		return false;
	}

	public static void Show(string type, Action<bool> callback = null)
	{
		if (Instance == null)
		{
			return;
		}
		Instance.AppCallback = callback;
		foreach (string key in Instance.AdList.Keys)
		{
			if (key.Contains(type))
			{
				AdModule adModule = Instance.AdList[key];
				if (!type.Contains("Reward") || adModule.IsReady())
				{
					adModule.Show();
					break;
				}
			}
		}
		if (!(Instance.BannerBG == null) && type.Contains("Banner"))
		{
			Instance.BannerBG.SetActive(true);
		}
	}

	public static void Hide(string type)
	{
		if (Instance == null)
		{
			return;
		}
		foreach (string key in Instance.AdList.Keys)
		{
			if (key.Contains(type))
			{
				Instance.AdList[key].Hide();
				break;
			}
		}
		if (!(Instance.BannerBG == null) && type.Contains("Banner"))
		{
			Instance.BannerBG.SetActive(false);
		}
	}

	public void OnEventFromAndroid(string message)
	{
		AdModule.AdEventArgs adEventArgs = new AdModule.AdEventArgs();
		adEventArgs.Type = "AndroidEvent";
		adEventArgs.Message = message;
		OnAdEvent(null, adEventArgs);
	}
}
