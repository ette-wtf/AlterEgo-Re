using System;
using App;
using UnityEngine;

public class CommentManager : MonoBehaviour
{
	private int PreColumnIndex;

	private readonly Vector2[][] POSITION1 = new Vector2[3][]
	{
		new Vector2[3]
		{
			new Vector2(-2f, 2f),
			new Vector2(-2f, 0f),
			new Vector2(-2f, -2f)
		},
		new Vector2[2]
		{
			new Vector2(0f, 1f),
			new Vector2(0f, -1f)
		},
		new Vector2[3]
		{
			new Vector2(2f, 2f),
			new Vector2(2f, 0f),
			new Vector2(2f, -2f)
		}
	};

	private readonly Vector2[][] POSITION2 = new Vector2[5][]
	{
		new Vector2[3]
		{
			new Vector2(-3f, 3.5f),
			new Vector2(-3f, 0f),
			new Vector2(-3f, -2.5f)
		},
		new Vector2[3]
		{
			new Vector2(-1.5f, 2f),
			new Vector2(-1.5f, 0f),
			new Vector2(-1.5f, -2f)
		},
		new Vector2[2]
		{
			new Vector2(0f, 1f),
			new Vector2(0f, -1f)
		},
		new Vector2[3]
		{
			new Vector2(1.5f, 2f),
			new Vector2(1.5f, 0f),
			new Vector2(1.5f, -2f)
		},
		new Vector2[3]
		{
			new Vector2(3f, 3.5f),
			new Vector2(3f, 0f),
			new Vector2(3f, -2.5f)
		}
	};

	private void FixedUpdate()
	{
		UpdateComment(false);
	}

	public void UpdateCommentWithDestroy()
	{
		UpdateComment(true);
	}

	private void UpdateComment(bool destroy)
	{
		bool isFirst = true;
		Comment[] componentsInChildren = GetComponentsInChildren<Comment>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].UpdateStatus(isFirst, destroy))
			{
				isFirst = false;
			}
		}
	}

	public void Generate(bool full, string mode, float rate)
	{
		bool flag = mode.Contains("HyperActive");
		bool num = !full && rate == 2f && AdManager.IsReady("Reward") && TimeManager.IsOverTime(TimeManager.TYPE.NEXT_BONUS_BOOK) && !BonusTime.IsActive && GameObject.Find("BonusButterfly") == null && !PlayerStatus.EgoPerSecond.IsZero();
		string text = "Comment";
		if (num)
		{
			TimeManager.Reset(TimeManager.TYPE.NEXT_BONUS_BOOK, TimeSpan.FromMinutes(0.5));
			text = "Butterfly";
		}
		for (int num2 = ((rate != 0f) ? ((!full) ? 1 : ((int)(12f / rate))) : 0) - 1; num2 >= 0; num2--)
		{
			Vector3 zero = Vector3.zero;
			zero.z = 12f + (float)num2 * -1.5f * 2f * rate;
			if (!(zero.z <= -3f))
			{
				Vector2[][] array = (flag ? POSITION2 : POSITION1);
				int num3 = PreColumnIndex;
				if (mode == "HyperActiveLast")
				{
					num3 = 2;
				}
				else
				{
					while (PreColumnIndex == num3)
					{
						num3 = UnityEngine.Random.Range(0, array.Length);
					}
				}
				PreColumnIndex = num3;
				int num4 = UnityEngine.Random.Range(0, array[num3].Length);
				zero.x = array[num3][num4].x;
				zero.y = array[num3][num4].y;
				string text2 = "";
				string text3 = Data.COMMENT_KEY_LIST[UnityEngine.Random.Range(0, Data.COMMENT_KEY_LIST.Length)];
				if (text == "Comment")
				{
					text2 = "";
					if (text3.Contains("[Es]"))
					{
						text2 = PlayerStatus.Route;
					}
				}
				else
				{
					text2 = "Butterfly";
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Comment" + text2), base.transform);
				if (text == "Comment")
				{
					gameObject.name = "FarComment" + text2;
				}
				else
				{
					gameObject.name = "ButterflyBonus";
				}
				gameObject.transform.position = zero;
				Comment component = gameObject.GetComponent<Comment>();
				component.Type = text2;
				component.CommentKey = text3;
				component.IsHyperActive = flag;
			}
		}
		UpdateComment(full);
	}

	public bool KeepHyperActive()
	{
		Comment[] componentsInChildren = GetComponentsInChildren<Comment>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].IsHyperActive)
			{
				return true;
			}
		}
		return false;
	}
}
