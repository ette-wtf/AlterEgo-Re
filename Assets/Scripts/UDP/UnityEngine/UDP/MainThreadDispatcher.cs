using System;
using System.Collections.Generic;

namespace UnityEngine.UDP
{
	[HideInInspector]
	internal class MainThreadDispatcher : MonoBehaviour
	{
		public static readonly string OBJECT_NAME = "UnityChannelMainThreadDispatcher";

		private static List<Action> s_Callbacks = new List<Action>();

		private static Dictionary<float, Action> delayAction = new Dictionary<float, Action>();

		private static volatile bool s_CallbacksPending;

		public static void RunOnMainThread(Action runnable)
		{
			lock (s_Callbacks)
			{
				s_Callbacks.Add(runnable);
				s_CallbacksPending = true;
			}
		}

		public static void DispatchDelayJob(float waitTime, Action runnable)
		{
			lock (s_Callbacks)
			{
				delayAction[waitTime] = runnable;
				s_CallbacksPending = true;
			}
		}

		private IEnumerator<WaitForSeconds> WaitAndDo(float waitTime, Action runnable)
		{
			yield return new WaitForSeconds(waitTime);
			runnable();
		}

		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void Update()
		{
			if (!s_CallbacksPending)
			{
				return;
			}
			Action[] array;
			Dictionary<float, Action> dictionary;
			lock (s_Callbacks)
			{
				if (s_Callbacks.Count == 0 && delayAction.Count == 0)
				{
					return;
				}
				array = new Action[s_Callbacks.Count];
				s_Callbacks.CopyTo(array);
				s_Callbacks.Clear();
				dictionary = new Dictionary<float, Action>(delayAction);
				delayAction.Clear();
				s_CallbacksPending = false;
			}
			Action[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i]();
			}
			foreach (KeyValuePair<float, Action> item in dictionary)
			{
				StartCoroutine(WaitAndDo(item.Key, item.Value));
			}
		}
	}
}
