using App;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneBase : SceneCommon
{
	private Text EgoCountText;

	private Text EgoPerSecondText;

	private bool InBonus;

	protected override void Awake()
	{
		base.Awake();
		if (GameObject.Find("EgoCount") == null)
		{
			return;
		}
		TimeManager.UpdateLoginDate();
		EgoCountText = GameObject.Find("UI_Header/Header1/EgoCount").GetComponent<Text>();
		EgoPerSecondText = GameObject.Find("UI_Header/Header1/EgoPerSecond").GetComponent<Text>();
		float num = TimeManager.SetLastTime(TimeManager.TYPE.LAST_EGO);
		if (num > 21600f)
		{
			num = 21600f;
		}
		EgoPoint egoPoint = PlayerStatus.EgoPerSecond * num;
		if (egoPoint.ToString() != "0.00")
		{
			PlayerStatus.EgoPoint += egoPoint;
			if (num >= 60f)
			{
				DialogManager.ShowDialog("ComebackDialog", egoPoint.ToString());
			}
		}
		UpdateHeader();
	}

	protected override void Start()
	{
		base.Start();
		EventSystem.current.pixelDragThreshold = 50;
		switch (base.gameObject.scene.name)
		{
		case "タイトル":
		case "探求":
		case "Ending":
			AdManager.Hide("Banner");
			break;
		default:
			AdManager.Show("Banner");
			break;
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!(EgoCountText == null))
		{
			float num = TimeManager.SetLastTime(TimeManager.TYPE.LAST_EGO);
			if (num > 21600f)
			{
				num = 21600f;
			}
			EgoPoint egoPoint = PlayerStatus.EgoPerSecond * num * Settings.GAME_SPEED;
			PlayerStatus.EgoPoint += egoPoint;
			UpdateHeader();
		}
	}

	protected void UpdateHeader()
	{
		if (!(EgoCountText == null) && !(base.transform.Find("UICanvas/UI_Header/Header1/NormalLabel") == null))
		{
			bool inBonus = InBonus;
			InBonus = BonusTime.IsActive;
			if (inBonus != InBonus)
			{
				Data.UpdateEgoPerSecond();
			}
			base.transform.Find("UICanvas/UI_Header/Header1/NormalLabel").gameObject.SetActive(!InBonus);
			base.transform.Find("UICanvas/UI_Header/Header1/BonusLabel").gameObject.SetActive(InBonus);
			if (InBonus)
			{
				string text = BonusTime.TimeLeft.ToString("mm\\:ss");
				base.transform.Find("UICanvas/UI_Header/Header1/BonusLabel/EgoUnit/BonusTimeText").GetComponent<Text>().text = text;
			}
			EgoCountText.text = PlayerStatus.EgoPoint.ToString(LanguageManager.Get("[UI]UI_Header/EgoCount"));
			string text2 = PlayerStatus.EgoPerSecond.ToString(LanguageManager.Get("[UI]UI_Header/EgoPerSecond"));
			EgoPerSecondText.text = text2;
		}
	}

	public bool OnClick(GameObject clickObject)
	{
		Debug.Log("SceneBase:OnClick " + clickObject.name);
		if (clickObject.name.Contains("GlobalButton"))
		{
			string text = SceneManager.GetActiveScene().name;
			string text2 = clickObject.name.Replace("GlobalButton", "");
			if (text2 == "アンケート")
			{
				Application.OpenURL("https://goo.gl/forms/8xsZWW9lvcY17Q9G3");
			}
			else if (text2 != text)
			{
				SceneTransition.LoadScene(text2);
			}
			return true;
		}
		switch (clickObject.name)
		{
		case "MenuButton":
			DialogManager.ShowDialog("MenuDialog");
			return true;
		case "ButterflyBonus":
			Object.Destroy(clickObject);
			DialogManager.ShowDialog("BonusMovieDialog");
			return true;
		case "ShareButton":
			clickObject.GetComponent<Button>().interactable = false;
			StartCoroutine(AppUtil.DelayAction(1f, delegate
			{
				clickObject.GetComponent<Button>().interactable = true;
			}));
			StartCoroutine(AppUtil.Share(true, "自分探しタップゲーム『ALTER EGO』 caracolu.com/app/alterego/ #ALTEREGO", null));
			return true;
		default:
			return false;
		}
	}
}
