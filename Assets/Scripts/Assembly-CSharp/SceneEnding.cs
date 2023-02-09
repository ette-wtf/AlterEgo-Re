using App;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneEnding : SceneBase
{
	public PlayableDirector EndingDirector;

	public AudioListener audioListener;

	protected void OnEnable()
	{
		AdManager.Hide("Banner");
		EndingDirector.stopped += OnPlayableDirectorStopped;
		EndingDirector.GetComponent<AudioSource>().mute = !Settings.BGM;
		audioListener.enabled = SceneManager.sceneCount == 1;
	}

	private void OnPlayableDirectorStopped(PlayableDirector aDirector)
	{
		if (!(EndingDirector == aDirector))
		{
			return;
		}
		Debug.Log("PlayableDirector#OnPlayableDirectorStopped " + SceneManager.sceneCount);
		if (SceneManager.sceneCount > 1)
		{
			SceneTransition.Transit(delegate
			{
				SceneManager.UnloadSceneAsync("Ending");
				AudioManager.ChangeBGM("Silence_OffV");
				AdManager.Show("Banner");
			});
		}
		else
		{
			SceneTransition.LoadScene("タイトル", Color.black, 3f);
		}
	}

	protected void OnDisable()
	{
		EndingDirector.stopped -= OnPlayableDirectorStopped;
	}
}
