using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VerticalText : BaseMeshEffect
{
	private const string SupportedTagRegexPattersn = "<b>|</b>|<i>|</i>|<size=.*?>|</size>|<color=.*?>|</color>|<material=.*?>|</material>";

	private const string RotatableCharacters = "…‥‐゠＝=―–-〜ー()（）「」『』【】<>＜＞0123456789,";

	private const string HalfCharacters = "()<>0123456789,";

	private const string VCenterCharacters = "()（）「」『』【】<>＜＞";

	private const string HCenterCharacters = "（）";

	private const string RightTopCharacters = "、。.";

	private const string LeftTopCharacters = ",.";

	private const string SmallCharacters = "っッぁぃぅぇぉァィゥェォゃゅょャュョゎヮ";

	private const string HYP_FRONT = ",)]｝、。）〕〉》」』】〙〗〟’”｠»‐゠–〜ー?!！？‼⁇⁈⁉・:;。.";

	[SerializeField]
	private int MaxLengthPerLine;

	[SerializeField]
	private float m_spacing;

	public string text { get; private set; }

	public float spacing
	{
		get
		{
			return m_spacing;
		}
		set
		{
			if (m_spacing != value)
			{
				m_spacing = value;
				if (base.graphic != null)
				{
					base.graphic.SetVerticesDirty();
				}
			}
		}
	}

	public override void ModifyMesh(VertexHelper vertexList)
	{
		if (IsActive())
		{
			List<UIVertex> list = new List<UIVertex>();
			vertexList.GetUIVertexStream(list);
			ModifyVertices(list);
			vertexList.Clear();
			vertexList.AddUIVertexTriangleStream(list);
		}
	}

	public void ModifyVertices(List<UIVertex> vertexList)
	{
		Text component = GetComponent<Text>();
		if (component == null || component.text == null || component.text.ToCharArray().Length == 0)
		{
			return;
		}
		string text = component.text;
		int fontSize = component.fontSize;
		float num = spacing * (float)fontSize / 100f;
		float num2 = 0f;
		int num3 = 0;
		bool supportRichText = component.supportRichText;
		IEnumerator enumerator = null;
		Match match = null;
		switch (component.alignment)
		{
		case TextAnchor.UpperLeft:
		case TextAnchor.MiddleLeft:
		case TextAnchor.LowerLeft:
			num2 = 0f;
			break;
		case TextAnchor.UpperCenter:
		case TextAnchor.MiddleCenter:
		case TextAnchor.LowerCenter:
			num2 = 0.5f;
			break;
		case TextAnchor.UpperRight:
		case TextAnchor.MiddleRight:
		case TextAnchor.LowerRight:
			num2 = 1f;
			break;
		}
		component.font.RequestCharactersInTexture(text, fontSize);
		string[] array = text.Split('\n');
		float[] array2 = new float[array[0].Length];
		foreach (string text2 in array)
		{
			int lineLengthWithoutTags = text2.Length;
			if (supportRichText)
			{
				enumerator = LetterSpacing.GetRegexMatchedTagCollection(text2, out lineLengthWithoutTags);
				match = null;
				if (enumerator.MoveNext())
				{
					match = (Match)enumerator.Current;
				}
			}
			float num4 = (float)(lineLengthWithoutTags - 1) * num * num2;
			int num5 = 0;
			int num6 = 0;
			while (num5 < text2.Length)
			{
				if (supportRichText && match != null && match.Index == num5)
				{
					num5 += match.Length - 1;
					num6--;
					num3 += match.Length;
					match = null;
					if (enumerator.MoveNext())
					{
						match = (Match)enumerator.Current;
					}
				}
				else
				{
					int num7 = num3 * 6;
					if (num7 + 5 > vertexList.Count - 1)
					{
						return;
					}
					CharacterInfo info;
					if (num5 - 1 >= 0)
					{
						component.font.GetCharacterInfo(text2[num5 - 1], out info, fontSize);
						if (num2 == 0f)
						{
							num4 -= (float)(fontSize - info.advance);
						}
						else if (num2 == 0.5f)
						{
							num4 -= (float)(fontSize - info.advance) * 0.5f;
						}
					}
					Vector3 vector = Vector3.right * (num * (float)num6 - num4);
					component.font.GetCharacterInfo(text2[num5], out info, fontSize);
					if (num2 == 0f)
					{
						vector += Vector3.right * (fontSize - info.advance) * 0.5f;
					}
					else if (num2 == 1f)
					{
						vector += Vector3.left * (fontSize - info.advance) * 0.5f;
					}
					if ("っッぁぃぅぇぉァィゥェォゃゅょャュョゎヮ".Contains(text2[num5]) || "()（）「」『』【】<>＜＞".Contains(text2[num5]) || "()<>0123456789,".Contains(text2[num5]) || "、。.".Contains(text2[num5]))
					{
						vector += Vector3.up * ((float)(fontSize - info.minY - info.maxY) / 2f - (float)fontSize * 0.2f);
					}
					if ("っッぁぃぅぇぉァィゥェォゃゅょャュョゎヮ".Contains(text2[num5]) || "、。.".Contains(text2[num5]))
					{
						vector += Vector3.right * (fontSize - info.minX - info.maxX) / 2f;
					}
					if ("（）".Contains(text2[num5]))
					{
						vector += Vector3.right * (info.glyphWidth - info.minX - info.maxX) / 2f;
					}
					if ("っッぁぃぅぇぉァィゥェォゃゅょャュョゎヮ".Contains(text2[num5]))
					{
						vector += Vector3.right * (fontSize - info.glyphWidth) / 2f / 2f;
					}
					if ("、。.".Contains(text2[num5]))
					{
						vector += new Vector3(info.glyphWidth, info.glyphHeight, 0f);
					}
					if (",.".Contains(text2[num5]))
					{
						vector += new Vector3((float)(-info.glyphWidth) / 2f, 0f, 0f);
					}
					if ("()<>0123456789,".Contains(text2[num5]))
					{
						array2[num5] += (float)(fontSize - info.glyphWidth) * 0.8f;
					}
					switch (text2[num5])
					{
					case ' ':
						array2[num5] += (float)fontSize / 2f;
						break;
					case '<':
						vector += Vector3.down * ((float)info.glyphWidth / 8f);
						break;
					case '>':
						vector += Vector3.down * ((float)info.glyphWidth / 4f);
						break;
					case '（':
					case '）':
						vector += Vector3.right * ((float)info.glyphHeight / 10f);
						break;
					case '「':
					case '『':
						vector += Vector3.right * ((float)info.glyphHeight / 8f);
						vector += Vector3.down * ((float)info.glyphWidth / 4f);
						break;
					case '」':
					case '』':
						vector += Vector3.left * ((float)info.glyphHeight / 8f);
						vector += Vector3.up * ((float)info.glyphWidth / 4f);
						break;
					}
					vector += Vector3.up * array2[num5];
					for (int j = 0; j < 6; j++)
					{
						UIVertex value = vertexList[num7 + j];
						value.position += vector;
						vertexList[num7 + j] = value;
					}
					if ("…‥‐゠＝=―–-〜ー()（）「」『』【】<>＜＞0123456789,".Contains(text2[num5]))
					{
						Vector2 vector2 = Vector2.Lerp(vertexList[num7].position, vertexList[num7 + 3].position, 0.5f);
						for (int k = 0; k < 6; k++)
						{
							UIVertex value2 = vertexList[num7 + k];
							Vector3 vector3 = value2.position - (Vector3)vector2;
							Vector2 vector4 = new Vector2(vector3.x * Mathf.Cos(-(float)Math.PI / 2f) - vector3.y * Mathf.Sin(-(float)Math.PI / 2f), vector3.x * Mathf.Sin(-(float)Math.PI / 2f) + vector3.y * Mathf.Cos(-(float)Math.PI / 2f));
							value2.position = vector4 + vector2;
							vertexList[num7 + k] = value2;
						}
					}
					num3++;
				}
				num5++;
				num6++;
			}
			num4 = 0f;
			int num8 = text2.Length - 1;
			int num9 = text2.Length - 1;
			while (num8 >= 0)
			{
				num3--;
				if (num3 * 6 + 5 > vertexList.Count - 1)
				{
					return;
				}
				if (num8 + 1 < text2.Length)
				{
					CharacterInfo info2;
					component.font.GetCharacterInfo(text2[num8 + 1], out info2, fontSize);
					if (num2 == 1f)
					{
						num4 += (float)(fontSize - info2.advance);
					}
					else if (num2 == 0.5f)
					{
						num4 += (float)(fontSize - info2.advance) * 0.5f;
					}
				}
				Vector3 vector5 = Vector3.left * num4;
				int num10 = num3 * 6;
				for (int l = 0; l < 6; l++)
				{
					UIVertex value3 = vertexList[num10 + l];
					value3.position += vector5;
					vertexList[num10 + l] = value3;
				}
				num8--;
				num9--;
			}
			num3 += text2.Length;
			num3++;
		}
	}

	public void SetText(string text)
	{
		if (this.text == text)
		{
			return;
		}
		this.text = text;
		text = text.Replace("！？", "!?").Replace("！！", "!!");
		string[] array = text.Split('\n');
		List<string> list = new List<string>();
		int num = 0;
		for (int i = 0; i < array.Length; i++)
		{
			while (MaxLengthPerLine > 0 && array[i].Length > MaxLengthPerLine)
			{
				int num2 = MaxLengthPerLine;
				if (",)]｝、。）〕〉》」』】〙〗〟’”｠»‐゠–〜ー?!！？‼⁇⁈⁉・:;。.".Contains(array[i][num2]))
				{
					num2++;
				}
				num = Mathf.Max(num, num2);
				list.Add(array[i].Substring(0, num2));
				array[i] = array[i].Substring(num2, array[i].Length - num2);
			}
			if (array[i] != "")
			{
				num = Mathf.Max(num, array[i].Length);
				list.Add(array[i]);
			}
		}
		string[] array2 = new string[num];
		for (int j = 0; j < num; j++)
		{
			string text2 = "";
			for (int k = 0; k < list.Count; k++)
			{
				if (list[k].Length > j)
				{
					if (list[k].Length > j + 1 && list[k][j] == '!')
					{
						text2 = list[k][j].ToString() + list[k][j + 1] + text2;
						list[k] = list[k].Substring(0, j) + "\u3000";
					}
					else
					{
						text2 = list[k][j] + text2;
					}
				}
				else
				{
					text2 = "\u3000" + text2;
				}
			}
			array2[j] = text2;
		}
		text = string.Join("\n", array2);
		GetComponent<Text>().text = text;
		base.transform.localScale = base.transform.parent.localScale;
		Graphic graphic = GetComponent<Graphic>();
		if (graphic != null)
		{
			AppUtil.DelayAction(this, 0f, delegate
			{
				graphic.SetVerticesDirty();
			});
		}
	}
}
