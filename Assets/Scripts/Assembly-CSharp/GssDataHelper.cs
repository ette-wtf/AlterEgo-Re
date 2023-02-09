using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GssDataHelper : MonoBehaviour
{
	public class TableData
	{
		public string ssID;

		public string sheet;

		public string data;

		public string cell;

		public string option;

		public TableData(string ssID, string sheet, string data, string cell, string option = "")
		{
			this.ssID = ssID;
			this.sheet = sheet;
			this.data = data;
			this.cell = cell;
			this.option = option;
		}
	}

	private static GssDataHelper Instance;

	private static string GssTextData;

	public void Awake()
	{
		if (!(Instance != null))
		{
			Instance = this;
			Init();
		}
	}

	public void OnDestroy()
	{
		Instance = null;
	}

	public static void Init()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("parameters");
		if (textAsset != null)
		{
			GssTextData = textAsset.text;
		}
	}

	public static IEnumerator GetText(string spreadSheetID, string sheet, Action<string> callback, string option = null)
	{
		string text = "https://script.google.com/macros/s/AKfycbx0OgWjabmLpq7UnE3HxZIRjAxlZs1-GMbJX5c9VWFZV0JDsmM/exec";
		text = text + "?ssID=" + spreadSheetID;
		text = text + "&sheet=" + sheet;
		text = text + "&option=" + option;
		using (UnityWebRequest www = UnityWebRequest.Get(text))
		{
			www.SendWebRequest();
			while (!www.isDone)
			{
				yield return null;
			}
			callback(www.downloadHandler.text);
		}
	}

	public static void PostData(string spreadSheetID, string sheet, string data, string cell = "", string option = "")
	{
		if (Instance != null)
		{
			Instance.StartCoroutine(PostDataCoroutine(spreadSheetID, sheet, data, cell, option));
			return;
		}
		GameObject caller = new GameObject("GssDataHelperObject");
		caller.AddComponent<GssDataHelper>().StartCoroutine(PostDataCoroutine(spreadSheetID, sheet, data, cell, option, delegate
		{
			UnityEngine.Object.Destroy(caller);
		}));
	}

	public static IEnumerator PostDataCoroutine(string spreadSheetID, string sheet, string data, string cell = "", string option = "", Action callback = null)
	{
		UnityWebRequest unityWebRequest = MakeRequestPostData(spreadSheetID, sheet, data, cell, option);
		UnityWebRequestAsyncOperation async = unityWebRequest.SendWebRequest();
		while (!async.isDone)
		{
			yield return null;
		}
		if (callback != null)
		{
			callback();
		}
	}

	private static UnityWebRequest MakeRequestPostData(string spreadSheetID, string sheet, string data, string cell = "", string option = "")
	{
		UnityWebRequest unityWebRequest = new UnityWebRequest("https://script.google.com/macros/s/AKfycbx0OgWjabmLpq7UnE3HxZIRjAxlZs1-GMbJX5c9VWFZV0JDsmM/exec", "POST");
		TableData obj = new TableData(spreadSheetID, sheet, data, cell, option);
		byte[] bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(obj));
		unityWebRequest.uploadHandler = new UploadHandlerRaw(bytes);
		unityWebRequest.SetRequestHeader("Content-Type", "application/json");
		return unityWebRequest;
	}

	public static string[][] GetSheet(string sheet)
	{
		if (GssTextData == null)
		{
			Init();
		}
		return (from x in GssTextData.Split('\n')
			where x.StartsWith(sheet + "\t")
			select x.Replace(sheet + "\t", "").Replace("<br>", "\n").Split('\t')).ToArray();
	}

	public static string[] GetData(string sheet, string key)
	{
		if (GssTextData == null)
		{
			Init();
		}
		IEnumerable<string[]> source = from x in GssTextData.Split('\n')
			where x.StartsWith(sheet + "\t" + key + "\t")
			select x.Replace(sheet + "\t", "").Replace("<br>", "\n").Split('\t');
		if (source.ToArray().Length != 1)
		{
			Debug.LogError("Error! - GssDataHelper#GetDataByKey sheet=" + sheet + " key= " + key);
			return null;
		}
		return source.ToArray()[0];
	}

	public static IEnumerator Overwrite()
	{
		yield break;
	}
}
