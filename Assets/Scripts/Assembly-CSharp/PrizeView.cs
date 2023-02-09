using UnityEngine;
using UnityEngine.UI;

public class PrizeView : MonoBehaviour
{
	[SerializeField]
	private Sprite Icon;

	private int ItemNum;

	private PrizeItem Prize;

	private void OnEnable()
	{
		ItemNum = int.Parse(base.gameObject.name.Replace("PrizeItem", ""));
		Prize = new PrizeItem(ItemNum);
		SetItem();
	}

	private void OnClick(GameObject clickObject)
	{
		Debug.Log(base.gameObject.name + ":OnClick " + clickObject.name);
		string text = clickObject.name;
		if (text == "GetPrizeButton")
		{
			DialogManager.ShowDialog("GetPrizeDialog", Prize.GetGift(), Prize.GetGiftLocalizeText(), Icon);
			Prize.AddLevel();
			PrizeView[] componentsInChildren = base.transform.parent.parent.GetComponentsInChildren<PrizeView>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetItem();
			}
		}
	}

	public void SetItem()
	{
		int itemNum = ItemNum;
		Image component = base.transform.Find("PrizeDetail/Icon").GetComponent<Image>();
		component.sprite = Icon;
		component.SetNativeSize();
		string text = string.Format(LanguageManager.Get("[Prize]Condition" + itemNum), Prize.GetNextCondition() + Prize.GetUnit());
		text = text.Replace("広", "<size=27>広</size>");
		base.transform.Find("PrizeDetail/TitleText").GetComponent<TextLocalization>().SetText(text);
		base.transform.Find("PrizeDetail/EffectText").GetComponent<TextLocalization>().SetText(LanguageManager.Get("[UI]PrizeDetail/EffectText") + "\n" + Prize.GetGiftLocalizeText());
		float currentValueRate = Prize.GetCurrentValueRate();
		bool flag = Prize.IsEnableButton();
		base.transform.Find("PrizeDetail/GetPrizeButton").GetComponent<Button>().interactable = flag;
		base.transform.Find("PrizeDetail/GetPrizeButton/Notice").gameObject.SetActive(flag);
		base.transform.Find("PrizeDetail/Bar/BarCurrent").GetComponent<Image>().fillAmount = currentValueRate;
		string currentValue = Prize.GetCurrentValue();
		string nextCondition = Prize.GetNextCondition();
		base.transform.Find("PrizeDetail/Bar/BarText").GetComponent<TextLocalization>().SetText(currentValue + Prize.GetUnit() + "/" + nextCondition + Prize.GetUnit());
		if (Prize.IsMax)
		{
			base.transform.Find("PrizeDetail/GetPrizeButton").GetComponentInChildren<TextLocalization>().SetKey("[UI]GetPrizeButton/TextMax");
			base.transform.Find("PrizeDetail/EffectText").gameObject.SetActive(false);
		}
	}
}
