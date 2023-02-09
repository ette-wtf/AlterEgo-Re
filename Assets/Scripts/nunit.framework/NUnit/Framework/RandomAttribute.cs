using System;
using System.Collections;
using NUnit.Compatibility;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Framework
{
	public class RandomAttribute : DataAttribute, IParameterDataSource
	{
		private abstract class RandomDataSource : IParameterDataSource
		{
			public Type DataType { get; protected set; }

			public abstract IEnumerable GetData(IParameterInfo parameter);
		}

		private abstract class RandomDataSource<T> : RandomDataSource
		{
			private T _min;

			private T _max;

			private int _count;

			private bool _inRange;

			protected Randomizer _randomizer;

			protected RandomDataSource(int count)
			{
				_count = count;
				_inRange = false;
				base.DataType = typeof(T);
			}

			protected RandomDataSource(T min, T max, int count)
			{
				_min = min;
				_max = max;
				_count = count;
				_inRange = true;
				base.DataType = typeof(T);
			}

			public override IEnumerable GetData(IParameterInfo parameter)
			{
				_randomizer = Randomizer.GetRandomizer(parameter.ParameterInfo);
				for (int i = 0; i < _count; i++)
				{
					yield return _inRange ? GetNext(_min, _max) : GetNext();
				}
			}

			protected abstract T GetNext();

			protected abstract T GetNext(T min, T max);
		}

		private class RandomDataConverter : RandomDataSource
		{
			private IParameterDataSource _source;

			public RandomDataConverter(IParameterDataSource source)
			{
				_source = source;
			}

			public override IEnumerable GetData(IParameterInfo parameter)
			{
				Type parmType = parameter.ParameterType;
				foreach (object obj in _source.GetData(parameter))
				{
					if (obj is int)
					{
						int ival = (int)obj;
						if ((object)parmType == typeof(short))
						{
							yield return (short)ival;
						}
						else if ((object)parmType == typeof(ushort))
						{
							yield return (ushort)ival;
						}
						else if ((object)parmType == typeof(byte))
						{
							yield return (byte)ival;
						}
						else if ((object)parmType == typeof(sbyte))
						{
							yield return (sbyte)ival;
						}
						else if ((object)parmType == typeof(decimal))
						{
							yield return (decimal)ival;
						}
					}
					else if (obj is double)
					{
						double d = (double)obj;
						if ((object)parmType == typeof(decimal))
						{
							yield return (decimal)d;
						}
					}
				}
			}
		}

		private class IntDataSource : RandomDataSource<int>
		{
			public IntDataSource(int count)
				: base(count)
			{
			}

			public IntDataSource(int min, int max, int count)
				: base(min, max, count)
			{
			}

			protected override int GetNext()
			{
				return _randomizer.Next();
			}

			protected override int GetNext(int min, int max)
			{
				return _randomizer.Next(min, max);
			}
		}

		private class UIntDataSource : RandomDataSource<uint>
		{
			public UIntDataSource(int count)
				: base(count)
			{
			}

			public UIntDataSource(uint min, uint max, int count)
				: base(min, max, count)
			{
			}

			protected override uint GetNext()
			{
				return _randomizer.NextUInt();
			}

			protected override uint GetNext(uint min, uint max)
			{
				return _randomizer.NextUInt(min, max);
			}
		}

		private class LongDataSource : RandomDataSource<long>
		{
			public LongDataSource(int count)
				: base(count)
			{
			}

			public LongDataSource(long min, long max, int count)
				: base(min, max, count)
			{
			}

			protected override long GetNext()
			{
				return _randomizer.NextLong();
			}

			protected override long GetNext(long min, long max)
			{
				return _randomizer.NextLong(min, max);
			}
		}

		private class ULongDataSource : RandomDataSource<ulong>
		{
			public ULongDataSource(int count)
				: base(count)
			{
			}

			public ULongDataSource(ulong min, ulong max, int count)
				: base(min, max, count)
			{
			}

			protected override ulong GetNext()
			{
				return _randomizer.NextULong();
			}

			protected override ulong GetNext(ulong min, ulong max)
			{
				return _randomizer.NextULong(min, max);
			}
		}

		private class ShortDataSource : RandomDataSource<short>
		{
			public ShortDataSource(int count)
				: base(count)
			{
			}

			public ShortDataSource(short min, short max, int count)
				: base(min, max, count)
			{
			}

			protected override short GetNext()
			{
				return _randomizer.NextShort();
			}

			protected override short GetNext(short min, short max)
			{
				return _randomizer.NextShort(min, max);
			}
		}

		private class UShortDataSource : RandomDataSource<ushort>
		{
			public UShortDataSource(int count)
				: base(count)
			{
			}

			public UShortDataSource(ushort min, ushort max, int count)
				: base(min, max, count)
			{
			}

			protected override ushort GetNext()
			{
				return _randomizer.NextUShort();
			}

			protected override ushort GetNext(ushort min, ushort max)
			{
				return _randomizer.NextUShort(min, max);
			}
		}

		private class DoubleDataSource : RandomDataSource<double>
		{
			public DoubleDataSource(int count)
				: base(count)
			{
			}

			public DoubleDataSource(double min, double max, int count)
				: base(min, max, count)
			{
			}

			protected override double GetNext()
			{
				return _randomizer.NextDouble();
			}

			protected override double GetNext(double min, double max)
			{
				return _randomizer.NextDouble(min, max);
			}
		}

		private class FloatDataSource : RandomDataSource<float>
		{
			public FloatDataSource(int count)
				: base(count)
			{
			}

			public FloatDataSource(float min, float max, int count)
				: base(min, max, count)
			{
			}

			protected override float GetNext()
			{
				return _randomizer.NextFloat();
			}

			protected override float GetNext(float min, float max)
			{
				return _randomizer.NextFloat(min, max);
			}
		}

		private class ByteDataSource : RandomDataSource<byte>
		{
			public ByteDataSource(int count)
				: base(count)
			{
			}

			public ByteDataSource(byte min, byte max, int count)
				: base(min, max, count)
			{
			}

			protected override byte GetNext()
			{
				return _randomizer.NextByte();
			}

			protected override byte GetNext(byte min, byte max)
			{
				return _randomizer.NextByte(min, max);
			}
		}

		private class SByteDataSource : RandomDataSource<sbyte>
		{
			public SByteDataSource(int count)
				: base(count)
			{
			}

			public SByteDataSource(sbyte min, sbyte max, int count)
				: base(min, max, count)
			{
			}

			protected override sbyte GetNext()
			{
				return _randomizer.NextSByte();
			}

			protected override sbyte GetNext(sbyte min, sbyte max)
			{
				return _randomizer.NextSByte(min, max);
			}
		}

		private class EnumDataSource : RandomDataSource
		{
			private int _count;

			public EnumDataSource(int count)
			{
				_count = count;
				base.DataType = typeof(Enum);
			}

			public override IEnumerable GetData(IParameterInfo parameter)
			{
				Guard.ArgumentValid(parameter.ParameterType.GetTypeInfo().IsEnum, "EnumDataSource requires an enum parameter", "parameter");
				Randomizer randomizer = Randomizer.GetRandomizer(parameter.ParameterInfo);
				base.DataType = parameter.ParameterType;
				for (int i = 0; i < _count; i++)
				{
					yield return randomizer.NextEnum(parameter.ParameterType);
				}
			}
		}

		private class DecimalDataSource : RandomDataSource<decimal>
		{
			public DecimalDataSource(int count)
				: base(count)
			{
			}

			public DecimalDataSource(decimal min, decimal max, int count)
				: base(min, max, count)
			{
			}

			protected override decimal GetNext()
			{
				return _randomizer.NextDecimal();
			}

			protected override decimal GetNext(decimal min, decimal max)
			{
				return _randomizer.NextDecimal(min, max);
			}
		}

		private RandomDataSource _source;

		private int _count;

		public RandomAttribute(int count)
		{
			_count = count;
		}

		public RandomAttribute(int min, int max, int count)
		{
			_source = new IntDataSource(min, max, count);
		}

		[CLSCompliant(false)]
		public RandomAttribute(uint min, uint max, int count)
		{
			_source = new UIntDataSource(min, max, count);
		}

		public RandomAttribute(long min, long max, int count)
		{
			_source = new LongDataSource(min, max, count);
		}

		[CLSCompliant(false)]
		public RandomAttribute(ulong min, ulong max, int count)
		{
			_source = new ULongDataSource(min, max, count);
		}

		public RandomAttribute(short min, short max, int count)
		{
			_source = new ShortDataSource(min, max, count);
		}

		[CLSCompliant(false)]
		public RandomAttribute(ushort min, ushort max, int count)
		{
			_source = new UShortDataSource(min, max, count);
		}

		public RandomAttribute(double min, double max, int count)
		{
			_source = new DoubleDataSource(min, max, count);
		}

		public RandomAttribute(float min, float max, int count)
		{
			_source = new FloatDataSource(min, max, count);
		}

		public RandomAttribute(byte min, byte max, int count)
		{
			_source = new ByteDataSource(min, max, count);
		}

		[CLSCompliant(false)]
		public RandomAttribute(sbyte min, sbyte max, int count)
		{
			_source = new SByteDataSource(min, max, count);
		}

		public IEnumerable GetData(IParameterInfo parameter)
		{
			Type parameterType = parameter.ParameterType;
			if (_source == null)
			{
				if ((object)parameterType == typeof(int))
				{
					_source = new IntDataSource(_count);
				}
				else if ((object)parameterType == typeof(uint))
				{
					_source = new UIntDataSource(_count);
				}
				else if ((object)parameterType == typeof(long))
				{
					_source = new LongDataSource(_count);
				}
				else if ((object)parameterType == typeof(ulong))
				{
					_source = new ULongDataSource(_count);
				}
				else if ((object)parameterType == typeof(short))
				{
					_source = new ShortDataSource(_count);
				}
				else if ((object)parameterType == typeof(ushort))
				{
					_source = new UShortDataSource(_count);
				}
				else if ((object)parameterType == typeof(double))
				{
					_source = new DoubleDataSource(_count);
				}
				else if ((object)parameterType == typeof(float))
				{
					_source = new FloatDataSource(_count);
				}
				else if ((object)parameterType == typeof(byte))
				{
					_source = new ByteDataSource(_count);
				}
				else if ((object)parameterType == typeof(sbyte))
				{
					_source = new SByteDataSource(_count);
				}
				else if ((object)parameterType == typeof(decimal))
				{
					_source = new DecimalDataSource(_count);
				}
				else if (parameterType.GetTypeInfo().IsEnum)
				{
					_source = new EnumDataSource(_count);
				}
				else
				{
					_source = new IntDataSource(_count);
				}
			}
			else if ((object)_source.DataType != parameterType && WeConvert(_source.DataType, parameterType))
			{
				_source = new RandomDataConverter(_source);
			}
			return _source.GetData(parameter);
		}

		private bool WeConvert(Type sourceType, Type targetType)
		{
			if ((object)targetType == typeof(short) || (object)targetType == typeof(ushort) || (object)targetType == typeof(byte) || (object)targetType == typeof(sbyte))
			{
				return (object)sourceType == typeof(int);
			}
			if ((object)targetType == typeof(decimal))
			{
				return (object)sourceType == typeof(int) || (object)sourceType == typeof(double);
			}
			return false;
		}
	}
}
