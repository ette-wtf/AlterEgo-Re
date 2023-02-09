using UnityEngine;
using UnityEngine.UI;

public class ImageLocalization : MonoBehaviour
{
	[SerializeField]
	private Sprite[] LocalizedSprite;

	private void OnEnable()
	{
		Image component = GetComponent<Image>();
		if (component != null)
		{
			component.sprite = LocalizedSprite[LanguageManager.GetIndex()];
		}
		SpriteRenderer component2 = GetComponent<SpriteRenderer>();
		if (component2 != null)
		{
			component2.sprite = LocalizedSprite[LanguageManager.GetIndex()];
		}
	}
}
