using System;
using System.Collections.Generic;

namespace UnityEngine.Store
{
	[HideInInspector]
	internal class MainThreadDispatcher : MonoBehaviour
	{
		public static readonly string OBJECT_NAME = "UnityChannelMainThreadDispatcher";

		private static List<Action> s_Callbacks = new List<Action>();

		private static volatile bool s_CallbacksPending;

		public static void RunOnMainThread(Action runnable)
		{
			lock (s_Callbacks)
			{
				s_Callbacks.Add(runnable);
				s_CallbacksPending = true;
			}
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
			lock (s_Callbacks)
			{
				if (s_Callbacks.Count == 0)
				{
					return;
				}
				array = new Action[s_Callbacks.Count];
				s_Callbacks.CopyTo(array);
				s_Callbacks.Clear();
				s_CallbacksPending = false;
			}
			Action[] array2 = array;
			foreach (Action action in array2)
			{
				action();
			}
		}
	}
}
