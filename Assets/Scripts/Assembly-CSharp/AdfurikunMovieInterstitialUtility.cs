using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class AdfurikunMovieInterstitialUtility : MonoBehaviour
{
	public enum ADF_MovieStatus
	{
		NotPrepared = 0,
		PrepareSuccess = 1,
		StartPlaying = 2,
		FinishedPlaying = 3,
		FailedPlaying = 4,
		AdClose = 5
	}

	public class AdfurikunUnityListener : AndroidJavaProxy
	{
		public AdfurikunUnityListener()
			: base("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.UnityMovieListener")
		{
		}

		public void onPrepareSuccess(string appId)
		{
			mInstance.onPrepareSuccess.NullSafe(appId);
			mInstance.sendMessage(ADF_MovieStatus.PrepareSuccess, appId, "");
		}

		public void onStartPlaying(string appId, string adnetworkKey)
		{
			mInstance.onStartPlaying.NullSafe(appId, adnetworkKey);
			mInstance.sendMessage(ADF_MovieStatus.StartPlaying, appId, adnetworkKey);
		}

		public void onFinishedPlaying(string appId, string adnetworkKey)
		{
			mInstance.onFinishPlaying.NullSafe(appId, adnetworkKey);
			mInstance.sendMessage(ADF_MovieStatus.FinishedPlaying, appId, adnetworkKey);
		}

		public void onFailedPlaying(string appId, string adnetworkKey)
		{
			mInstance.onFailedPlaying.NullSafe(appId, adnetworkKey);
			mInstance.sendMessage(ADF_MovieStatus.FailedPlaying, appId, adnetworkKey);
		}

		public void onAdClose(string appId, string adnetworkKey)
		{
			mInstance.onCloseAd.NullSafe(appId, adnetworkKey);
			mInstance.sendMessage(ADF_MovieStatus.AdClose, appId, adnetworkKey);
		}
	}

	public Action<string> onNotPrepared;

	public Action<string> onPrepareSuccess;

	public Action<string, string> onStartPlaying;

	public Action<string, string> onFinishPlaying;

	public Action<string, string> onFailedPlaying;

	public Action<string, string> onCloseAd;

	public AdfurikunMovieInterstitialAdConfig config;

	private static AdfurikunMovieInterstitialUtility mInstance;

	private GameObject mMovieInterstitialSrcObject;

	private AdfurikunUnityListener mAdfurikunUnityListener;

	private string unityPluginVersion = "2.20.0";

	public static AdfurikunMovieInterstitialUtility instance
	{
		get
		{
			return mInstance;
		}
	}

	public void Awake()
	{
		if (mInstance == null)
		{
			mInstance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void OnDestroy()
	{
		if (!Application.isEditor)
		{
			bool flag = mInstance == this;
		}
	}

	public void OnApplicationPause(bool pause)
	{
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
		{
			if (pause)
			{
				callAndroidMovieInterstitialMethod("onPause");
			}
			else
			{
				callAndroidMovieInterstitialMethod("onResume");
			}
		}
	}

	public void Start()
	{
		if (!Application.isEditor)
		{
			initializeMovieInterstitial();
		}
	}

	public void initializeMovieInterstitial()
	{
		initializeMovieInterstitial(getAppID());
	}

	public void initializeMovieInterstitial(string appId)
	{
		if (isValidAppID(appId) && Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			if (mAdfurikunUnityListener == null)
			{
				mAdfurikunUnityListener = new AdfurikunUnityListener();
			}
			new AndroidJavaClass("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.AdfurikunUnityManager").CallStatic("initialize", @static, unityPluginVersion);
			makeInstance_AdfurikunMovieInterstitialController().CallStatic("initialize", @static, appId, mAdfurikunUnityListener);
		}
	}

	public bool isPreparedMovieInterstitial()
	{
		return isPreparedMovieInterstitial(getAppID());
	}

	public bool isPreparedMovieInterstitial(string appId)
	{
		if (!isValidAppID(appId))
		{
			return false;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			return makeInstance_AdfurikunMovieInterstitialController().CallStatic<bool>("isPrepared", new object[1] { appId });
		}
		return false;
	}

	[Obsolete("Use Action delegate instead.")]
	public void setMovieInterstitialSrcObject(GameObject movieInterstitialSrcObject)
	{
		setMovieInterstitialSrcObject(movieInterstitialSrcObject, getAppID());
	}

	public void setMovieInterstitialSrcObject(GameObject movieInterstitialSrcObject, string appId)
	{
		if (isValidAppID(appId))
		{
			mMovieInterstitialSrcObject = movieInterstitialSrcObject;
			if (isPreparedMovieInterstitial(appId))
			{
				onPrepareSuccess.NullSafe(appId);
				sendMessage(ADF_MovieStatus.PrepareSuccess, appId, "");
			}
		}
	}

	public void playMovieInterstitial()
	{
		playMovieInterstitial(getAppID());
	}

	public void playMovieInterstitial(string appId)
	{
		if (isValidAppID(appId) && Application.platform == RuntimePlatform.Android)
		{
			if (!isPreparedMovieInterstitial(appId))
			{
				onNotPrepared.NullSafe(appId);
				sendMessage(ADF_MovieStatus.NotPrepared, appId, "");
			}
			else
			{
				makeInstance_AdfurikunMovieInterstitialController().CallStatic("play", appId);
			}
		}
	}

	public void MovieInterstitialCallback(string param)
	{
		string[] array = param.Split(';');
		string text = array[0].Split(':')[1];
		string text2 = array[1].Split(':')[1];
		string text3 = "";
		if (array.Length > 2)
		{
			text3 = array[2].Split(':')[1];
		}
		ADF_MovieStatus status;
		switch (text)
		{
		default:
			return;
		case "PrepareSuccess":
			status = ADF_MovieStatus.PrepareSuccess;
			onPrepareSuccess.NullSafe(text2);
			break;
		case "StartPlaying":
			status = ADF_MovieStatus.StartPlaying;
			onStartPlaying.NullSafe(text2, text3);
			break;
		case "FinishedPlaying":
			status = ADF_MovieStatus.FinishedPlaying;
			onFinishPlaying.NullSafe(text2, text3);
			break;
		case "FailedPlaying":
			status = ADF_MovieStatus.FailedPlaying;
			onFailedPlaying.NullSafe(text2, text3);
			break;
		case "AdClose":
			status = ADF_MovieStatus.AdClose;
			onCloseAd.NullSafe(text2, text3);
			break;
		}
		sendMessage(status, text2, text3);
	}

	public void sendMessage(ADF_MovieStatus status, string appId, string adnetworkKey)
	{
		if (mMovieInterstitialSrcObject != null)
		{
			ArrayList arrayList = new ArrayList();
			arrayList.Add((int)status);
			arrayList.Add(appId);
			arrayList.Add(adnetworkKey);
			mMovieInterstitialSrcObject.SendMessage("MovieInterstitialCallback", arrayList);
		}
	}

	public void dispose()
	{
		disposeResource();
	}

	public void disposeResource()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			callAndroidMovieInterstitialMethod("onDestroy");
		}
	}

	private string getAppID()
	{
		string result = "";
		if (Application.platform == RuntimePlatform.Android)
		{
			result = config.androidAppID;
		}
		return result;
	}

	private bool isValidAppID(string appId)
	{
		return Regex.IsMatch(appId, "^[a-f0-9]{24}$");
	}

	private AndroidJavaClass makeInstance_AdfurikunMovieInterstitialController()
	{
		return new AndroidJavaClass("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.AdfurikunMovieInterController");
	}

	private void callAndroidMovieInterstitialMethod(string methodName)
	{
		makeInstance_AdfurikunMovieInterstitialController().CallStatic(methodName);
	}
}
