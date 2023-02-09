using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NUnit.Framework.Internal
{
	public class Randomizer : Random
	{
		public const string DefaultStringChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_";

		private const int DefaultStringLength = 25;

		private static Random _seedGenerator;

		private static int _initialSeed;

		private static Dictionary<MemberInfo, Randomizer> Randomizers;

		public static int InitialSeed
		{
			get
			{
				return _initialSeed;
			}
			set
			{
				_initialSeed = value;
				_seedGenerator = new Random(_initialSeed);
			}
		}

		static Randomizer()
		{
			InitialSeed = new Random().Next();
			Randomizers = new Dictionary<MemberInfo, Randomizer>();
		}

		public static Randomizer GetRandomizer(MemberInfo member)
		{
			if (Randomizers.ContainsKey(member))
			{
				return Randomizers[member];
			}
			Randomizer randomizer = CreateRandomizer();
			Randomizers[member] = randomizer;
			return randomizer;
		}

		public static Randomizer GetRandomizer(ParameterInfo parameter)
		{
			return GetRandomizer(parameter.Member);
		}

		public static Randomizer CreateRandomizer()
		{
			return new Randomizer(_seedGenerator.Next());
		}

		public Randomizer()
		{
		}

		public Randomizer(int seed)
			: base(seed)
		{
		}

		[CLSCompliant(false)]
		public uint NextUInt()
		{
			return NextUInt(0u, uint.MaxValue);
		}

		[CLSCompliant(false)]
		public uint NextUInt(uint max)
		{
			return NextUInt(0u, max);
		}

		[CLSCompliant(false)]
		public uint NextUInt(uint min, uint max)
		{
			Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");
			if (min == max)
			{
				return min;
			}
			uint num = max - min;
			uint num2 = uint.MaxValue - uint.MaxValue % num;
			uint num3;
			do
			{
				num3 = RawUInt();
			}
			while (num3 > num2);
			return num3 % num + min;
		}

		public short NextShort()
		{
			return NextShort(0, short.MaxValue);
		}

		public short NextShort(short max)
		{
			return NextShort(0, max);
		}

		public short NextShort(short min, short max)
		{
			return (short)Next(min, max);
		}

		[CLSCompliant(false)]
		public ushort NextUShort()
		{
			return NextUShort(0, ushort.MaxValue);
		}

		[CLSCompliant(false)]
		public ushort NextUShort(ushort max)
		{
			return NextUShort(0, max);
		}

		[CLSCompliant(false)]
		public ushort NextUShort(ushort min, ushort max)
		{
			return (ushort)Next(min, max);
		}

		public long NextLong()
		{
			return NextLong(0L, long.MaxValue);
		}

		public long NextLong(long max)
		{
			return NextLong(0L, max);
		}

		public long NextLong(long min, long max)
		{
			Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");
			if (min == max)
			{
				return min;
			}
			ulong num = (ulong)(max - min);
			ulong num2 = ulong.MaxValue - ulong.MaxValue % num;
			ulong num3;
			do
			{
				num3 = RawULong();
			}
			while (num3 > num2);
			return (long)(num3 % num) + min;
		}

		[CLSCompliant(false)]
		public ulong NextULong()
		{
			return NextULong(0uL, ulong.MaxValue);
		}

		[CLSCompliant(false)]
		public ulong NextULong(ulong max)
		{
			return NextULong(0uL, max);
		}

		[CLSCompliant(false)]
		public ulong NextULong(ulong min, ulong max)
		{
			Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");
			ulong num = max - min;
			if (num == 0)
			{
				return min;
			}
			ulong num2 = ulong.MaxValue - ulong.MaxValue % num;
			ulong num3;
			do
			{
				num3 = RawULong();
			}
			while (num3 > num2);
			return num3 % num + min;
		}

		public byte NextByte()
		{
			return NextByte(0, byte.MaxValue);
		}

		public byte NextByte(byte max)
		{
			return NextByte(0, max);
		}

		public byte NextByte(byte min, byte max)
		{
			return (byte)Next(min, max);
		}

		[CLSCompliant(false)]
		public sbyte NextSByte()
		{
			return NextSByte(0, sbyte.MaxValue);
		}

		[CLSCompliant(false)]
		public sbyte NextSByte(sbyte max)
		{
			return NextSByte(0, max);
		}

		[CLSCompliant(false)]
		public sbyte NextSByte(sbyte min, sbyte max)
		{
			return (sbyte)Next(min, max);
		}

		public bool NextBool()
		{
			return NextDouble() < 0.5;
		}

		public bool NextBool(double probability)
		{
			Guard.ArgumentInRange(probability >= 0.0 && probability <= 1.0, "Probability must be from 0.0 to 1.0", "probability");
			return NextDouble() < probability;
		}

		public double NextDouble(double max)
		{
			return NextDouble() * max;
		}

		public double NextDouble(double min, double max)
		{
			Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");
			if (max == min)
			{
				return min;
			}
			double num = max - min;
			return NextDouble() * num + min;
		}

		public float NextFloat()
		{
			return (float)NextDouble();
		}

		public float NextFloat(float max)
		{
			return (float)NextDouble(max);
		}

		public float NextFloat(float min, float max)
		{
			return (float)NextDouble(min, max);
		}

		public object NextEnum(Type type)
		{
			Array enumValues = TypeHelper.GetEnumValues(type);
			return enumValues.GetValue(Next(0, enumValues.Length));
		}

		public T NextEnum<T>()
		{
			return (T)NextEnum(typeof(T));
		}

		public string GetString(int outputLength, string allowedChars)
		{
			StringBuilder stringBuilder = new StringBuilder(outputLength);
			for (int i = 0; i < outputLength; i++)
			{
				stringBuilder.Append(allowedChars[Next(0, allowedChars.Length)]);
			}
			return stringBuilder.ToString();
		}

		public string GetString(int outputLength)
		{
			return GetString(outputLength, "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_");
		}

		public string GetString()
		{
			return GetString(25, "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789_");
		}

		public decimal NextDecimal()
		{
			int lo = Next(0, int.MaxValue);
			int mid = Next(0, int.MaxValue);
			int hi = Next(0, int.MaxValue);
			return new decimal(lo, mid, hi, false, 0);
		}

		public decimal NextDecimal(decimal max)
		{
			return NextDecimal() % max;
		}

		public decimal NextDecimal(decimal min, decimal max)
		{
			Guard.ArgumentInRange(max >= min, "Maximum value must be greater than or equal to minimum.", "max");
			Guard.ArgumentValid(max < 0m == min < 0m || min + decimal.MaxValue >= max, "Range too great for decimal data, use double range", "max");
			if (min == max)
			{
				return min;
			}
			decimal num = max - min;
			decimal num2 = decimal.MaxValue - decimal.MaxValue % num;
			decimal num3;
			do
			{
				num3 = NextDecimal();
			}
			while (num3 > num2);
			return num3 % num + min;
		}

		private uint RawUInt()
		{
			byte[] array = new byte[4];
			NextBytes(array);
			return BitConverter.ToUInt32(array, 0);
		}

		private uint RawUShort()
		{
			byte[] array = new byte[4];
			NextBytes(array);
			return BitConverter.ToUInt32(array, 0);
		}

		private ulong RawULong()
		{
			byte[] array = new byte[8];
			NextBytes(array);
			return BitConverter.ToUInt64(array, 0);
		}

		private long RawLong()
		{
			byte[] array = new byte[8];
			NextBytes(array);
			return BitConverter.ToInt64(array, 0);
		}

		private decimal RawDecimal()
		{
			int lo = Next(0, int.MaxValue);
			int mid = Next(0, int.MaxValue);
			int hi = Next(0, int.MaxValue);
			bool isNegative = NextBool();
			byte scale = NextByte(29);
			return new decimal(lo, mid, hi, isNegative, scale);
		}
	}
}
