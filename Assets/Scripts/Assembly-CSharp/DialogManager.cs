using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
	private static DialogManager Instance;

	private GameObject FilterGroup;

	private GameObject CanvasDialogBG;

	private GameObject CanvasDialog;

	private IDictionary<string, IEnumerator> ClosingDialog = new Dictionary<string, IEnumerator>();

	private void Awake()
	{
		Instance = this;
		FilterGroup = base.transform.Find("FilterGroup").gameObject;
		FilterGroup.SetActive(false);
		CanvasDialog = base.transform.Find("DialogCanvas").gameObject;
		CanvasDialog.SetActive(false);
		GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("FakeMovieAdScreen"), base.transform.Find("DialogCanvas"));
		Sprite sprite = Resources.Load<Sprite>("FakeAdImage");
		if (sprite != null)
		{
			gameObject.transform.Find("Rectangle").GetComponent<Image>().sprite = sprite;
		}
		Dialog[] componentsInChildren = base.transform.Find("DialogCanvas").GetComponentsInChildren<Dialog>(true);
		foreach (Dialog dialog in componentsInChildren)
		{
			dialog.gameObject.SetActive(false);
			dialog.gameObject.name = dialog.gameObject.name.Replace("(Clone)", "");
		}
	}

	public void OnClick(GameObject clickObject)
	{
		Debug.Log("DialogManager:OnClick " + clickObject.name);
	}

	public static bool IsShowing()
	{
		if (Instance == null)
		{
			return false;
		}
		return GetShowingDialogCnt() > 0;
	}

	public static void ShowDialog(string dialogName, params object[] args)
	{
		if (!(Instance == null))
		{
			Debug.Log("DialogManager args=" + ((args != null) ? args.Length : 0));
			if (GetShowingDialogCnt() == 0)
			{
				Instance.FilterGroup.SetActive(true);
				Instance.StartCoroutine(AppUtil.FadeIn(Instance.FilterGroup.GetComponentInChildren<CanvasGroup>(), 0.25f));
				Instance.CanvasDialog.SetActive(true);
				Time.timeScale = 0f;
			}
			Dialog component = Instance.transform.Find("DialogCanvas/" + dialogName).GetComponent<Dialog>();
			if (Instance.ClosingDialog.ContainsKey(component.name))
			{
				Instance.StopCoroutine(Instance.ClosingDialog[component.name]);
				Instance.ClosingDialog.Remove(component.name);
				component.gameObject.SetActive(false);
			}
			component.OnShown(args);
		}
	}

	public static void CloseAllDialog()
	{
		Dialog[] componentsInChildren = Instance.transform.Find("DialogCanvas").GetComponentsInChildren<Dialog>();
		foreach (Dialog dialog in componentsInChildren)
		{
			Instance.StartCoroutine(Instance.CloseDialogAsync(dialog.name));
		}
		if (GetShowingDialogCnt() > 1)
		{
			Instance.StartCoroutine(AppUtil.FadeOut(Instance.FilterGroup.GetComponentInChildren<CanvasGroup>(), 0.25f));
		}
	}

	public static void CloseDialog(string dialogName)
	{
		if (Instance == null)
		{
			return;
		}
		Dialog[] componentsInChildren = Instance.transform.Find("DialogCanvas").GetComponentsInChildren<Dialog>();
		foreach (Dialog dialog in componentsInChildren)
		{
			if (dialog.name != dialogName)
			{
				dialog.OnShown();
			}
		}
		Instance.StartCoroutine(Instance.CloseDialogAsync(dialogName));
	}

	private IEnumerator CloseDialogAsync(string dialogName)
	{
		Dialog dialog = base.transform.Find("DialogCanvas/" + dialogName).GetComponent<Dialog>();
		if (GetShowingDialogCnt() == 1)
		{
			StartCoroutine(AppUtil.FadeOut(FilterGroup.GetComponentInChildren<CanvasGroup>(), 0.25f));
		}
		IEnumerator enumerator = dialog.OnClosed();
		ClosingDialog.Add(dialog.name, enumerator);
		yield return enumerator;
		ClosingDialog.Remove(dialog.name);
		if (GetShowingDialogCnt() == 0)
		{
			FilterGroup.SetActive(false);
			CanvasDialog.SetActive(false);
			Time.timeScale = Settings.GAME_SPEED;
		}
	}

	private static int GetShowingDialogCnt()
	{
		return Instance.transform.Find("DialogCanvas").GetComponentsInChildren<Dialog>().Length;
	}
}
