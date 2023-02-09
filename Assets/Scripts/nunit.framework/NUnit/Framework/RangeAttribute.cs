using System;

namespace NUnit.Framework
{
	public class RangeAttribute : ValuesAttribute
	{
		public RangeAttribute(int from, int to)
			: this(from, to, (from <= to) ? 1 : (-1))
		{
		}

		public RangeAttribute(int from, int to, int step)
		{
			Guard.ArgumentValid((step > 0 && to >= from) || (step < 0 && to <= from), "Step must be positive with to >= from or negative with to <= from", "step");
			int num = (to - from) / step + 1;
			data = new object[num];
			int num2 = 0;
			int num3 = from;
			while (num2 < num)
			{
				data[num2++] = num3;
				num3 += step;
			}
		}

		[CLSCompliant(false)]
		public RangeAttribute(uint from, uint to)
			: this(from, to, 1u)
		{
		}

		[CLSCompliant(false)]
		public RangeAttribute(uint from, uint to, uint step)
		{
			Guard.ArgumentValid(step != 0, "Step must be greater than zero", "step");
			Guard.ArgumentValid(to >= from, "Value of to must be greater than or equal to from", "to");
			uint num = (to - from) / step + 1;
			data = new object[num];
			uint num2 = 0u;
			uint num3 = from;
			while (num2 < num)
			{
				data[num2++] = num3;
				num3 += step;
			}
		}

		public RangeAttribute(long from, long to)
			: this(from, to, (from > to) ? (-1) : 1)
		{
		}

		public RangeAttribute(long from, long to, long step)
		{
			Guard.ArgumentValid((step > 0 && to >= from) || (step < 0 && to <= from), "Step must be positive with to >= from or negative with to <= from", "step");
			long num = (to - from) / step + 1;
			data = new object[num];
			int num2 = 0;
			long num3 = from;
			while (num2 < num)
			{
				data[num2++] = num3;
				num3 += step;
			}
		}

		[CLSCompliant(false)]
		public RangeAttribute(ulong from, ulong to)
			: this(from, to, 1uL)
		{
		}

		[CLSCompliant(false)]
		public RangeAttribute(ulong from, ulong to, ulong step)
		{
			Guard.ArgumentValid(step != 0, "Step must be greater than zero", "step");
			Guard.ArgumentValid(to >= from, "Value of to must be greater than or equal to from", "to");
			ulong num = (to - from) / step + 1;
			data = new object[num];
			ulong num2 = 0uL;
			ulong num3 = from;
			while (num2 < num)
			{
				data[num2++] = num3;
				num3 += step;
			}
		}

		public RangeAttribute(double from, double to, double step)
		{
			Guard.ArgumentValid((step > 0.0 && to >= from) || (step < 0.0 && to <= from), "Step must be positive with to >= from or negative with to <= from", "step");
			double num = Math.Abs(step);
			double num2 = num / 1000.0;
			int num3 = (int)(Math.Abs(to - from) / num + num2 + 1.0);
			data = new object[num3];
			int num4 = 0;
			double num5 = from;
			while (num4 < num3)
			{
				data[num4++] = num5;
				num5 += step;
			}
		}

		public RangeAttribute(float from, float to, float step)
		{
			Guard.ArgumentValid((step > 0f && to >= from) || (step < 0f && to <= from), "Step must be positive with to >= from or negative with to <= from", "step");
			float num = Math.Abs(step);
			float num2 = num / 1000f;
			int num3 = (int)(Math.Abs(to - from) / num + num2 + 1f);
			data = new object[num3];
			int num4 = 0;
			float num5 = from;
			while (num4 < num3)
			{
				data[num4++] = num5;
				num5 += step;
			}
		}
	}
}
