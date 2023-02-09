using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnLongClickHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	public void OnPointerDown(PointerEventData eventData)
	{
		StartCoroutine("HandleLongClick");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		StopCoroutine("HandleLongClick");
	}

	private IEnumerator HandleLongClick()
	{
		float totalTime = 0f;
		while (totalTime < 1f)
		{
			totalTime += Time.unscaledDeltaTime;
			yield return null;
		}
		base.gameObject.SendMessageUpwards("OnLongClick", base.gameObject.name, SendMessageOptions.DontRequireReceiver);
	}
}
