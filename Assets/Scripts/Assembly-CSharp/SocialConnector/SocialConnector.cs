using System;
using UnityEngine;

namespace SocialConnector
{
	public class SocialConnector
	{
		private static AndroidJavaObject clazz = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

		private static AndroidJavaObject activity = clazz.GetStatic<AndroidJavaObject>("currentActivity");

		private static void _Share(string text, string url, string textureUrl)
		{
			using (AndroidJavaObject androidJavaObject = new AndroidJavaObject("android.content.Intent"))
			{
				androidJavaObject.Call<AndroidJavaObject>("setAction", new object[1] { "android.intent.action.SEND" });
				androidJavaObject.Call<AndroidJavaObject>("setType", new object[1] { string.IsNullOrEmpty(textureUrl) ? "text/plain" : "image/png" });
				if (!string.IsNullOrEmpty(url))
				{
					text = text + "\t" + url;
				}
				if (!string.IsNullOrEmpty(text))
				{
					androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2] { "android.intent.extra.TEXT", text });
				}
				if (!string.IsNullOrEmpty(textureUrl))
				{
					int @static = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
					AndroidJavaObject androidJavaObject4;
					if (24 <= @static)
					{
						AndroidJavaObject androidJavaObject2 = activity.Call<AndroidJavaObject>("getApplicationContext", Array.Empty<object>());
						AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.support.v4.content.FileProvider");
						AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("java.io.File", textureUrl);
						androidJavaObject4 = androidJavaClass.CallStatic<AndroidJavaObject>("getUriForFile", new object[3]
						{
							androidJavaObject2,
							Application.identifier + ".fileprovider",
							androidJavaObject3
						});
					}
					else
					{
						AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.net.Uri");
						AndroidJavaObject androidJavaObject5 = new AndroidJavaObject("java.io.File", textureUrl);
						androidJavaObject4 = androidJavaClass2.CallStatic<AndroidJavaObject>("fromFile", new object[1] { androidJavaObject5 });
					}
					androidJavaObject.Call<AndroidJavaObject>("putExtra", new object[2] { "android.intent.extra.STREAM", androidJavaObject4 });
				}
				AndroidJavaObject androidJavaObject6 = androidJavaObject.CallStatic<AndroidJavaObject>("createChooser", new object[2] { androidJavaObject, "" });
				androidJavaObject6.Call<AndroidJavaObject>("putExtra", new object[2] { "android.intent.extra.EXTRA_INITIAL_INTENTS", androidJavaObject });
				activity.Call("startActivity", androidJavaObject6);
			}
		}

		public static void Share(string text)
		{
			Share(text, null, null);
		}

		public static void Share(string text, string url)
		{
			Share(text, url, null);
		}

		public static void Share(string text, string url, string textureUrl)
		{
			_Share(text, url, textureUrl);
		}
	}
}
