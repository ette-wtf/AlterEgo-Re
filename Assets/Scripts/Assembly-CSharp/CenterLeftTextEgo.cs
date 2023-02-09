using UnityEngine;
using UnityEngine.UI;

public class CenterLeftTextEgo : CenterLeftText
{
	public float LeftPadding;

	public override void OnEnable()
	{
		if (base.gameObject.name == "TalkText")
		{
			GetComponent<Text>().fontSize = 44;
			float width = GetComponent<RectTransform>().rect.width;
			float preferredWidth = GetComponent<Text>().preferredWidth;
			if (preferredWidth > width * 0.95f)
			{
				GetComponent<Text>().fontSize = 40;
			}
			preferredWidth = GetComponent<Text>().preferredWidth;
			if (preferredWidth <= width * 0.8f)
			{
				GetComponent<RectTransform>().anchoredPosition = new Vector2(LeftPadding, GetComponent<RectTransform>().anchoredPosition.y);
			}
			else
			{
				GetComponent<RectTransform>().anchoredPosition = new Vector2(LeftPadding - 45f + (width - preferredWidth) / 2f, GetComponent<RectTransform>().anchoredPosition.y);
			}
		}
		else
		{
			base.OnEnable();
		}
	}
}
