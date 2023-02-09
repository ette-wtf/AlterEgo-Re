using System;
using System.Runtime.InteropServices;

namespace NUnit.Framework.Constraints
{
	public class FloatingPointNumerics
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct FloatIntUnion
		{
			[FieldOffset(0)]
			public float Float;

			[FieldOffset(0)]
			public int Int;

			[FieldOffset(0)]
			public uint UInt;
		}

		[StructLayout(LayoutKind.Explicit)]
		private struct DoubleLongUnion
		{
			[FieldOffset(0)]
			public double Double;

			[FieldOffset(0)]
			public long Long;

			[FieldOffset(0)]
			public ulong ULong;
		}

		public static bool AreAlmostEqualUlps(float left, float right, int maxUlps)
		{
			FloatIntUnion floatIntUnion = default(FloatIntUnion);
			FloatIntUnion floatIntUnion2 = default(FloatIntUnion);
			floatIntUnion.Float = left;
			floatIntUnion2.Float = right;
			uint num = floatIntUnion.UInt >> 31;
			uint num2 = floatIntUnion2.UInt >> 31;
			uint num3 = (2147483648u - floatIntUnion.UInt) & num;
			floatIntUnion.UInt = num3 | (floatIntUnion.UInt & ~num);
			uint num4 = (2147483648u - floatIntUnion2.UInt) & num2;
			floatIntUnion2.UInt = num4 | (floatIntUnion2.UInt & ~num2);
			if (num != num2 && (Math.Abs(floatIntUnion.Int) > maxUlps || Math.Abs(floatIntUnion2.Int) > maxUlps))
			{
				return false;
			}
			return Math.Abs(floatIntUnion.Int - floatIntUnion2.Int) <= maxUlps;
		}

		public static bool AreAlmostEqualUlps(double left, double right, long maxUlps)
		{
			DoubleLongUnion doubleLongUnion = default(DoubleLongUnion);
			DoubleLongUnion doubleLongUnion2 = default(DoubleLongUnion);
			doubleLongUnion.Double = left;
			doubleLongUnion2.Double = right;
			ulong num = doubleLongUnion.ULong >> 63;
			ulong num2 = doubleLongUnion2.ULong >> 63;
			ulong num3 = (9223372036854775808uL - doubleLongUnion.ULong) & num;
			doubleLongUnion.ULong = num3 | (doubleLongUnion.ULong & ~num);
			ulong num4 = (9223372036854775808uL - doubleLongUnion2.ULong) & num2;
			doubleLongUnion2.ULong = num4 | (doubleLongUnion2.ULong & ~num2);
			if (num != num2 && (Math.Abs(doubleLongUnion.Long) > maxUlps || Math.Abs(doubleLongUnion2.Long) > maxUlps))
			{
				return false;
			}
			return Math.Abs(doubleLongUnion.Long - doubleLongUnion2.Long) <= maxUlps;
		}

		public static int ReinterpretAsInt(float value)
		{
			FloatIntUnion floatIntUnion = default(FloatIntUnion);
			floatIntUnion.Float = value;
			return floatIntUnion.Int;
		}

		public static long ReinterpretAsLong(double value)
		{
			DoubleLongUnion doubleLongUnion = default(DoubleLongUnion);
			doubleLongUnion.Double = value;
			return doubleLongUnion.Long;
		}

		public static float ReinterpretAsFloat(int value)
		{
			FloatIntUnion floatIntUnion = default(FloatIntUnion);
			floatIntUnion.Int = value;
			return floatIntUnion.Float;
		}

		public static double ReinterpretAsDouble(long value)
		{
			DoubleLongUnion doubleLongUnion = default(DoubleLongUnion);
			doubleLongUnion.Long = value;
			return doubleLongUnion.Double;
		}

		private FloatingPointNumerics()
		{
		}
	}
}
