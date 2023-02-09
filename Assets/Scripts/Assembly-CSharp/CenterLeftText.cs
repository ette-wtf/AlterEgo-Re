using UnityEngine;
using UnityEngine.UI;

public class CenterLeftText : MonoBehaviour
{
	public virtual void OnEnable()
	{
		float width = GetComponent<RectTransform>().rect.width;
		float preferredWidth = GetComponent<Text>().preferredWidth;
		GetComponent<RectTransform>().anchoredPosition = new Vector2((width - preferredWidth) / 2f, GetComponent<RectTransform>().anchoredPosition.y);
	}
}
