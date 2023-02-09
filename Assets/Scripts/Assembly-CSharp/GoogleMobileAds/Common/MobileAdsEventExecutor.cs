using System;
using System.Collections.Generic;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	public class MobileAdsEventExecutor : MonoBehaviour
	{
		public static MobileAdsEventExecutor instance = null;

		private static List<Action> adEventsQueue = new List<Action>();

		private static volatile bool adEventsQueueEmpty = true;

		public static void Initialize()
		{
			if (!IsActive())
			{
				GameObject obj = new GameObject("MobileAdsMainThreadExecuter")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				UnityEngine.Object.DontDestroyOnLoad(obj);
				instance = obj.AddComponent<MobileAdsEventExecutor>();
			}
		}

		public static bool IsActive()
		{
			return instance != null;
		}

		public void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		public static void ExecuteInUpdate(Action action)
		{
			lock (adEventsQueue)
			{
				adEventsQueue.Add(action);
				adEventsQueueEmpty = false;
			}
		}

		public void Update()
		{
			if (adEventsQueueEmpty)
			{
				return;
			}
			List<Action> list = new List<Action>();
			lock (adEventsQueue)
			{
				list.AddRange(adEventsQueue);
				adEventsQueue.Clear();
				adEventsQueueEmpty = true;
			}
			foreach (Action item in list)
			{
				item();
			}
		}

		public void OnDisable()
		{
			instance = null;
		}
	}
}
