using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextLocalization : MonoBehaviour
{
	public string PriorKey = "";

	[SerializeField]
	private bool IsVertical;

	private string PriorText = "";

	private object[] Parameters;

	private Quaternion DefaultRotation;

	private static Type[] TypeList = new Type[4]
	{
		typeof(TextMeshPro),
		typeof(TextMeshProUGUI),
		typeof(Text),
		typeof(TextMesh)
	};

	private void Awake()
	{
		DefaultRotation = base.transform.localRotation;
	}

	private void OnEnable()
	{
		UpdateText();
	}

	public void SetKey(string key)
	{
		PriorKey = key;
		UpdateText();
	}

	public void SetText(string input)
	{
		PriorText = input;
		UpdateText();
	}

	public string GetKey()
	{
		if (PriorKey != "")
		{
			return PriorKey;
		}
		return ("[UI]" + base.transform.parent.name + "/" + base.gameObject.name).Replace("(Clone)", "");
	}

	public void SetParameters(params object[] parameters)
	{
		Parameters = parameters;
		UpdateText();
	}

	private void UpdateText()
	{
		string text = ((!(PriorText == "")) ? PriorText : LanguageManager.Get(GetKey()));
		if (Parameters != null)
		{
			text = string.Format(text, Parameters);
		}
		if (IsVertical)
		{
			if (LanguageManager.AllowVertical())
			{
				if (base.transform.localRotation == DefaultRotation)
				{
					base.transform.Rotate(new Vector3(0f, 0f, 90f));
				}
				char[] array = text.ToCharArray();
				text = "";
				char[] array2 = array;
				foreach (char c in array2)
				{
					text = text + c + "\n";
				}
				text = text.Substring(0, text.Length - 1);
			}
			else
			{
				base.transform.localRotation = DefaultRotation;
			}
		}
		text = text.Replace("<br>", "\n").Replace("、", "､").Replace("「", "｢")
			.Replace("」", "｣");
		Type[] typeList = TypeList;
		foreach (Type type in typeList)
		{
			object component = GetComponent(type);
			if (component != null)
			{
				if (component is TextMeshProUGUI)
				{
					((TextMeshProUGUI)component).text = text;
				}
				else
				{
					type.GetProperty("text").SetValue(component, text, null);
				}
				break;
			}
		}
		Font font = LanguageManager.GetFont(GetKey());
		SetFont(font);
		if (GetComponent<CenterLeftText>() != null)
		{
			GetComponent<CenterLeftText>().OnEnable();
		}
	}

	public void SetFont(string fontName)
	{
		if (LanguageManager.FONTDATA_TABLE.ContainsKey(fontName))
		{
			Font font = LanguageManager.FONTDATA_TABLE[fontName];
			SetFont(font);
		}
	}

	public void SetFont(Font font)
	{
		if (font == null)
		{
			return;
		}
		Type[] typeList = TypeList;
		foreach (Type type in typeList)
		{
			if ((object)GetComponent(type) != null)
			{
				if (type == typeof(Text))
				{
					GetComponent<Text>().font = font;
				}
				if (type == typeof(TextMesh))
				{
					GetComponent<TextMesh>().font = font;
					GetComponent<Renderer>().sharedMaterial = font.material;
				}
				break;
			}
		}
	}
}
