using System;
using System.Collections;
using App;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
	private static SceneTransition Instance;

	private static GameObject TransitionScreen;

	public static Image Filter;

	public static void Transit(Action process, Color? filterColor = null)
	{
		SetActive(true);
		Instance.StartCoroutine(Instance.TransitFromStatic(process, filterColor));
	}

	public static void LoadScene(string scenename, Color? filterColor = null, float time = 0.5f)
	{
		SetActive(true);
		Instance.StartCoroutine(Instance.LoadSceneFromStatic(scenename, filterColor, time));
		GameObject.Find("AppCommon").GetComponent<AudioManager>().OnPreSceneLoaded(scenename);
	}

	public static void SetActive(bool value)
	{
		TransitionScreen.SetActive(value);
	}

	static SceneTransition()
	{
		Debug.Log("static SceneTransition");
		TransitionScreen = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TransitionScreen"));
		TransitionScreen.name = "TransitionScreen";
		TransitionScreen.AddComponent<SceneTransition>();
		Filter = TransitionScreen.GetComponentInChildren<Image>();
		UnityEngine.Object.DontDestroyOnLoad(TransitionScreen);
		SetActive(false);
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private IEnumerator LoadSceneFromStatic(string scenename, Color? filterColor, float time)
	{
		Filter.color = filterColor ?? Settings.TRANSITION_COLOR;
		AsyncOperation async = SceneManager.LoadSceneAsync(scenename);
		async.allowSceneActivation = false;
		StartCoroutine(AppUtil.FadeIn(Filter, time, delegate
		{
			async.allowSceneActivation = true;
		}));
		float passedTime = 0f;
		while (!async.isDone)
		{
			passedTime += Time.unscaledDeltaTime * Settings.GAME_SPEED;
			yield return new WaitForEndOfFrame();
		}
		yield return null;
		float waitTime = Mathf.Max(1f - passedTime, 0.1f);
		yield return AppUtil.WaitRealtime(waitTime);
		yield return null;
		yield return AppUtil.FadeOut(Filter);
		SetActive(false);
		Debug.Log("AutoTestEvent:スクリーンショット");
	}

	private IEnumerator TransitFromStatic(Action process, Color? filterColor)
	{
		Filter.color = filterColor ?? Settings.TRANSITION_COLOR;
		yield return AppUtil.FadeIn(Filter);
		process();
		yield return AppUtil.WaitRealtime(0.5f);
		yield return AppUtil.FadeOut(Filter);
		SetActive(false);
		Debug.Log("AutoTestEvent:スクリーンショット");
	}
}
