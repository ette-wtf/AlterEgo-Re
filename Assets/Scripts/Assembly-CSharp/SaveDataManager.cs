using System;
using System.Collections;
using App;
using NCMB;
using UnityEngine;

public class SaveDataManager
{
	private const string CLASS_NAME = "SaveData";

	private static readonly string[] EXCEPTION_KEY = new string[6] { "acl", "objectId", "createDate", "updateDate", "LoadEnable", "IsInfinity" };

	public static DateTime SaveDataTime
	{
		get
		{
			if (PlayerPrefs.HasKey("SaveDataTime"))
			{
				return DateTime.FromBinary(Convert.ToInt64(PlayerPrefs.GetString("SaveDataTime")));
			}
			return DateTime.MinValue;
		}
		set
		{
			PlayerPrefs.SetString("SaveDataTime", value.ToBinary().ToString());
		}
	}

	public static IEnumerator SetUserID()
	{
		NCMBObject ncmbData = new NCMBObject("SaveData");
		string resultMessage = "";
		try
		{
			ncmbData.SaveAsync(delegate(NCMBException e)
			{
				if (e == null)
				{
					Settings.SaveDataID = ncmbData.ObjectId;
					resultMessage = "SUCCESS";
				}
				else
				{
					resultMessage = "ERROR";
				}
			});
		}
		catch (Exception ex)
		{
			Debug.Log("NCMB:SaveAsync " + ex.ToString());
			resultMessage = "ERROR";
		}
		yield return new WaitUntil(() => resultMessage != "");
	}

	public static IEnumerator Save(Action<string> callback)
	{
		string resultMessage = "";
		NCMBObject saveData = new NCMBObject("SaveData");
		if (string.IsNullOrEmpty(Settings.SaveDataID))
		{
			resultMessage = "SUCCESS";
		}
		else
		{
			saveData.ObjectId = Settings.SaveDataID;
			try
			{
				saveData.FetchAsync(delegate(NCMBException e)
				{
					resultMessage = "SUCCESS";
					if (e != null)
					{
						saveData.ObjectId = null;
					}
				});
			}
			catch (Exception ex)
			{
				Debug.LogError("NCMB:FetchAsync " + ex.ToString());
				resultMessage = "ERROR";
			}
		}
		yield return new WaitUntil(() => resultMessage != "");
		if (resultMessage == "SUCCESS")
		{
			yield return SaveToNCMB(saveData, delegate(string ret)
			{
				resultMessage = ret;
			});
		}
		callback(resultMessage);
	}

	private static IEnumerator SaveToNCMB(NCMBObject target, Action<string> callback)
	{
		string resultMessage = "";
		string[] statusStringArray = StringStatusConverter.GetStatusStringArray(SaveDataInfo.TypeList, false, false);
		string text = "UserID\t" + statusStringArray[2];
		string text2 = AnalyticsManager.GetUserID() + "\t" + statusStringArray[1];
		string[] array = text.Split('\t');
		string[] array2 = text2.Split('\t');
		for (int i = 0; i < array.Length - 1; i++)
		{
			target[array[i]] = array2[i];
		}
		target["LoadEnable"] = true;
		try
		{
			target.SaveAsync(delegate(NCMBException e)
			{
				if (e == null)
				{
					Settings.SaveDataID = target.ObjectId;
					SaveDataTime = target.UpdateDate ?? DateTime.MinValue;
					resultMessage = "SUCCESS";
				}
				else
				{
					resultMessage = "ERROR";
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogError("NCMB:SaveAsync " + ex.ToString());
			resultMessage = "ERROR";
		}
		yield return new WaitUntil(() => resultMessage != "");
		callback(resultMessage);
	}

	public static IEnumerator Load(string loadID, Action<bool> callback)
	{
		string resultMessage = "";
		NCMBObject loadData = new NCMBObject("SaveData");
		loadData.ObjectId = loadID;
		try
		{
			loadData.FetchAsync(delegate(NCMBException e)
			{
				if (e == null)
				{
					if (loadData.ContainsKey("LoadEnable") && (bool)loadData["LoadEnable"])
					{
						resultMessage = "SUCCESS";
					}
					else
					{
						resultMessage = "ERROR";
					}
				}
				else
				{
					resultMessage = "ERROR";
				}
			});
		}
		catch (Exception ex)
		{
			Debug.LogError("NCMB:FetchAsync " + ex.ToString());
			resultMessage = "ERROR";
		}
		yield return new WaitUntil(() => resultMessage != "");
		if (resultMessage == "ERROR")
		{
			callback(false);
			yield break;
		}
		foreach (string key in loadData.Keys)
		{
			if (Array.IndexOf(EXCEPTION_KEY, key) < 0)
			{
				StringStatusConverter.SetEachProperty(key, (string)loadData[key]);
			}
		}
		if (loadData.ContainsKey("IsInfinity") && bool.Parse((string)loadData["IsInfinity"]))
		{
			callback(true);
			yield break;
		}
		loadData["LoadEnable"] = false;
		try
		{
			loadData.SaveAsync();
		}
		catch (Exception ex2)
		{
			Debug.LogError("NCMB:SaveAsync " + ex2.ToString());
		}
		callback(true);
	}
}
