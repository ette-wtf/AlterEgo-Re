using System;

public static class ActionExtensions
{
	public static void NullSafe<T>(this Action<T> action, T arg)
	{
		if (action != null)
		{
			action(arg);
		}
	}

	public static void NullSafe<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
	{
		if (action != null)
		{
			action(arg1, arg2);
		}
	}
}
