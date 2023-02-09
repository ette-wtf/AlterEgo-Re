using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace NCMB.Internal
{
	internal static class NCMBDebug
	{
		private static string newLine = "\n";

		[Conditional("DEBUG")]
		public static void Log(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		[Conditional("DEBUG")]
		public static void LogWarning(string message)
		{
			UnityEngine.Debug.LogWarning(message);
		}

		[Conditional("DEBUG")]
		public static void LogError(string message)
		{
			UnityEngine.Debug.LogError(message);
		}

		[Conditional("DEBUG")]
		public static void LogError(object message, object context)
		{
		}

		[Conditional("DEBUG")]
		public static void List(string title, IList list)
		{
			string text = null;
			text += string.Format(title + newLine);
			for (int i = 0; i < list.Count; i++)
			{
				text += string.Format("【" + i + "】" + list[i].ToString() + "{0}", (i < list.Count - 1) ? newLine : "");
			}
			UnityEngine.Debug.Log(text);
		}

		[Conditional("DEBUG")]
		public static void Dictionary<T, K>(string title, Dictionary<T, K> dictionary)
		{
			int num = 0;
			string text = null;
			text += string.Format(title + newLine);
			foreach (KeyValuePair<T, K> item in dictionary)
			{
				text += string.Format("【" + num + "】 Key : " + item.Key.ToString() + " Value : " + item.Value.ToString() + "{0}", (num < dictionary.Count - 1) ? newLine : "");
				num++;
			}
			UnityEngine.Debug.Log(text);
		}
	}
}
