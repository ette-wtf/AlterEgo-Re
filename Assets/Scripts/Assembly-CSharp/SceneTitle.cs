using System.Collections;
using App;
using UnityEngine;
using UnityEngine.UI;

public class SceneTitle : SceneBase
{
	private static string USERID_HEADER = "User ID : ";

	public GameObject TitleLogo;

	public GameObject LoadingMessage;

	public GameObject StartMessage;

	public GameObject MoveDataButton;

	public Button StartScreen;

	protected override void Awake()
	{
		base.Awake();
		AppUtil.SetAlpha(StartMessage, 0f);
		AppUtil.SetAlpha(base.transform.Find("Header/VersionText"), 0f);
		AppUtil.SetAlpha(base.transform.Find("Header/UserIDText"), 0f);
		StartScreen.enabled = false;
		LoadingMessage.SetActive(false);
		MoveDataButton.SetActive(false);
	}

	protected override void Start()
	{
		base.Start();
		StartCoroutine(RenderTitle());
		base.transform.Find("Header/VersionText").GetComponent<Text>().text = "ver. " + BuildInfo.APP_VERSION;
		if (!SetUserID())
		{
			StartCoroutine(UpdateUserID());
		}
		StartCoroutine(AppUtil.FadeIn(base.transform.Find("Header/VersionText")));
		StartCoroutine(AppUtil.FadeIn(base.transform.Find("Header/UserIDText").GetComponent<Text>()));
	}

	private IEnumerator UpdateUserID()
	{
		yield return SaveDataManager.SetUserID();
		SetUserID();
	}

	private bool SetUserID()
	{
		string text = Settings.SaveDataID;
		bool result = true;
		if (string.IsNullOrEmpty(text))
		{
			text = "";
			result = false;
		}
		base.transform.Find("Header/UserIDText").GetComponent<Text>().text = USERID_HEADER + text;
		return result;
	}

	private IEnumerator RenderTitle()
	{
		TitleLogo.GetComponent<Animator>().enabled = true;
		float length = TitleLogo.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;
		yield return AppUtil.Wait(length * 0.8f);
		LoadingMessage.SetActive(true);
		while (!SceneCommon.IsInitialized)
		{
			yield return null;
		}
		if (PlayerResult.Ending.Contains("AE"))
		{
			Utility.PromptReview("AEクリア");
		}
		LoadingMessage.SetActive(false);
		MoveDataButton.SetActive(true);
		yield return AppUtil.FadeIn(StartMessage);
		Debug.Log("AutoTestEvent:Screenshot");
		StartScreen.enabled = true;
		while (true)
		{
			yield return AppUtil.Wait(1.2f);
			yield return AppUtil.FadeOut(StartMessage, 0.3f);
			yield return AppUtil.Wait(0.6f);
			yield return AppUtil.FadeIn(StartMessage, 0.3f);
		}
	}

	public new void OnClick(GameObject clickObject)
	{
		if (base.OnClick(clickObject))
		{
			return;
		}
		Debug.Log("OnClick " + clickObject.name);
		switch (clickObject.name)
		{
		case "StartScreen":
			SceneTransition.LoadScene("探求");
			if (!PlayerPrefs.HasKey("ScenarioNo"))
			{
				PlayerStatus.TutorialLv = 0;
				PlayerStatus.ScenarioNo = PlayerStatus.SCENARIO_NO_LIST[0];
			}
			break;
		case "UserIDText":
			AnalyticsManager.SendEvent(new string[3] { "", "", "" }, "1Jyb1Gc-XDgXGFebWkWco-tO56PVYTeS9GJ24x8ONO8Q");
			if (AppUtil.SetClipBoard(base.transform.Find("Header/UserIDText").GetComponent<Text>().text.Replace(USERID_HEADER, "")))
			{
				base.transform.Find("Header/CopyToast/CopyText").GetComponent<TextLocalization>().SetKey("[UI]CopyToast/CopyText");
			}
			else
			{
				base.transform.Find("Header/CopyToast/CopyText").GetComponent<TextLocalization>().SetKey("[UI]CopyToast/CopyTextFailed");
			}
			StopCoroutine("DisplayToast");
			StartCoroutine("DisplayToast");
			break;
		case "MoveDataButton":
			DialogManager.ShowDialog("MoveDataDialog");
			break;
		}
	}

	private IEnumerator DisplayToast()
	{
		yield return AppUtil.FadeIn(base.transform.Find("Header/CopyToast"));
		yield return AppUtil.Wait(2f);
		yield return AppUtil.FadeOut(base.transform.Find("Header/CopyToast"));
	}
}
