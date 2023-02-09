using System;

internal static class TimeExtensions
{
	public static string ToBinaryString(this DateTime dt)
	{
		return dt.ToBinary().ToString();
	}

	public static DateTime Convert2DateTime(this string dt)
	{
		return DateTime.FromBinary(Convert.ToInt64(dt));
	}
}
