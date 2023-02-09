using System;
using System.Collections.Generic;
using UnityEngine;

public class EgoPoint : ICloneable
{
	public List<long> Point = new List<long> { 0L, 0L };

	public EgoPoint()
	{
	}

	public EgoPoint(float pointFloat)
	{
		SetFloat(pointFloat, 1);
		FormatEgoPoint();
	}

	public EgoPoint(string pointInString)
	{
		if (pointInString == "")
		{
			return;
		}
		char c = pointInString[pointInString.Length - 1];
		int num = 0;
		if (c >= 'a' && c <= 'z')
		{
			c = Convert.ToChar(65 + c - 97);
		}
		if (c >= 'A')
		{
			pointInString = pointInString.Substring(0, pointInString.Length - 1);
			num += c - 65 + 1;
			for (int i = 0; i < num; i++)
			{
				Point.Add(0L);
			}
		}
		SetFloat(float.Parse(pointInString), num + 1);
	}

	private void SetFloat(float value, int digit)
	{
		string[] array = value.ToString("0.000").Split('.');
		Point[digit] = long.Parse(array[0]);
		if (digit > 0 && array.Length > 1)
		{
			Point[digit - 1] += long.Parse(array[1]);
		}
	}

	public override string ToString()
	{
		return ToString("0.00");
	}

	public bool IsZero()
	{
		return ToString() == "0.00";
	}

	public void debug()
	{
		string text = "";
		for (int num = Point.Count - 1; num >= 0; num--)
		{
			text = text + Point[num] + ", ";
		}
		Debug.Log("this.Point=" + text);
	}

	private void FormatEgoPoint()
	{
		for (int i = 0; i < Point.Count; i++)
		{
			long num = (long)((float)Point[i] / 1000f);
			if (num > 0)
			{
				Point[i] -= num * 1000;
				if (i == Point.Count - 1)
				{
					Point.Add(num);
				}
				else
				{
					Point[i + 1] += num;
				}
			}
			if (i < Point.Count - 1)
			{
				while (Point[i] < 0)
				{
					Point[i + 1]--;
					Point[i] += 1000L;
				}
			}
		}
		int num2 = Point.Count - 1;
		while (num2 > 1 && Point[num2] <= 0)
		{
			Point.RemoveAt(num2);
			num2--;
		}
		if (this >= new EgoPoint("1["))
		{
			Point = new EgoPoint("999.99Z").Point;
		}
	}

	public string ToString(string format)
	{
		int num = Point.Count - 1;
		float num2 = (float)Point[num] + (float)Point[num - 1] / 1000f;
		if (num2 == 0f)
		{
			return num2.ToString(format);
		}
		if (format.Contains("0.000"))
		{
			format = format.Replace("0.000", "0.000" + GetUnit(num));
			num2 -= 0.0005f;
		}
		else if (format.Contains("0.00"))
		{
			format = format.Replace("0.00", "0.00" + GetUnit(num));
			num2 -= 0.005f;
		}
		else if (format.Contains("0.0"))
		{
			format = format.Replace("0.0", "0.0" + GetUnit(num));
			num2 -= 0.05f;
		}
		else
		{
			format = format.Replace("0", "0" + GetUnit(num));
			num2 -= 0.5f;
		}
		return num2.ToString(format);
	}

	public int GetDigit()
	{
		return Point.Count - 1;
	}

	private string GetUnit(int index)
	{
		if (index < 2)
		{
			return "";
		}
		index -= 2;
		return " " + Convert.ToChar(65 + index);
	}

	public object Clone()
	{
		return new EgoPoint
		{
			Point = new List<long>(Point)
		};
	}

	public static EgoPoint operator +(EgoPoint org, EgoPoint value)
	{
		EgoPoint egoPoint = (EgoPoint)org.Clone();
		for (int i = egoPoint.Point.Count; i < value.Point.Count; i++)
		{
			egoPoint.Point.Add(0L);
		}
		for (int j = 0; j < value.Point.Count; j++)
		{
			egoPoint.Point[j] += value.Point[j];
		}
		egoPoint.FormatEgoPoint();
		return egoPoint;
	}

	public static EgoPoint operator *(EgoPoint org, float value)
	{
		EgoPoint egoPoint = (EgoPoint)org.Clone();
		for (int i = 0; i < egoPoint.Point.Count; i++)
		{
			egoPoint.SetFloat((float)org.Point[i] * value, i);
		}
		egoPoint.FormatEgoPoint();
		return egoPoint;
	}

	public static EgoPoint operator /(EgoPoint org, float value)
	{
		EgoPoint egoPoint = (EgoPoint)org.Clone();
		for (int i = 0; i < egoPoint.Point.Count; i++)
		{
			egoPoint.SetFloat((float)org.Point[i] / value, i);
		}
		egoPoint.FormatEgoPoint();
		return egoPoint;
	}

	public static EgoPoint operator /(EgoPoint org, long value)
	{
		EgoPoint egoPoint = (EgoPoint)org.Clone();
		while (value > 1000)
		{
			value /= 1000;
			egoPoint /= 1000L;
		}
		egoPoint /= (float)value;
		egoPoint.FormatEgoPoint();
		return egoPoint;
	}

	public static float operator /(EgoPoint org, EgoPoint value)
	{
		float result = 1f;
		EgoPoint egoPoint = value / 100f;
		for (int i = 0; i < 100; i++)
		{
			if (egoPoint * i <= org && org < egoPoint * (i + 1))
			{
				result = 0.01f * (float)i;
				break;
			}
		}
		return result;
	}

	public static EgoPoint operator -(EgoPoint org, EgoPoint value)
	{
		if (org <= value)
		{
			return new EgoPoint(0f);
		}
		EgoPoint egoPoint = (EgoPoint)org.Clone();
		int num = Mathf.Min(org.Point.Count, value.Point.Count);
		for (int i = 0; i < num; i++)
		{
			egoPoint.Point[i] -= value.Point[i];
		}
		egoPoint.FormatEgoPoint();
		return egoPoint;
	}

	public static bool operator >=(EgoPoint a, EgoPoint b)
	{
		return IsBigger(a, b, true);
	}

	public static bool operator <=(EgoPoint a, EgoPoint b)
	{
		return IsBigger(b, a, true);
	}

	public static bool operator >(EgoPoint a, EgoPoint b)
	{
		return IsBigger(a, b, false);
	}

	public static bool operator <(EgoPoint a, EgoPoint b)
	{
		return IsBigger(b, a, false);
	}

	public static bool IsBigger(EgoPoint a, EgoPoint b, bool equal)
	{
		if (a == null)
		{
			a = new EgoPoint();
		}
		if (b == null)
		{
			b = new EgoPoint();
		}
		int count = a.Point.Count;
		int count2 = b.Point.Count;
		if (count > count2)
		{
			return true;
		}
		if (count < count2)
		{
			return false;
		}
		for (int num = a.Point.Count - 1; num >= 0; num--)
		{
			if (a.Point[num] > b.Point[num])
			{
				return true;
			}
			if (a.Point[num] < b.Point[num])
			{
				return false;
			}
		}
		return equal;
	}

	public static EgoPoint CreateInstance(string value)
	{
		return new EgoPoint(value);
	}
}
