using System;
using System.Collections.Generic;
using System.Linq;
using App;
using UnityEngine;

public class SceneCommon : MonoBehaviour
{
	protected static bool IsInitialized = false;

	public static List<OnClickHandler> BackButtonList = new List<OnClickHandler>();

	protected virtual void Awake()
	{
		if (base.gameObject.scene.name != "タイトル" && !IsInitialized)
		{
			Initialize();
		}
		BackButtonList.Clear();
	}

	protected virtual void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (BackButtonList.Count > 0)
			{
				BackButtonList.Last().Click();
			}
			else
			{
				new AndroidJavaObject("com.caracolu.appcommon.Util").Call("CallHomeScreen");
			}
		}
	}

	protected virtual void FixedUpdate()
	{
	}

	protected virtual void LateUpdate()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void Start()
	{
		if (base.gameObject.scene.name == "タイトル" && !IsInitialized)
		{
			Invoke("Initialize", 2f);
		}
	}

	protected virtual void Initialize()
	{
		Time.timeScale = Settings.GAME_SPEED;
		GameObject gameObject = new GameObject("AppCommon");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		Type[] defaultComponent = AppInfo.DefaultComponent;
		foreach (Type componentType in defaultComponent)
		{
			gameObject.AddComponent(componentType);
		}
		gameObject.AddComponent<GssDataHelper>();
		IsInitialized = true;
	}
}
