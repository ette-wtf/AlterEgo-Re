using System;
using UnityEngine;

public class SafeAreaAdjuster : MonoBehaviour
{
	private enum TYPE
	{
		NONE = 0,
		EXPAND_TOP = 1,
		SHRINK_Y = 2,
		MOVE_DOWN = 3,
		MOVE_UP = 4,
		EXPAND_BOTTOM = 5,
		SHRINK_TOP = 6,
		SHRINK_BOTTOM = 7
	}

	public delegate void OnAction(float size1, float size2);

	private Vector2 SizeOrg;

	private Vector2 PositionOrg;

	[SerializeField]
	private bool ShowAd = true;

	private static int _SafeAreaHeight;

	private static float FooterSize;

	private static Rect SafeArea = Rect.zero;

	public static Rect _BottomBannerRect = default(Rect);

	public static OnAction OnSizeChanged;

	[SerializeField]
	private TYPE Type;

	public static int SafeAreaHeight
	{
		get
		{
			return _SafeAreaHeight;
		}
		set
		{
			_SafeAreaHeight = value;
			Debug.Log("SafeAreaHeight=" + value);
		}
	}

	public static Rect BottomBannerRect
	{
		get
		{
			return _BottomBannerRect;
		}
		set
		{
			_BottomBannerRect = value;
			UpdateLayout();
		}
	}

	private void Start()
	{
		SizeOrg = GetComponent<RectTransform>().sizeDelta;
		PositionOrg = GetComponent<RectTransform>().anchoredPosition;
		OnSizeChanged = (OnAction)Delegate.Combine(OnSizeChanged, new OnAction(AdjustLayout));
		UpdateLayout();
	}

	private void OnDestroy()
	{
		OnSizeChanged = (OnAction)Delegate.Remove(OnSizeChanged, new OnAction(AdjustLayout));
	}

	private void AdjustLayout(float bannerHeight, float navBarHeight)
	{
		Vector2 sizeOrg = SizeOrg;
		Vector2 positionOrg = PositionOrg;
		float b = PixelToCanvas(GetSafeAreaTop());
		b = Mathf.Max(0f, b);
		float b2 = PixelToCanvas(GetSafeAreaBottom()) + navBarHeight;
		b2 = Mathf.Max(0f, b2);
		if (ShowAd)
		{
			b2 += bannerHeight;
		}
		FooterSize = b2;
		switch (Type)
		{
		case TYPE.EXPAND_TOP:
			sizeOrg.y += b;
			GetComponent<RectTransform>().sizeDelta = sizeOrg;
			break;
		case TYPE.EXPAND_BOTTOM:
			sizeOrg.y += b2;
			GetComponent<RectTransform>().sizeDelta = sizeOrg;
			break;
		case TYPE.SHRINK_TOP:
			sizeOrg.y -= b;
			positionOrg.y -= b / 2f;
			GetComponent<RectTransform>().sizeDelta = sizeOrg;
			GetComponent<RectTransform>().anchoredPosition = positionOrg;
			break;
		case TYPE.SHRINK_BOTTOM:
			sizeOrg.y -= b2;
			positionOrg.y += b2 / 2f;
			GetComponent<RectTransform>().sizeDelta = sizeOrg;
			GetComponent<RectTransform>().anchoredPosition = positionOrg;
			break;
		case TYPE.SHRINK_Y:
			sizeOrg.y -= b + b2;
			positionOrg.y -= (b - b2) / 2f;
			GetComponent<RectTransform>().sizeDelta = sizeOrg;
			GetComponent<RectTransform>().anchoredPosition = positionOrg;
			break;
		case TYPE.MOVE_DOWN:
			positionOrg.y -= b;
			GetComponent<RectTransform>().anchoredPosition = positionOrg;
			break;
		case TYPE.MOVE_UP:
			positionOrg.y += b2;
			GetComponent<RectTransform>().anchoredPosition = positionOrg;
			break;
		}
	}

	public static Rect GetSafeArea()
	{
		Rect safeArea = Screen.safeArea;
		if (SafeArea != safeArea)
		{
			SafeArea = safeArea;
			Debug.Log("SafeArea: \nScreenSize=" + GetScreenSize().ToString() + "\nUnity SafeArea=" + Screen.safeArea.ToString() + "\nEGO SafeArea=" + safeArea.ToString());
		}
		return SafeArea;
	}

	public static float GetSafeAreaTop()
	{
		return GetSafeArea().y;
	}

	public static float GetSafeAreaBottom()
	{
		Rect safeArea = GetSafeArea();
		return GetScreenSize().y - safeArea.y - safeArea.height;
	}

	public static float GetFooterSize()
	{
		return FooterSize;
	}

	public static void UpdateLayout()
	{
		if (OnSizeChanged != null)
		{
			float size = new AndroidJavaObject("com.caracolu.appcommon.Util").Call<float>("GetNavigationBarSize", Array.Empty<object>());
			OnSizeChanged(BottomBannerRect.height, size);
		}
	}

	public static Vector2 PixelToCanvas(Vector2 pixel)
	{
		pixel.x /= GetPixelRate().x;
		pixel.y /= GetPixelRate().y;
		return pixel;
	}

	public static float PixelToCanvas(float pixel, bool height = true)
	{
		if (height)
		{
			return pixel / GetPixelRate().y;
		}
		return pixel / GetPixelRate().x;
	}

	public static Vector2 CanvasToPixel(Vector2 canvas)
	{
		canvas.x *= GetPixelRate().x;
		canvas.y *= GetPixelRate().y;
		return canvas;
	}

	public static int CanvasToPixel(float canvas, bool height = true)
	{
		if (height)
		{
			return (int)(canvas * GetPixelRate().y);
		}
		return (int)(canvas * GetPixelRate().x);
	}

	public static Vector2 GetScreenSize()
	{
		return new Vector2(Screen.width, Screen.height);
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
}
