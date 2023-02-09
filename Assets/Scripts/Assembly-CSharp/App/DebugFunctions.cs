using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace App
{
	[AppUtil.Title("デバッグ機能")]
	public static class DebugFunctions
	{
		[AppUtil.Title("Analyticsデータ送信")]
		public static Action SEND_ANALYTICS = delegate
		{
			AnalyticsManager.SendEvent(new string[3] { "デバッグ", "", "" });
		};

		[AppUtil.Title("ロード")]
		public static AppUtil.Coroutine LOAD_GSSDATA = LOAD_GSSDATA_METHOD;

		[AppUtil.Title("シーン切替")]
		[AppUtil.ArrayValue("SCENE_LIST")]
		public static string SCENE
		{
			get
			{
				return SceneManager.GetActiveScene().name;
			}
			set
			{
				Debug.Log("set SCENE " + value);
				SceneManager.LoadScene(value);
			}
		}

		[AppUtil.Hide]
		public static string[] SCENE_LIST
		{
			get
			{
				int sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
				string[] array = new string[sceneCountInBuildSettings];
				for (int i = 0; i < sceneCountInBuildSettings; i++)
				{
					string text = SceneUtility.GetScenePathByBuildIndex(i).Replace("Assets/AppData/Scenes/", "").Replace(".unity", "");
					array[i] = text;
				}
				return array;
			}
		}

		[AppUtil.Title("所持EGO")]
		public static EgoPoint EgoPoint2
		{
			get
			{
				return PlayerStatus.EgoPoint;
			}
			set
			{
				PlayerStatus.EgoPoint = value;
			}
		}

		[AppUtil.Title("時間を進める(h)")]
		public static float TIME_FORWARD
		{
			get
			{
				return 0f;
			}
			set
			{
				TimeManager.Add(TimeManager.TYPE.REAL_TOTAL, TimeSpan.FromHours(value * Settings.GAME_SPEED));
				PlayerStatus.EnableDailyBonus = true;
			}
		}

		private static IEnumerator LOAD_GSSDATA_METHOD()
		{
			string savedata = null;
			yield return GssDataHelper.GetText("1csBkwtdEPOM-dk0kNER5lvOsPDmWs2Gft9y_CGyr898", "SaveData_v2", delegate(string result)
			{
				savedata = result;
			});
			string[] array = savedata.Split('\n')[2].Split('\t');
			string[] array2 = savedata.Split('\n')[3].Split('\t');
			for (int i = 2; i < array.Length; i++)
			{
				StringStatusConverter.SetEachProperty(array[i], array2[i]);
			}
		}
	}
}
