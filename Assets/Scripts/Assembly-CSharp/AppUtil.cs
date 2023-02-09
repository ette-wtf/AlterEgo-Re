using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using App;
using SocialConnector;
using UnityEngine;
using UnityEngine.UI;

public class AppUtil
{
	public class JsonString
	{
		public string[] string_array;

		public JsonString()
		{
			string_array = new string[0];
		}

		public JsonString(string[] data)
		{
			string_array = data;
		}
	}

	public delegate IEnumerator Coroutine();

	[AttributeUsage(AttributeTargets.Class)]
	public class DataRangeAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
	public class HideAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
	public class TitleAttribute : Attribute
	{
		public string Title;

		public TitleAttribute(string title)
		{
			Title = title;
		}
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
	public class ArrayValueAttribute : Attribute
	{
		public string Data;

		public ArrayValueAttribute(string data)
		{
			Data = data;
		}
	}

	public static string[] ToFullWidth = new string[10] { "０", "１", "２", "３", "４", "５", "６", "７", "８", "９" };

	public static IEnumerator FadeIn(UnityEngine.Object target, float time = 0.5f, Action<UnityEngine.Object> callback = null)
	{
		yield return FadeInOut(target, true, time, callback);
	}

	public static IEnumerator FadeOut(UnityEngine.Object target, float time = 0.5f, Action<UnityEngine.Object> callback = null)
	{
		yield return FadeInOut(target, false, time, callback);
	}

	private static IEnumerator FadeInOut(UnityEngine.Object target, bool fadein, float totalTime, Action<UnityEngine.Object> callback)
	{
		if (target == null)
		{
			yield break;
		}
		float start = ((!fadein) ? 1 : 0);
		float end = (fadein ? 1 : 0);
		float gap = end - start;
		SetAlpha(target, start);
		float time = 0f;
		while (true)
		{
			time += Time.unscaledDeltaTime * Settings.GAME_SPEED;
			float num = start + gap * time / totalTime;
			if ((end - num) * gap < 0f)
			{
				break;
			}
			SetAlpha(target, num);
			yield return null;
		}
		SetAlpha(target, end);
		if (callback != null)
		{
			callback(target);
		}
	}

	public static void SetAlphaChildren(GameObject target, float alpha)
	{
		Transform[] componentsInChildren = target.GetComponentsInChildren<Transform>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			SetAlpha(componentsInChildren[i], alpha);
		}
	}

	public static bool SetAlpha(UnityEngine.Object target, float alpha)
	{
		if (target == null)
		{
			return false;
		}
		if (target is GameObject)
		{
			GameObject gameObject = (GameObject)target;
			if (SetAlpha(gameObject.GetComponentInChildren<CanvasGroup>(), alpha))
			{
				return true;
			}
			bool flag = SetAlpha(gameObject.GetComponentInChildren<Image>(), alpha);
			bool flag2 = SetAlpha(gameObject.GetComponentInChildren<SpriteRenderer>(), alpha);
			bool flag3 = SetAlpha(gameObject.GetComponentInChildren<Text>(), alpha);
			bool flag4 = SetAlpha(gameObject.GetComponentInChildren<TextMesh>(), alpha);
			return flag || flag2 || flag3 || flag4;
		}
		if (target is Transform)
		{
			Transform transform = (Transform)target;
			if (SetAlpha(transform.GetComponentInChildren<CanvasGroup>(), alpha))
			{
				return true;
			}
			bool flag5 = SetAlpha(transform.GetComponentInChildren<Image>(), alpha);
			bool flag6 = SetAlpha(transform.GetComponentInChildren<SpriteRenderer>(), alpha);
			bool flag7 = SetAlpha(transform.GetComponentInChildren<Text>(), alpha);
			bool flag8 = SetAlpha(transform.GetComponentInChildren<TextMesh>(), alpha);
			return flag5 || flag6 || flag7 || flag8;
		}
		if (target is Image)
		{
			Image obj = (Image)target;
			Color color = obj.color;
			color.a = alpha;
			obj.color = color;
			return true;
		}
		if (target is Text)
		{
			Text obj2 = (Text)target;
			Color color2 = obj2.color;
			color2.a = alpha;
			obj2.color = color2;
			return true;
		}
		if (target is SpriteRenderer)
		{
			SpriteRenderer obj3 = (SpriteRenderer)target;
			Color color3 = obj3.color;
			color3.a = alpha;
			obj3.color = color3;
			return true;
		}
		if (target is CanvasGroup)
		{
			((CanvasGroup)target).alpha = alpha;
			return true;
		}
		if (target is TextMesh)
		{
			TextMesh obj4 = (TextMesh)target;
			Color color4 = obj4.color;
			color4.a = alpha;
			obj4.color = color4;
			return true;
		}
		return false;
	}

	public static IEnumerator TypeText(string text, UnityEngine.Object target)
	{
		float time = 0f;
		int length = 0;
		string orgText = "";
		if (target is Text)
		{
			orgText = ((Text)target).text;
		}
		else if (target is TextMesh)
		{
			orgText = ((TextMesh)target).text;
		}
		while (length <= text.Length)
		{
			time += Time.deltaTime;
			length = (int)(time * 20f);
			int length2 = Mathf.Min(length, text.Length);
			if (target is Text)
			{
				((Text)target).text = orgText + text.Substring(0, length2);
			}
			else if (target is TextMesh)
			{
				((TextMesh)target).text = orgText + text.Substring(0, length2);
			}
			yield return null;
		}
	}

	public static IEnumerator ShakeObject(GameObject target, float totalTime = 0.25f)
	{
		float strength = ((target.GetComponentInChildren<MaskableGraphic>() == null) ? 0.2f : 20f);
		Vector3 orgPosition = target.transform.position;
		int max = (int)(totalTime / 0.05f);
		for (int i = 0; i < max; i++)
		{
			target.transform.position = new Vector3(orgPosition.x + UnityEngine.Random.Range(0f - strength, strength), orgPosition.y + UnityEngine.Random.Range(0f - strength, strength), orgPosition.z);
			yield return Wait(0.05f);
		}
		target.transform.position = orgPosition;
	}

	public static Vector3 GetMousePosition()
	{
		Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z);
		return Camera.main.ScreenToWorldPoint(position);
	}

	public static IEnumerator Wait(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
	}

	public static IEnumerator WaitRealtime(float waitTime)
	{
		yield return new WaitForSecondsRealtime(waitTime / Settings.GAME_SPEED);
	}

	public static string ConvertToJson(byte[] data)
	{
		return JsonUtility.ToJson(new JsonString(ReadDataSJIS(data, false)), true);
	}

	public static string ConvertSJISToJson(byte[] data)
	{
		return JsonUtility.ToJson(new JsonString(ReadDataSJIS(data)), true);
	}

	public static string[] ReadDataSJIS(byte[] data, bool sjis = true)
	{
		string path = Application.temporaryCachePath + "/sjisdata.txt";
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.WriteAllBytes(path, data);
		FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		StreamReader streamReader = new StreamReader(fileStream, sjis ? Encoding.GetEncoding(932) : Encoding.UTF8);
		List<string> list = new List<string>();
		while (!streamReader.EndOfStream)
		{
			string text = streamReader.ReadLine();
			if (sjis)
			{
				text = text.Replace("(?；ω；\uff40)", "(\u00b4；ω；\uff40)").Replace("(\uff40；ω；?)", "(\uff40；ω；\u00b4)");
			}
			list.Add(text);
		}
		streamReader.Close();
		fileStream.Close();
		return list.ToArray();
	}

	public static IEnumerator MoveEasingFloat(float startValue, float endValue, Action<float> callback, bool unscaled = true, float time = 0.75f, EasingFunction.Ease type = EasingFunction.Ease.EaseInOutQuint, Action callback2 = null)
	{
		callback(startValue);
		EasingFunction.Function easingFunction = EasingFunction.GetEasingFunction(type);
		float t = 0f;
		while (t <= time)
		{
			yield return new WaitForEndOfFrame();
			t = ((!unscaled) ? (t + Time.deltaTime * Settings.GAME_SPEED) : (t + Time.unscaledDeltaTime * Settings.GAME_SPEED));
			callback(easingFunction(startValue, endValue, t / time));
		}
		callback(endValue);
		if (callback2 != null)
		{
			callback2();
		}
		yield return new WaitForEndOfFrame();
	}

	public static IEnumerator MoveEasingVector3(Vector3 startValue, Vector3 endValue, Action<Vector3> callback, bool unscaled = true, float time = 0.75f, EasingFunction.Ease type = EasingFunction.Ease.EaseInOutQuint, Action callback2 = null)
	{
		callback(startValue);
		EasingFunction.Function easingFunction = EasingFunction.GetEasingFunction(type);
		float t = 0f;
		while (t <= time)
		{
			yield return new WaitForEndOfFrame();
			t = ((!unscaled) ? (t + Time.deltaTime * Settings.GAME_SPEED) : (t + Time.unscaledDeltaTime * Settings.GAME_SPEED));
			Vector3 obj = new Vector3(easingFunction(startValue.x, endValue.x, t / time), easingFunction(startValue.y, endValue.y, t / time), easingFunction(startValue.z, endValue.z, t / time));
			callback(obj);
		}
		callback(endValue);
		if (callback2 != null)
		{
			callback2();
		}
		yield return new WaitForEndOfFrame();
	}

	public static void DelayAction(MonoBehaviour caller, float waitTime, Action waitAction, bool unscaled = true)
	{
		caller.StartCoroutine(DelayAction(waitTime, waitAction, unscaled));
	}

	public static IEnumerator DelayAction(float waitTime, Action waitAction, bool unscaled = true)
	{
		if (unscaled)
		{
			yield return WaitRealtime(waitTime);
		}
		else
		{
			yield return Wait(waitTime);
		}
		waitAction();
	}

	public static void DelayAction(MonoBehaviour caller, float waitTime, IEnumerator waitAction, bool unscaled = true)
	{
		caller.StartCoroutine(DelayAction(waitTime, waitAction, unscaled));
	}

	public static IEnumerator DelayAction(float waitTime, IEnumerator waitAction, bool unscaled = true)
	{
		if (unscaled)
		{
			yield return WaitRealtime(waitTime);
		}
		else
		{
			yield return Wait(waitTime);
		}
		yield return waitAction;
	}

	public static bool HasAttribute(MemberInfo info, Type checkType)
	{
		return Attribute.GetCustomAttributes(info, checkType).Length != 0;
	}

	public static string GetTitle(MemberInfo info)
	{
		Attribute customAttribute = Attribute.GetCustomAttribute(info, typeof(TitleAttribute));
		if (customAttribute == null)
		{
			return info.Name;
		}
		return ((TitleAttribute)customAttribute).Title;
	}

	public static string[] GetValueArray(MemberInfo info)
	{
		Attribute customAttribute = Attribute.GetCustomAttribute(info, typeof(ArrayValueAttribute));
		if (customAttribute == null)
		{
			return null;
		}
		FieldInfo field = info.DeclaringType.GetField(((ArrayValueAttribute)customAttribute).Data);
		if (field != null)
		{
			return (string[])field.GetValue(null);
		}
		return (string[])info.DeclaringType.GetProperty(((ArrayValueAttribute)customAttribute).Data).GetValue(null, null);
	}

	public static List<KeyValuePair<int, int>> ReverseByValue(Dictionary<int, int> dict)
	{
		List<KeyValuePair<int, int>> list = new List<KeyValuePair<int, int>>(dict);
		list.Sort((KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2) => (kvp2.Value == kvp1.Value) ? (kvp1.Key - kvp2.Key) : (kvp2.Value - kvp1.Value));
		return list;
	}

	public static List<KeyValuePair<string, int>> ReverseByValueS(Dictionary<string, int> dict)
	{
		List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>(dict);
		list.Sort((KeyValuePair<string, int> kvp1, KeyValuePair<string, int> kvp2) => kvp2.Value - kvp1.Value);
		return list;
	}

	public static T Shift<T>(IList<T> self)
	{
		T result = self[0];
		self.RemoveAt(0);
		return result;
	}

	public static T Pop<T>(IList<T> self)
	{
		T result = self[self.Count - 1];
		self.RemoveAt(self.Count - 1);
		return result;
	}

	public static T GetAt<T>(IList<T> self, T main, int index = 1)
	{
		int num = self.IndexOf(main);
		num += index;
		if (num < 0)
		{
			num = self.Count - 1;
		}
		else if (num >= self.Count)
		{
			num = 0;
		}
		return self[num];
	}

	public static string ToString(int[] array)
	{
		return string.Join(",", array.Select((int x) => x.ToString()).ToArray());
	}

	public static int SetBitFlag(int baseNum, int flagNum, bool flagOn)
	{
		int num = (int)Mathf.Log(2.1474836E+09f, 2f) + 1;
		if (flagNum >= num)
		{
			Debug.LogError("SetBitFlag() 桁あふれエラー flagNum(" + flagNum + ")>=maxDigit(" + num + ")" + Convert.ToString(int.MaxValue, 2));
		}
		int num2 = 1 << flagNum;
		baseNum = ((!flagOn) ? (baseNum & ~num2) : (baseNum | num2));
		return baseNum;
	}

	public static bool BitFlagIsSet(int baseNum, int flagNum)
	{
		int num = 1 << flagNum;
		return (baseNum & num) != 0;
	}

	public static Vector2 Vector2(string data, float factor = 1f)
	{
		char[] separator = new char[1] { ',' };
		string[] array = data.Split(separator);
		return new Vector2(float.Parse(array[0]) / factor, float.Parse(array[1]) / factor);
	}

	public static IEnumerator WaitAnimation(GameObject animObject, Action callback = null)
	{
		if (!(animObject == null))
		{
			yield return WaitAnimation(animObject.GetComponent<Animator>(), callback);
		}
	}

	public static IEnumerator WaitAnimation(Animator anim, Action callback = null)
	{
		if (!(anim == null))
		{
			if (anim.updateMode == AnimatorUpdateMode.UnscaledTime)
			{
				yield return WaitRealtime(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
			}
			else
			{
				yield return Wait(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
			}
			if (callback != null)
			{
				callback();
			}
		}
	}

	public static Color CreateColor(string htmlColor)
	{
		Color color;
		ColorUtility.TryParseHtmlString(htmlColor, out color);
		return color;
	}

	public static int[] RandomArray(int count)
	{
		return (from tmp in Enumerable.Range(0, count)
			orderby Guid.NewGuid()
			select tmp).ToArray();
	}

	public static int[] RandomArray(int start, int count)
	{
		return (from tmp in Enumerable.Range(start, count)
			orderby Guid.NewGuid()
			select tmp).ToArray();
	}

	public static IEnumerator RectTransformCapture(RectTransform captureTarget)
	{
		yield return new WaitForEndOfFrame();
		string path = "Externals/Log/Temp/" + "Screenshot" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".png";
		Vector3[] array = new Vector3[4];
		captureTarget.GetWorldCorners(array);
		Rect source = new Rect(array[0].x, array[0].y, array[2].x - array[0].x, array[2].y - array[0].y);
		if (captureTarget.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
		{
			Vector2 vector = Camera.main.WorldToScreenPoint(array[0]);
			Vector2 vector2 = Camera.main.WorldToScreenPoint(array[2]);
			source = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
		}
		Texture2D texture2D = new Texture2D((int)source.width, (int)source.height);
		texture2D.ReadPixels(source, 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToPNG();
		File.WriteAllBytes(path, bytes);
	}

	public static void ShareLocalImage(string msg, string url, string resourcePath)
	{
		byte[] bytes = Resources.Load<TextAsset>(resourcePath).bytes;
		string text = Application.persistentDataPath + "/shareimage.jpg";
		File.WriteAllBytes(text, bytes);
		global::SocialConnector.SocialConnector.Share(msg, url, text);
	}

	public static IEnumerator Share(bool showDialog, string msg, string url, RectTransform captureTarget = null)
	{
		yield return new WaitForEndOfFrame();
		Rect source = new Rect(0f, 0f, Screen.width, Screen.height);
		if (captureTarget != null)
		{
			Vector3[] array = new Vector3[4];
			captureTarget.GetWorldCorners(array);
			source = new Rect(array[0].x, array[0].y, array[2].x - array[0].x, array[2].y - array[0].y);
			if (captureTarget.GetComponentInParent<Canvas>().renderMode == RenderMode.ScreenSpaceCamera)
			{
				Vector2 vector = Camera.main.WorldToScreenPoint(array[0]);
				Vector2 vector2 = Camera.main.WorldToScreenPoint(array[2]);
				source = new Rect(vector.x, vector.y, vector2.x - vector.x, vector2.y - vector.y);
				if (source.x < 0f)
				{
					source.width += source.x;
					source.x = 0f;
				}
				if (source.width + source.x > (float)Screen.width)
				{
					source.width = (float)Screen.width - source.x;
				}
				if (source.y < 0f)
				{
					source.height += source.y;
					source.y = 0f;
				}
				if (source.height + source.y > (float)Screen.height)
				{
					source.height = (float)Screen.height - source.y;
				}
			}
		}
		Texture2D texture2D = new Texture2D((int)source.width, (int)source.height);
		texture2D.ReadPixels(source, 0, 0);
		texture2D.Apply();
		byte[] bytes = texture2D.EncodeToJPG();
		string text = Application.persistentDataPath + "/shareimage.jpg";
		File.WriteAllBytes(text, bytes);
		if (showDialog)
		{
			DialogManager.ShowDialog("ShareDialog", msg, url, texture2D, text);
		}
		else
		{
			global::SocialConnector.SocialConnector.Share(msg, url, text);
		}
	}

	public static string[] GetChildArray(string[] parent, int start, bool trim = true, int childCount = 0)
	{
		if (childCount == 0)
		{
			childCount = parent.Length - start;
		}
		for (int num = start + childCount - 1; num >= start; num--)
		{
			if (trim && (parent[num] == null || parent[num] == ""))
			{
				childCount--;
			}
		}
		string[] array = new string[childCount];
		Array.Copy(parent, start, array, 0, childCount);
		return array;
	}

	public static void SetFalse(GameObject target)
	{
		if (target != null)
		{
			target.SetActive(false);
		}
	}

	public static void SetFalse(string name)
	{
		SetFalse(GameObject.Find(name));
	}

	public static float GetOSVersion()
	{
		return new AndroidJavaClass("android.os.Build$VERSION").GetStatic<float>("SDK_INT");
	}

	public static void OpenReviewURL()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=" + BuildInfo.APP_BUNDLE_ID);
	}

	public static bool CallReviewView()
	{
		return false;
	}

	public static bool SetClipBoard(string copyText)
	{
		return new AndroidJavaObject("com.caracolu.appcommon.Util").Call<bool>("SetClipData", new object[1] { copyText });
	}
}
