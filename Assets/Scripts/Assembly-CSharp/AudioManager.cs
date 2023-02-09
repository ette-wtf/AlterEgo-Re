using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
	private static AudioManager Instance;

	private string CurrentBGM;

	private List<AudioSource> AudioList = new List<AudioSource>();

	private AudioSource AudioSE;

	private List<AudioClip> ClipList = new List<AudioClip>();

	private void Awake()
	{
		Instance = this;
		foreach (string value in Settings.AUDIO_LIST.Values)
		{
			if (!(value == ""))
			{
				AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
				audioSource.loop = true;
				audioSource.clip = Resources.Load<AudioClip>(value);
				AudioList.Add(audioSource);
			}
		}
		AudioSE = base.gameObject.AddComponent<AudioSource>();
		AudioSE.loop = false;
		StartCoroutine(PlayBGM(Settings.AUDIO_LIST["Default"]));
		SceneManager.sceneLoaded += OnSceneLoaded;
		ChangeSettings();
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public static void PlaySound(params string[] eventList)
	{
		if (Instance == null)
		{
			return;
		}
		string soundName = Utility.GetSoundName(eventList);
		if (soundName == null || soundName == "")
		{
			return;
		}
		AudioClip audioClip = null;
		foreach (AudioClip clip in Instance.ClipList)
		{
			if (clip.name == soundName)
			{
				audioClip = clip;
				break;
			}
		}
		if (audioClip == null)
		{
			audioClip = Resources.Load<AudioClip>("SE/" + soundName);
			if (audioClip == null)
			{
				Debug.LogWarning("音声ファイル「" + soundName + "」は存在しません");
				return;
			}
			Instance.ClipList.Add(audioClip);
		}
		Instance.AudioSE.PlayOneShot(audioClip);
	}

	public static void ChangeSettings()
	{
		foreach (AudioSource audio in Instance.AudioList)
		{
			audio.mute = !Settings.BGM;
		}
		Instance.AudioSE.mute = !Settings.SE;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode SceneMode)
	{
		string key = "Default";
		if (Settings.AUDIO_LIST.ContainsKey(scene.name))
		{
			key = scene.name;
		}
		StartCoroutine(PlayBGM(Settings.AUDIO_LIST[key]));
	}

	public void OnPreSceneLoaded(string scenename)
	{
		string key = "Default";
		if (Settings.AUDIO_LIST.ContainsKey(scenename))
		{
			key = scenename;
		}
		StopBGM(Settings.AUDIO_LIST[key]);
	}

	public static void ChangeBGM(string bgm)
	{
		Instance.StartCoroutine(Instance.PlayBGM(bgm));
	}

	private bool StopBGM(string nextbgm)
	{
		if (CurrentBGM != null && CurrentBGM == nextbgm)
		{
			return false;
		}
		foreach (AudioSource audio in AudioList)
		{
			if (CurrentBGM == audio.clip.name)
			{
				StartCoroutine(AppUtil.MoveEasingFloat(1f, 0f, delegate(float tmp)
				{
					audio.volume = tmp;
				}, true, 3f, EasingFunction.Ease.EaseOutQuint));
				CurrentBGM = null;
				return true;
			}
		}
		return false;
	}

	private IEnumerator PlayBGM(string bgm)
	{
		if (CurrentBGM != null && CurrentBGM == bgm)
		{
			yield break;
		}
		if (bgm == "")
		{
			StopBGM(bgm);
			yield break;
		}
		if (StopBGM(bgm))
		{
			yield return AppUtil.WaitRealtime(1f);
		}
		CurrentBGM = bgm;
		yield return AppUtil.WaitRealtime(1f);
		foreach (AudioSource audio in AudioList)
		{
			if (audio.clip.name == bgm)
			{
				audio.Play();
				audio.volume = 1f;
			}
			else
			{
				audio.Stop();
			}
		}
	}
}
