using System;
using System.Collections;
using App;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdModule
{
	public class AdEventArgs : EventArgs
	{
		public string Type { get; set; }

		public string Message { get; set; }
	}

	public bool IsLoaded;

	public bool IsShowing;

	protected string Type;

	private string UnitId;

	protected bool IsRewarded;

	private bool IsWaiting;

	private static float _Density;

	private int RetryCount = 5;

	private Vector2 PixelSize = Vector2.zero;

	private BannerView AdMobView;

	private RewardBasedVideoAd AdMobVideo;

	private AdfurikunMovieNativeAdViewUtility AdfuriNative;

	private AdfurikunMovieRewardUtility AdfuriReward;

	private static float Density
	{
		get
		{
			if (_Density == 0f)
			{
				BannerView bannerView = new BannerView("", AdSize.Banner, AdPosition.Center);
				_Density = bannerView.GetHeightInPixels() / 50f;
				Debug.Log("AdModule:_Density=" + _Density + "\nScreenSize=" + GetScreenSize().ToString());
				bannerView.Destroy();
			}
			return _Density;
		}
	}

	public event EventHandler<AdEventArgs> OnAdEvent;

	public AdModule()
	{
	}

	public AdModule(AdManager mgr, string type, string[] args)
	{
		Type = type;
		switch (type)
		{
		case "AdMobBanner":
		{
			MobileAds.Initialize(args[0]);
			UnitId = args[1];
			AdSize smartBanner = AdSize.SmartBanner;
			AdMobView = new BannerView(UnitId, smartBanner, AdPosition.Bottom);
			AdMobView.OnAdLoaded += OnLoadedAdMob;
			AdMobView.OnAdFailedToLoad += OnFailedAdMob;
			OnLoadedAdMob(null, null);
			break;
		}
		case "AdMobRectangle":
		{
			MobileAds.Initialize(args[0]);
			UnitId = args[1];
			AdSize mediumRectangle = AdSize.MediumRectangle;
			Vector2 adjustPos = Vector2.zero;
			if (args.Length > 3)
			{
				adjustPos = AppUtil.Vector2(args[3]);
			}
			Vector2Int positionCenter = GetPositionCenter(mediumRectangle, adjustPos);
			AdMobView = new BannerView(UnitId, mediumRectangle, positionCenter.x, positionCenter.y);
			if (args.Length > 2)
			{
				Vector2 canvas = AppUtil.Vector2(args[2]);
				Vector2 vector = SafeAreaAdjuster.CanvasToPixel(canvas);
				if (AdMobView.GetWidthInPixels() > vector.x || AdMobView.GetHeightInPixels() > vector.y)
				{
					Vector2 vector2 = CanvasToPoint(canvas);
					mediumRectangle = new AdSize((int)vector2.x, (int)vector2.y);
					positionCenter = GetPositionCenter(mediumRectangle, adjustPos);
					AdMobView.Destroy();
					AdMobView = new BannerView(UnitId, mediumRectangle, positionCenter.x, positionCenter.y);
				}
			}
			AdMobView.OnAdLoaded += OnLoadedAdMob;
			AdMobView.OnAdFailedToLoad += OnFailedAdMob;
			break;
		}
		case "AdMobReward":
			MobileAds.Initialize(args[0]);
			UnitId = args[1];
			AdMobVideo = RewardBasedVideoAd.Instance;
			AdMobVideo.OnAdLoaded += OnLoadedAdMob;
			AdMobVideo.OnAdFailedToLoad += OnFailedAdMob;
			AdMobVideo.OnAdRewarded += OnRewardAdMob;
			AdMobVideo.OnAdClosed += OnClosedAdMob;
			break;
		case "AdfuriNative":
		{
			GameObject gameObject2 = new GameObject("AdfurikunMovieNativeAdViewUtility");
			gameObject2.transform.SetParent(mgr.transform);
			AdfuriNative = gameObject2.AddComponent<AdfurikunMovieNativeAdViewUtility>();
			AdfuriNative.config = new AdfurikunMovieNativeAdViewAdConfig();
			AdfuriNative.config.androidAppID = args[0];
			AdfuriNative.config.iPhoneAppID = args[0];
			AdfuriNative.onLoadFinish = OnLoadAdfuri;
			AdfuriNative.onLoadError = OnLoadFailedAdfuri;
			AdfuriNative.onPlayStart = OnStartAdfuri;
			AdfuriNative.onPlayFinish = OnFinishedAdfuri;
			AdfuriNative.onPlayError = OnPlayFailedAdfuri;
			break;
		}
		case "AdfuriReward":
		{
			GameObject gameObject = new GameObject("AdfurikunMovieRewardUtility");
			gameObject.transform.SetParent(mgr.transform);
			AdfuriReward = gameObject.AddComponent<AdfurikunMovieRewardUtility>();
			AdfuriReward.config = new AdfurikunMovieRewardAdConfig();
			AdfuriReward.config.androidAppID = args[0];
			AdfuriReward.config.iPhoneAppID = args[0];
			AdfuriReward.initializeMovieReward();
			AdfuriReward.onPrepareSuccess = OnLoadAdfuri;
			AdfuriReward.onStartPlaying = OnStartAdfuriReward;
			AdfuriReward.onFinishPlaying = OnFinishedAdfuriReward;
			AdfuriReward.onFailedPlaying = OnPlayFailedAdfuriReward;
			AdfuriReward.onCloseAd = OnClosedAdfuriReward;
			break;
		}
		}
		Load();
	}

	public void Load(bool waiting = false)
	{
		IsLoaded = false;
		IsWaiting = waiting;
		switch (Type)
		{
		case "AdMobBanner":
		case "AdMobRectangle":
		{
			AdRequest adMobRequest2 = GetAdMobRequest();
			AdMobView.LoadAd(adMobRequest2);
			Debug.Log("Load AdMobView pixelSize=" + AdMobView.GetWidthInPixels() + "," + AdMobView.GetHeightInPixels());
			if (!waiting)
			{
				AdMobView.Hide();
			}
			break;
		}
		case "AdMobReward":
		{
			AdRequest adMobRequest = GetAdMobRequest();
			AdMobVideo.LoadAd(adMobRequest, UnitId);
			break;
		}
		case "SkipReward":
		case "FakeReward":
			IsLoaded = true;
			break;
		case "AdfuriNative":
			AdfuriNative.loadMovieNativeAdView();
			break;
		}
	}

	public void Hide()
	{
		switch (Type)
		{
		case "AdMobBanner":
			AdMobView.Hide();
			break;
		case "AdMobRectangle":
			AdMobView.Hide();
			Load();
			break;
		case "AdfuriNative":
			AdfuriNative.hideMovieNativeAdView();
			Load();
			break;
		}
		IsWaiting = false;
		IsShowing = false;
	}

	public virtual void Show()
	{
		IsRewarded = false;
		switch (Type)
		{
		case "AdMobBanner":
			if (IsLoaded)
			{
				AdMobView.Show();
				break;
			}
			RetryCount = 1;
			Load(true);
			break;
		case "AdMobRectangle":
			AdMobView.Show();
			break;
		case "AdMobReward":
			if (IsLoaded)
			{
				AdMobVideo.Show();
			}
			break;
		case "SkipReward":
			OnEvent("OnFinished");
			OnEvent("OnClosed");
			break;
		case "FakeReward":
			GameObject.Find("AppCommon").GetComponent<AdManager>().StartCoroutine(StartFakeAdMovie());
			break;
		case "AdfuriNative":
			if (IsLoaded)
			{
				float height = Mathf.Min((float)Screen.width * 0.5625f, (float)Screen.height * 0.375f);
				AdfuriNative.setFitWidthFrame(Screen.height, height, 2);
				AdfuriNative.playMovieNativeAdView();
			}
			else
			{
				Load(true);
			}
			break;
		case "AdfuriReward":
			AdfuriReward.playMovieReward();
			break;
		}
		IsShowing = true;
	}

	public void Destroy()
	{
		switch (Type)
		{
		case "AdMobBanner":
			AdMobView.Destroy();
			SafeAreaAdjuster.BottomBannerRect = default(Rect);
			break;
		case "AdMobRectangle":
			AdMobView.Destroy();
			break;
		case "AdfuriNative":
			AdfuriNative.disposeResource();
			UnityEngine.Object.Destroy(AdfuriNative.gameObject);
			break;
		case "AdfuriReward":
			AdfuriReward.disposeResource();
			UnityEngine.Object.Destroy(AdfuriReward.gameObject);
			break;
		}
	}

	public virtual bool IsReady()
	{
		if (Type == "AdfuriReward")
		{
			return AdfuriReward.isPreparedMovieReward();
		}
		bool isLoaded = IsLoaded;
		if (!isLoaded)
		{
			RetryCount = 5;
			Load();
		}
		return isLoaded;
	}

	public Vector2 GetSize()
	{
		string type = Type;
		if (type == "AdMobBanner" || type == "AdMobRectangle")
		{
			return SafeAreaAdjuster.PixelToCanvas(PixelSize);
		}
		return Vector2.one * -1f;
	}

	protected void OnEvent(string message)
	{
		Debug.Log("AdModule#OnEvent " + Type + " " + message);
		if (this.OnAdEvent == null)
		{
			return;
		}
		AdEventArgs adEventArgs = new AdEventArgs();
		adEventArgs.Type = Type;
		adEventArgs.Message = message;
		this.OnAdEvent(null, adEventArgs);
		switch (message)
		{
		case "OnLoaded":
			IsLoaded = true;
			if (IsWaiting)
			{
				Show();
			}
			break;
		case "OnFinished":
			IsRewarded = true;
			break;
		case "OnClosed":
		{
			AdEventArgs adEventArgs2 = new AdEventArgs();
			adEventArgs2.Type = Type;
			if (IsRewarded)
			{
				adEventArgs2.Message = "OnReward";
			}
			else
			{
				adEventArgs2.Message = "NoReward";
			}
			this.OnAdEvent(null, adEventArgs2);
			break;
		}
		}
	}

	private IEnumerator StartFakeAdMovie()
	{
		DialogManager.ShowDialog("FakeMovieAdScreen");
		while (DialogManager.IsShowing())
		{
			yield return null;
		}
		OnEvent("OnFinished");
		OnEvent("OnClosed");
	}

	private AdRequest GetAdMobRequest()
	{
		return new AdRequest.Builder().AddTestDevice("SIMULATOR").AddTestDevice("2B3B9715CF716A80B923B0A8FDEC4C6E").AddTestDevice("603FCAEF19A587D0DE46759DA4E29B05")
			.AddTestDevice("65C19A2EEDACE8CFE707F06CE8ADCBB9")
			.AddTestDevice("c279074adac7c39288a6b9cfb587d913")
			.AddTestDevice("6423d13eae0bdae39e50249c3f682d87")
			.AddTestDevice("DDA86E100D011A140E624641373F350D")
			.Build();
	}

	private Vector2Int GetPositionCenter(AdSize size, Vector2 adjustPos)
	{
		Vector2Int one = Vector2Int.one;
		one.x = PixelToPoint((GetScreenSize().x - (float)PointToPixel(size.Width)) / 2f - (float)CanvasToPixel(adjustPos.x, false));
		one.y = PixelToPoint((GetScreenSize().y - (float)PointToPixel(size.Height)) / 2f - (float)CanvasToPixel(adjustPos.y));
		float safeAreaTop = GetSafeAreaTop();
		one.y -= PixelToPoint(safeAreaTop);
		return one;
	}

	public static Vector2 GetScreenSize()
	{
		return new Vector2(Screen.width, Screen.height);
	}

	public static float GetSafeAreaTop()
	{
		Rect safeArea = SafeAreaAdjuster.GetSafeArea();
		GetScreenSize();
		return GetScreenSize().y - safeArea.y - safeArea.height;
	}

	private static Vector2 GetPixelRate()
	{
		float num = 0.5625f;
		float x = GetScreenSize().x;
		float y = GetScreenSize().y;
		float num2 = x / y;
		Vector2 one = Vector2.one;
		if (num2 < num)
		{
			one.y = x / 1080f;
			one.x = x / 1080f;
		}
		else
		{
			one.x = y / 1920f;
			one.y = y / 1920f;
		}
		return one;
	}

	public static int CanvasToPixel(float canvas, bool height = true)
	{
		if (height)
		{
			return (int)(canvas * GetPixelRate().y);
		}
		return (int)(canvas * GetPixelRate().x);
	}

	private int PixelToPoint(float pixel)
	{
		return (int)(pixel / Density);
	}

	private int PointToPixel(float point)
	{
		return (int)(point * Density);
	}

	private Vector2 PointToCanvas(Vector2 point)
	{
		return SafeAreaAdjuster.PixelToCanvas(point) * Density;
	}

	private Vector2 CanvasToPoint(Vector2 canvas)
	{
		return SafeAreaAdjuster.CanvasToPixel(canvas) / Density;
	}

	public void OnLoadedAdMob(object sender, EventArgs args)
	{
		if (!AdInfo.ENABLE)
		{
			return;
		}
		OnEvent("OnLoaded");
		PixelSize.x = AdMobView.GetWidthInPixels();
		PixelSize.y = AdMobView.GetHeightInPixels();
		if (!(Type != "AdMobBanner"))
		{
			Rect bottomBannerRect = default(Rect);
			bottomBannerRect.width = GetSize().x;
			bottomBannerRect.height = GetSize().y;
			if (bottomBannerRect.height < 50f)
			{
				bottomBannerRect.height = 150f;
			}
			SafeAreaAdjuster.BottomBannerRect = bottomBannerRect;
		}
	}

	public void OnFailedAdMob(object sender, AdFailedToLoadEventArgs args)
	{
		OnEvent("OnFailed " + args.Message);
		if (RetryCount > 0)
		{
			Load();
			RetryCount--;
		}
	}

	public void OnRewardAdMob(object sender, Reward reward)
	{
		OnEvent("OnFinished");
	}

	public void OnClosedAdMob(object sender, EventArgs args)
	{
		OnEvent("OnClosed");
		Load();
	}

	public void OnLoadAdfuri(string appID)
	{
		Debug.Log("AdModule#OnLoadAdfuri");
		OnEvent("OnLoaded");
	}

	public void OnStartAdfuri(string appID)
	{
		Debug.Log("AdModule#OnStartAdfuri");
		OnEvent("OnStarted");
	}

	public void OnFinishedAdfuri(string appID, bool isVideo)
	{
		Debug.Log("AdModule#OnFinishedAdfuri");
		OnEvent("OnFinished " + (isVideo ? "Video" : "NoVideo"));
	}

	public void OnLoadFailedAdfuri(string appID, string errorCode)
	{
		Debug.Log("AdModule#OnLoadFailedAdfuri");
		OnEvent("OnFailed(Load) " + errorCode);
	}

	public void OnPlayFailedAdfuri(string appID, string errorCode)
	{
		Debug.Log("AdModule#OnPlayFailedAdfuri");
		OnEvent("OnFailed(Play)");
	}

	public void OnStartAdfuriReward(string appID, string adnetworkKey)
	{
		OnEvent("OnStarted " + adnetworkKey);
	}

	public void OnFinishedAdfuriReward(string appID, string adnetworkKey)
	{
		OnEvent("OnFinished");
	}

	public void OnPlayFailedAdfuriReward(string appID, string adnetworkKey)
	{
		OnEvent("OnFailed(Play)");
	}

	public void OnClosedAdfuriReward(string appID, string adnetworkKey)
	{
		OnEvent("OnClosed");
	}
}
