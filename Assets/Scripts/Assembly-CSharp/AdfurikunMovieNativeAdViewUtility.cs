using System;
using System.Collections;
using UnityEngine;

public class AdfurikunMovieNativeAdViewUtility : MonoBehaviour
{
	public enum ADF_MovieStatus
	{
		LoadFinish = 0,
		LoadError = 1,
		PlayStart = 2,
		PlayFinish = 3,
		PlayError = 4
	}

	public Action<string> onLoadFinish;

	public Action<string, string> onLoadError;

	public Action<string> onPlayStart;

	public Action<string, bool> onPlayFinish;

	public Action<string, string> onPlayError;

	public AdfurikunMovieNativeAdViewAdConfig config;

	private static AdfurikunMovieNativeAdViewUtility mInstance;

	private GameObject mMovieNativeAdViewSrcObject;

	private string unityPluginVersion = "2.20.0";

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

	private void Start()
	{
		if (Application.isEditor)
		{
			return;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getWindowManager", Array.Empty<object>());
			if (androidJavaObject != null)
			{
				AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getDefaultDisplay", Array.Empty<object>());
				if (androidJavaObject2 != null)
				{
					AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("android.util.DisplayMetrics");
					androidJavaObject2.Call("getMetrics", androidJavaObject3);
				}
			}
		}
		initializeMovieNativeAdView();
	}

	public void OnApplicationPause(bool pause)
	{
		if (!Application.isEditor && Application.platform == RuntimePlatform.Android)
		{
			if (pause)
			{
				callAndroidMovieNativeAdViewMethod("onPause");
			}
			else
			{
				callAndroidMovieNativeAdViewMethod("onResume");
			}
		}
	}

	public void initializeMovieNativeAdView()
	{
		initializeMovieNativeAdView(getAppID());
	}

	public void initializeMovieNativeAdView(string appId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject @static = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
			new AndroidJavaClass("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.AdfurikunUnityManager").CallStatic("initialize", @static, unityPluginVersion);
			makeInstance_AdfurikunMovieNativeAdViewController().CallStatic("initialize", @static, appId);
		}
	}

	public void loadMovieNativeAdView()
	{
		loadMovieNativeAdView(getAppID());
	}

	public void loadMovieNativeAdView(string appId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			makeInstance_AdfurikunMovieNativeAdViewController().CallStatic("load", appId);
		}
	}

	public void setMovieNativeAdView(float x, float y, float width, float height)
	{
		setMovieNativeAdView(getAppID(), x, y, width, height);
	}

	public void setMovieNativeAdView(string appId, float x, float y, float width, float height)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getWindow", Array.Empty<object>()).Call<AndroidJavaObject>("getDecorView", Array.Empty<object>());
			int num = androidJavaObject.Call<int>("getWidth", Array.Empty<object>());
			int num2 = androidJavaObject.Call<int>("getHeight", Array.Empty<object>());
			float num3 = (float)num * (width / (float)Screen.width);
			float num4 = (float)num2 * (height / (float)Screen.height);
			float num5 = (float)num * (x / (float)Screen.width);
			float num6 = (float)num2 * (y / (float)Screen.height);
			makeInstance_AdfurikunMovieNativeAdViewController().CallStatic("show", appId, num5, num6, num3, num4);
		}
	}

	public void setMovieNativeAdViewFrame(float x, float y, float width, float height)
	{
		setMovieNativeAdViewFrame(getAppID(), x, y, width, height);
	}

	public void setMovieNativeAdViewFrame(string appId, float x, float y, float width, float height)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			setMovieNativeAdView(appId, x, y, width, height);
		}
	}

	public void setFrameGravity(float displaySizeW, float displaySizeH, float width, float height, int horizontalGravity, int verticalGravity)
	{
		setFrameGravity(getAppID(), displaySizeW, displaySizeH, width, height, horizontalGravity, verticalGravity);
	}

	public void setFrameGravity(string appId, float displaySizeW, float displaySizeH, float width, float height, int horizontalGravity, int verticalGravity)
	{
		float num = (float)Screen.width / displaySizeW;
		float num2 = width * num;
		float num3 = height * num;
		float screenPositionByGravity = getScreenPositionByGravity(horizontalGravity, Screen.width, num2);
		float screenPositionByGravity2 = getScreenPositionByGravity(verticalGravity, Screen.height, num3);
		setMovieNativeAdViewFrame(appId, screenPositionByGravity, screenPositionByGravity2, num2, num3);
	}

	public void setFitWidthFrame(float displaySizeH, float height, int verticalGravity)
	{
		setFitWidthFrame(getAppID(), displaySizeH, height, verticalGravity);
	}

	public void setFitWidthFrame(string appId, float displaySizeH, float height, int verticalGravity)
	{
		float num = (float)Screen.height / displaySizeH;
		float num2 = height * num;
		float width = Screen.width;
		float screenPositionByGravity = getScreenPositionByGravity(verticalGravity, Screen.height, num2);
		setMovieNativeAdViewFrame(appId, 0f, screenPositionByGravity, width, num2);
	}

	public void playMovieNativeAdView()
	{
		playMovieNativeAdViewNative(getAppID());
	}

	public void playMovieNativeAdViewNative(string appId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			makeInstance_AdfurikunMovieNativeAdViewController().CallStatic("play", appId);
		}
	}

	public void hideMovieNativeAdView()
	{
		hideMovieNativeAdView(getAppID());
	}

	public void hideMovieNativeAdView(string appId)
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			makeInstance_AdfurikunMovieNativeAdViewController().CallStatic("hide", appId);
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
			callAndroidMovieNativeAdViewMethod("onDestroy");
		}
	}

	[Obsolete("Use Action delegate instead.")]
	public void setMovieNativeAdViewSrcObject(GameObject movieNativeAdViewSrcObject)
	{
		setMovieNativeAdViewSrcObject(movieNativeAdViewSrcObject, getAppID());
	}

	public void setMovieNativeAdViewSrcObject(GameObject movieNativeAdViewSrcObject, string appId)
	{
		mMovieNativeAdViewSrcObject = movieNativeAdViewSrcObject;
	}

	private void Update()
	{
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

	private float getScreenPositionByGravity(int gravity, float screenSize, float contentSize)
	{
		float result = 0f;
		switch (gravity)
		{
		case 0:
			result = 0f;
			break;
		case 1:
			result = (screenSize - contentSize) / 2f;
			break;
		case 2:
			result = screenSize - contentSize;
			break;
		}
		return result;
	}

	public void MovieNativeAdViewCallback(string param_str)
	{
		switch (param_str.Split(';')[0].Split(':')[1])
		{
		case "LoadFinish":
			MovieNativeAdViewLoadFinish(param_str);
			break;
		case "LoadError":
			MovieNativeAdViewLoadError(param_str);
			break;
		case "PlayStart":
			MovieNativeAdViewPlayStart(param_str);
			break;
		case "PlayFinish":
			MovieNativeAdViewPlayFinish(param_str);
			break;
		case "PlayFail":
			MovieNativeAdViewPlayError(param_str);
			break;
		}
	}

	private void MovieNativeAdViewLoadFinish(string param_str)
	{
		string text = param_str.Split(';')[1].Split(':')[1];
		string value = "";
		onLoadFinish.NullSafe(text);
		ADF_MovieStatus aDF_MovieStatus = ADF_MovieStatus.LoadFinish;
		ArrayList arrayList = new ArrayList();
		arrayList.Add((int)aDF_MovieStatus);
		arrayList.Add(text);
		arrayList.Add(value);
		if (mMovieNativeAdViewSrcObject != null)
		{
			mMovieNativeAdViewSrcObject.SendMessage("MovieNativeAdViewCallback", arrayList);
		}
	}

	private void MovieNativeAdViewLoadError(string param_str)
	{
		string[] array = param_str.Split(';');
		string text = array[1].Split(':')[1];
		string text2 = "";
		if (array.Length > 2)
		{
			text2 = array[2].Split(':')[1];
		}
		onLoadError.NullSafe(text, text2);
		ADF_MovieStatus aDF_MovieStatus = ADF_MovieStatus.LoadError;
		ArrayList arrayList = new ArrayList();
		arrayList.Add((int)aDF_MovieStatus);
		arrayList.Add(text);
		arrayList.Add(text2);
		if (mMovieNativeAdViewSrcObject != null)
		{
			mMovieNativeAdViewSrcObject.SendMessage("MovieNativeAdViewCallback", arrayList);
		}
	}

	private void MovieNativeAdViewPlayStart(string param_str)
	{
		string arg = param_str.Split(';')[1].Split(':')[1];
		onPlayStart.NullSafe(arg);
	}

	private void MovieNativeAdViewPlayFinish(string param_str)
	{
		string[] array = param_str.Split(';');
		string arg = array[1].Split(':')[1];
		bool arg2 = true;
		if (array.Length > 2 && array[2].Split(':')[1] == "false")
		{
			arg2 = false;
		}
		onPlayFinish.NullSafe(arg, arg2);
	}

	private void MovieNativeAdViewPlayError(string param_str)
	{
		string[] array = param_str.Split(';');
		string arg = array[1].Split(':')[1];
		string arg2 = "";
		if (array.Length > 2)
		{
			arg2 = array[2].Split(':')[1];
		}
		onPlayError.NullSafe(arg, arg2);
	}

	private AndroidJavaClass makeInstance_AdfurikunMovieNativeAdViewController()
	{
		return new AndroidJavaClass("jp.tjkapp.adfurikunsdk.moviereward.unityplugin.AdfurikunMovieNativeAdViewController");
	}

	private void callAndroidMovieNativeAdViewMethod(string methodName)
	{
		makeInstance_AdfurikunMovieNativeAdViewController().CallStatic(methodName);
	}
}
