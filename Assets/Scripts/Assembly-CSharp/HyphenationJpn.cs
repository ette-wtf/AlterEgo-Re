using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class HyphenationJpn : UIBehaviour
{
	[TextArea(3, 10)]
	[SerializeField]
	private string text;

	private RectTransform _rectTransform;

	private Text _text;

	private static readonly string RITCH_TEXT_REPLACE = "(\\<color=.*\\>|</color>|\\<size=.n\\>|</size>|<b>|</b>|<i>|</i>)";

	private static readonly char[] HYP_FRONT = ",)]｝、。）〕〉》」』】〙〗〟’”｠»ァィゥェォッャュョヮヵヶっぁぃぅぇぉっゃゅょゎ‐゠–〜ー?!！？‼⁇⁈⁉・:;。.".ToCharArray();

	private static readonly char[] HYP_BACK = "(（[｛〔〈《「『【〘〖〝‘“｟«".ToCharArray();

	private static readonly char[] HYP_LATIN = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789<>=/().,".ToCharArray();

	private RectTransform _RectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	private Text _Text
	{
		get
		{
			if (_text == null)
			{
				_text = GetComponent<Text>();
			}
			return _text;
		}
	}

	public float textWidth
	{
		get
		{
			return _RectTransform.rect.width;
		}
		set
		{
			_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
		}
	}

	public int fontSize
	{
		get
		{
			return _Text.fontSize;
		}
		set
		{
			_Text.fontSize = value;
		}
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		UpdateText(text);
	}

	private void UpdateText(string str)
	{
		_Text.text = GetFormatedText(_Text, str);
	}

	public void SetText(string str)
	{
		text = str;
		UpdateText(text);
	}

	private float GetSpaceWidth(Text textComp)
	{
		float num = GetTextWidth(textComp, "m m");
		float num2 = GetTextWidth(textComp, "mm");
		return num - num2;
	}

	private float GetTextWidth(Text textComp, string message)
	{
		if (_text.supportRichText)
		{
			message = Regex.Replace(message, RITCH_TEXT_REPLACE, string.Empty);
		}
		textComp.text = message;
		return textComp.preferredWidth;
	}

	private string GetFormatedText(Text textComp, string msg)
	{
		if (string.IsNullOrEmpty(msg))
		{
			return string.Empty;
		}
		float width = _RectTransform.rect.width;
		float spaceWidth = GetSpaceWidth(textComp);
		textComp.horizontalOverflow = HorizontalWrapMode.Overflow;
		StringBuilder stringBuilder = new StringBuilder();
		float num = 0f;
		foreach (string word in GetWordList(msg))
		{
			num += GetTextWidth(textComp, word);
			if (word == Environment.NewLine)
			{
				num = 0f;
			}
			else
			{
				if (word == " ")
				{
					num += spaceWidth;
				}
				if (num > width)
				{
					stringBuilder.Append(Environment.NewLine);
					num = GetTextWidth(textComp, word);
				}
			}
			stringBuilder.Append(word);
		}
		return stringBuilder.ToString();
	}

	private List<string> GetWordList(string tmpText)
	{
		List<string> list = new List<string>();
		StringBuilder stringBuilder = new StringBuilder();
		char c = '\0';
		for (int i = 0; i < tmpText.Length; i++)
		{
			char c2 = tmpText[i];
			char c3 = ((i < tmpText.Length - 1) ? tmpText[i + 1] : c);
			char c4 = ((i <= 0) ? c : (c4 = tmpText[i - 1]));
			stringBuilder.Append(c2);
			if ((IsLatin(c2) && IsLatin(c4) && IsLatin(c2) && !IsLatin(c4)) || (!IsLatin(c2) && CHECK_HYP_BACK(c4)) || (!IsLatin(c3) && !CHECK_HYP_FRONT(c3) && !CHECK_HYP_BACK(c2)) || i == tmpText.Length - 1)
			{
				list.Add(stringBuilder.ToString());
				stringBuilder = new StringBuilder();
			}
		}
		return list;
	}

	private static bool CHECK_HYP_FRONT(char str)
	{
		return Array.Exists(HYP_FRONT, (char item) => item == str);
	}

	private static bool CHECK_HYP_BACK(char str)
	{
		return Array.Exists(HYP_BACK, (char item) => item == str);
	}

	private static bool IsLatin(char s)
	{
		return Array.Exists(HYP_LATIN, (char item) => item == s);
	}
}
