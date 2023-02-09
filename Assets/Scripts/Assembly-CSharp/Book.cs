using App;
using UnityEngine;
using UnityEngine.UI;

public class Book : MonoBehaviour
{
	[SerializeField]
	private string BookNo;

	private EgoPoint BookPrice;

	private void Awake()
	{
		BookNo = base.gameObject.name.Replace("BookItem", "");
	}

	private void OnEnable()
	{
		UpdateButton();
	}

	private void FixedUpdate()
	{
		UpdateStatus();
	}

	public void OnClick(GameObject clickObject)
	{
		Debug.Log("Book:OnClick " + clickObject.name);
		if (clickObject.name.Contains("ReadButton"))
		{
			if (PlayerStatus.EgoPoint < BookPrice)
			{
				return;
			}
			PlayerStatus.EgoPoint -= BookPrice;
			BookLevel.Add(BookNo.ToString());
			int num = BookLevel.Get(BookNo);
			if (num == 0)
			{
				DialogManager.ShowDialog("BookDialog", BookNo, 1);
			}
			else
			{
				GameObject flipAnimation = base.transform.Find("ReadButton2/FlipPage").gameObject;
				if (!flipAnimation.activeSelf)
				{
					flipAnimation.SetActive(true);
					StartCoroutine(AppUtil.WaitAnimation(flipAnimation, delegate
					{
						flipAnimation.SetActive(false);
					}));
				}
				if (BookEgoPoint.IsRankUp(BookNo, num))
				{
					base.transform.Find("BookStatus/ProgressBar").GetComponent<Image>().fillAmount = 1f;
					DialogManager.ShowDialog("RankupDialog", BookNo);
				}
			}
			UpdateButton();
		}
		else if (clickObject.name == "AuthorImage")
		{
			DialogManager.ShowDialog("BookDialog", BookNo, 2);
		}
	}

	private void UpdateButton()
	{
		if (!base.gameObject.activeSelf || BookNo == null || BookNo == "")
		{
			return;
		}
		int num = BookLevel.Get(BookNo);
		int rank = BookLevel.GetRank(BookNo);
		EgoPoint bookEgo = BookEgoPoint.GetBookEgo(BookNo, "per_second", (num >= 0) ? num : 0);
		base.transform.Find("BookStatus/EgoPerSecond").GetComponent<TextLocalization>().SetText(bookEgo.ToString(LanguageManager.Get("[UI]BookStatus/EgoPerSecond")).Replace(" ", ""));
		base.transform.Find("BookStatus/EgoPerSecond/EgoPerSecondUnit").GetComponent<RectTransform>().anchoredPosition = new Vector2(base.transform.Find("BookStatus/EgoPerSecond").GetComponent<Text>().preferredWidth, 0f);
		if (rank >= 5)
		{
			base.transform.Find("ReadButton1").gameObject.SetActive(false);
			base.transform.Find("ReadButton2").gameObject.SetActive(false);
			base.transform.Find("Complete").gameObject.SetActive(true);
			base.transform.Find("BookStatus/ProgressBar").GetComponent<Image>().fillAmount = 1f;
			BookPrice = null;
			return;
		}
		base.transform.Find("ReadButton1").gameObject.SetActive(num == -1);
		base.transform.Find("ReadButton2").gameObject.SetActive(num >= 0);
		base.transform.Find("InActiveFilter").gameObject.SetActive(num == -1);
		float num2 = BookLevel.GetCurrentInRank(BookNo);
		float num3 = BookLevel.GetMaxPageInRank(BookNo);
		base.transform.Find("BookStatus/ProgressBar").GetComponent<Image>().fillAmount = num2 / num3;
		BookPrice = BookEgoPoint.GetBookEgo(BookNo, "price", num);
		if (base.transform.Find("ReadButton1").gameObject.activeSelf)
		{
			base.transform.Find("ReadButton1/PriceText").GetComponent<TextLocalization>().SetText(BookPrice.ToString(LanguageManager.Get("[UI]ReadButton1/PriceText")));
		}
		if (base.transform.Find("ReadButton2").gameObject.activeSelf)
		{
			base.transform.Find("ReadButton2/ReadCountText").GetComponent<TextLocalization>().SetText((num + 1).ToString(LanguageManager.Get("[UI]ReadButton2/ReadCountText")));
			EgoPoint bookEgo2 = BookEgoPoint.GetBookEgo(BookNo, "add_per_second");
			base.transform.Find("ReadButton2/PerSecondText").GetComponent<TextLocalization>().SetText(bookEgo2.ToString(LanguageManager.Get("[UI]ReadButton2/PerSecondText")));
			base.transform.Find("ReadButton2/PriceText").GetComponent<TextLocalization>().SetText(BookPrice.ToString(LanguageManager.Get("[UI]ReadButton2/PriceText")));
			for (int i = 1; i <= rank; i++)
			{
				base.transform.Find("ReadButton2/Marker" + i).gameObject.SetActive(true);
			}
		}
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (BookPrice != null)
		{
			bool interactable = PlayerStatus.EgoPoint >= BookPrice;
			base.transform.Find("ReadButton1").GetComponent<Button>().interactable = interactable;
			base.transform.Find("ReadButton2").GetComponent<Button>().interactable = interactable;
		}
	}
}
