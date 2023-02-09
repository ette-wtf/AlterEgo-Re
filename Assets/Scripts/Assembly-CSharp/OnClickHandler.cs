using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnClickHandler : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	private enum STATE
	{
		NONE = 0,
		DOWN = 1,
		DRAG = 2,
		LONGTAP = 3,
		LONGDRAG = 4,
		SCROLL = 5
	}

	public enum EFFECT
	{
		NONE = 0,
		DOWN = 1,
		SHRINK = 2
	}

	public float RepeatWait = -1f;

	public EFFECT Effect;

	private bool IsUI;

	private STATE State;

	private ScrollRect Scroll;

	private bool EffectOn;

	private void Start()
	{
		IsUI = GetComponentsInChildren<Graphic>().Length != 0;
		if (IsUI)
		{
			Scroll = GetComponentInParent<ScrollRect>();
		}
		else if (GetComponent<Rigidbody2D>() == null)
		{
			base.gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
		}
	}

	public void OnMouseDown()
	{
		OnPointerDown(null);
	}

	public void OnMouseUp()
	{
		OnPointerExit(null);
	}

	public void OnMouseUpAsButton()
	{
		OnPointerUp(null);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!IsEnable())
		{
			State = STATE.NONE;
			return;
		}
		if (EffectOn)
		{
			EndRepeat();
		}
		EffectOn = true;
		switch (Effect)
		{
		case EFFECT.DOWN:
			GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, 5f);
			break;
		case EFFECT.SHRINK:
			base.transform.localScale *= 0.95f;
			break;
		}
		State = STATE.DOWN;
		if (RepeatWait >= 0f)
		{
			StartCoroutine("HandleClickRepeat");
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		STATE state = State;
		if ((uint)(state - 1) <= 3u)
		{
			EndRepeat();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (State == STATE.DRAG)
		{
			EndRepeat();
			State = STATE.SCROLL;
		}
		if (eventData != null && State == STATE.SCROLL && Scroll != null)
		{
			Scroll.OnDrag(eventData);
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		switch (State)
		{
		case STATE.NONE:
		case STATE.DOWN:
			State = STATE.DRAG;
			if (eventData != null && Scroll != null)
			{
				Scroll.StopMovement();
				Scroll.OnBeginDrag(eventData);
			}
			break;
		case STATE.LONGTAP:
			State = STATE.LONGDRAG;
			break;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		switch (State)
		{
		case STATE.DRAG:
		case STATE.SCROLL:
			if (eventData != null && Scroll != null)
			{
				Scroll.OnEndDrag(eventData);
				break;
			}
			{
				foreach (GameObject item in eventData.hovered)
				{
					if (base.gameObject.name == item.name)
					{
						Click();
						EndRepeat();
						break;
					}
				}
				break;
			}
		case STATE.LONGDRAG:
			EndRepeat();
			break;
		case STATE.LONGTAP:
			break;
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		switch (State)
		{
		case STATE.DOWN:
			Click();
			EndRepeat();
			break;
		case STATE.LONGTAP:
			EndRepeat();
			break;
		}
	}

	public void Click()
	{
		if (IsEnable())
		{
			AudioManager.PlaySound("Click", base.transform.parent.name, base.gameObject.name);
			base.gameObject.SendMessageUpwards("OnClick", base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void EndRepeat()
	{
		if (State == STATE.NONE)
		{
			return;
		}
		if (EffectOn)
		{
			switch (Effect)
			{
			case EFFECT.DOWN:
				GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 5f);
				break;
			case EFFECT.SHRINK:
				base.transform.localScale /= 0.95f;
				break;
			}
			EffectOn = false;
		}
		StopCoroutine("HandleClickRepeat");
		State = STATE.NONE;
	}

	private IEnumerator HandleClickRepeat()
	{
		yield return AppUtil.Wait(0.5f);
		switch (State)
		{
		case STATE.DOWN:
			State = STATE.LONGTAP;
			break;
		case STATE.DRAG:
			State = STATE.LONGDRAG;
			break;
		}
		Click();
		while (State == STATE.LONGTAP || State == STATE.LONGDRAG)
		{
			yield return AppUtil.Wait(RepeatWait);
			Click();
		}
	}

	private bool IsEnable()
	{
		if (IsUI)
		{
			Button component = GetComponent<Button>();
			if (component == null)
			{
				return true;
			}
			if (!component.gameObject.activeInHierarchy || !component.enabled || !component.interactable)
			{
				return false;
			}
		}
		return true;
	}

	public static void SendEventOperationLog(string eventData, Transform target)
	{
		List<string> list = new List<string>();
		list.Add(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
		list.Add(SceneManager.GetActiveScene().name);
		list.Add(eventData);
		list.Add(target.name);
		list.Add(StringStatusConverter.GetHierarchyPath(target));
		string[] statusStringArray = StringStatusConverter.GetStatusStringArray();
		list.Add(statusStringArray[1]);
		GssDataHelper.PostData("1olHdM1G5zhODy6IbT6ii-Xj2JdEH4oGYyigzAor2zJY", "操作ログ", string.Join("\t", list.ToArray()));
	}
}
